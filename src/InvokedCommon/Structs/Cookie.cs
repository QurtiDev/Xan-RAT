

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct Cookie
	{
		[ProtoMember(1)]
		public string domain;
		[ProtoMember(2)]
		public string path;
		[ProtoMember(3)]
		public string name;
		[ProtoMember(4)]
		public string value;
		[ProtoMember(5)]
		public int expiry;
		[ProtoMember(6)]
		public bool isSecure;
		[ProtoMember(7)]
		public bool isHttpOnly;
		[ProtoMember(8)]
		public bool expired;

		public Cookie(
		    string _domain,
		    string _path,
		    string _name,
		    string _value,
		    int _expiry,
		    bool _isSecure,
		    bool _isHttpOnly)
		{
			this.domain = _domain;
			this.path = _path;
			this.name = _name;
			this.value = _value;
			this.expiry = _expiry;
			this.isSecure = _isSecure;
			this.isHttpOnly = _isHttpOnly;
			this.expired = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds >= (double)_expiry;
		}

		public override string ToString()
		{
			return "DOMAIN: " + this.domain + Environment.NewLine + "PATH: " + this.path + Environment.NewLine + "NAME: " + this.name + Environment.NewLine + "VALUE: " + this.value + Environment.NewLine + "EXPIRY: " + this.expiry.ToString() + Environment.NewLine + "IS_SECURE: " + this.isSecure.ToString() + Environment.NewLine + "IS_HTTP_ONLY: " + this.isHttpOnly.ToString() + Environment.NewLine + "EXPIRED: " + this.expired.ToString();
		}
	}
}