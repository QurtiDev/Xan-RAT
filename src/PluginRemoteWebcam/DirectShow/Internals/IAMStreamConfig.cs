

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("C6E13340-30AC-11d0-A18C-00A0C9118956")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IAMStreamConfig
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SetFormat([MarshalAs(UnmanagedType.LPStruct), In] AMMediaType mediaType);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetFormat([MarshalAs(UnmanagedType.LPStruct)] out AMMediaType mediaType);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetNumberOfCapabilities(out int count, out int size);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetStreamCaps([In] int index, [MarshalAs(UnmanagedType.LPStruct)] out AMMediaType mediaType, [MarshalAs(UnmanagedType.LPStruct), In] VideoStreamConfigCaps streamConfigCaps);
	}
}
