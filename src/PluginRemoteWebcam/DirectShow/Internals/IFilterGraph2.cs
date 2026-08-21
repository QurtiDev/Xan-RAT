

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("36B73882-C2C8-11CF-8B46-00805F6CEF60")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IFilterGraph2
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int AddFilter([In] IBaseFilter filter, [MarshalAs(UnmanagedType.LPWStr), In] string name);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int RemoveFilter([In] IBaseFilter filter);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int EnumFilters(out IEnumFilters enumerator);

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

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Connect([In] IPin pinOut, [In] IPin pinIn);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Render([In] IPin pinOut);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int RenderFile([MarshalAs(UnmanagedType.LPWStr), In] string file, [MarshalAs(UnmanagedType.LPWStr), In] string playList);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int AddSourceFilter([MarshalAs(UnmanagedType.LPWStr), In] string fileName, [MarshalAs(UnmanagedType.LPWStr), In] string filterName, out IBaseFilter filter);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SetLogFile(IntPtr hFile);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Abort();

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int ShouldOperationContinue();

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int AddSourceFilterForMoniker(
			[In] IMoniker moniker,
			[In] IBindCtx bindContext,
			[MarshalAs(UnmanagedType.LPWStr), In] string filterName,
			out IBaseFilter filter);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int ReconnectEx([In] IPin pin, [MarshalAs(UnmanagedType.LPStruct), In] AMMediaType mediaType);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int RenderEx([In] IPin outputPin, [In] int flags, [In] IntPtr context);
	}
}
