

using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedCommon.Networking;


namespace InvokedServer.Messages
{
	public class TcpConnectionsHandler : MessageProcessorBase<TcpConnection[]>
	{
		private readonly InvokedServer.Networking.Client _client;

		public TcpConnectionsHandler(InvokedServer.Networking.Client client)
		  : base(true)
		{
			this._client = client;
		}

		public override bool CanExecute(IMessage message) => message is GetConnectionsResponse;

		public override bool CanExecuteFrom(ISender sender) => this._client.Equals((object)sender);

		public override void Execute(ISender sender, IMessage message)
		{
			if (!(message is GetConnectionsResponse message1))
				return;
			this.Execute(sender, message1);
		}

		public void RefreshTcpConnections() => this._client.Send<GetConnections>(new GetConnections());

		public void CloseTcpConnection(
		  string localAddress,
		  ushort localPort,
		  string remoteAddress,
		  ushort remotePort)
		{
			this._client.Send<DoCloseConnection>(new DoCloseConnection()
			{
				LocalAddress = localAddress,
				LocalPort = localPort,
				RemoteAddress = remoteAddress,
				RemotePort = remotePort
			});
		}

		private void Execute(ISender client, GetConnectionsResponse message)
		{
			this.OnReport(message.Connections);
		}
	}
}