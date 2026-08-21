

using System.Runtime.InteropServices;


namespace Plugin.Helper.Browsers
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("3C374A42-BAE4-11CF-BF7D-00AA006946EE")]
	[ComImport]
	public interface IEnumSTATURL
	{
		void Next(int celt, ref STATURL rgelt, out int pceltFetched);

		void Skip(int celt);

		void Reset();

		void Clone(out IEnumSTATURL ppenum);

		void SetFilter([MarshalAs(UnmanagedType.LPWStr)] string poszFilter, STATURLFLAGS dwFlags);
	}
}
