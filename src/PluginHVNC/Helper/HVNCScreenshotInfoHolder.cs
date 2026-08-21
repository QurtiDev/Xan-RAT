

using System.Drawing;


namespace Plugin.Helper
{
	public struct HVNCScreenshotInfoHolder
	{
		public Bitmap Image { get; private set; }

		public int ScreenshotWindowCount { get; private set; }

		public HVNCScreenshotInfoHolder(Bitmap _image, int _windowcount)
		{
			this.Image = _image;
			this.ScreenshotWindowCount = _windowcount;
		}
	}
}
