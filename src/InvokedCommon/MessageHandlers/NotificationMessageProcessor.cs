

using InvokedCommon.Messages;


namespace InvokedCommon.MessageHandlers
{
	public abstract class NotificationMessageProcessor : MessageProcessorBase<string>
	{
		protected NotificationMessageProcessor()
			: base(true)
		{
		}
	}
}
