

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class CheckPlugin : IMessage
	{
		[ProtoMember(1)]
		public string PluginName { get; set; }
	}
}
