

using InvokedCommon.Messages;
using PluginSurvival.MessageHandlers;
using System;


namespace Plugin
{
	public class Plugin
	{
		public void ExecuteCallback(Action<IMessageProcessor, string> callback, string PluginName)
		{
			try
			{
				IMessageProcessor messageProcessor = (IMessageProcessor)new PluginSurvivalHandler();
				callback(messageProcessor, PluginName);
			}
			catch
			{
			}
		}
	}
}