

using ProtoBuf;


namespace InvokedCommon.Messages.ReverseProxy
{
	[ProtoContract]
	public class ReverseProxyDisconnect : IMessage
	{
		[ProtoMember(1)]
		public int ConnectionId { get; set; }
	}
}
