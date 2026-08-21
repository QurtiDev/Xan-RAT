

using InvokedCommon.Models;
using ProtoBuf;
using System.Collections.Generic;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetStartupItemsResponse : IMessage
	{
		[ProtoMember(1)]
		public List<StartupItem> StartupItems { get; set; }
	}
}
