

using InvokedCommon.Cryptography;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace InvokedClient.Config
{
	public static class Settings
	{
		public static string VERSION = "";
		public static string HOSTS = "";
		public static int RECONNECTDELAY = 5000;
		public static Environment.SpecialFolder SPECIALFOLDER = Environment.SpecialFolder.ApplicationData;
		public static string DIRECTORY = Environment.GetFolderPath(Settings.SPECIALFOLDER);
		public static string SUBDIRECTORY = "";
		public static string INSTALLNAME = "";
		public static bool INSTALL = false;
		public static bool STARTUP = false;
		public static string MUTEX = "";
		public static string STARTUPKEY = "";
		public static bool HIDEFILE = false;
		public static bool ENABLELOGGER = false;
		public static string ENCRYPTIONKEY = "";
		public static string TAG = "";
		public static string LOGDIRECTORYNAME = "";
		public static string SERVERSIGNATURE = "";
		public static string SERVERCERTIFICATESTR = "";
		public static X509Certificate2 SERVERCERTIFICATE;
		public static bool HIDELOGDIRECTORY = false;
		public static bool HIDEINSTALLSUBDIRECTORY = false;
		public static string INSTALLPATH = "";
		public static string LOGSPATH = "";
		public static bool UNATTENDEDMODE = false;

		public static bool Initialize()
		{
			if (string.IsNullOrEmpty(Settings.VERSION))
				return false;
			Aes256 aes256 = new Aes256(Settings.ENCRYPTIONKEY);
			Settings.TAG = aes256.Decrypt(Settings.TAG);
			Settings.VERSION = aes256.Decrypt(Settings.VERSION);
			Settings.HOSTS = aes256.Decrypt(Settings.HOSTS);
			Settings.SUBDIRECTORY = aes256.Decrypt(Settings.SUBDIRECTORY);
			Settings.INSTALLNAME = aes256.Decrypt(Settings.INSTALLNAME);
			Settings.MUTEX = aes256.Decrypt(Settings.MUTEX);
			Settings.STARTUPKEY = aes256.Decrypt(Settings.STARTUPKEY);
			Settings.LOGDIRECTORYNAME = aes256.Decrypt(Settings.LOGDIRECTORYNAME);
			Settings.SERVERSIGNATURE = aes256.Decrypt(Settings.SERVERSIGNATURE);
			Settings.SERVERCERTIFICATE = new X509Certificate2(Convert.FromBase64String(aes256.Decrypt(Settings.SERVERCERTIFICATESTR)));
			Settings.SetupPaths();
			return Settings.VerifyHash();
		}

		private static void SetupPaths()
		{
			Settings.LOGSPATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Settings.LOGDIRECTORYNAME);
			Settings.INSTALLPATH = Path.Combine(Settings.DIRECTORY, (!string.IsNullOrEmpty(Settings.SUBDIRECTORY) ? Settings.SUBDIRECTORY + "\\" : "") + Settings.INSTALLNAME);
		}

		private static bool VerifyHash()
		{
			try
			{
				return ((RSACryptoServiceProvider)Settings.SERVERCERTIFICATE.PublicKey.Key).VerifyHash(Sha256.ComputeHash(Encoding.UTF8.GetBytes(Settings.ENCRYPTIONKEY)), CryptoConfig.MapNameToOID("SHA256"), Convert.FromBase64String(Settings.SERVERSIGNATURE));
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}