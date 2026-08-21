

using System;
using System.Runtime.InteropServices;


namespace Plugin.Helper.Browsers
{
	public struct STATURL
	{
		public int cbSize;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string pwcsUrl;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string pwcsTitle;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftLastVisited;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftLastUpdated;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftExpires;
		public STATURLFLAGS dwFlags;

		public string URL => this.pwcsUrl;

		public string UrlString
		{
			get
			{
				int length = this.pwcsUrl.IndexOf('?');
				return length >= 0 ? this.pwcsUrl.Substring(0, length) : this.pwcsUrl;
			}
		}

		public string Title
		{
			get
			{
				return this.pwcsUrl.StartsWith("file:") ? Win32api.CannonializeURL(this.pwcsUrl, Win32api.shlwapi_URL.URL_UNESCAPE).Substring(8).Replace('/', '\\') : this.pwcsTitle;
			}
		}

		public DateTime LastVisited => Win32api.FileTimeToDateTime(this.ftLastVisited).ToLocalTime();

		public DateTime LastUpdated => Win32api.FileTimeToDateTime(this.ftLastUpdated).ToLocalTime();

		public DateTime Expires
		{
			get
			{
				try
				{
					DateTime expires = Win32api.FileTimeToDateTime(this.ftExpires);
					expires = expires.ToLocalTime();
					return expires;
				}
				catch (Exception ex)
				{
					return DateTime.Now;
				}
			}
		}

		public override string ToString() => this.pwcsUrl;
	}
}
