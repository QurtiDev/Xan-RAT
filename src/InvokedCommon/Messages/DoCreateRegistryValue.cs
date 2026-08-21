

using Microsoft.Win32;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoCreateRegistryValue : IMessage
	{
		[ProtoMember(1)]
		public string KeyPath { get; set; }

		[ProtoMember(2)]
		public RegistryValueKind Kind { get; set; }
	}
}
