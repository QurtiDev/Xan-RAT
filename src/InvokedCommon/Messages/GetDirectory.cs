

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetDirectory : IMessage
	{
		[ProtoMember(1)]
		public string RemotePath { get; set; }
	}
}
