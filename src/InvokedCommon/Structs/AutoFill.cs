

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct AutoFill
	{
		[ProtoMember(1)]
		public string name;
		[ProtoMember(2)]
		public string value;

		public AutoFill(string _name, string _value)
		{
			this.name = _name;
			this.value = _value;
		}

		public override string ToString()
		{
			return "NAME: " + this.name + Environment.NewLine + "VALUE: " + this.value;
		}
	}
}