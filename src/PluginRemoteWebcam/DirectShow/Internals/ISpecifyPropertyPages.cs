

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("B196B28B-BAB4-101A-B69C-00AA00341D07")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface ISpecifyPropertyPages
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetPages(out CAUUID pPages);
	}
}
