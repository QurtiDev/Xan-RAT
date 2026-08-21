

using InvokedCommon.Models;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetDirectoryResponse : IMessage
	{
		[ProtoMember(1)]
		public string RemotePath { get; set; }

		[ProtoMember(2)]
		public FileSystemEntry[] Items { get; set; }
	}
}
