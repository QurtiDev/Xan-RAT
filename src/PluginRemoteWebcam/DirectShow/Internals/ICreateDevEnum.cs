

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("29840822-5B84-11D0-BD3B-00A0C911CE86")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface ICreateDevEnum
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int CreateClassEnumerator([In] ref Guid type, out IEnumMoniker enumMoniker, [In] int flags);
	}
}
