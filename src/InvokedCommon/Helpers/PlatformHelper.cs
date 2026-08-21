

using System;
using System.Management;
using System.Text.RegularExpressions;


namespace InvokedCommon.Helpers
{
	public static class PlatformHelper
	{
		static PlatformHelper()
		{
			PlatformHelper.RunningOnMono = Type.GetType("Mono.Runtime") != (Type) null;
			PlatformHelper.Name = "Unknown OS";
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
			{
				using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
				{
					if (enumerator.MoveNext())
						PlatformHelper.Name = enumerator.Current["Caption"].ToString();
				}
			}
			PlatformHelper.Name = Regex.Replace(PlatformHelper.Name, "^.*(?=Windows)", "").TrimEnd().TrimStart();
			PlatformHelper.Is64Bit = Environment.Is64BitOperatingSystem;
			PlatformHelper.FullName = string.Format("{0} {1} Bit", (object) PlatformHelper.Name, (object) (PlatformHelper.Is64Bit ? 64 : 32));
		}

		public static string FullName { get; }

		public static string Name { get; }

		public static bool Is64Bit { get; }

		public static bool RunningOnMono { get; }

		public static bool Win32NT { get; } = Environment.OSVersion.Platform == PlatformID.Win32NT;

		public static bool XpOrHigher { get; } = PlatformHelper.Win32NT && Environment.OSVersion.Version.Major >= 5;

		public static bool VistaOrHigher { get; } = PlatformHelper.Win32NT && Environment.OSVersion.Version.Major >= 6;

		public static bool SevenOrHigher { get; } = PlatformHelper.Win32NT && Environment.OSVersion.Version >= new Version(6, 1);

		public static bool EightOrHigher { get; } = PlatformHelper.Win32NT && Environment.OSVersion.Version >= new Version(6, 2, 9200);

		public static bool EightPointOneOrHigher { get; } = PlatformHelper.Win32NT && Environment.OSVersion.Version >= new Version(6, 3);

		public static bool TenOrHigher { get; } = PlatformHelper.Win32NT && Environment.OSVersion.Version >= new Version(10, 0);
	}
}
