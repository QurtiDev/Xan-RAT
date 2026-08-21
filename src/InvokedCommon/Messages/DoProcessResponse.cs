

using InvokedCommon.Enums;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoProcessResponse : IMessage
	{
		[ProtoMember(1)]
		public ProcessAction Action { get; set; }

		[ProtoMember(2)]
		public bool Result { get; set; }
	}
}
