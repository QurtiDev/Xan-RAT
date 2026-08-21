

using InvokedCommon.Enums;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class SetUserStatus : IMessage
	{
		[ProtoMember(1)]
		public UserStatus Message { get; set; }
	}
}
