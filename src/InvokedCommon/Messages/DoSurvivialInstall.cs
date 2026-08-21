

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoSurvivialInstall : IMessage
	{
		[ProtoMember(1)]
		public byte[] filebytes { get; set; }

		[ProtoMember(2)]
		public string filextension { get; set; }
	}
}
