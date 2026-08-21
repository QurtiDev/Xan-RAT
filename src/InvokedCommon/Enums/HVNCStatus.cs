

using ProtoBuf;
using System;


namespace InvokedCommon.Enums
{
	[Flags]
	[ProtoContract]
	public enum HVNCStatus
	{
		Start = 0,
		Stopped = 1,
		Stop = 2,
		CheckServer = Stop | Stopped, // 0x00000003
		ResetClientFrameCount = 4,
	}
}
