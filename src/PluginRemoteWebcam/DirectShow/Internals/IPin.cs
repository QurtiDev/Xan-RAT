

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("56A86891-0AD4-11CE-B03A-0020AF0BA770")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IPin
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Connect([In] IPin receivePin, [MarshalAs(UnmanagedType.LPStruct), In] AMMediaType mediaType);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int ReceiveConnection([In] IPin receivePin, [MarshalAs(UnmanagedType.LPStruct), In] AMMediaType mediaType);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Disconnect();

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int ConnectedTo(out IPin pin);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int ConnectionMediaType([MarshalAs(UnmanagedType.LPStruct), Out] AMMediaType mediaType);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int QueryPinInfo(out PinInfo pinInfo);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int QueryDirection(out PinDirection pinDirection);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int QueryId([MarshalAs(UnmanagedType.LPWStr)] out string id);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int QueryAccept([MarshalAs(UnmanagedType.LPStruct), In] AMMediaType mediaType);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int EnumMediaTypes(IntPtr enumerator);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int QueryInternalConnections(IntPtr apPin, [In, Out] ref int nPin);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int EndOfStream();

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int BeginFlush();

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int EndFlush();

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int NewSegment(long start, long stop, double rate);
	}
}
