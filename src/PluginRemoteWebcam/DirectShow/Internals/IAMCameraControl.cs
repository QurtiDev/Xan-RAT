

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("C6E13370-30AC-11d0-A18C-00A0C9118956")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IAMCameraControl
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetRange(
			[In] CameraControlProperty Property,
			out int pMin,
			out int pMax,
			out int pSteppingDelta,
			out int pDefault,
			out CameraControlFlags pCapsFlags);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Set([In] CameraControlProperty Property, [In] int lValue, [In] CameraControlFlags Flags);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Get([In] CameraControlProperty Property, out int lValue, out CameraControlFlags Flags);
	}
}
