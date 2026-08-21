

using InvokedClient.Utilities;
using System.Diagnostics;
using System.Text;


namespace InvokedClient.Extensions
{
	public static class ProcessExtensions
	{
		public static string GetMainModuleFileName(this Process proc)
		{
			uint lpdwSize = 260;
			StringBuilder lpExeName = new StringBuilder((int)lpdwSize);
			return !NativeMethods.QueryFullProcessImageName(proc.Handle, 0U, lpExeName, ref lpdwSize) ? (string)null : lpExeName.ToString();
		}
	}
}