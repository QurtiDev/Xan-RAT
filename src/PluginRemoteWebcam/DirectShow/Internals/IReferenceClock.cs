

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace AForge.Video.DirectShow.Internals
{
	[SuppressUnmanagedCodeSecurity]
	[Guid("56a86897-0ad4-11ce-b03a-0020af0ba770")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IReferenceClock
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetTime(out long pTime);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int AdviseTime([In] long baseTime, [In] long streamTime, [In] IntPtr hEvent, out int pdwAdviseCookie);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int AdvisePeriodic(
			[In] long startTime,
			[In] long periodTime,
			[In] IntPtr hSemaphore,
			out int pdwAdviseCookie);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Unadvise([In] int dwAdviseCookie);
	}
}
