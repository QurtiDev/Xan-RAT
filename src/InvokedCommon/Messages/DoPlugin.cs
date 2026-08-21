

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoPlugin : IMessage
	{
		[ProtoMember(1)]
		public string PluginName { get; set; }

		[ProtoMember(2)]
		public byte[] Data { get; set; }

		[ProtoMember(3)]
		public byte[] MsgpackData { get; set; }
	}
}
