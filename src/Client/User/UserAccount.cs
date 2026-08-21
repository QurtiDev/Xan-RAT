

using InvokedCommon.Enums;
using System;
using System.Security.Principal;


namespace InvokedClient.User
{
	public class UserAccount
	{
		public string UserName { get; }

		public AccountType Type { get; }

		public UserAccount()
		{
			this.UserName = Environment.UserName;
			using (WindowsIdentity current = WindowsIdentity.GetCurrent())
			{
				WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
				if (windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
					this.Type = AccountType.Admin;
				else if (windowsPrincipal.IsInRole(WindowsBuiltInRole.User))
					this.Type = AccountType.User;
				else if (windowsPrincipal.IsInRole(WindowsBuiltInRole.Guest))
					this.Type = AccountType.Guest;
				else
					this.Type = AccountType.Unknown;
			}
		}
	}
}