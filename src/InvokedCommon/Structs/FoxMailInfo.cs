

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct FoxMailInfo
	{
		[ProtoMember(1)]
		public string account;
		[ProtoMember(2)]
		public string password;
		[ProtoMember(3)]
		public bool pop3;

		public FoxMailInfo(string _account, string _password, bool _pop3)
		{
			this.account = _account;
			this.password = _password;
			this.pop3 = _pop3;
		}

		public override string ToString()
		{
			return "ACCOUNT: " + this.account + Environment.NewLine + "PASSWORD: " + this.password + Environment.NewLine + "POP3: " + this.pop3.ToString().ToUpper();
		}
	}
}