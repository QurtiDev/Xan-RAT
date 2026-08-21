

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("6B652FFF-11FE-4FCE-92AD-0266B5D7C78F")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface ISampleGrabber
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SetOneShot([MarshalAs(UnmanagedType.Bool), In] bool oneShot);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SetMediaType([MarshalAs(UnmanagedType.LPStruct), In] AMMediaType mediaType);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetConnectedMediaType([MarshalAs(UnmanagedType.LPStruct), Out] AMMediaType mediaType);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SetBufferSamples([MarshalAs(UnmanagedType.Bool), In] bool bufferThem);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetCurrentBuffer(ref int bufferSize, IntPtr buffer);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetCurrentSample(IntPtr sample);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SetCallback(ISampleGrabberCB callback, int whichMethodToCallback);
	}
}
