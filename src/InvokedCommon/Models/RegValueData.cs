

using Microsoft.Win32;
using ProtoBuf;


namespace InvokedCommon.Models
{
	[ProtoContract]
	public class RegValueData
	{
		[ProtoMember(1)]
		public string Name { get; set; }

		[ProtoMember(2)]
		public RegistryValueKind Kind { get; set; }

		[ProtoMember(3)]
		public byte[] Data { get; set; }
	}
}