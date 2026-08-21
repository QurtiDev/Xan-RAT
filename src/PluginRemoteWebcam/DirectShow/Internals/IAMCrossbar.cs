

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace AForge.Video.DirectShow.Internals
{
	[SuppressUnmanagedCodeSecurity]
	[Guid("C6E13380-30AC-11D0-A18C-00A0C9118956")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IAMCrossbar
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int get_PinCounts(out int outputPinCount, out int inputPinCount);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int CanRoute([In] int outputPinIndex, [In] int inputPinIndex);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Route([In] int outputPinIndex, [In] int inputPinIndex);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int get_IsRoutedTo([In] int outputPinIndex, out int inputPinIndex);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int get_CrossbarPinInfo(
			[MarshalAs(UnmanagedType.Bool), In] bool isInputPin,
			[In] int pinIndex,
			out int pinIndexRelated,
			out PhysicalConnectorType physicalType);
	}
}
