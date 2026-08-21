

using System;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[ComVisible(false)]
	internal static class Clsid
	{
		public static readonly Guid SystemDeviceEnum = new Guid(1656642832, (short) 24811, (short) 4560, (byte) 189, (byte) 59, (byte) 0, (byte) 160, (byte) 201, (byte) 17, (byte) 206, (byte) 134);
		public static readonly Guid FilterGraph = new Guid(3828804531U, (ushort) 21071, (ushort) 4558, (byte) 159, (byte) 83, (byte) 0, (byte) 32, (byte) 175, (byte) 11, (byte) 167, (byte) 112);
		public static readonly Guid SampleGrabber = new Guid(3253993632U, (ushort) 16136, (ushort) 4563, (byte) 159, (byte) 11, (byte) 0, (byte) 96, (byte) 8, (byte) 3, (byte) 158, (byte) 55);
		public static readonly Guid CaptureGraphBuilder2 = new Guid(3213342433U, (ushort) 35879, (ushort) 4560, (byte) 179, (byte) 240, (byte) 0, (byte) 170, (byte) 0, (byte) 55, (byte) 97, (byte) 197);
		public static readonly Guid AsyncReader = new Guid(3828804533U, (ushort) 21071, (ushort) 4558, (byte) 159, (byte) 83, (byte) 0, (byte) 32, (byte) 175, (byte) 11, (byte) 167, (byte) 112);
	}
}
