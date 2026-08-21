

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoInputEventHVNC : IMessage
	{
		[ProtoMember(1)]
		public int HVNCImsg { get; set; }

		[ProtoMember(2)]
		public long HVNCIwParam { get; set; }

		[ProtoMember(3)]
		public long HVNCIlParam { get; set; }
	}
}
