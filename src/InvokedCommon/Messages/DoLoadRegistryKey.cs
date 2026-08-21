

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoLoadRegistryKey : IMessage
	{
		[ProtoMember(1)]
		public string RootKeyName { get; set; }
	}
}
