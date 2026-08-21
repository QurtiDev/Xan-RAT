

using InvokedCommon.Enums;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetDesktopHVNC : IMessage
	{
		[ProtoMember(1)]
		public bool CreateNew { get; set; }

		[ProtoMember(2)]
		public int Quality { get; set; }

		[ProtoMember(3)]
		public HVNCStatus HvncStatus { get; set; }
	}
}
