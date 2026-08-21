

using System;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[ComVisible(false)]
	[StructLayout(LayoutKind.Sequential)]
	internal class AMMediaType : IDisposable
	{
		public Guid MajorType;
		public Guid SubType;
		[MarshalAs(UnmanagedType.Bool)]
		public bool FixedSizeSamples = true;
		[MarshalAs(UnmanagedType.Bool)]
		public bool TemporalCompression;
		public int SampleSize = 1;
		public Guid FormatType;
		public IntPtr unkPtr;
		public int FormatSize;
		public IntPtr FormatPtr;

		~AMMediaType() => this.Dispose(false);

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object) this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.FormatSize != 0 && this.FormatPtr != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(this.FormatPtr);
				this.FormatSize = 0;
			}
			if (!(this.unkPtr != IntPtr.Zero))
				return;
			Marshal.Release(this.unkPtr);
			this.unkPtr = IntPtr.Zero;
		}
	}
}
