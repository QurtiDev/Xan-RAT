

using InvokedCommon.Models;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoStartupItemAdd : IMessage
	{
		[ProtoMember(1)]
		public StartupItem StartupItem { get; set; }
	}
}
