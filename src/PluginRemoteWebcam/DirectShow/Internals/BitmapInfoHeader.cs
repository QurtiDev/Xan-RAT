

using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[ComVisible(false)]
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct BitmapInfoHeader
	{
		public int Size;
		public int Width;
		public int Height;
		public short Planes;
		public short BitCount;
		public int Compression;
		public int ImageSize;
		public int XPelsPerMeter;
		public int YPelsPerMeter;
		public int ColorsUsed;
		public int ColorsImportant;
	}
}
