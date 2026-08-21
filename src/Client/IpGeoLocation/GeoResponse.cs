

using System.Runtime.Serialization;


namespace InvokedClient.IpGeoLocation
{
	[DataContract]
	public class GeoResponse
	{
		[DataMember(Name = "ip")]
		public string Ip { get; set; }

		[DataMember(Name = "continent_code")]
		public string ContinentCode { get; set; }

		[DataMember(Name = "country")]
		public string Country { get; set; }

		[DataMember(Name = "country_code")]
		public string CountryCode { get; set; }

		[DataMember(Name = "timezone")]
		public Time Timezone { get; set; }

		[DataMember(Name = "connection")]
		public Conn Connection { get; set; }
	}
}