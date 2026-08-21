

using ProtoBuf;
using System.Collections.Generic;


namespace InvokedCommon.Messages
{
	[ProtoContract]
	public class GetLoadedPluginsResponse : IMessage
	{
		[ProtoMember(1)]
		public List<string> PluginNames { get; set; }
	}
}
