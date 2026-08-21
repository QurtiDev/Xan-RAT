

using System.Globalization;
using System.IO;
using System.Net;
using InvokedClient.Helper;


namespace InvokedClient.IpGeoLocation
{
	public class GeoInformationRetriever
	{
		private readonly string[] _imageList = new string[247]
		{
			"ad",
			"ae",
			"af",
			"ag",
			"ai",
			"al",
			"am",
			"an",
			"ao",
			"ar",
			"as",
			"at",
			"au",
			"aw",
			"ax",
			"az",
			"ba",
			"bb",
			"bd",
			"be",
			"bf",
			"bg",
			"bh",
			"bi",
			"bj",
			"bm",
			"bn",
			"bo",
			"br",
			"bs",
			"bt",
			"bv",
			"bw",
			"by",
			"bz",
			"ca",
			"catalonia",
			"cc",
			"cd",
			"cf",
			"cg",
			"ch",
			"ci",
			"ck",
			"cl",
			"cm",
			"cn",
			"co",
			"cr",
			"cs",
			"cu",
			"cv",
			"cx",
			"cy",
			"cz",
			"de",
			"dj",
			"dk",
			"dm",
			"do",
			"dz",
			"ec",
			"ee",
			"eg",
			"eh",
			"england",
			"er",
			"es",
			"et",
			"europeanunion",
			"fam",
			"fi",
			"fj",
			"fk",
			"fm",
			"fo",
			"fr",
			"ga",
			"gb",
			"gd",
			"ge",
			"gf",
			"gh",
			"gi",
			"gl",
			"gm",
			"gn",
			"gp",
			"gq",
			"gr",
			"gs",
			"gt",
			"gu",
			"gw",
			"gy",
			"hk",
			"hm",
			"hn",
			"hr",
			"ht",
			"hu",
			"id",
			"ie",
			"il",
			"in",
			"io",
			"iq",
			"ir",
			"is",
			"it",
			"jm",
			"jo",
			"jp",
			"ke",
			"kg",
			"kh",
			"ki",
			"km",
			"kn",
			"kp",
			"kr",
			"kw",
			"ky",
			"kz",
			"la",
			"lb",
			"lc",
			"li",
			"lk",
			"lr",
			"ls",
			"lt",
			"lu",
			"lv",
			"ly",
			"ma",
			"mc",
			"md",
			"me",
			"mg",
			"mh",
			"mk",
			"ml",
			"mm",
			"mn",
			"mo",
			"mp",
			"mq",
			"mr",
			"ms",
			"mt",
			"mu",
			"mv",
			"mw",
			"mx",
			"my",
			"mz",
			"na",
			"nc",
			"ne",
			"nf",
			"ng",
			"ni",
			"nl",
			"no",
			"np",
			"nr",
			"nu",
			"nz",
			"om",
			"pa",
			"pe",
			"pf",
			"pg",
			"ph",
			"pk",
			"pl",
			"pm",
			"pn",
			"pr",
			"ps",
			"pt",
			"pw",
			"py",
			"qa",
			"re",
			"ro",
			"rs",
			"ru",
			"rw",
			"sa",
			"sb",
			"sc",
			"scotland",
			"sd",
			"se",
			"sg",
			"sh",
			"si",
			"sj",
			"sk",
			"sl",
			"sm",
			"sn",
			"so",
			"sr",
			"st",
			"sv",
			"sy",
			"sz",
			"tc",
			"td",
			"tf",
			"tg",
			"th",
			"tj",
			"tk",
			"tl",
			"tm",
			"tn",
			"to",
			"tr",
			"tt",
			"tv",
			"tw",
			"tz",
			"ua",
			"ug",
			"um",
			"us",
			"uy",
			"uz",
			"va",
			"vc",
			"ve",
			"vg",
			"vi",
			"vn",
			"vu",
			"wales",
			"wf",
			"ws",
			"ye",
			"yt",
			"za",
			"zm",
			"zw"
		};

		public GeoInformation Retrieve()
		{
			GeoInformation geoInformation = this.TryRetrieveOnline() ?? this.TryRetrieveLocally();
			if (string.IsNullOrEmpty(geoInformation.IpAddress))
				geoInformation.IpAddress = this.TryGetWanIp();
			geoInformation.IpAddress = string.IsNullOrEmpty(geoInformation.IpAddress) ? "Unknown" : geoInformation.IpAddress;
			geoInformation.Country = string.IsNullOrEmpty(geoInformation.Country) ? "Unknown" : geoInformation.Country;
			geoInformation.CountryCode = string.IsNullOrEmpty(geoInformation.CountryCode) ? "-" : geoInformation.CountryCode;
			geoInformation.Timezone = string.IsNullOrEmpty(geoInformation.Timezone) ? "Unknown" : geoInformation.Timezone;
			geoInformation.Asn = string.IsNullOrEmpty(geoInformation.Asn) ? "Unknown" : geoInformation.Asn;
			geoInformation.Isp = string.IsNullOrEmpty(geoInformation.Isp) ? "Unknown" : geoInformation.Isp;
			geoInformation.ImageIndex = 0;
			for (int index = 0; index < this._imageList.Length; ++index)
			{
				if (this._imageList[index] == geoInformation.CountryCode.ToLower())
				{
					geoInformation.ImageIndex = index;
					break;
				}
			}
			if (geoInformation.ImageIndex == 0)
				geoInformation.ImageIndex = 247;
			return geoInformation;
		}

		private GeoInformation TryRetrieveOnline()
		{
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://ipwho.is/");
				httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:76.0) Gecko/20100101 Firefox/76.0";
				httpWebRequest.Proxy = null;
				httpWebRequest.Timeout = 10000;
				using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (Stream responseStream = response.GetResponseStream())
					{
						GeoResponse geoResponse = JsonHelper.Deserialize<GeoResponse>(responseStream);
						return new GeoInformation()
						{
							IpAddress = geoResponse.Ip,
							Country = geoResponse.Country,
							CountryCode = geoResponse.CountryCode,
							Timezone = geoResponse.Timezone.UTC,
							Asn = geoResponse.Connection.ASN.ToString(),
							Isp = geoResponse.Connection.ISP
						};
					}
				}
			}
			catch
			{
				return null;
			}
		}

		private GeoInformation TryRetrieveLocally()
		{
			try
			{
				GeoInformation geoInformation = new GeoInformation();
				RegionInfo regionInfo = new RegionInfo(CultureInfo.CurrentUICulture.LCID);
				geoInformation.Country = regionInfo.DisplayName;
				geoInformation.CountryCode = regionInfo.TwoLetterISORegionName;
				geoInformation.Timezone = DateTimeHelper.GetLocalTimeZone();
				return geoInformation;
			}
			catch
			{
				return null;
			}
		}

		private string TryGetWanIp()
		{
			string wanIp = "";
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.ipify.org/");
				httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:76.0) Gecko/20100101 Firefox/76.0";
				httpWebRequest.Proxy = null;
				httpWebRequest.Timeout = 5000;
				using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (Stream responseStream = response.GetResponseStream())
					{
						using (StreamReader streamReader = new StreamReader(responseStream))
							wanIp = streamReader.ReadToEnd();
					}
				}
			}
			catch
			{
			}
			return wanIp;
		}
	}
}
