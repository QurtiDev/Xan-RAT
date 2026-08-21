

using InvokedCommon.Models;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoStartupItemRemove : IMessage
	{
		[ProtoMember(1)]
		public StartupItem StartupItem { get; set; }
	}
}
