

using System;


namespace AForge.Video
{
	public class VideoSourceErrorEventArgs : EventArgs
	{
		private string description;

		public VideoSourceErrorEventArgs(string description) => this.description = description;

		public string Description => this.description;
	}
}
