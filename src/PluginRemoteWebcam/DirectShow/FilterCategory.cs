

using System;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow
{
	[ComVisible(false)]
	public static class FilterCategory
	{
		public static readonly Guid AudioInputDevice = new Guid(869902178U, (ushort) 37064, (ushort) 4560, (byte) 189, (byte) 67, (byte) 0, (byte) 160, (byte) 201, (byte) 17, (byte) 206, (byte) 134);
		public static readonly Guid VideoInputDevice = new Guid(2248913680U, (ushort) 23809, (ushort) 4560, (byte) 189, (byte) 59, (byte) 0, (byte) 160, (byte) 201, (byte) 17, (byte) 206, (byte) 134);
		public static readonly Guid VideoCompressorCategory = new Guid(869902176U, (ushort) 37064, (ushort) 4560, (byte) 189, (byte) 67, (byte) 0, (byte) 160, (byte) 201, (byte) 17, (byte) 206, (byte) 134);
		public static readonly Guid AudioCompressorCategory = new Guid(869902177U, (ushort) 37064, (ushort) 4560, (byte) 189, (byte) 67, (byte) 0, (byte) 160, (byte) 201, (byte) 17, (byte) 206, (byte) 134);
	}
}
