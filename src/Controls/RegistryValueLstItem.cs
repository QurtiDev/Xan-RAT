

using InvokedCommon.Models;
using InvokedServer.Extensions;
using InvokedServer.Registry;
using System.Windows.Forms;


namespace InvokedServer.Controls
{
	public class RegistryValueLstItem : ListViewItem
	{
		private string _type { get; set; }

		private string _data { get; set; }

		public string RegName
		{
			get => this.Name;
			set
			{
				this.Name = value;
				this.Text = RegValueHelper.GetName(value);
			}
		}

		public string Type
		{
			get => this._type;
			set
			{
				this._type = value;
				if (this.SubItems.Count < 2)
					this.SubItems.Add(this._type);
				else
					this.SubItems[1].Text = this._type;
				this.ImageIndex = this.GetRegistryValueImgIndex(this._type);
			}
		}

		public string Data
		{
			get => this._data;
			set
			{
				this._data = value;
				if (this.SubItems.Count < 3)
					this.SubItems.Add(this._data);
				else
					this.SubItems[2].Text = this._data;
			}
		}

		public RegistryValueLstItem(RegValueData value)
		{
			this.RegName = value.Name;
			this.Type = value.Kind.RegistryTypeToString();
			this.Data = RegValueHelper.RegistryValueToString(value);
		}

		private int GetRegistryValueImgIndex(string type)
		{
			switch (type)
			{
				case "REG_MULTI_SZ":
				case "REG_SZ":
				case "REG_EXPAND_SZ":
					return 0;
				default:
					return 1;
			}
		}
	}
}