

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoDeleteRegistryKey : IMessage
	{
		[ProtoMember(1)]
		public string ParentPath { get; set; }

		[ProtoMember(2)]
		public string KeyName { get; set; }
	}
}
