

using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[ComVisible(false)]
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
	internal struct FilterInfo
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Name;
		public IFilterGraph FilterGraph;
	}
}
