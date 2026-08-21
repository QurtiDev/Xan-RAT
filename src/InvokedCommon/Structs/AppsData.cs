

using ProtoBuf;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct AppsData
	{
		[ProtoMember(1)]
		public AppsOptions options;
		[ProtoMember(2)]
		public DiscordUserData[] DiscordUserDatas;
		[ProtoMember(3)]
		public TelegramInfo? telegramInfo;
		[ProtoMember(4)]
		public SteamInfo? steamInfos;
		[ProtoMember(5)]
		public OBSInfo[] obsInfos;
		[ProtoMember(6)]
		public NgrokInfo? ngrokInfos;
		[ProtoMember(7)]
		public WinScpInfo[] winscpInfos;
		[ProtoMember(8)]
		public FileZillaInfo[] filezillaInfos;
		[ProtoMember(9)]
		public FoxMailInfo[] foxMailInfos;
		[ProtoMember(10)]
		public CryptoInfo[] cryptoInfos;

		public AppsData(
		    AppsOptions _options,
		    DiscordUserData[] _DiscordUserDatas,
		    TelegramInfo? _telegramInfo,
		    SteamInfo? _steamInfos,
		    OBSInfo[] _obsInfos,
		    NgrokInfo? _ngrokInfos,
		    WinScpInfo[] _winscpInfos,
		    FileZillaInfo[] _filezillaInfos,
		    FoxMailInfo[] _foxMailInfos,
		    CryptoInfo[] _cryptoInfos)
		{
			this.options = _options;
			this.DiscordUserDatas = _DiscordUserDatas != null ? _DiscordUserDatas : new DiscordUserData[0];
			this.telegramInfo = _telegramInfo.HasValue ? _telegramInfo : new TelegramInfo?();
			this.steamInfos = _steamInfos.HasValue ? _steamInfos : new SteamInfo?();
			this.obsInfos = _obsInfos != null ? _obsInfos : new OBSInfo[0];
			this.ngrokInfos = _ngrokInfos.HasValue ? _ngrokInfos : new NgrokInfo?();
			this.winscpInfos = _winscpInfos != null ? _winscpInfos : new WinScpInfo[0];
			this.filezillaInfos = _filezillaInfos != null ? _filezillaInfos : new FileZillaInfo[0];
			this.foxMailInfos = _foxMailInfos != null ? _foxMailInfos : new FoxMailInfo[0];
			if (_cryptoInfos == null)
				this.cryptoInfos = new CryptoInfo[0];
			else
				this.cryptoInfos = _cryptoInfos;
		}
	}
}