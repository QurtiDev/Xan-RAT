

using InvokedCommon.Models;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetProcessesResponse : IMessage
	{
		[ProtoMember(1)]
		public Process[] Processes { get; set; }
	}
}
