

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoCreateRegistryKey : IMessage
	{
		[ProtoMember(1)]
		public string ParentPath { get; set; }
	}
}
