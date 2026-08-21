

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class SendHvncLog : IMessage
	{
		[ProtoMember(1)]
		public uint LogType { get; set; }

		[ProtoMember(2)]
		public string Log { get; set; }
	}
}
