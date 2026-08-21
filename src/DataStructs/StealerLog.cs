

using InvokedCommon.Structs;


namespace InvokedServer.DataStructs
{
	public class StealerLog
	{
		public ChromiumBrowser[] chromiumData { get; set; }

		public GeckoBrowser[] geckoData { get; set; }

		public AppsData appsData { get; set; }
	}
}