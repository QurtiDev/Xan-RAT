

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct ChromiumPasswordExtension
	{
		[ProtoMember(1)]
		public string name;
		[ProtoMember(2)]
		public string path;

		public ChromiumPasswordExtension(string _name, string _path)
		{
			this.name = _name;
			this.path = _path;
		}

		public override string ToString()
		{
			return "NAME: " + this.name + Environment.NewLine + "PATH: " + this.path;
		}
	}
}