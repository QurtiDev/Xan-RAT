

using System.Collections;
using System.Runtime.InteropServices;


namespace Plugin.Helper.Browsers
{
	public class SortFileTimeAscendingHelper : IComparer
	{
		int IComparer.Compare(object a, object b)
        {
            STATURL statA = (STATURL)a;
            STATURL statB = (STATURL)b;

            return SortFileTimeAscendingHelper.CompareFileTime(
                ref statA.ftLastVisited,
                ref statB.ftLastVisited
            );
        }

		[DllImport("Kernel32.dll")]
		private static extern int CompareFileTime([In] ref System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime1, [In] ref System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime2);

		public static IComparer SortFileTimeAscending()
		{
			return (IComparer) new SortFileTimeAscendingHelper();
		}
	}
}
