

using InvokedClient.IO;
using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;


namespace InvokedClient
{
	internal static class Program
	{
		[DllImport("user32.dll")]
		public static extern bool SetProcessDPIAware();

		[STAThread]
		private static void Main()
		{
			Program.SetProcessDPIAware();
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += new ThreadExceptionEventHandler(Program.HandleThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.HandleUnhandledException);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run((Form)new InvokedClientApplication());
		}

		private static void HandleThreadException(object sender, ThreadExceptionEventArgs e)
		{
			try
			{
				string restartBatch = BatchFile.CreateRestartBatch(Application.ExecutablePath);
				Process.Start(new ProcessStartInfo()
				{
					WindowStyle = ProcessWindowStyle.Hidden,
					UseShellExecute = true,
					FileName = restartBatch
				});
			}
			catch (Exception ex)
			{
			}
			finally
			{
				Environment.Exit(0);
			}
		}

		private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (!e.IsTerminating)
				return;
			try
			{
				string restartBatch = BatchFile.CreateRestartBatch(Application.ExecutablePath);
				Process.Start(new ProcessStartInfo()
				{
					WindowStyle = ProcessWindowStyle.Hidden,
					UseShellExecute = true,
					FileName = restartBatch
				});
			}
			catch (Exception ex)
			{
			}
			finally
			{
				Environment.Exit(0);
			}
		}
	}
}