

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class PluginStarted : IMessage
	{
		[ProtoMember(1)]
		public string Message { get; set; }
	}
}
