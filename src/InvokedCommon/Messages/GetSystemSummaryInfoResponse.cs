

using ProtoBuf;
using System;
using System.Collections.Generic;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetSystemSummaryInfoResponse : IMessage
	{
		[ProtoMember(1)]
		public List<Tuple<string, string>> SystemSummaryInfos { get; set; }
	}
}
