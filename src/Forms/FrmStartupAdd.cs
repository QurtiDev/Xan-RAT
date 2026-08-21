

using System;
using System.IO;
using System.Windows.Forms;
using InvokedCommon.Enums;
using InvokedCommon.Helpers;
using InvokedCommon.Models;


namespace InvokedServer.Forms
{
	public partial class FrmStartupAdd : Form
	{
		public StartupItem StartupItem { get; set; }

		public FrmStartupAdd()
		{
			this.InitializeComponent();
			this.AddTypes();
		}

		public FrmStartupAdd(string startupPath)
		{
			this.InitializeComponent();
			this.AddTypes();
			this.txtName.Text = Path.GetFileNameWithoutExtension(startupPath);
			this.txtPath.Text = startupPath;
		}

		private void AddTypes()
		{
			this.cmbType.Items.Add((object)"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
			this.cmbType.Items.Add((object)"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce");
			this.cmbType.Items.Add((object)"HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
			this.cmbType.Items.Add((object)"HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce");
			this.cmbType.Items.Add((object)"%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\Startup");
			this.cmbType.SelectedIndex = 0;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			this.StartupItem = new StartupItem()
			{
				Name = this.txtName.Text,
				Path = this.txtPath.Text,
				Type = (StartupType)this.cmbType.SelectedIndex
			};
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void txtName_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = (e.KeyChar == '\\' || FileHelper.HasIllegalCharacters(e.KeyChar.ToString())) && !char.IsControl(e.KeyChar);
		}

		private void txtPath_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = (e.KeyChar == '\\' || FileHelper.HasIllegalCharacters(e.KeyChar.ToString())) && !char.IsControl(e.KeyChar);
		}
	}
}