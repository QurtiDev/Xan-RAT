

using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace InvokedCommon.DNS
{
	public class HostsConverter
	{
		public List<Host> RawHostsToList(string rawHosts)
		{
			List<Host> list = new List<Host>();
			if (string.IsNullOrEmpty(rawHosts))
				return list;
			string str = rawHosts;
			char[] chArray = new char[1]{ ';' };
			foreach (string source in str.Split(chArray))
			{
				ushort result;
				if (!string.IsNullOrEmpty(source) && source.Contains<char>(':') && ushort.TryParse(source.Substring(source.LastIndexOf(':') + 1), out result))
					list.Add(new Host()
					{
						Hostname = source.Substring(0, source.LastIndexOf(':')),
						Port = result
					});
			}
			return list;
		}

		public string ListToRawHosts(IList<Host> hosts)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Host host in (IEnumerable<Host>) hosts)
				stringBuilder.Append(host?.ToString() + ";");
			return stringBuilder.ToString();
		}
	}
}
