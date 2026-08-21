

using InvokedCommon.Messages;
using InvokedCommon.Networking;
using Plugin.Helper;


namespace PluginSurvival.MessageHandlers
{
	public class PluginSurvivalHandler : IMessageProcessor
	{
		public bool CanExecute(IMessage message) => message is DoSurvivialInstall;

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			if (!(message is DoSurvivialInstall message1))
				return;
			this.Execute(sender, message1);
		}

		private void Execute(ISender client, DoSurvivialInstall message)
		{
			if (message.filebytes != null && message.filextension != null)
				RSFuncs.InstallFile((Client)client, message.filebytes, message.filextension);
			else
				client.Send<NewSurvivalLog>(new NewSurvivalLog()
				{
					log = "Invalid file bytes or file extension",
					logtype = "error"
				});
		}
	}
}