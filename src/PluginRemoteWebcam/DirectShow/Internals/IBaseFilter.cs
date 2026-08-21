

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("56A86895-0AD4-11CE-B03A-0020AF0BA770")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IBaseFilter
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetClassID(out Guid ClassID);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Stop();

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Pause();

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Run(long start);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetState(int milliSecsTimeout, out int filterState);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SetSyncSource([In] IntPtr clock);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetSyncSource(out IntPtr clock);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int EnumPins(out IEnumPins enumPins);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int FindPin([MarshalAs(UnmanagedType.LPWStr), In] string id, out IPin pin);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int QueryFilterInfo(out FilterInfo filterInfo);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int JoinFilterGraph([In] IFilterGraph graph, [MarshalAs(UnmanagedType.LPWStr), In] string name);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int QueryVendorInfo([MarshalAs(UnmanagedType.LPWStr)] out string vendorInfo);
	}
}
