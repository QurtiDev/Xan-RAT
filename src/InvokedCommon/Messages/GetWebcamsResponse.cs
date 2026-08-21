

using ProtoBuf;
using System.Collections.Generic;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetWebcamsResponse : IMessage
	{
		[ProtoMember(1)]
		public int Number { get; set; }

		[ProtoMember(2)]
		public List<string> Webcams { get; set; }
	}
}
