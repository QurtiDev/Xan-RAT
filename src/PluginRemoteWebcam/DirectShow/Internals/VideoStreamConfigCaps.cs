

using System;
using System.Drawing;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[ComVisible(false)]
	[StructLayout(LayoutKind.Sequential)]
	internal class VideoStreamConfigCaps
	{
		public Guid Guid;
		public AnalogVideoStandard VideoStandard;
		public Size InputSize;
		public Size MinCroppingSize;
		public Size MaxCroppingSize;
		public int CropGranularityX;
		public int CropGranularityY;
		public int CropAlignX;
		public int CropAlignY;
		public Size MinOutputSize;
		public Size MaxOutputSize;
		public int OutputGranularityX;
		public int OutputGranularityY;
		public int StretchTapsX;
		public int StretchTapsY;
		public int ShrinkTapsX;
		public int ShrinkTapsY;
		public long MinFrameInterval;
		public long MaxFrameInterval;
		public int MinBitsPerSecond;
		public int MaxBitsPerSecond;
	}
}
