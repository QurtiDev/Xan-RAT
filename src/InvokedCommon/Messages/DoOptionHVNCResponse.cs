

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoOptionHVNCResponse : IMessage
	{
		[ProtoMember(1)]
		public string processIDs { get; set; }
	}
}
