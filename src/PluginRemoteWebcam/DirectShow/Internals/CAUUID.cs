

using System;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[ComVisible(false)]
	internal struct CAUUID
	{
		public int cElems;
		public IntPtr pElems;

		public Guid[] ToGuidArray()
		{
			Guid[] guidArray = new Guid[this.cElems];
			for (int index = 0; index < this.cElems; ++index)
			{
				IntPtr ptr = new IntPtr(this.pElems.ToInt64() + (long) (index * Marshal.SizeOf(typeof (Guid))));
				guidArray[index] = (Guid) Marshal.PtrToStructure(ptr, typeof (Guid));
			}
			return guidArray;
		}
	}
}
