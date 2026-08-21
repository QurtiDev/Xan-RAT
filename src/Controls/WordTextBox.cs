

using InvokedServer.Enums;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;


namespace InvokedServer.Controls
{
	public class WordTextBox : TextBox
	{
		private bool isHexNumber;
		private WordType type;
		private IContainer components;

		public override int MaxLength
		{
			get => base.MaxLength;
			set
			{
			}
		}

		public bool IsHexNumber
		{
			get => this.isHexNumber;
			set
			{
				if (this.isHexNumber == value)
					return;
				if (value)
				{
					if (this.Type == WordType.DWORD)
						this.Text = this.UIntValue.ToString("x");
					else
						this.Text = this.ULongValue.ToString("x");
				}
				else if (this.Type == WordType.DWORD)
					this.Text = this.UIntValue.ToString();
				else
					this.Text = this.ULongValue.ToString();
				this.isHexNumber = value;
				this.UpdateMaxLength();
			}
		}

		public WordType Type
		{
			get => this.type;
			set
			{
				if (this.type == value)
					return;
				this.type = value;
				this.UpdateMaxLength();
			}
		}

		public uint UIntValue
		{
			get
			{
				try
				{
					if (string.IsNullOrEmpty(this.Text))
						return 0;
					return this.IsHexNumber ? uint.Parse(this.Text, NumberStyles.HexNumber) : uint.Parse(this.Text);
				}
				catch
				{
					return uint.MaxValue;
				}
			}
		}

		public ulong ULongValue
		{
			get
			{
				try
				{
					if (string.IsNullOrEmpty(this.Text))
						return 0;
					return this.IsHexNumber ? ulong.Parse(this.Text, NumberStyles.HexNumber) : ulong.Parse(this.Text);
				}
				catch
				{
					return ulong.MaxValue;
				}
			}
		}

		public bool IsConversionValid()
		{
			return string.IsNullOrEmpty(this.Text) || this.IsHexNumber || this.ConvertToHex();
		}

		public WordTextBox()
		{
			this.InitializeComponent();
			base.MaxLength = 8;
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			e.Handled = !this.IsValidChar(e.KeyChar);
		}

		private bool IsValidChar(char ch)
		{
			if (char.IsControl(ch) || char.IsDigit(ch))
				return true;
			return this.IsHexNumber && char.IsLetter(ch) && char.ToLower(ch) <= 'f';
		}

		private void UpdateMaxLength()
		{
			if (this.Type == WordType.DWORD)
			{
				if (this.IsHexNumber)
					base.MaxLength = 8;
				else
					base.MaxLength = 10;
			}
			else if (this.IsHexNumber)
				base.MaxLength = 16;
			else
				base.MaxLength = 20;
		}

		private bool ConvertToHex()
		{
			try
			{
				if (this.Type == WordType.DWORD)
				{
					int num1 = (int)uint.Parse(this.Text);
				}
				else
				{
					long num2 = (long)ulong.Parse(this.Text);
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
				this.components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent() => this.components = (IContainer)new System.ComponentModel.Container();
	}
}