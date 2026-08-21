

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct OBSInfo
	{
		[ProtoMember(1)]
		public string service;
		[ProtoMember(2)]
		public string streamKey;

		public OBSInfo(string _service, string _streamKey)
		{
			this.service = _service;
			this.streamKey = _streamKey;
		}

		public override string ToString()
		{
			return "SERVICE: " + this.service + Environment.NewLine + "STREAM KEY: " + this.streamKey;
		}
	}
}