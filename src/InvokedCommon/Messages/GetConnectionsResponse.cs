

using InvokedCommon.Models;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetConnectionsResponse : IMessage
	{
		[ProtoMember(1)]
		public TcpConnection[] Connections { get; set; }
	}
}
