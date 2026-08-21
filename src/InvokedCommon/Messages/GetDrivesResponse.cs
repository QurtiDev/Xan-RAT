

using InvokedCommon.Models;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetDrivesResponse : IMessage
	{
		[ProtoMember(1)]
		public Drive[] Drives { get; set; }
	}
}
