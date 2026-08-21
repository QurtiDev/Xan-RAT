

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct Download
	{
		[ProtoMember(1)]
		public string url;
		[ProtoMember(2)]
		public string path;

		public Download(string _url, string _path)
		{
			this.path = _path;
			this.url = _url;
		}

		public override string ToString()
		{
			return "URL: " + this.url + Environment.NewLine + "DOWNLOAD PATH: " + this.path;
		}
	}
}