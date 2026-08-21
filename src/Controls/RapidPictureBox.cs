

using InvokedServer.Utilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;


namespace InvokedServer.Controls
{
	public class RapidPictureBox : PictureBox, IRapidPictureBox
	{
		private readonly object _imageLock = new object();
		private Stopwatch _sWatch;
		private Stopwatch _sFrameCapWatch;
		private const float TargetFrameTime = 0.001667f;
		private float _elapsedTime;
		public FrameCounter _frameCounter;

		public bool Running { get; set; }

		public bool CapFPS { get; set; }

		public int ScreenWidth { get; private set; }

		public int ScreenHeight { get; private set; }

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
			if (this.CapFPS && !this.CheckAgainstFPScap())
				return;
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

		public RapidPictureBox()
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
	}
}