

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Guid("56A868B1-0AD4-11CE-B03A-0020AF0BA770")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[ComImport]
	internal interface IMediaControl
	{
		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Run();

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Pause();

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int Stop();

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int GetState(int timeout, out int filterState);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int RenderFile(string fileName);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int AddSourceFilter([In] string fileName, [MarshalAs(UnmanagedType.IDispatch)] out object filterInfo);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int get_FilterCollection([MarshalAs(UnmanagedType.IDispatch)] out object collection);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int get_RegFilterCollection([MarshalAs(UnmanagedType.IDispatch)] out object collection);

		[MethodImpl(MethodImplOptions.PreserveSig)]
		int StopWhenReady();
	}
}
