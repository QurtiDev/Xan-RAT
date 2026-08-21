

using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[ComVisible(false)]
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
	internal struct PinInfo
	{
		public IBaseFilter Filter;
		public PinDirection Direction;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Name;
	}
}
