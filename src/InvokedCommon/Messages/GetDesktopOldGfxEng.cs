

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetDesktopOldGfxEng : IMessage
	{
		[ProtoMember(1)]
		public bool CreateNew { get; set; }

		[ProtoMember(2)]
		public int Quality { get; set; }

		[ProtoMember(3)]
		public int DisplayIndex { get; set; }

		[ProtoMember(4)]
		public bool ShowCursor { get; set; }
	}
}
