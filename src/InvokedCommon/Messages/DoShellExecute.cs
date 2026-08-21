

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class DoShellExecute : IMessage
	{
		[ProtoMember(1)]
		public string Command { get; set; }
	}
}
