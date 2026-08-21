

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[ComVisible(true)]
	[Guid("56a868c0-0ad4-11ce-b03a-0020af0ba770")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[ComImport]
	internal interface IMediaEventEx
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetEventHandle(out IntPtr hEvent);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetEvent([MarshalAs(UnmanagedType.I4)] out DsEvCode lEventCode, out IntPtr lParam1, out IntPtr lParam2, int msTimeout);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int WaitForCompletion(int msTimeout, out int pEvCode);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int CancelDefaultHandling(int lEvCode);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int RestoreDefaultHandling(int lEvCode);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int FreeEventParams([MarshalAs(UnmanagedType.I4), In] DsEvCode lEvCode, IntPtr lParam1, IntPtr lParam2);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SetNotifyWindow(IntPtr hwnd, int lMsg, IntPtr lInstanceData);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SetNotifyFlags(int lNoNotifyFlags);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetNotifyFlags(out int lplNoNotifyFlags);
	}
}
