

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;


namespace InvokedServer.Forms
{
	public partial class FrmStartupManager : Form
	{
		private readonly Client _connectClient;
		private readonly StartupManagerHandler _startupManagerHandler;
		private static readonly Dictionary<Client, FrmStartupManager> OpenedForms = new Dictionary<Client, FrmStartupManager>();

		public static FrmStartupManager CreateNewOrGetExisting(Client client)
		{
			if (FrmStartupManager.OpenedForms.ContainsKey(client))
				return FrmStartupManager.OpenedForms[client];
			FrmStartupManager newOrGetExisting = new FrmStartupManager(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmStartupManager.OpenedForms.Remove(client));
			FrmStartupManager.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmStartupManager(Client client)
		{
			this._connectClient = client;
			this._startupManagerHandler = new StartupManagerHandler(client);
			this.RegisterMessageHandler();
			this.InitializeComponent();
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this._startupManagerHandler.ProgressChanged += new MessageProcessorBase<List<StartupItem>>.ReportProgressEventHandler(this.StartupItemsChanged);
			MessageHandler.Register((IMessageProcessor)this._startupManagerHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._startupManagerHandler);
			this._startupManagerHandler.ProgressChanged -= new MessageProcessorBase<List<StartupItem>>.ReportProgressEventHandler(this.StartupItemsChanged);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Invoke(new MethodInvoker(((Form)this).Close));
		}

		private void FrmStartupManager_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle("Startup Manager", this._connectClient);
			this.AddGroups();
			this._startupManagerHandler.RefreshStartupItems();
		}

		private void FrmStartupManager_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.UnregisterMessageHandler();
		}

		private void AddGroups()
		{
			this.lstStartupItems.Groups.Add(new ListViewGroup("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run")
			{
				Tag = (object)StartupType.LocalMachineRun
			});
			this.lstStartupItems.Groups.Add(new ListViewGroup("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce")
			{
				Tag = (object)StartupType.LocalMachineRunOnce
			});
			this.lstStartupItems.Groups.Add(new ListViewGroup("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run")
			{
				Tag = (object)StartupType.CurrentUserRun
			});
			this.lstStartupItems.Groups.Add(new ListViewGroup("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce")
			{
				Tag = (object)StartupType.CurrentUserRunOnce
			});
			this.lstStartupItems.Groups.Add(new ListViewGroup("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Run")
			{
				Tag = (object)StartupType.LocalMachineRunX86
			});
			this.lstStartupItems.Groups.Add(new ListViewGroup("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce")
			{
				Tag = (object)StartupType.LocalMachineRunOnceX86
			});
			this.lstStartupItems.Groups.Add(new ListViewGroup("%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")
			{
				Tag = (object)StartupType.StartMenu
			});
		}

		private void StartupItemsChanged(object sender, List<StartupItem> startupItems)
		{
			this.lstStartupItems.Items.Clear();
			foreach (StartupItem startupItem in startupItems)
			{
				StartupItem item = startupItem;
				ListViewGroup listViewGroup = this.lstStartupItems.Groups.Cast<ListViewGroup>().First<ListViewGroup>((Func<ListViewGroup, bool>)(x => (StartupType)x.Tag == item.Type));
				this.lstStartupItems.Items.Add(new ListViewItem(new string[2]
				{
		  item.Name,
		  item.Path
				})
				{
					Group = listViewGroup,
					Tag = (object)item
				});
			}
		}

		private void addEntryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FrmStartupAdd frmStartupAdd = new FrmStartupAdd())
			{
				if (frmStartupAdd.ShowDialog() != DialogResult.OK)
					return;
				this._startupManagerHandler.AddStartupItem(frmStartupAdd.StartupItem);
				this._startupManagerHandler.RefreshStartupItems();
			}
		}

		private void removeEntryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flag = false;
			foreach (ListViewItem selectedItem in this.lstStartupItems.SelectedItems)
			{
				this._startupManagerHandler.RemoveStartupItem((StartupItem)selectedItem.Tag);
				flag = true;
			}
			if (!flag)
				return;
			this._startupManagerHandler.RefreshStartupItems();
		}
	}
}