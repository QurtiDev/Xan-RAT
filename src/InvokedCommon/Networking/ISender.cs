

using InvokedCommon.Messages;


namespace InvokedCommon.Networking
{
	public interface ISender
	{
		void Send<T>(T message) where T : IMessage;

		void Disconnect();
	}
}