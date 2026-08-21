

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;


namespace Plugin.Helper
{
	internal class _ProcessHelper
	{
		private const uint SE_GROUP_INTEGRITY = 32;

		public static bool RunAsRestrictedUser(string fileName, string DesktopName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof (fileName));
			IntPtr token;
			if (!_ProcessHelper.GetRestrictedSessionUserToken(out token))
				return false;
			try
			{
				_ProcessHelper.STARTUPINFO lpStartupInfo = new _ProcessHelper.STARTUPINFO();
				lpStartupInfo.cb = Marshal.SizeOf<_ProcessHelper.STARTUPINFO>(lpStartupInfo);
				lpStartupInfo.lpDesktop = DesktopName;
				_ProcessHelper.PROCESS_INFORMATION lpProcessInformation = new _ProcessHelper.PROCESS_INFORMATION();
				StringBuilder lpCommandLine = new StringBuilder();
				lpCommandLine.Append(fileName);
				return _ProcessHelper.CreateProcessAsUser(token, (string) null, lpCommandLine, IntPtr.Zero, IntPtr.Zero, true, 0U, IntPtr.Zero, Path.GetDirectoryName(fileName), ref lpStartupInfo, out lpProcessInformation);
			}
			finally
			{
				_ProcessHelper.CloseHandle(token);
			}
		}

		private static bool GetRestrictedSessionUserToken(out IntPtr token)
		{
			token = IntPtr.Zero;
			IntPtr pLevelHandle;
			if (!_ProcessHelper.SaferCreateLevel(_ProcessHelper.SaferScope.User, _ProcessHelper.SaferLevel.NormalUser, _ProcessHelper.SaferOpenFlags.Open, out pLevelHandle, IntPtr.Zero))
				return false;
			IntPtr OutAccessToken = IntPtr.Zero;
			_ProcessHelper.TOKEN_MANDATORY_LABEL structure = new _ProcessHelper.TOKEN_MANDATORY_LABEL();
			structure.Label.Sid = IntPtr.Zero;
			IntPtr num = IntPtr.Zero;
			try
			{
				if (!_ProcessHelper.SaferComputeTokenFromLevel(pLevelHandle, IntPtr.Zero, out OutAccessToken, 0, IntPtr.Zero))
					return false;
				structure.Label.Attributes = 32U;
				structure.Label.Sid = IntPtr.Zero;
				if (!_ProcessHelper.ConvertStringSidToSid("S-1-16-8192", out structure.Label.Sid))
					return false;
				num = Marshal.AllocHGlobal(Marshal.SizeOf<_ProcessHelper.TOKEN_MANDATORY_LABEL>(structure));
				Marshal.StructureToPtr<_ProcessHelper.TOKEN_MANDATORY_LABEL>(structure, num, false);
				if (!_ProcessHelper.SetTokenInformation(OutAccessToken, _ProcessHelper.TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, num, (uint) Marshal.SizeOf<_ProcessHelper.TOKEN_MANDATORY_LABEL>(structure)))
					return false;
				token = OutAccessToken;
				OutAccessToken = IntPtr.Zero;
			}
			finally
			{
				_ProcessHelper.SaferCloseLevel(pLevelHandle);
				if (structure.Label.Sid != IntPtr.Zero)
					_ProcessHelper.LocalFree(structure.Label.Sid);
				if (num != IntPtr.Zero)
					Marshal.FreeHGlobal(num);
			}
			return true;
		}

		[DllImport("advapi32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		private static extern bool SaferCreateLevel(
			_ProcessHelper.SaferScope scope,
			_ProcessHelper.SaferLevel level,
			_ProcessHelper.SaferOpenFlags openFlags,
			out IntPtr pLevelHandle,
			IntPtr lpReserved);

		[DllImport("advapi32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		private static extern bool SaferComputeTokenFromLevel(
			IntPtr LevelHandle,
			IntPtr InAccessToken,
			out IntPtr OutAccessToken,
			int dwFlags,
			IntPtr lpReserved);

		[DllImport("advapi32", SetLastError = true)]
		private static extern bool SaferCloseLevel(IntPtr hLevelHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern bool ConvertStringSidToSid(string StringSid, out IntPtr ptrSid);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr hObject);

		private static bool SafeCloseHandle(IntPtr hObject)
		{
			return hObject == IntPtr.Zero || _ProcessHelper.CloseHandle(hObject);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr LocalFree(IntPtr hMem);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool SetTokenInformation(
			IntPtr TokenHandle,
			_ProcessHelper.TOKEN_INFORMATION_CLASS TokenInformationClass,
			IntPtr TokenInformation,
			uint TokenInformationLength);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern bool CreateProcessAsUser(
			IntPtr hToken,
			string lpApplicationName,
			StringBuilder lpCommandLine,
			IntPtr lpProcessAttributes,
			IntPtr lpThreadAttributes,
			bool bInheritHandles,
			uint dwCreationFlags,
			IntPtr lpEnvironment,
			string lpCurrentDirectory,
			ref _ProcessHelper.STARTUPINFO lpStartupInfo,
			out _ProcessHelper.PROCESS_INFORMATION lpProcessInformation);

		private struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
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

		private struct SID_AND_ATTRIBUTES
		{
			public IntPtr Sid;
			public uint Attributes;
		}

		private struct TOKEN_MANDATORY_LABEL
		{
			public _ProcessHelper.SID_AND_ATTRIBUTES Label;
		}

		public enum SaferLevel : uint
		{
			Disallowed = 0,
			Untrusted = 4096, // 0x00001000
			Constrained = 65536, // 0x00010000
			NormalUser = 131072, // 0x00020000
			FullyTrusted = 262144, // 0x00040000
		}

		public enum SaferScope : uint
		{
			Machine = 1,
			User = 2,
		}

		[Flags]
		public enum SaferOpenFlags : uint
		{
			Open = 1,
		}

		private enum TOKEN_INFORMATION_CLASS
		{
			TokenUser = 1,
			TokenGroups = 2,
			TokenPrivileges = 3,
			TokenOwner = 4,
			TokenPrimaryGroup = 5,
			TokenDefaultDacl = 6,
			TokenSource = 7,
			TokenType = 8,
			TokenImpersonationLevel = 9,
			TokenStatistics = 10, // 0x0000000A
			TokenRestrictedSids = 11, // 0x0000000B
			TokenSessionId = 12, // 0x0000000C
			TokenGroupsAndPrivileges = 13, // 0x0000000D
			TokenSessionReference = 14, // 0x0000000E
			TokenSandBoxInert = 15, // 0x0000000F
			TokenAuditPolicy = 16, // 0x00000010
			TokenOrigin = 17, // 0x00000011
			TokenElevationType = 18, // 0x00000012
			TokenLinkedToken = 19, // 0x00000013
			TokenElevation = 20, // 0x00000014
			TokenHasRestrictions = 21, // 0x00000015
			TokenAccessInformation = 22, // 0x00000016
			TokenVirtualizationAllowed = 23, // 0x00000017
			TokenVirtualizationEnabled = 24, // 0x00000018
			TokenIntegrityLevel = 25, // 0x00000019
			TokenUIAccess = 26, // 0x0000001A
			TokenMandatoryPolicy = 27, // 0x0000001B
			TokenLogonSid = 28, // 0x0000001C
			MaxTokenInfoClass = 29, // 0x0000001D
		}
	}
}
