

using System.Net;


namespace InvokedCommon.DNS
{
	public class Host
	{
		public string Hostname { get; set; }

		public IPAddress IpAddress { get; set; }

		public ushort Port { get; set; }

		public override string ToString() => this.Hostname + ":" + this.Port.ToString();
	}
}
