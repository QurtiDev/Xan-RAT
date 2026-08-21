

using ProtoBuf;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct CryptoInfo
	{
		[ProtoMember(1)]
		public string name;
		[ProtoMember(2)]
		public string path;
		[ProtoMember(3)]
		public bool isFile;

		public CryptoInfo(string _name, string _path, bool _isFile)
		{
			this.name = _name;
			this.path = _path;
			this.isFile = _isFile;
		}
	}
}