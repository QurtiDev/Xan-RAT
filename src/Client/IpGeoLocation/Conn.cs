

using System.Runtime.Serialization;


namespace InvokedClient.IpGeoLocation
{
	[DataContract]
	public class Conn
	{
		[DataMember(Name = "asn")]
		public string ASN { get; set; }

		[DataMember(Name = "isp")]
		public string ISP { get; set; }
	}
}