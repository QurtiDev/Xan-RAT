

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("93E5A4E0-2D50-11d2-ABFA-00A0C9C6E38D")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface ICaptureGraphBuilder2
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SetFiltergraph([In] IGraphBuilder graphBuilder);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetFiltergraph(out IGraphBuilder graphBuilder);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SetOutputFileName(
			[MarshalAs(UnmanagedType.LPStruct), In] Guid type,
			[MarshalAs(UnmanagedType.LPWStr), In] string fileName,
			out IBaseFilter baseFilter,
			out IntPtr fileSinkFilter);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int FindInterface(
			[MarshalAs(UnmanagedType.LPStruct), In] Guid category,
			[MarshalAs(UnmanagedType.LPStruct), In] Guid type,
			[In] IBaseFilter baseFilter,
			[MarshalAs(UnmanagedType.LPStruct), In] Guid interfaceID,
			[MarshalAs(UnmanagedType.IUnknown)] out object retInterface);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int RenderStream(
			[MarshalAs(UnmanagedType.LPStruct), In] Guid category,
			[MarshalAs(UnmanagedType.LPStruct), In] Guid mediaType,
			[MarshalAs(UnmanagedType.IUnknown), In] object source,
			[In] IBaseFilter compressor,
			[In] IBaseFilter renderer);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int ControlStream(
			[MarshalAs(UnmanagedType.LPStruct), In] Guid category,
			[MarshalAs(UnmanagedType.LPStruct), In] Guid mediaType,
			[MarshalAs(UnmanagedType.Interface), In] IBaseFilter filter,
			[In] long start,
			[In] long stop,
			[In] short startCookie,
			[In] short stopCookie);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int AllocCapFile([MarshalAs(UnmanagedType.LPWStr), In] string fileName, [In] long size);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int CopyCaptureFile(
			[MarshalAs(UnmanagedType.LPWStr), In] string oldFileName,
			[MarshalAs(UnmanagedType.LPWStr), In] string newFileName,
			[MarshalAs(UnmanagedType.Bool), In] bool allowEscAbort,
			[In] IntPtr callback);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int FindPin(
			[MarshalAs(UnmanagedType.IUnknown), In] object source,
			[In] PinDirection pinDirection,
			[MarshalAs(UnmanagedType.LPStruct), In] Guid category,
			[MarshalAs(UnmanagedType.LPStruct), In] Guid mediaType,
			[MarshalAs(UnmanagedType.Bool), In] bool unconnected,
			[In] int index,
			[MarshalAs(UnmanagedType.Interface)] out IPin pin);
	}
}
