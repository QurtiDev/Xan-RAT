

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class NewSurvivalLog : IMessage
	{
		[ProtoMember(1)]
		public string log { get; set; }

		[ProtoMember(2)]
		public string logtype { get; set; }
	}
}
