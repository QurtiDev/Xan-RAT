

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("0579154A-2B53-4994-B0D0-E773148EFF85")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface ISampleGrabberCB
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int SampleCB(double sampleTime, IntPtr sample);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int BufferCB(double sampleTime, IntPtr buffer, int bufferLen);
	}
}
