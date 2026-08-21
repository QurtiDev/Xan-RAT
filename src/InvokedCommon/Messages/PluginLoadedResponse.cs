

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class PluginLoadedResponse : IMessage
	{
		[ProtoMember(1)]
		public string PluginName { get; set; }
	}
}
