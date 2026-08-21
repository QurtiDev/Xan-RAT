

using InvokedCommon.Extensions;
using InvokedCommon.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;


namespace InvokedServer.Networking
{
	public class Server
	{
		private const int BufferSize = 16384;
		private const uint KeepAliveTime = 25000;
		private const uint KeepAliveInterval = 35000;
		private readonly BufferPool _bufferPool = new BufferPool(16384, 1)
		{
			ClearOnReturn = false
		};
		private Socket _handle;
		protected readonly X509Certificate2 ServerCertificate;
		private SocketAsyncEventArgs _item;
		private readonly List<Client> _clients = new List<Client>();
		private UPnPService _UPnPService;
		private readonly object _clientsLock = new object();

		public event Server.ServerStateEventHandler ServerState;

		private void OnServerState(bool listening)
		{
			if (this.Listening == listening)
				return;
			this.Listening = listening;
			Server.ServerStateEventHandler serverState = this.ServerState;
			if (serverState == null)
				return;
			serverState(this, listening, this.Port);
		}

		public event Server.ClientStateEventHandler ClientState;

		private void OnClientState(Client c, bool connected)
		{
			if (!connected)
				this.RemoveClient(c);
			Server.ClientStateEventHandler clientState = this.ClientState;
			if (clientState == null)
				return;
			clientState(this, c, connected);
		}

		public event Server.ClientReadEventHandler ClientRead;

		private void OnClientRead(Client c, IMessage message, int messageLength)
		{
			this.BytesReceived += (long)messageLength;
			Server.ClientReadEventHandler clientRead = this.ClientRead;
			if (clientRead == null)
				return;
			clientRead(this, c, message);
		}

		public event Server.ClientWriteEventHandler ClientWrite;

		private void OnClientWrite(Client c, IMessage message, int messageLength)
		{
			this.BytesSent += (long)messageLength;
			Server.ClientWriteEventHandler clientWrite = this.ClientWrite;
			if (clientWrite == null)
				return;
			clientWrite(this, c, message);
		}

		public ushort Port { get; private set; }

		public long BytesReceived { get; set; }

		public long BytesSent { get; set; }

		public bool Listening { get; private set; }

		protected Client[] Clients
		{
			get
			{
				lock (this._clientsLock)
					return this._clients.ToArray();
			}
		}

		protected bool ProcessingDisconnect { get; set; }

		protected Server(X509Certificate2 serverCertificate)
		{
			this.ServerCertificate = serverCertificate;
			TypeRegistry.AddTypesToSerializer(typeof(IMessage), TypeRegistry.GetPacketTypes(typeof(IMessage)).ToArray<Type>());
		}

		public void Listen(ushort port, bool ipv6, bool enableUPnP)
		{
			if (this.Listening)
				return;
			this.Port = port;
			if (enableUPnP)
			{
				this._UPnPService = new UPnPService();
				this._UPnPService.CreatePortMapAsync((int)port);
			}
			if (Socket.OSSupportsIPv6 & ipv6)
			{
				this._handle = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
				this._handle.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0);
				this._handle.Bind((EndPoint)new IPEndPoint(IPAddress.IPv6Any, (int)port));
			}
			else
			{
				this._handle = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				this._handle.Bind((EndPoint)new IPEndPoint(IPAddress.Any, (int)port));
			}
			this._handle.Listen(1000);
			this.OnServerState(true);
			this._item = new SocketAsyncEventArgs();
			this._item.Completed += new EventHandler<SocketAsyncEventArgs>(this.AcceptClient);
			if (this._handle.AcceptAsync(this._item))
				return;
			this.AcceptClient((object)this, this._item);
		}

		private void AcceptClient(object s, SocketAsyncEventArgs e)
		{
			try
			{
				do
				{
					switch (e.SocketError)
					{
						case SocketError.Success:
							SslStream sslStream = (SslStream)null;
							try
							{
								Socket acceptSocket = e.AcceptSocket;
								acceptSocket.SetKeepAliveEx(35000U, 25000U);
								sslStream = new SslStream((Stream)new NetworkStream(acceptSocket, true), false);
								sslStream.BeginAuthenticateAsServer((X509Certificate)this.ServerCertificate, false, SslProtocols.Tls12, false, new AsyncCallback(this.EndAuthenticateClient), (object)new Server.PendingClient()
								{
									Stream = sslStream,
									EndPoint = (IPEndPoint)acceptSocket.RemoteEndPoint
								});
								goto case SocketError.ConnectionReset;
							}
							catch
							{
								if (sslStream != null)
								{
									sslStream.Close();
									goto case SocketError.ConnectionReset;
								}
								else
									goto case SocketError.ConnectionReset;
							}
						case SocketError.ConnectionReset:
							e.AcceptSocket = (Socket)null;
							continue;
						default:
							throw new SocketException((int)e.SocketError);
					}
				}
				while (!this._handle.AcceptAsync(e));
			}
			catch (ObjectDisposedException)
			{
			}
			catch
			{
				this.Disconnect();
			}
		}

		private void EndAuthenticateClient(IAsyncResult ar)
		{
			Server.PendingClient asyncState = (Server.PendingClient)ar.AsyncState;
			try
			{
				asyncState.Stream.EndAuthenticateAsServer(ar);
				Client client = new Client(this._bufferPool, asyncState.Stream, asyncState.EndPoint);
				this.AddClient(client);
				this.OnClientState(client, true);
			}
			catch
			{
				asyncState.Stream.Close();
			}
		}

		private void AddClient(Client client)
		{
			lock (this._clientsLock)
			{
				client.ClientState += new Client.ClientStateEventHandler(this.OnClientState);
				client.ClientRead += new Client.ClientReadEventHandler(this.OnClientRead);
				client.ClientWrite += new Client.ClientWriteEventHandler(this.OnClientWrite);
				this._clients.Add(client);
			}
		}

		private void RemoveClient(Client client)
		{
			if (this.ProcessingDisconnect)
				return;
			lock (this._clientsLock)
			{
				client.ClientState -= new Client.ClientStateEventHandler(this.OnClientState);
				client.ClientRead -= new Client.ClientReadEventHandler(this.OnClientRead);
				client.ClientWrite -= new Client.ClientWriteEventHandler(this.OnClientWrite);
				this._clients.Remove(client);
			}
		}

		public void Disconnect()
		{
			if (this.ProcessingDisconnect)
				return;
			this.ProcessingDisconnect = true;
			if (this._handle != null)
			{
				this._handle.Close();
				this._handle = (Socket)null;
			}
			if (this._item != null)
			{
				this._item.Dispose();
				this._item = (SocketAsyncEventArgs)null;
			}
			if (this._UPnPService != null)
			{
				this._UPnPService.DeletePortMapAsync((int)this.Port);
				this._UPnPService = (UPnPService)null;
			}
			lock (this._clientsLock)
			{
				while (this._clients.Count != 0)
				{
					try
					{
						this._clients[0].Disconnect();
						this._clients[0].ClientState -= new Client.ClientStateEventHandler(this.OnClientState);
						this._clients[0].ClientRead -= new Client.ClientReadEventHandler(this.OnClientRead);
						this._clients[0].ClientWrite -= new Client.ClientWriteEventHandler(this.OnClientWrite);
						this._clients.RemoveAt(0);
					}
					catch
					{
					}
				}
			}
			this.ProcessingDisconnect = false;
			this.OnServerState(false);
		}

		public delegate void ServerStateEventHandler(Server s, bool listening, ushort port);

		public delegate void ClientStateEventHandler(Server s, Client c, bool connected);

		public delegate void ClientReadEventHandler(Server s, Client c, IMessage message);

		public delegate void ClientWriteEventHandler(Server s, Client c, IMessage message);

		private class PendingClient
		{
			public SslStream Stream { get; set; }

			public IPEndPoint EndPoint { get; set; }
		}
	}
}