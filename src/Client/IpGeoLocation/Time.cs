

using System.Runtime.Serialization;


namespace InvokedClient.IpGeoLocation
{
	[DataContract]
	public class Time
	{
		[DataMember(Name = "utc")]
		public string UTC { get; set; }
	}
}