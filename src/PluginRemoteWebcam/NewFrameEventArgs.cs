

using System;
using System.Drawing;


namespace AForge.Video
{
	public class NewFrameEventArgs : EventArgs
	{
		private Bitmap frame;

		public NewFrameEventArgs(Bitmap frame) => this.frame = frame;

		public Bitmap Frame => this.frame;
	}
}
