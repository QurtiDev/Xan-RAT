

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class SetStatus : IMessage
	{
		[ProtoMember(1)]
		public string Message { get; set; }
	}
}
