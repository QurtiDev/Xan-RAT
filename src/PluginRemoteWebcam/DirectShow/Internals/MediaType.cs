

using System;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[ComVisible(false)]
	internal static class MediaType
	{
		public static readonly Guid Video = new Guid(1935960438, (short) 0, (short) 16, (byte) 128, (byte) 0, (byte) 0, (byte) 170, (byte) 0, (byte) 56, (byte) 155, (byte) 113);
		public static readonly Guid Interleaved = new Guid(1937138025, (short) 0, (short) 16, (byte) 128, (byte) 0, (byte) 0, (byte) 170, (byte) 0, (byte) 56, (byte) 155, (byte) 113);
		public static readonly Guid Audio = new Guid(1935963489, (short) 0, (short) 16, (byte) 128, (byte) 0, (byte) 0, (byte) 170, (byte) 0, (byte) 56, (byte) 155, (byte) 113);
		public static readonly Guid Text = new Guid(1937012852, (short) 0, (short) 16, (byte) 128, (byte) 0, (byte) 0, (byte) 170, (byte) 0, (byte) 56, (byte) 155, (byte) 113);
		public static readonly Guid Stream = new Guid(3828804483U, (ushort) 21071, (ushort) 4558, (byte) 159, (byte) 83, (byte) 0, (byte) 32, (byte) 175, (byte) 11, (byte) 167, (byte) 112);
	}
}
