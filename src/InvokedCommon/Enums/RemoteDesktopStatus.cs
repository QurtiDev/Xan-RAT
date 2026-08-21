

using ProtoBuf;
using System;


namespace InvokedCommon.Enums
{
	[Flags]
	[ProtoContract]
	public enum RemoteDesktopStatus
	{
		Start = 0,
		Stopped = 1,
		Stop = 2,
		CheckServer = Stop | Stopped, // 0x00000003
		ResetClientFrameCount = 4,
		EnableOldGraphicsEngine = ResetClientFrameCount | Stopped, // 0x00000005
		DisableOldGraphicsEngine = ResetClientFrameCount | Stop, // 0x00000006
		OldGraphicsEngineGetDesktop = DisableOldGraphicsEngine | Stopped, // 0x00000007
	}
}
