

using InvokedCommon.Networking;


namespace InvokedCommon.Messages
{
	public interface IMessageProcessor
	{
		bool CanExecute(IMessage message);

		bool CanExecuteFrom(ISender sender);

		void Execute(ISender sender, IMessage message);
	}
}
