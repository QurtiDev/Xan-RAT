

using InvokedServer.Networking;


namespace InvokedServer.Helper
{
	public static class WindowHelper
	{
		public static string GetWindowTitle(string title, Client c)
		{
			return string.Format("{0} - {1}@{2} [{3}:{4}]", (object)title, (object)c.Value.Username, (object)c.Value.PcName, (object)c.EndPoint.Address.ToString(), (object)c.EndPoint.Port.ToString());
		}

		public static string GetWindowTitle(string title, int count)
		{
			return string.Format("{0} [Selected: {1}]", (object)title, (object)count);
		}
	}
}