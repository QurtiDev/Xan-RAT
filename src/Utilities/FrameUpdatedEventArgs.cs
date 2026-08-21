

using System;


namespace InvokedServer.Utilities
{
	public class FrameUpdatedEventArgs : EventArgs
	{
		public float CurrentFramesPerSecond { get; private set; }

		public FrameUpdatedEventArgs(float _CurrentFramesPerSecond)
		{
			this.CurrentFramesPerSecond = _CurrentFramesPerSecond;
		}
	}
}