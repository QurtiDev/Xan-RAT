

using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetWebcamImage : IMessage
	{
		[ProtoMember(1)]
		public int Webcam { get; set; }

		[ProtoMember(2)]
		public int Resolution { get; set; }

		[ProtoMember(3)]
		public int Quality { get; set; }
	}
}
