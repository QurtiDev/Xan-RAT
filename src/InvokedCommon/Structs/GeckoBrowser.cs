

using ProtoBuf;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct GeckoBrowser
	{
		[ProtoMember(1)]
		public string browserName;
		[ProtoMember(2)]
		public GeckoProfile[] profiles;

		public GeckoBrowser(GeckoProfile[] _profiles, string _browserName)
		{
			this.browserName = _browserName;
			if (_profiles == null)
				this.profiles = new GeckoProfile[0];
			else
				this.profiles = _profiles;
		}
	}
}