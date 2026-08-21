

using System;
using System.Collections.Generic;
using System.Linq;


namespace InvokedServer.Utilities
{
	public class FrameCounter
	{
		public const int MAXIMUM_SAMPLES = 50;
		private Queue<float> _sampleBuffer = new Queue<float>();

		public long TotalFrames { get; private set; }

		public float TotalSeconds { get; private set; }

		public float AverageFramesPerSecond { get; private set; }

		public event FrameUpdatedEventHandler FrameUpdated;

		public void Update(float deltaTime)
		{
			float num1 = 1f / deltaTime;
			this._sampleBuffer.Enqueue(num1);
			if (this._sampleBuffer.Count > 50)
			{
				double num2 = (double)this._sampleBuffer.Dequeue();
				this.AverageFramesPerSecond = this._sampleBuffer.Average<float>((Func<float, float>)(i => i));
			}
			else
				this.AverageFramesPerSecond = num1;
			this.OnFrameUpdated(new FrameUpdatedEventArgs(this.AverageFramesPerSecond));
			++this.TotalFrames;
			this.TotalSeconds += deltaTime;
		}

		protected virtual void OnFrameUpdated(FrameUpdatedEventArgs e)
		{
			FrameUpdatedEventHandler frameUpdated = this.FrameUpdated;
			if (frameUpdated == null)
				return;
			frameUpdated(e);
		}
	}
}