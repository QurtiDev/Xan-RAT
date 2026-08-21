

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[Flags]
	[ProtoContract]
	public enum AppsOptions
	{
		None = 0,
		Discord = 1,
		Telegram = 2,
		Steam = 4,
		Obs = 8,
		Ngrok = 16, // 0x00000010
		Winscp = 32, // 0x00000020
		Filazilla = 64, // 0x00000040
		Foxmail = 128, // 0x00000080
		Crypto = 256, // 0x00000100
		All = Crypto | Foxmail | Filazilla | Winscp | Obs | Steam | Telegram | Discord, // 0x000001EF
	}
}