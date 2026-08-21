

using InvokedServer.Models;
using System.Collections;
using System.Windows.Forms;


namespace InvokedServer.Utilities
{
	public class ListViewColumnSorter : IComparer
	{
		private int _columnToSort;
		private SortOrder _orderOfSort;
		private readonly CaseInsensitiveComparer _objectCompare;
		private bool _needNumberCompare;

		public ListViewColumnSorter()
		{
			this._columnToSort = 0;
			this._orderOfSort = SortOrder.None;
			this._objectCompare = new CaseInsensitiveComparer();
			this._needNumberCompare = false;
		}

		public int Compare(object x, object y)
		{
			ListViewItem listViewItem1 = (ListViewItem)x;
			ListViewItem listViewItem2 = (ListViewItem)y;
			if (listViewItem1.SubItems[0].Text == ".." || listViewItem2.SubItems[0].Text == "..")
				return 0;
			int num;
			if (this._needNumberCompare)
			{
				if (listViewItem1.Tag is FileManagerListTag)
				{
					long fileSize1 = (listViewItem1.Tag as FileManagerListTag).FileSize;
					long fileSize2 = (listViewItem2.Tag as FileManagerListTag).FileSize;
					num = fileSize1 >= fileSize2 ? (fileSize1 == fileSize2 ? 0 : 1) : -1;
				}
				else
				{
					long result1;
					long result2;
					num = !long.TryParse(listViewItem1.SubItems[this._columnToSort].Text, out result1) || !long.TryParse(listViewItem2.SubItems[this._columnToSort].Text, out result2) ? this._objectCompare.Compare((object)listViewItem1.SubItems[this._columnToSort].Text, (object)listViewItem2.SubItems[this._columnToSort].Text) : (result1 >= result2 ? (result1 == result2 ? 0 : 1) : -1);
				}
			}
			else
				num = this._objectCompare.Compare((object)listViewItem1.SubItems[this._columnToSort].Text, (object)listViewItem2.SubItems[this._columnToSort].Text);
			if (this._orderOfSort == SortOrder.Ascending)
				return num;
			return this._orderOfSort == SortOrder.Descending ? -num : 0;
		}

		public int SortColumn
		{
			set => this._columnToSort = value;
			get => this._columnToSort;
		}

		public SortOrder Order
		{
			set => this._orderOfSort = value;
			get => this._orderOfSort;
		}

		public bool NeedNumberCompare
		{
			set => this._needNumberCompare = value;
			get => this._needNumberCompare;
		}
	}
}