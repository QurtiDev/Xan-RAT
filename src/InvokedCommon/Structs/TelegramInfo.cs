

using ProtoBuf;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct TelegramInfo
	{
		[ProtoMember(1)]
		public string rootPath;
		[ProtoMember(2)]
		public string[] files;

		public TelegramInfo(string _rootPath, string[] _files)
		{
			this.rootPath = _rootPath;
			this.files = _files;
		}
	}
}