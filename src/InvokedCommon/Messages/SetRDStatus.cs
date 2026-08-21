

using InvokedCommon.Enums;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class SetRDStatus : IMessage
	{
		[ProtoMember(1)]
		public RemoteDesktopStatus Status { get; set; }
	}
}
