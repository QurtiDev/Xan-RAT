

using System;
using System.Windows.Forms;
using InvokedCommon.Models;
using InvokedServer.Enums;
using Microsoft.Win32;


namespace InvokedServer.Forms
{
	public partial class FrmRegValueEditWord : Form
	{
		private readonly RegValueData _value;
		private const string DWORD_WARNING = "The decimal value entered is greater than the maximum value of a DWORD (32-bit number). Should the value be truncated in order to continue?";
		private const string QWORD_WARNING = "The decimal value entered is greater than the maximum value of a QWORD (64-bit number). Should the value be truncated in order to continue?";

		public FrmRegValueEditWord(RegValueData value)
		{
			this._value = value;
			this.InitializeComponent();
			this.valueNameTxtBox.Text = value.Name;
			if (value.Kind == RegistryValueKind.DWord)
			{
				this.Text = "Edit DWORD (32-bit) Value";
				this.valueDataTxtBox.Type = WordType.DWORD;
				this.valueDataTxtBox.Text = InvokedCommon.Utilities.ByteConverter.ToUInt32(value.Data).ToString("x");
			}
			else
			{
				this.Text = "Edit QWORD (64-bit) Value";
				this.valueDataTxtBox.Type = WordType.QWORD;
				this.valueDataTxtBox.Text = InvokedCommon.Utilities.ByteConverter.ToUInt64(value.Data).ToString("x");
			}
		}

		private void radioHex_CheckboxChanged(object sender, EventArgs e)
		{
			if (this.valueDataTxtBox.IsHexNumber == this.radioHexa.Checked)
				return;
			if (this.valueDataTxtBox.IsConversionValid() || this.IsOverridePossible())
				this.valueDataTxtBox.IsHexNumber = this.radioHexa.Checked;
			else
				this.radioDecimal.Checked = true;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (this.valueDataTxtBox.IsConversionValid() || this.IsOverridePossible())
			{
				this._value.Data = this._value.Kind == RegistryValueKind.DWord ? InvokedCommon.Utilities.ByteConverter.GetBytes(this.valueDataTxtBox.UIntValue) : InvokedCommon.Utilities.ByteConverter.GetBytes(this.valueDataTxtBox.ULongValue);
				this.Tag = (object)this._value;
				this.DialogResult = DialogResult.OK;
			}
			else
				this.DialogResult = DialogResult.None;
			this.Close();
		}

		private DialogResult ShowWarning(string msg, string caption)
		{
			return MessageBox.Show(msg, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
		}

		private bool IsOverridePossible()
		{
			return this.ShowWarning(this._value.Kind == RegistryValueKind.DWord ? "The decimal value entered is greater than the maximum value of a DWORD (32-bit number). Should the value be truncated in order to continue?" : "The decimal value entered is greater than the maximum value of a QWORD (64-bit number). Should the value be truncated in order to continue?", "Overflow") == DialogResult.Yes;
		}
	}
}