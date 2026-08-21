

using System;


namespace InvokedClient.Helper
{
	public static class DateTimeHelper
	{
		public static string GetLocalTimeZone()
		{
			TimeZoneInfo local = TimeZoneInfo.Local;
			TimeSpan utcOffset = local.GetUtcOffset(DateTime.Now);
			string str = utcOffset >= TimeSpan.Zero ? "+" : "";
			return string.Format("{0} (UTC {1}{2}{3})", (object)(!local.SupportsDaylightSavingTime || !local.IsDaylightSavingTime(DateTime.Now) ? local.StandardName : local.DaylightName), (object)str, (object)utcOffset.Hours, utcOffset.Minutes != 0 ? (object)string.Format(":{0}", (object)Math.Abs(utcOffset.Minutes)) : (object)"");
		}
	}
}