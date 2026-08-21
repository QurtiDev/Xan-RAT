

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("56A86892-0AD4-11CE-B03A-0020AF0BA770")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IEnumPins
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Next([In] int cPins, [MarshalAs(UnmanagedType.LPArray), Out] IPin[] pins, out int pinsFetched);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Skip([In] int cPins);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Reset();

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Clone(out IEnumPins enumPins);
	}
}
