

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;


namespace InvokedServer.Models
{
	public class BuilderProfile
	{
		private readonly string _profilePath;

		public string Hosts
		{
			get => this.ReadValueSafe(nameof(Hosts));
			set => this.WriteValue(nameof(Hosts), value);
		}

		public string Tag
		{
			get => this.ReadValueSafe(nameof(Tag), "Office04");
			set => this.WriteValue(nameof(Tag), value);
		}

		public int Delay
		{
			get => int.Parse(this.ReadValueSafe(nameof(Delay), "3000"));
			set => this.WriteValue(nameof(Delay), value.ToString());
		}

		public string Mutex
		{
			get => this.ReadValueSafe(nameof(Mutex), Guid.NewGuid().ToString());
			set => this.WriteValue(nameof(Mutex), value);
		}

		public bool UnattendedMode
		{
			get => bool.Parse(this.ReadValueSafe(nameof(UnattendedMode), "False"));
			set => this.WriteValue(nameof(UnattendedMode), value.ToString());
		}

		public bool InstallClient
		{
			get => bool.Parse(this.ReadValueSafe(nameof(InstallClient), "False"));
			set => this.WriteValue(nameof(InstallClient), value.ToString());
		}

		public string InstallName
		{
			get => this.ReadValueSafe(nameof(InstallName), "Client");
			set => this.WriteValue(nameof(InstallName), value);
		}

		public short InstallPath
		{
			get => short.Parse(this.ReadValueSafe(nameof(InstallPath), "1"));
			set => this.WriteValue(nameof(InstallPath), value.ToString());
		}

		public string InstallSub
		{
			get => this.ReadValueSafe(nameof(InstallSub), "SubDir");
			set => this.WriteValue(nameof(InstallSub), value);
		}

		public bool HideFile
		{
			get => bool.Parse(this.ReadValueSafe(nameof(HideFile), "False"));
			set => this.WriteValue(nameof(HideFile), value.ToString());
		}

		public bool HideSubDirectory
		{
			get => bool.Parse(this.ReadValueSafe(nameof(HideSubDirectory), "False"));
			set => this.WriteValue(nameof(HideSubDirectory), value.ToString());
		}

		public bool AddStartup
		{
			get => bool.Parse(this.ReadValueSafe(nameof(AddStartup), "False"));
			set => this.WriteValue(nameof(AddStartup), value.ToString());
		}

		public string RegistryName
		{
			get => this.ReadValueSafe(nameof(RegistryName), "Quasar Client Startup");
			set => this.WriteValue(nameof(RegistryName), value);
		}

		public bool ChangeIcon
		{
			get => bool.Parse(this.ReadValueSafe(nameof(ChangeIcon), "False"));
			set => this.WriteValue(nameof(ChangeIcon), value.ToString());
		}

		public string IconPath
		{
			get => this.ReadValueSafe(nameof(IconPath));
			set => this.WriteValue(nameof(IconPath), value);
		}

		public bool ChangeAsmInfo
		{
			get => bool.Parse(this.ReadValueSafe(nameof(ChangeAsmInfo), "False"));
			set => this.WriteValue(nameof(ChangeAsmInfo), value.ToString());
		}

		public bool Keylogger
		{
			get => bool.Parse(this.ReadValueSafe(nameof(Keylogger), "False"));
			set => this.WriteValue(nameof(Keylogger), value.ToString());
		}

		public string LogDirectoryName
		{
			get => this.ReadValueSafe(nameof(LogDirectoryName), "Logs");
			set => this.WriteValue(nameof(LogDirectoryName), value);
		}

		public bool HideLogDirectory
		{
			get => bool.Parse(this.ReadValueSafe(nameof(HideLogDirectory), "False"));
			set => this.WriteValue(nameof(HideLogDirectory), value.ToString());
		}

		public string ProductName
		{
			get => this.ReadValueSafe(nameof(ProductName));
			set => this.WriteValue(nameof(ProductName), value);
		}

		public string Description
		{
			get => this.ReadValueSafe(nameof(Description));
			set => this.WriteValue(nameof(Description), value);
		}

		public string CompanyName
		{
			get => this.ReadValueSafe(nameof(CompanyName));
			set => this.WriteValue(nameof(CompanyName), value);
		}

		public string Copyright
		{
			get => this.ReadValueSafe(nameof(Copyright));
			set => this.WriteValue(nameof(Copyright), value);
		}

		public string Trademarks
		{
			get => this.ReadValueSafe(nameof(Trademarks));
			set => this.WriteValue(nameof(Trademarks), value);
		}

		public string OriginalFilename
		{
			get => this.ReadValueSafe(nameof(OriginalFilename));
			set => this.WriteValue(nameof(OriginalFilename), value);
		}

		public string ProductVersion
		{
			get => this.ReadValueSafe(nameof(ProductVersion));
			set => this.WriteValue(nameof(ProductVersion), value);
		}

		public string FileVersion
		{
			get => this.ReadValueSafe(nameof(FileVersion));
			set => this.WriteValue(nameof(FileVersion), value);
		}

		public BuilderProfile(string profileName)
		{
			this._profilePath = !string.IsNullOrEmpty(profileName) ? Path.Combine(Application.StartupPath, "Profiles\\" + profileName + ".xml") : throw new ArgumentException("Invalid Profile Path");
		}

		private string ReadValue(string pstrValueToRead)
		{
			try
			{
				XPathNavigator navigator = new XPathDocument(this._profilePath).CreateNavigator();
				XPathNodeIterator xpathNodeIterator = navigator.Select(navigator.Compile("/settings/" + pstrValueToRead));
				return xpathNodeIterator.MoveNext() ? xpathNodeIterator.Current.Value : string.Empty;
			}
			catch
			{
				return string.Empty;
			}
		}

		private string ReadValueSafe(string pstrValueToRead, string defaultValue = "")
		{
			string str = this.ReadValue(pstrValueToRead);
			return string.IsNullOrEmpty(str) ? defaultValue : str;
		}

		private void WriteValue(string pstrValueToRead, string pstrValueToWrite)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				if (File.Exists(this._profilePath))
				{
					using (XmlTextReader reader = new XmlTextReader(this._profilePath))
						xmlDocument.Load((XmlReader)reader);
				}
				else
				{
					string directoryName = Path.GetDirectoryName(this._profilePath);
					if (!Directory.Exists(directoryName))
						Directory.CreateDirectory(directoryName);
					xmlDocument.AppendChild((XmlNode)xmlDocument.CreateElement("settings"));
				}
				XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("/settings/" + pstrValueToRead);
				if (xmlNode == null)
				{
					xmlDocument.SelectSingleNode("settings").AppendChild((XmlNode)xmlDocument.CreateElement(pstrValueToRead)).InnerText = pstrValueToWrite;
					xmlDocument.Save(this._profilePath);
				}
				else
				{
					xmlNode.InnerText = pstrValueToWrite;
					xmlDocument.Save(this._profilePath);
				}
			}
			catch
			{
			}
		}
	}
}
