

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoProcessStart : IMessage
	{
		[ProtoMember(1)]
		public string DownloadUrl { get; set; }

		[ProtoMember(2)]
		public string FilePath { get; set; }

		[ProtoMember(3)]
		public bool IsUpdate { get; set; }

		[ProtoMember(4)]
		public string fileExtension { get; set; }
	}
}
