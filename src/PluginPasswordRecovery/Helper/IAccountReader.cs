

using InvokedCommon.Models;
using System.Collections.Generic;


namespace Plugin.Helper
{
	public interface IAccountReader
	{
		IEnumerable<RecoveredAccount> ReadAccounts();

		string ApplicationName { get; }
	}
}
