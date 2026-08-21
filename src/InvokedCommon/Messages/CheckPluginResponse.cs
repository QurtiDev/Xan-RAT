

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class CheckPluginResponse : IMessage
	{
		[ProtoMember(1)]
		public string PluginName { get; set; }

		[ProtoMember(2)]
		public bool Status { get; set; }
	}
}
