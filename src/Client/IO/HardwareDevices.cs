

using InvokedCommon.Cryptography;
using InvokedCommon.Helpers;
using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;


namespace InvokedClient.IO
{
	public static class HardwareDevices
	{
		private static string _hardwareId;
		private static string _cpuName;
		private static string _gpuName;
		private static string _biosManufacturer;
		private static string _mainboardName;
		private static int? _totalPhysicalMemory;

		public static string HardwareId
		{
			get
			{
				return HardwareDevices._hardwareId ?? (HardwareDevices._hardwareId = Sha256.ComputeHash(HardwareDevices.CpuName + HardwareDevices.MainboardName + HardwareDevices.BiosManufacturer));
			}
		}

		public static string CpuName
		{
			get => HardwareDevices._cpuName ?? (HardwareDevices._cpuName = HardwareDevices.GetCpuName());
		}

		public static string GpuName
		{
			get => HardwareDevices._gpuName ?? (HardwareDevices._gpuName = HardwareDevices.GetGpuName());
		}

		public static string BiosManufacturer
		{
			get
			{
				return HardwareDevices._biosManufacturer ?? (HardwareDevices._biosManufacturer = HardwareDevices.GetBiosManufacturer());
			}
		}

		public static string MainboardName
		{
			get
			{
				return HardwareDevices._mainboardName ?? (HardwareDevices._mainboardName = HardwareDevices.GetMainboardName());
			}
		}

		public static int? TotalPhysicalMemory
		{
			get
			{
				return HardwareDevices._totalPhysicalMemory ?? (HardwareDevices._totalPhysicalMemory = new int?(HardwareDevices.GetTotalPhysicalMemoryInMb()));
			}
		}

		public static string LanIpAddress => HardwareDevices.GetLanIpAddress();

		public static string MacAddress => HardwareDevices.GetMacAddress();

		private static string GetBiosManufacturer()
		{
			try
			{
				string empty = string.Empty;
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS"))
				{
					using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
					{
						if (enumerator.MoveNext())
							empty = enumerator.Current["Manufacturer"].ToString();
					}
				}
				return !string.IsNullOrEmpty(empty) ? empty : "N/A";
			}
			catch
			{
			}
			return "Unknown";
		}

		private static string GetMainboardName()
		{
			try
			{
				string str = string.Empty;
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard"))
				{
					using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							ManagementObject current = (ManagementObject)enumerator.Current;
							str = current["Manufacturer"].ToString() + " " + current["Product"].ToString();
						}
					}
				}
				return !string.IsNullOrEmpty(str) ? str : "N/A";
			}
			catch
			{
			}
			return "Unknown";
		}

		private static string GetCpuName()
		{
			try
			{
				string input = string.Empty;
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
				{
					foreach (ManagementObject managementObject in managementObjectSearcher.Get())
						input = input + managementObject["Name"].ToString() + "; ";
				}
				string str = StringHelper.RemoveLastChars(input);
				return !string.IsNullOrEmpty(str) ? str : "N/A";
			}
			catch
			{
			}
			return "Unknown";
		}

		private static int GetTotalPhysicalMemoryInMb()
		{
			try
			{
				int physicalMemoryInMb = 0;
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * From Win32_ComputerSystem"))
				{
					using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
					{
						if (enumerator.MoveNext())
							physicalMemoryInMb = (int)(Convert.ToDouble(enumerator.Current["TotalPhysicalMemory"]) / 1048576.0);
					}
				}
				return physicalMemoryInMb;
			}
			catch
			{
				return -1;
			}
		}

		private static string GetGpuName()
		{
			try
			{
				string input = string.Empty;
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration"))
				{
					foreach (ManagementObject managementObject in managementObjectSearcher.Get())
						input = input + managementObject["Description"].ToString() + "; ";
				}
				string str = StringHelper.RemoveLastChars(input);
				return !string.IsNullOrEmpty(str) ? str : "N/A";
			}
			catch
			{
				return "Unknown";
			}
		}

		private static string GetLanIpAddress()
		{
			foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (networkInterface.GetIPProperties().GatewayAddresses.FirstOrDefault<GatewayIPAddressInformation>() != null && (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet && networkInterface.OperationalStatus == OperationalStatus.Up))
				{
					foreach (UnicastIPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
					{
						if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork && unicastAddress.AddressPreferredLifetime != (long)uint.MaxValue)
							return unicastAddress.Address.ToString();
					}
				}
			}
			return "-";
		}

		private static string GetMacAddress()
		{
			foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet && networkInterface.OperationalStatus == OperationalStatus.Up)
				{
					bool flag = false;
					foreach (UnicastIPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
					{
						if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork && unicastAddress.AddressPreferredLifetime != (long)uint.MaxValue)
							flag = unicastAddress.Address.ToString() == HardwareDevices.GetLanIpAddress();
					}
					if (flag)
						return StringHelper.GetFormattedMacAddress(networkInterface.GetPhysicalAddress().ToString());
				}
			}
			return "-";
		}
	}
}