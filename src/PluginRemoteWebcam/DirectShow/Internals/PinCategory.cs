

using System;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[ComVisible(false)]
	internal static class PinCategory
	{
		public static readonly Guid Capture = new Guid(4218176129U, (ushort) 851, (ushort) 4561, (byte) 144, (byte) 95, (byte) 0, (byte) 0, (byte) 192, (byte) 204, (byte) 22, (byte) 186);
		public static readonly Guid StillImage = new Guid(4218176138U, (ushort) 851, (ushort) 4561, (byte) 144, (byte) 95, (byte) 0, (byte) 0, (byte) 192, (byte) 204, (byte) 22, (byte) 186);
	}
}
