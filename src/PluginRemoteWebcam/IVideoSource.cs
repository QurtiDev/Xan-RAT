


namespace AForge.Video
{
	public interface IVideoSource
	{
	event NewFrameEventHandler NewFrame;

	event VideoSourceErrorEventHandler VideoSourceError;

	event PlayingFinishedEventHandler PlayingFinished;

	string Source { get; }

	int FramesReceived { get; }

	long BytesReceived { get; }

	bool IsRunning { get; }

	void Start();

	void SignalToStop();

	void WaitForStop();

	void Stop();
	}
}
