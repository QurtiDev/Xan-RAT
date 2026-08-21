

using System.IO;


namespace InvokedCommon.Extensions
{
	public static class DriveTypeExtensions
	{
		public static string ToFriendlyString(this DriveType type)
		{
			switch (type)
			{
				case DriveType.Removable:
					return "Removable Drive";
				case DriveType.Fixed:
					return "Local Disk";
				case DriveType.Network:
					return "Network Drive";
				default:
					return type.ToString();
			}
		}
	}
}
