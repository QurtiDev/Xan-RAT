

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("56A86893-0AD4-11CE-B03A-0020AF0BA770")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IEnumFilters
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Next([In] int cFilters, [MarshalAs(UnmanagedType.LPArray), Out] IBaseFilter[] filters, out int filtersFetched);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Skip([In] int cFilters);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Reset();

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Clone(out IEnumFilters enumFilters);
	}
}
