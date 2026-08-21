

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


namespace InvokedCommon.DNS
{
	public class HostsManager
	{
		private readonly Queue<Host> _hosts = new Queue<Host>();

		public bool IsEmpty => this._hosts.Count == 0;

		public HostsManager(List<Host> hosts)
		{
			foreach (Host host in hosts)
				this._hosts.Enqueue(host);
		}

		public Host GetNextHost()
		{
			Host host = this._hosts.Dequeue();
			this._hosts.Enqueue(host);
			host.IpAddress = HostsManager.ResolveHostname(host);
			return host;
		}

		private static IPAddress ResolveHostname(Host host)
		{
			if (string.IsNullOrEmpty(host.Hostname))
				return (IPAddress) null;
			IPAddress address;
			if (IPAddress.TryParse(host.Hostname, out address))
				return address.AddressFamily == AddressFamily.InterNetworkV6 && !Socket.OSSupportsIPv6 ? (IPAddress) null : address;
			IPAddress[] addressList = Dns.GetHostEntry(host.Hostname).AddressList;
			foreach (IPAddress ipAddress in addressList)
			{
				switch (ipAddress.AddressFamily)
				{
					case AddressFamily.InterNetwork:
						return ipAddress;
					case AddressFamily.InterNetworkV6:
						if (addressList.Length == 1)
							return ipAddress;
						break;
				}
			}
			return address;
		}
	}
}
