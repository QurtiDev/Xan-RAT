

using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;


namespace InvokedServer.Models
{
	public static class Settings
	{
		private static readonly string SettingsPath = Path.Combine(Application.StartupPath, "settings.xml");
		public static readonly string CertificatePath = Path.Combine(Application.StartupPath, "quasar.p12");

		public static string GetCustomName
		{
			get => Settings.ReadValueSafe("CustomName");
			set => Settings.WriteValue("CustomName", value.ToString());
		}

		public static ushort ListenPort
		{
			get => ushort.Parse(Settings.ReadValueSafe(nameof(ListenPort), "4782"));
			set => Settings.WriteValue(nameof(ListenPort), value.ToString());
		}

		public static bool IPv6Support
		{
			get => bool.Parse(Settings.ReadValueSafe(nameof(IPv6Support), "False"));
			set => Settings.WriteValue(nameof(IPv6Support), value.ToString());
		}

		public static bool AutoListen
		{
			get => bool.Parse(Settings.ReadValueSafe(nameof(AutoListen), "False"));
			set => Settings.WriteValue(nameof(AutoListen), value.ToString());
		}

		public static bool ShowPopup
		{
			get => bool.Parse(Settings.ReadValueSafe(nameof(ShowPopup), "False"));
			set => Settings.WriteValue(nameof(ShowPopup), value.ToString());
		}

		public static bool UseUPnP
		{
			get => bool.Parse(Settings.ReadValueSafe(nameof(UseUPnP), "False"));
			set => Settings.WriteValue(nameof(UseUPnP), value.ToString());
		}

		public static bool ShowToolTip
		{
			get => bool.Parse(Settings.ReadValueSafe(nameof(ShowToolTip), "False"));
			set => Settings.WriteValue(nameof(ShowToolTip), value.ToString());
		}

		public static bool EnableNoIPUpdater
		{
			get => bool.Parse(Settings.ReadValueSafe(nameof(EnableNoIPUpdater), "False"));
			set => Settings.WriteValue(nameof(EnableNoIPUpdater), value.ToString());
		}

		public static string NoIPHost
		{
			get => Settings.ReadValueSafe(nameof(NoIPHost));
			set => Settings.WriteValue(nameof(NoIPHost), value);
		}

		public static string NoIPUsername
		{
			get => Settings.ReadValueSafe(nameof(NoIPUsername));
			set => Settings.WriteValue(nameof(NoIPUsername), value);
		}

		public static string NoIPPassword
		{
			get => Settings.ReadValueSafe(nameof(NoIPPassword));
			set => Settings.WriteValue(nameof(NoIPPassword), value);
		}

		public static string SaveFormat
		{
			get => Settings.ReadValueSafe(nameof(SaveFormat), "APP - URL - USER:PASS");
			set => Settings.WriteValue(nameof(SaveFormat), value);
		}

		public static ushort ReverseProxyPort
		{
			get => ushort.Parse(Settings.ReadValueSafe(nameof(ReverseProxyPort), "3128"));
			set => Settings.WriteValue(nameof(ReverseProxyPort), value.ToString());
		}

		private static string ReadValue(string pstrValueToRead)
		{
			try
			{
				XPathNavigator navigator = new XPathDocument(Settings.SettingsPath).CreateNavigator();
				XPathNodeIterator xpathNodeIterator = navigator.Select(navigator.Compile("/settings/" + pstrValueToRead));
				return xpathNodeIterator.MoveNext() ? xpathNodeIterator.Current.Value : string.Empty;
			}
			catch
			{
				return string.Empty;
			}
		}

		private static string ReadValueSafe(string pstrValueToRead, string defaultValue = "")
		{
			string str = Settings.ReadValue(pstrValueToRead);
			return string.IsNullOrEmpty(str) ? defaultValue : str;
		}

		private static void WriteValue(string pstrValueToRead, string pstrValueToWrite)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				if (File.Exists(Settings.SettingsPath))
				{
					using (XmlTextReader reader = new XmlTextReader(Settings.SettingsPath))
						xmlDocument.Load((XmlReader)reader);
				}
				else
				{
					string directoryName = Path.GetDirectoryName(Settings.SettingsPath);
					if (!Directory.Exists(directoryName))
						Directory.CreateDirectory(directoryName);
					xmlDocument.AppendChild((XmlNode)xmlDocument.CreateElement("settings"));
				}
				XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("/settings/" + pstrValueToRead);
				if (xmlNode == null)
				{
					xmlDocument.SelectSingleNode("settings").AppendChild((XmlNode)xmlDocument.CreateElement(pstrValueToRead)).InnerText = pstrValueToWrite;
					xmlDocument.Save(Settings.SettingsPath);
				}
				else
				{
					xmlNode.InnerText = pstrValueToWrite;
					xmlDocument.Save(Settings.SettingsPath);
				}
			}
			catch
			{
			}
		}
	}
}