

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct Login
	{
		[ProtoMember(1)]
		public string hostname;
		[ProtoMember(2)]
		public string username;
		[ProtoMember(3)]
		public string password;

		public Login(string _username, string _password, string _hostname)
		{
			this.hostname = _hostname;
			this.username = _username;
			this.password = _password;
		}

		public override string ToString()
		{
			return "HOSTNAME: " + this.hostname + Environment.NewLine + "USERNAME: " + this.username + Environment.NewLine + "PASSWORD: " + this.password;
		}
	}
}