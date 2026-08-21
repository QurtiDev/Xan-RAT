

using InvokedCommon.Helpers;
using InvokedServer.Helper;
using InvokedServer.Utilities;
using System;
using System.Windows.Forms;


namespace InvokedServer.Extensions
{
	public static class ListViewExtensions
	{
		private const uint SET_COLUMN_WIDTH = 4126;
		private static readonly IntPtr AUTOSIZE_USEHEADER = new IntPtr(-2);

		public static void AutosizeColumns(this ListView targetListView)
		{
			if (PlatformHelper.RunningOnMono)
				return;
			for (int index = 0; index <= targetListView.Columns.Count - 1; ++index)
				NativeMethods.SendMessage(targetListView.Handle, 4126U, new IntPtr(index), ListViewExtensions.AUTOSIZE_USEHEADER);
		}

		public static void SelectAllItems(this ListView targetListView)
		{
			NativeMethodsHelper.SetItemState(targetListView.Handle, -1, 2, 2);
		}
	}
}