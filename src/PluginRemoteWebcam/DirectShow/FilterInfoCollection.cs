

using AForge.Video.DirectShow.Internals;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;


namespace AForge.Video.DirectShow
{
	public class FilterInfoCollection : CollectionBase
	{
		public FilterInfoCollection(Guid category) => this.CollectFilters(category);

		public FilterInfo this[int index] => (FilterInfo) this.InnerList[index];

		private void CollectFilters(Guid category)
		{
			object o = (object) null;
			IEnumMoniker enumMoniker = (IEnumMoniker) null;
			IMoniker[] rgelt = new IMoniker[1];
			try
			{
				Type typeFromClsid = Type.GetTypeFromCLSID(Clsid.SystemDeviceEnum);
				o = !(typeFromClsid == (Type) null) ? Activator.CreateInstance(typeFromClsid) : throw new ApplicationException("Failed creating device enumerator");
				if (((ICreateDevEnum) o).CreateClassEnumerator(ref category, out enumMoniker, 0) != 0)
					throw new ApplicationException("No devices of the category");
				IntPtr zero = IntPtr.Zero;
				while (enumMoniker.Next(1, rgelt, zero) == 0 && rgelt[0] != null)
				{
					this.InnerList.Add((object) new FilterInfo(rgelt[0]));
					Marshal.ReleaseComObject((object) rgelt[0]);
					rgelt[0] = (IMoniker) null;
				}
				this.InnerList.Sort();
			}
			catch
			{
			}
			finally
			{
				if (o != null)
					Marshal.ReleaseComObject(o);
				if (enumMoniker != null)
					Marshal.ReleaseComObject((object) enumMoniker);
				if (rgelt[0] != null)
				{
					Marshal.ReleaseComObject((object) rgelt[0]);
					rgelt[0] = (IMoniker) null;
				}
			}
		}
	}
}
