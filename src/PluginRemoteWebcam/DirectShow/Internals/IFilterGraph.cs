

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("56A8689F-0AD4-11CE-B03A-0020AF0BA770")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IFilterGraph
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int AddFilter([In] IBaseFilter filter, [MarshalAs(UnmanagedType.LPWStr), In] string name);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int RemoveFilter([In] IBaseFilter filter);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int EnumFilters(out IntPtr enumerator);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int FindFilterByName([MarshalAs(UnmanagedType.LPWStr), In] string name, out IBaseFilter filter);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int ConnectDirect([In] IPin pinOut, [In] IPin pinIn, [MarshalAs(UnmanagedType.LPStruct), In] AMMediaType mediaType);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Reconnect([In] IPin pin);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Disconnect([In] IPin pin);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SetDefaultSyncSource();
	}
}
