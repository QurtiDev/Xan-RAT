

using System;


namespace InvokedClient.IpGeoLocation
{
	public static class GeoInformationFactory
	{
		private static readonly GeoInformationRetriever Retriever = new GeoInformationRetriever();
		private static GeoInformation _geoInformation;
		private static DateTime _lastSuccessfulLocation = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		private const int MINIMUM_VALID_TIME = 720;

		public static GeoInformation GetGeoInformation()
		{
			TimeSpan timeSpan = new TimeSpan(DateTime.UtcNow.Ticks - GeoInformationFactory._lastSuccessfulLocation.Ticks);
			if (GeoInformationFactory._geoInformation == null || timeSpan.TotalMinutes > 720.0)
			{
				GeoInformationFactory._geoInformation = GeoInformationFactory.Retriever.Retrieve();
				GeoInformationFactory._lastSuccessfulLocation = DateTime.UtcNow;
			}
			return GeoInformationFactory._geoInformation;
		}
	}
}