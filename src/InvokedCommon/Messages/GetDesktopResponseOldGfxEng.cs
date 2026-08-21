

using InvokedCommon.Video;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetDesktopResponseOldGfxEng : IMessage
	{
		[ProtoMember(1)]
		public byte[] Image { get; set; }

		[ProtoMember(2)]
		public int Quality { get; set; }

		[ProtoMember(3)]
		public int Monitor { get; set; }

		[ProtoMember(4)]
		public Resolution Resolution { get; set; }
	}
}
