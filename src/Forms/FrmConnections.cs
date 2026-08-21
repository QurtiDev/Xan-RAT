

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;


namespace InvokedServer.Forms
{
	public partial class FrmConnections : Form
	{
		private readonly Client _connectClient;
		private readonly TcpConnectionsHandler _connectionsHandler;
		private readonly Dictionary<string, ListViewGroup> _groups = new Dictionary<string, ListViewGroup>();
		private static readonly Dictionary<Client, FrmConnections> OpenedForms = new Dictionary<Client, FrmConnections>();

		public static FrmConnections CreateNewOrGetExisting(Client client)
		{
			if (FrmConnections.OpenedForms.ContainsKey(client))
				return FrmConnections.OpenedForms[client];
			FrmConnections newOrGetExisting = new FrmConnections(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmConnections.OpenedForms.Remove(client));
			FrmConnections.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmConnections(Client client)
		{
			this._connectClient = client;
			this._connectionsHandler = new TcpConnectionsHandler(client);
			this.RegisterMessageHandler();
			this.InitializeComponent();
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this._connectionsHandler.ProgressChanged += new MessageProcessorBase<TcpConnection[]>.ReportProgressEventHandler(this.TcpConnectionsChanged);
			MessageHandler.Register((IMessageProcessor)this._connectionsHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._connectionsHandler);
			this._connectionsHandler.ProgressChanged -= new MessageProcessorBase<TcpConnection[]>.ReportProgressEventHandler(this.TcpConnectionsChanged);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Invoke(new MethodInvoker(((Form)this).Close));
		}

		private void TcpConnectionsChanged(object sender, TcpConnection[] connections)
		{
			this.lstConnections.Items.Clear();
			foreach (TcpConnection connection in connections)
			{
				string str = connection.State.ToString();
				ListViewItem listViewItem = new ListViewItem(new string[6]
				{
					connection.ProcessName,
					connection.LocalAddress,
					connection.LocalPort.ToString(),
					connection.RemoteAddress,
					connection.RemotePort.ToString(),
					str
				});
				if (!this._groups.ContainsKey(str))
				{
					ListViewGroup group = new ListViewGroup(str, str);
					this.lstConnections.Groups.Add(group);
					this._groups.Add(str, group);
				}
				listViewItem.Group = this.lstConnections.Groups[str];
				this.lstConnections.Items.Add(listViewItem);
			}
		}

		private void FrmConnections_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle("Connections", this._connectClient);
			this._connectionsHandler.RefreshTcpConnections();
		}

		private void FrmConnections_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.UnregisterMessageHandler();
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._connectionsHandler.RefreshTcpConnections();
		}

		private void closeConnectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flag = false;
			foreach (ListViewItem selectedItem in this.lstConnections.SelectedItems)
			{
				this._connectionsHandler.CloseTcpConnection(selectedItem.SubItems[1].Text, ushort.Parse(selectedItem.SubItems[2].Text), selectedItem.SubItems[3].Text, ushort.Parse(selectedItem.SubItems[4].Text));
				flag = true;
			}
			if (!flag)
				return;
			this._connectionsHandler.RefreshTcpConnections();
		}

		private void lstConnections_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			this.lstConnections.LvwColumnSorter.NeedNumberCompare = e.Column == 2 || e.Column == 4;
		}
	}
}