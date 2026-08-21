

using System;
using System.Windows.Forms;
using InvokedCommon.Models;


namespace InvokedServer.Forms
{
	public partial class FrmRegValueEditMultiString : Form
	{
		private readonly RegValueData _value;

		public FrmRegValueEditMultiString(RegValueData value)
		{
			this._value = value;
			this.InitializeComponent();
			this.valueNameTxtBox.Text = value.Name;
			this.valueDataTxtBox.Text = string.Join("\r\n", InvokedCommon.Utilities.ByteConverter.ToStringArray(value.Data));
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			this._value.Data = InvokedCommon.Utilities.ByteConverter.GetBytes(this.valueDataTxtBox.Text.Split(new string[1]
			{
				"\r\n"
			}, StringSplitOptions.RemoveEmptyEntries));
			this.Tag = (object)this._value;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}