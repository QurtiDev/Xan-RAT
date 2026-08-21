

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoProcessEnd : IMessage
	{
		[ProtoMember(1)]
		public int Pid { get; set; }
	}
}
