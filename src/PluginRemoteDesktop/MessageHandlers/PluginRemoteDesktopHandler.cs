

using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Networking;
using InvokedCommon.Video;
using InvokedCommon.Video.Codecs;
using Plugin.Helper;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;


namespace Plugin.MessageHandlers
{
	public class PluginRemoteDesktopHandler : IMessageProcessor, IDisposable
	{
		private static UnsafeStreamCodec _streamCodec;
		public static bool _showCursor = false;
		public static int _displayIndex = 1;
		public Client _client;
		private int _sentFramesCount;
		private int _refreshFramesNum = 30;
		private int _waitFramesNum = 60;
		private bool _sentCheckServerRequest;
		private readonly object _streamcodecLock = new object();
		public static bool Active = false;
		public bool _enableOldGraphicsEngine;

		public bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case GetDesktop _:
				case DoMouseEvent _:
				case DoKeyboardEvent _:
				case GetMonitors _:
				case GetDesktopOldGfxEng _:
					return true;
				default:
					return message is SetRDStatus;
			}
		}

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case GetDesktop message1:
					this.Execute(sender, message1);
					break;
				case DoMouseEvent message2:
					this.Execute(sender, message2);
					break;
				case DoKeyboardEvent message3:
					this.Execute(sender, message3);
					break;
				case GetMonitors message4:
					this.Execute(sender, message4);
					break;
				case SetRDStatus message5:
					this.Execute(sender, message5);
					break;
				case GetDesktopOldGfxEng message6:
					this.Execute(sender, message6);
					break;
			}
		}

		public void Execute(ISender client, SetRDStatus message)
		{
			if (message.Status != RemoteDesktopStatus.ResetClientFrameCount)
				return;
			this._sentFramesCount = 0;
			this._sentCheckServerRequest = false;
		}

		private void StopRemoteDesktop(ISender client)
		{
			PluginRemoteDesktopHandler.Active = false;
			this._sentFramesCount = 0;
			this._sentCheckServerRequest = false;
			try
			{
				client.Send<GetDesktopResponse>(new GetDesktopResponse()
				{
					Image = (byte[]) null,
					Quality = PluginRemoteDesktopHandler._streamCodec.ImageQuality,
					Monitor = PluginRemoteDesktopHandler._streamCodec.Monitor,
					Resolution = PluginRemoteDesktopHandler._streamCodec.Resolution,
					RDStatus = RemoteDesktopStatus.Stopped
				});
			}
			catch
			{
			}
		}

		private void Execute(ISender client, GetDesktopOldGfxEng message)
		{
			PluginRemoteDesktopHandler.Active = false;
			this._client = (Client) client;
			PluginRemoteDesktopHandler._displayIndex = message.DisplayIndex;
			PluginRemoteDesktopHandler._showCursor = message.ShowCursor;
			Rectangle bounds = RemoteDesktopHelper.GetBounds(message.DisplayIndex);
			Resolution resolution = new Resolution()
			{
				Height = bounds.Height,
				Width = bounds.Width
			};
			lock (this._streamcodecLock)
			{
				if (PluginRemoteDesktopHandler._streamCodec == null)
					PluginRemoteDesktopHandler._streamCodec = new UnsafeStreamCodec(message.Quality, message.DisplayIndex, resolution);
				if (message.CreateNew)
				{
					PluginRemoteDesktopHandler._streamCodec?.Dispose();
					PluginRemoteDesktopHandler._streamCodec = new UnsafeStreamCodec(message.Quality, message.DisplayIndex, resolution);
				}
				if (PluginRemoteDesktopHandler._streamCodec.ImageQuality == message.Quality && PluginRemoteDesktopHandler._streamCodec.Monitor == message.DisplayIndex)
				{
					if (!(PluginRemoteDesktopHandler._streamCodec.Resolution != resolution))
						goto label_15;
				}
				PluginRemoteDesktopHandler._streamCodec?.Dispose();
				PluginRemoteDesktopHandler._streamCodec = new UnsafeStreamCodec(message.Quality, message.DisplayIndex, resolution);
			}
label_15:
			this.CaptureAndSendFrame(true);
		}

		private void Execute(ISender client, GetDesktop message)
		{
			if (message.RDStatus == RemoteDesktopStatus.Stop)
				this.StopRemoteDesktop(client);
			if (message.RDStatus != RemoteDesktopStatus.Start || PluginRemoteDesktopHandler.Active)
				return;
			lock (this._streamcodecLock)
			{
				PluginRemoteDesktopHandler.Active = true;
				this._client = (Client) client;
				PluginRemoteDesktopHandler._displayIndex = message.DisplayIndex;
				PluginRemoteDesktopHandler._showCursor = message.ShowCursor;
				Rectangle bounds = RemoteDesktopHelper.GetBounds(message.DisplayIndex);
				Resolution resolution = new Resolution()
				{
					Height = bounds.Height,
					Width = bounds.Width
				};
				PluginRemoteDesktopHandler._streamCodec?.Dispose();
				PluginRemoteDesktopHandler._streamCodec = new UnsafeStreamCodec(message.Quality, message.DisplayIndex, resolution);
				new Thread((ThreadStart) (() => this.CaptureAndSendThread())).Start();
			}
		}

		private void CaptureAndSendFrame(bool OldGfxEng = false)
		{
			RemoteDesktopHelper.SetThreadDesktop(RemoteDesktopHelper.OpenDesktop("Default", 0, true, 511U));
			BitmapData bitmapdata = (BitmapData) null;
			Bitmap bitmap = (Bitmap) null;
			try
			{
				bitmap = RemoteDesktopHelper.CaptureScreen(PluginRemoteDesktopHandler._displayIndex, PluginRemoteDesktopHandler._showCursor);
				bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
				using (MemoryStream outStream = new MemoryStream())
				{
					lock (this._streamcodecLock)
					{
						if (PluginRemoteDesktopHandler._streamCodec == null)
						{
							PluginRemoteDesktopHandler.Active = false;
							this.StopRemoteDesktop((ISender) this._client);
						}
						PluginRemoteDesktopHandler._streamCodec.CodeImage(bitmapdata.Scan0, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Size(bitmap.Width, bitmap.Height), bitmap.PixelFormat, (Stream) outStream);
						if (this._client == null)
							PluginRemoteDesktopHandler.Active = false;
						else if (OldGfxEng)
						{
							this._client.Send<GetDesktopResponseOldGfxEng>(new GetDesktopResponseOldGfxEng()
							{
								Image = outStream.ToArray(),
								Quality = PluginRemoteDesktopHandler._streamCodec.ImageQuality,
								Monitor = PluginRemoteDesktopHandler._streamCodec.Monitor,
								Resolution = PluginRemoteDesktopHandler._streamCodec.Resolution
							});
							++this._sentFramesCount;
						}
						else
						{
							this._client.Send<GetDesktopResponse>(new GetDesktopResponse()
							{
								Image = outStream.ToArray(),
								Quality = PluginRemoteDesktopHandler._streamCodec.ImageQuality,
								Monitor = PluginRemoteDesktopHandler._streamCodec.Monitor,
								Resolution = PluginRemoteDesktopHandler._streamCodec.Resolution
							});
							++this._sentFramesCount;
						}
					}
				}
			}
			catch (Exception ex)
			{
				lock (this._streamcodecLock)
				{
					PluginRemoteDesktopHandler._streamCodec?.Dispose();
					PluginRemoteDesktopHandler._streamCodec = (UnsafeStreamCodec) null;
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

		private void CaptureAndSendThread()
		{
			Thread.Sleep(1);
label_5:
			while (PluginRemoteDesktopHandler.Active && this._client != null && this._client.Connected && !this._enableOldGraphicsEngine)
			{
				this.CaptureAndSendFrame();
				if (this._sentFramesCount > this._refreshFramesNum && !this._sentCheckServerRequest)
				{
					this._client.Send<SetRDStatus>(new SetRDStatus()
					{
						Status = RemoteDesktopStatus.CheckServer
					});
					this._sentCheckServerRequest = true;
				}
				while (true)
				{
					if (this._sentFramesCount > this._waitFramesNum && PluginRemoteDesktopHandler.Active && this._client != null && this._client.Connected)
						Thread.Sleep(1);
					else
						goto label_5;
				}
			}
			PluginRemoteDesktopHandler.Active = false;
			lock (this._streamcodecLock)
			{
				if (PluginRemoteDesktopHandler._streamCodec != null)
					PluginRemoteDesktopHandler._streamCodec.Dispose();
				PluginRemoteDesktopHandler._streamCodec = (UnsafeStreamCodec) null;
			}
		}

		private void Execute(ISender sender, DoMouseEvent message)
		{
			try
			{
				Screen[] allScreens = Screen.AllScreens;
				int x = allScreens[message.MonitorIndex].Bounds.X;
				int y = allScreens[message.MonitorIndex].Bounds.Y;
				Point p = new Point(message.X + x, message.Y + y);
				switch (message.Action)
				{
					case MouseAction.LeftDown:
					case MouseAction.LeftUp:
					case MouseAction.RightDown:
					case MouseAction.RightUp:
					case MouseAction.MoveCursor:
						if (NativeMethodsHelper.IsScreensaverActive())
						{
							NativeMethodsHelper.DisableScreensaver();
							break;
						}
						break;
				}
				switch (message.Action)
				{
					case MouseAction.LeftDown:
					case MouseAction.LeftUp:
						NativeMethodsHelper.DoMouseLeftClick(p, message.IsMouseDown);
						break;
					case MouseAction.RightDown:
					case MouseAction.RightUp:
						NativeMethodsHelper.DoMouseRightClick(p, message.IsMouseDown);
						break;
					case MouseAction.MoveCursor:
						NativeMethodsHelper.DoMouseMove(p);
						break;
					case MouseAction.ScrollUp:
						NativeMethodsHelper.DoMouseScroll(p, false);
						break;
					case MouseAction.ScrollDown:
						NativeMethodsHelper.DoMouseScroll(p, true);
						break;
				}
			}
			catch
			{
			}
		}

		private void Execute(ISender sender, DoKeyboardEvent message)
		{
			if (NativeMethodsHelper.IsScreensaverActive())
				NativeMethodsHelper.DisableScreensaver();
			NativeMethodsHelper.DoKeyPress(message.Key, message.KeyDown);
		}

		private void Execute(ISender client, GetMonitors message)
		{
			client.Send<GetMonitorsResponse>(new GetMonitorsResponse()
			{
				Number = Screen.AllScreens.Length
			});
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
			PluginRemoteDesktopHandler._streamCodec?.Dispose();
		}
	}
}
