

using InvokedCommon.Enums;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoShutdownAction : IMessage
	{
		[ProtoMember(1)]
		public ShutdownAction Action { get; set; }
	}
}
