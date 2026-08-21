

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("55272A00-42CB-11CE-8135-00AA004BB851")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IPropertyBag
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Read([MarshalAs(UnmanagedType.LPWStr), In] string propertyName, [MarshalAs(UnmanagedType.Struct), In, Out] ref object pVar, [In] IntPtr pErrorLog);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Write([MarshalAs(UnmanagedType.LPWStr), In] string propertyName, [MarshalAs(UnmanagedType.Struct), In] ref object pVar);
	}
}
