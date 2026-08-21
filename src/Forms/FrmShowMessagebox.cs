

using System;
using System.Windows.Forms;
using InvokedServer.Helper;


namespace InvokedServer.Forms
{
	public partial class FrmShowMessagebox : Form
	{
		private readonly int _selectedClients;

		public string MsgBoxCaption { get; set; }

		public string MsgBoxText { get; set; }

		public string MsgBoxButton { get; set; }

		public string MsgBoxIcon { get; set; }

		public FrmShowMessagebox(int selected)
		{
			this._selectedClients = selected;
			this.InitializeComponent();
		}

		private void FrmShowMessagebox_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle("Show Messagebox", this._selectedClients);
			this.cmbMsgButtons.Items.AddRange((object[])new string[6]
			{
		"AbortRetryIgnore",
		"OK",
		"OKCancel",
		"RetryCancel",
		"YesNo",
		"YesNoCancel"
			});
			this.cmbMsgButtons.SelectedIndex = 0;
			this.cmbMsgIcon.Items.AddRange((object[])new string[8]
			{
		"None",
		"Error",
		"Hand",
		"Question",
		"Exclamation",
		"Warning",
		"Information",
		"Asterisk"
			});
			this.cmbMsgIcon.SelectedIndex = 0;
		}

		private void btnPreview_Click(object sender, EventArgs e)
		{
			int num = (int)MessageBox.Show((IWin32Window)null, this.txtText.Text, this.txtCaption.Text, (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), this.GetMessageBoxButton(this.cmbMsgButtons.SelectedIndex)), (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), this.GetMessageBoxIcon(this.cmbMsgIcon.SelectedIndex)));
		}

		private void btnSend_Click(object sender, EventArgs e)
		{
			this.MsgBoxCaption = this.txtCaption.Text;
			this.MsgBoxText = this.txtText.Text;
			this.MsgBoxButton = this.GetMessageBoxButton(this.cmbMsgButtons.SelectedIndex);
			this.MsgBoxIcon = this.GetMessageBoxIcon(this.cmbMsgIcon.SelectedIndex);
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private string GetMessageBoxButton(int selectedIndex)
		{
			switch (selectedIndex)
			{
				case 0:
					return "AbortRetryIgnore";
				case 1:
					return "OK";
				case 2:
					return "OKCancel";
				case 3:
					return "RetryCancel";
				case 4:
					return "YesNo";
				case 5:
					return "YesNoCancel";
				default:
					return "OK";
			}
		}

		private string GetMessageBoxIcon(int selectedIndex)
		{
			switch (selectedIndex)
			{
				case 0:
					return "None";
				case 1:
					return "Error";
				case 2:
					return "Hand";
				case 3:
					return "Question";
				case 4:
					return "Exclamation";
				case 5:
					return "Warning";
				case 6:
					return "Information";
				case 7:
					return "Asterisk";
				default:
					return "None";
			}
		}
	}
}