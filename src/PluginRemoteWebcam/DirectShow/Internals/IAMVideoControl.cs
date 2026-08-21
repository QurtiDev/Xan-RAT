

using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("6A2E0670-28E4-11D0-A18c-00A0C9118956")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IAMVideoControl
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetCaps([In] IPin pin, [MarshalAs(UnmanagedType.I4)] out VideoControlFlags flags);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SetMode([In] IPin pin, [MarshalAs(UnmanagedType.I4), In] VideoControlFlags mode);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetMode([In] IPin pin, [MarshalAs(UnmanagedType.I4)] out VideoControlFlags mode);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetCurrentActualFrameRate([In] IPin pin, [MarshalAs(UnmanagedType.I8)] out long actualFrameRate);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetMaxAvailableFrameRate(
			[In] IPin pin,
			[In] int index,
			[In] Size dimensions,
			out long maxAvailableFrameRate);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetFrameRateList(
			[In] IPin pin,
			[In] int index,
			[In] Size dimensions,
			out int listSize,
			out IntPtr frameRate);
	}
}
