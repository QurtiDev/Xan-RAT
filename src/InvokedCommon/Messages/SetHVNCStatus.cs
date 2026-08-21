

using InvokedCommon.Enums;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class SetHVNCStatus : IMessage
	{
		[ProtoMember(1)]
		public HVNCStatus Status { get; set; }
	}
}
