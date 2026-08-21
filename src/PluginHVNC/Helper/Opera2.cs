

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace Plugin.Helper
{
	internal class Opera2
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			IntPtr lpBuffer,
			UIntPtr dwSize,
			ref UIntPtr lpNumberOfBytesRead);

		[DllImport("ntdll.dll", EntryPoint = "NtWow64ReadVirtualMemory64", SetLastError = true)]
		public static extern int ReadProcessMemory64From32(
			IntPtr ProcessHandle,
			ulong BaseAddress,
			IntPtr Buffer,
			ulong BufferSize,
			ref ulong NumberOfBytesWritten);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern void SetLastError(uint dwErrCode);

		[DllImport("ntdll.dll", EntryPoint = "NtQueryInformationProcess", SetLastError = true)]
		public static extern int NtQueryPbi64From64(
			IntPtr ProcessHandle,
			Opera2.PROCESSINFOCLASS ProcessInformationClass,
			ref Opera2.PROCESS_BASIC_INFORMATION ProcessInformation,
			uint BufferSize,
			ref uint NumberOfBytesRead);

		[DllImport("ntdll.dll", EntryPoint = "NtWow64QueryInformationProcess64", SetLastError = true)]
		public static extern int NtQueryPBI64From32(
			IntPtr ProcessHandle,
			Opera2.PROCESSINFOCLASS ProcessInformationClass,
			ref Opera2.PROCESS_BASIC_INFORMATION64 ProcessInformation,
			uint BufferSize,
			ref uint NumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(
			uint dwDesiredAccess,
			bool bInheritHandle,
			uint dwProcessId);

		public static ulong GetLdr64(ulong addr) => addr + 24UL;

		private static bool ReadProperBitnessProcessMemory(
			IntPtr hProcess,
			ulong lpBaseAddress,
			IntPtr lpBuffer,
			UIntPtr dwSize,
			ref UIntPtr lpNumberOfBytesRead)
		{
			if (Environment.Is64BitProcess)
				return Opera2.ReadProcessMemory(hProcess, (IntPtr) (long) lpBaseAddress, lpBuffer, dwSize, ref lpNumberOfBytesRead);
			ulong NumberOfBytesWritten = 0;
			int dwErrCode = Opera2.ReadProcessMemory64From32(hProcess, lpBaseAddress, lpBuffer, (ulong) dwSize, ref NumberOfBytesWritten);
			int num = dwErrCode == 0 ? 1 : 0;
			if (num == 0)
				Opera2.SetLastError((uint) dwErrCode);
			lpNumberOfBytesRead = (UIntPtr) NumberOfBytesWritten;
			return num != 0;
		}

		public static ulong GetRemoteModuleHandle64Bit(
			IntPtr hProcess,
			string DllBaseName,
			int max_flink_count = 600)
		{
			if (hProcess == IntPtr.Zero)
				throw new Exception("The supplied hProcess is Null!");
			UIntPtr zero = UIntPtr.Zero;
			ulong pebBaseAddress;
			if (Environment.Is64BitProcess)
			{
				Opera2.PROCESS_BASIC_INFORMATION ProcessInformation = new Opera2.PROCESS_BASIC_INFORMATION();
				uint BufferSize = (uint) Marshal.SizeOf<Opera2.PROCESS_BASIC_INFORMATION>(ProcessInformation);
				uint NumberOfBytesRead = 0;
				if (Opera2.NtQueryPbi64From64(hProcess, Opera2.PROCESSINFOCLASS.ProcessBasicInformation, ref ProcessInformation, BufferSize, ref NumberOfBytesRead) != 0 || ProcessInformation.PebBaseAddress == IntPtr.Zero)
					throw new Exception("couldnt read PBI from process!");
				pebBaseAddress = (ulong) (long) ProcessInformation.PebBaseAddress;
			}
			else
			{
				Opera2.PROCESS_BASIC_INFORMATION64 ProcessInformation = new Opera2.PROCESS_BASIC_INFORMATION64();
				uint BufferSize = (uint) Marshal.SizeOf<Opera2.PROCESS_BASIC_INFORMATION64>(ProcessInformation);
				uint NumberOfBytesRead = 0;
				if (Opera2.NtQueryPBI64From32(hProcess, Opera2.PROCESSINFOCLASS.ProcessBasicInformation, ref ProcessInformation, BufferSize, ref NumberOfBytesRead) != 0 || ProcessInformation.PebBaseAddress == 0UL)
					throw new Exception("couldnt read PBI from process!");
				pebBaseAddress = ProcessInformation.PebBaseAddress;
			}
			if (pebBaseAddress == 0UL)
				return 0;
			ulong ldr64 = Opera2.GetLdr64(pebBaseAddress);
			int num1 = Marshal.SizeOf(typeof (Opera2.ULONGRESULT));
			IntPtr num2 = Marshal.AllocHGlobal(num1);
			if (!Opera2.ReadProperBitnessProcessMemory(hProcess, ldr64, num2, (UIntPtr) (ulong) num1, ref zero))
			{
				Marshal.FreeHGlobal(num2);
				throw new Exception("couldnt read pLdrData. ERR CODE: " + Marshal.GetLastWin32Error().ToString());
			}
			ulong lpBaseAddress = Marshal.PtrToStructure<Opera2.ULONGRESULT>(num2).Value;
			Marshal.FreeHGlobal(num2);
			if (lpBaseAddress == 0UL)
				return 0;
			if (!Opera2.ReadProperBitnessProcessMemory(hProcess, ldr64, num2, (UIntPtr) (ulong) num1, ref zero))
			{
				Marshal.FreeHGlobal(num2);
				throw new Exception("couldnt read pLdrData. ERR CODE: " + Marshal.GetLastWin32Error().ToString());
			}
			int num3 = Marshal.SizeOf(typeof (Opera2.PEB_LDR_DATA64));
			IntPtr num4 = Marshal.AllocHGlobal(num3);
			if (!Opera2.ReadProperBitnessProcessMemory(hProcess, lpBaseAddress, num4, (UIntPtr) (ulong) num3, ref zero))
			{
				Marshal.FreeHGlobal(num4);
				throw new Exception("couldnt read ldr64. ERR CODE: " + Marshal.GetLastWin32Error().ToString());
			}
			Opera2.PEB_LDR_DATA64 structure1 = Marshal.PtrToStructure<Opera2.PEB_LDR_DATA64>(num4);
			Marshal.FreeHGlobal(num4);
			ulong flink = structure1.InLoadOrderModuleList.Flink;
			ulong num5 = (ulong) ((uint) ldr64 + (uint) (int) Marshal.OffsetOf(typeof (Opera2.PEB_LDR_DATA64), "InLoadOrderModuleList"));
			int num6 = Marshal.SizeOf(typeof (Opera2.LDR_DATA_TABLE_ENTRY64_SNAP));
			IntPtr num7 = Marshal.AllocHGlobal(num6);
			ulong moduleHandle64Bit = 0;
			int num8 = 0;
			while ((long) flink != (long) num5 && num8 < max_flink_count)
			{
				++num8;
				if (Opera2.ReadProperBitnessProcessMemory(hProcess, flink, num7, (UIntPtr) (ulong) num6, ref zero))
				{
					Opera2.LDR_DATA_TABLE_ENTRY64_SNAP structure2 = Marshal.PtrToStructure<Opera2.LDR_DATA_TABLE_ENTRY64_SNAP>(num7);
					if (DllBaseName == null)
					{
						moduleHandle64Bit = structure2.DllBase;
						break;
					}
					flink = structure2.InLoadOrderLinks.Flink;
					if ((int) structure2.BaseDllName.Length / 2 == DllBaseName.Length)
					{
						IntPtr num9 = Marshal.AllocHGlobal((int) structure2.BaseDllName.Length);
						if (!Opera2.ReadProperBitnessProcessMemory(hProcess, structure2.BaseDllName.Buffer, num9, (UIntPtr) (uint) structure2.BaseDllName.Length, ref zero))
						{
							Marshal.FreeHGlobal(num9);
							break;
						}
						string stringUni = Marshal.PtrToStringUni(num9, (int) structure2.BaseDllName.Length / 2);
						Marshal.FreeHGlobal(num9);
						if (stringUni.ToLower() == DllBaseName.ToLower())
						{
							moduleHandle64Bit = structure2.DllBase;
							break;
						}
					}
				}
				else
					break;
			}
			Marshal.FreeHGlobal(num7);
			return moduleHandle64Bit;
		}

		public static IntPtr GetProcessHandleWithRequiredRights(int pid)
		{
			return Opera2.OpenProcess((uint) (2 | 1024) | 8U | 32U | 16U, false, (uint) pid);
		}

		public static Opera2.IMAGE_NT_HEADERS64 GetNtHeader64(IntPtr hProcess, ulong hModule)
		{
			UIntPtr zero = UIntPtr.Zero;
			int num1 = Marshal.SizeOf(typeof (Opera2.IMAGE_NT_HEADERS64));
			IntPtr num2 = Marshal.AllocHGlobal(num1);
			if (!Opera2.ReadProperBitnessProcessMemory(hProcess, Opera2.GetNtHeader64Addr(hProcess, hModule), num2, (UIntPtr) (ulong) num1, ref zero))
			{
				Marshal.FreeHGlobal(num2);
				throw new Exception("couldnt read NTHeader. ERR CODE: " + Marshal.GetLastWin32Error().ToString());
			}
			Opera2.IMAGE_NT_HEADERS64 structure = Marshal.PtrToStructure<Opera2.IMAGE_NT_HEADERS64>(num2);
			Marshal.FreeHGlobal(num2);
			return structure;
		}

		public static ulong GetNtHeader64Addr(IntPtr hProcess, ulong hModule)
		{
			int num1 = Marshal.SizeOf(typeof (Opera2.IMAGE_DOS_HEADER));
			IntPtr num2 = Marshal.AllocHGlobal(num1);
			UIntPtr zero = UIntPtr.Zero;
			if (!Opera2.ReadProperBitnessProcessMemory(hProcess, hModule, num2, (UIntPtr) (ulong) num1, ref zero))
			{
				Marshal.FreeHGlobal(num2);
				throw new Exception("couldnt read DosHeader. ERR CODE: " + Marshal.GetLastWin32Error().ToString());
			}
			Opera2.IMAGE_DOS_HEADER structure = Marshal.PtrToStructure<Opera2.IMAGE_DOS_HEADER>(num2);
			Marshal.FreeHGlobal(num2);
			return hModule + (ulong) (uint) structure.e_lfanew;
		}

		private static string ReadRemoteAnsiString64(IntPtr hProcess, ulong lpAddress)
		{
			string str = "";
			UIntPtr zero = UIntPtr.Zero;
			IntPtr num1;
			for (num1 = Marshal.AllocHGlobal(1); Opera2.ReadProperBitnessProcessMemory(hProcess, lpAddress, num1, (UIntPtr) 1UL, ref zero); ++lpAddress)
			{
				byte num2 = Marshal.ReadByte(num1);
				if (num2 == (byte) 0)
					return str;
				str += ((char) num2).ToString();
			}
			Marshal.FreeHGlobal(num1);
			throw new Exception("couldnt read AnsiString. ERR CODE: " + Marshal.GetLastWin32Error().ToString());
		}

		public static ulong GetRemoteProcAddress64Bit(
			IntPtr hProcess,
			ulong hModule,
			string FunctionName)
		{
			if (hModule == 0UL)
				throw new Exception("couldnt read hModule is null or 0.");
			UIntPtr zero = UIntPtr.Zero;
			Opera2.IMAGE_DATA_DIRECTORY imageDataDirectory = Opera2.GetNtHeader64(hProcess, hModule).OptionalHeader.DataDirectory[0];
			if (imageDataDirectory.Size == 0U || imageDataDirectory.VirtualAddress == 0U)
				return 0;
			int num1 = Marshal.SizeOf(typeof (Opera2.IMAGE_EXPORT_DIRECTORY));
			IntPtr num2 = Marshal.AllocHGlobal(num1);
			if (!Opera2.ReadProperBitnessProcessMemory(hProcess, hModule + (ulong) imageDataDirectory.VirtualAddress, num2, (UIntPtr) (ulong) num1, ref zero))
			{
				Marshal.FreeHGlobal(num2);
				throw new Exception("couldnt read IMAGE EXPORT DIRECTORY. ERR CODE: " + Marshal.GetLastWin32Error().ToString());
			}
			Opera2.IMAGE_EXPORT_DIRECTORY structure = Marshal.PtrToStructure<Opera2.IMAGE_EXPORT_DIRECTORY>(num2);
			Marshal.FreeHGlobal(num2);
			if (structure.NumberOfNames == 0U)
				return 0;
			int num3 = (int) structure.NumberOfFunctions * Marshal.SizeOf(typeof (int));
			IntPtr num4 = Marshal.AllocHGlobal(num3);
			if (!Opera2.ReadProperBitnessProcessMemory(hProcess, hModule + (ulong) structure.AddressOfFunctions, num4, (UIntPtr) (ulong) num3, ref zero))
			{
				Marshal.FreeHGlobal(num4);
				throw new Exception("couldnt read RVA table. ERR CODE: " + Marshal.GetLastWin32Error().ToString());
			}
			uint[] numArray1 = new uint[(int) structure.NumberOfFunctions];
			for (int index = 0; index < (int) structure.NumberOfFunctions; ++index)
				numArray1[index] = Marshal.PtrToStructure<Opera2.UINTRESULT>(num4 + index * 4).Value;
			Marshal.FreeHGlobal(num4);
			int num5 = (int) structure.NumberOfFunctions * Marshal.SizeOf(typeof (short));
			IntPtr num6 = Marshal.AllocHGlobal(num5);
			if (!Opera2.ReadProperBitnessProcessMemory(hProcess, hModule + (ulong) structure.AddressOfNameOrdinals, num6, (UIntPtr) (ulong) num5, ref zero))
			{
				Marshal.FreeHGlobal(num6);
				throw new Exception("couldnt read ORD table. ERR CODE: " + Marshal.GetLastWin32Error().ToString());
			}
			ushort[] numArray2 = new ushort[(int) structure.NumberOfFunctions];
			for (int index = 0; index < (int) structure.NumberOfFunctions; ++index)
				numArray2[index] = Marshal.PtrToStructure<Opera2.USHORTRESULT>(num6 + index * 2).Value;
			Marshal.FreeHGlobal(num6);
			int num7 = (int) structure.NumberOfNames * Marshal.SizeOf(typeof (int));
			IntPtr num8 = Marshal.AllocHGlobal(num7);
			if (!Opera2.ReadProperBitnessProcessMemory(hProcess, hModule + (ulong) structure.AddressOfNames, num8, (UIntPtr) (ulong) num7, ref zero))
			{
				Marshal.FreeHGlobal(num8);
				throw new Exception("couldnt read NAME table. ERR CODE: " + Marshal.GetLastWin32Error().ToString());
			}
			uint[] numArray3 = new uint[(int) structure.NumberOfNames];
			for (int index = 0; index < (int) structure.NumberOfNames; ++index)
				numArray3[index] = Marshal.PtrToStructure<Opera2.UINTRESULT>(num8 + index * 4).Value;
			Marshal.FreeHGlobal(num8);
			int num9 = FunctionName.Length + 1;
			IntPtr num10 = Marshal.AllocHGlobal(num9);
			ulong procAddress64Bit = 0;
			for (int index = 0; index < numArray3.Length; ++index)
			{
				if (!Opera2.ReadProperBitnessProcessMemory(hProcess, hModule + (ulong) numArray3[index], num10, (UIntPtr) (ulong) num9, ref zero))
				{
					Marshal.FreeHGlobal(num10);
					throw new Exception("couldnt read name buffer. ERR CODE: " + Marshal.GetLastWin32Error().ToString());
				}
				byte num11 = Marshal.ReadByte(num10 + num9 - 2);
				if (Marshal.ReadByte(num10 + num9 - 1) == (byte) 0 && num11 != (byte) 0)
				{
					uint num12 = numArray1[(int) numArray2[index]];
					if (Marshal.PtrToStringAnsi(num10) == FunctionName)
					{
						if (num12 >= imageDataDirectory.VirtualAddress && num12 < imageDataDirectory.VirtualAddress + imageDataDirectory.Size)
						{
							string str = Opera2.ReadRemoteAnsiString64(hProcess, hModule + (ulong) num12);
							if (!str.Contains("."))
							{
								Marshal.FreeHGlobal(num10);
								throw new Exception("Couldnt the Forwarder info!");
							}
							string[] source = str.Split('.');
							string DllBaseName = string.Join(".", ((IEnumerable<string>) source).Take<string>(source.Length - 1).ToArray<string>()) + ".dll";
							string FunctionName1 = source[source.Length - 1];
							if (FunctionName1.Contains("#"))
							{
								Marshal.FreeHGlobal(num10);
								throw new Exception("Ordinal forwarder function is not supported at this time!");
							}
							ulong moduleHandle64Bit = Opera2.GetRemoteModuleHandle64Bit(hProcess, DllBaseName);
							if (moduleHandle64Bit == 0UL)
							{
								Marshal.FreeHGlobal(num10);
								throw new Exception("Couldnt the Forwarder dll!");
							}
							return Opera2.GetRemoteProcAddress64Bit(hProcess, moduleHandle64Bit, FunctionName1);
						}
						procAddress64Bit = hModule + (ulong) num12;
						break;
					}
				}
			}
			Marshal.FreeHGlobal(num10);
			return procAddress64Bit;
		}

		public struct PROCESS_BASIC_INFORMATION
		{
			public int ExitStatus;
			public IntPtr PebBaseAddress;
			public UIntPtr AffinityMask;
			public uint BasePriority;
			public UIntPtr UniqueProcessId;
			public UIntPtr InheritedFromUniqueProcessId;
		}

		public enum PROCESSINFOCLASS : uint
		{
			ProcessBasicInformation,
			ProcessQuotaLimits,
			ProcessIoCounters,
			ProcessVmCounters,
			ProcessTimes,
			ProcessBasePriority,
			ProcessRaisePriority,
			ProcessDebugPort,
			ProcessExceptionPort,
			ProcessAccessToken,
			ProcessLdtInformation,
			ProcessLdtSize,
			ProcessDefaultHardErrorMode,
			ProcessIoPortHandlers,
			ProcessPooledUsageAndLimits,
			ProcessWorkingSetWatch,
			ProcessUserModeIOPL,
			ProcessEnableAlignmentFaultFixup,
			ProcessPriorityClass,
			ProcessWx86Information,
			ProcessHandleCount,
			ProcessAffinityMask,
			ProcessPriorityBoost,
			ProcessDeviceMap,
			ProcessSessionInformation,
			ProcessForegroundInformation,
			ProcessWow64Information,
			ProcessImageFileName,
			ProcessLUIDDeviceMapsEnabled,
			ProcessBreakOnTermination,
			ProcessDebugObjectHandle,
			ProcessDebugFlags,
			ProcessHandleTracing,
			ProcessIoPriority,
			ProcessExecuteFlags,
			ProcessResourceManagement,
			ProcessCookie,
			ProcessImageInformation,
			ProcessCycleTime,
			ProcessPagePriority,
			ProcessInstrumentationCallback,
			ProcessThreadStackAllocation,
			ProcessWorkingSetWatchEx,
			ProcessImageFileNameWin32,
			ProcessImageFileMapping,
			ProcessAffinityUpdateMode,
			ProcessMemoryAllocationMode,
			ProcessGroupInformation,
			ProcessTokenVirtualizationEnabled,
			ProcessConsoleHostProcess,
			ProcessWindowInformation,
			ProcessHandleInformation,
			ProcessMitigationPolicy,
			ProcessDynamicFunctionTableInformation,
			ProcessHandleCheckingMode,
			ProcessKeepAliveCount,
			ProcessRevokeFileHandles,
			ProcessWorkingSetControl,
			ProcessHandleTable,
			ProcessCheckStackExtentsMode,
			ProcessCommandLineInformation,
			ProcessProtectionInformation,
			ProcessMemoryExhaustion,
			ProcessFaultInformation,
			ProcessTelemetryIdInformation,
			ProcessCommitReleaseInformation,
			ProcessDefaultCpuSetsInformation,
			ProcessAllowedCpuSetsInformation,
			ProcessSubsystemProcess,
			ProcessJobMemoryInformation,
			ProcessInPrivate,
			ProcessRaiseUMExceptionOnInvalidHandleClose,
			ProcessIumChallengeResponse,
			ProcessChildProcessInformation,
			ProcessHighGraphicsPriorityInformation,
			ProcessSubsystemInformation,
			ProcessEnergyValues,
			ProcessActivityThrottleState,
			ProcessActivityThrottlePolicy,
			ProcessWin32kSyscallFilterInformation,
			ProcessDisableSystemAllowedCpuSets,
			ProcessWakeInformation,
			ProcessEnergyTrackingState,
			ProcessManageWritesToExecutableMemory,
			ProcessCaptureTrustletLiveDump,
			ProcessTelemetryCoverage,
			ProcessEnclaveInformation,
			ProcessEnableReadWriteVmLogging,
			ProcessUptimeInformation,
			ProcessImageSection,
			ProcessDebugAuthInformation,
			ProcessSystemResourceManagement,
			ProcessSequenceNumber,
			ProcessLoaderDetour,
			ProcessSecurityDomainInformation,
			ProcessCombineSecurityDomainsInformation,
			ProcessEnableLogging,
			ProcessLeapSecondInformation,
			ProcessFiberShadowStackAllocation,
			ProcessFreeFiberShadowStackAllocation,
			MaxProcessInfoClass,
		}

		public struct PROCESS_BASIC_INFORMATION64
		{
			public int ExitStatus;
			public ulong PebBaseAddress;
			public ulong AffinityMask;
			public uint BasePriority;
			public ulong UniqueProcessId;
			public ulong InheritedFromUniqueProcessId;
		}

		public struct LIST_ENTRY64
		{
			public ulong Flink;
			public ulong Blink;
		}

		public struct PEB_LDR_DATA64
		{
			public uint Length;
			public bool Initialized;
			public ulong SsHandle;
			public Opera2.LIST_ENTRY64 InLoadOrderModuleList;
			public Opera2.LIST_ENTRY64 InMemoryOrderModuleList;
			public Opera2.LIST_ENTRY64 InInitializationOrderModuleList;
			public ulong EntryInProgress;
			public bool ShutdownInProgress;
			public ulong ShutdownThreadId;
		}

		public struct LDR_DATA_TABLE_ENTRY64_SNAP
		{
			public Opera2.LIST_ENTRY64 InLoadOrderLinks;
			public Opera2.LIST_ENTRY64 InMemoryOrderLinks;
			public Opera2.LIST_ENTRY64 InInitializationOrderLinks;
			public ulong DllBase;
			public ulong EntryPoint;
			public uint SizeOfImage;
			public Opera2.UNICODE_STRING64 FullDllName;
			public Opera2.UNICODE_STRING64 BaseDllName;
		}

		public struct UNICODE_STRING64
		{
			public ushort Length;
			public ushort MaximumLength;
			public ulong Buffer;
		}

		public struct ULONGRESULT
		{
			public ulong Value;
		}

		public struct IMAGE_NT_HEADERS64
		{
			public uint Signature;
			public Opera2.IMAGE_FILE_HEADER FileHeader;
			public Opera2.IMAGE_OPTIONAL_HEADER64 OptionalHeader;
		}

		public struct IMAGE_OPTIONAL_HEADER64
		{
			public ushort Magic;
			public byte MajorLinkerVersion;
			public byte MinorLinkerVersion;
			public uint SizeOfCode;
			public uint SizeOfInitializedData;
			public uint SizeOfUninitializedData;
			public uint AddressOfEntryPoint;
			public uint BaseOfCode;
			public ulong ImageBase;
			public uint SectionAlignment;
			public uint FileAlignment;
			public ushort MajorOperatingSystemVersion;
			public ushort MinorOperatingSystemVersion;
			public ushort MajorImageVersion;
			public ushort MinorImageVersion;
			public ushort MajorSubsystemVersion;
			public ushort MinorSubsystemVersion;
			public uint Win32VersionValue;
			public uint SizeOfImage;
			public uint SizeOfHeaders;
			public uint CheckSum;
			public ushort Subsystem;
			public ushort DllCharacteristics;
			public ulong SizeOfStackReserve;
			public ulong SizeOfStackCommit;
			public ulong SizeOfHeapReserve;
			public ulong SizeOfHeapCommit;
			public uint LoaderFlags;
			public uint NumberOfRvaAndSizes;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public Opera2.IMAGE_DATA_DIRECTORY[] DataDirectory;
		}

		public struct IMAGE_FILE_HEADER
		{
			public ushort Machine;
			public ushort NumberOfSections;
			public uint TimeDateStamp;
			public uint PointerToSymbolTable;
			public uint NumberOfSymbols;
			public ushort SizeOfOptionalHeader;
			public ushort Characteristics;
		}

		public struct IMAGE_OPTIONAL_HEADER32
		{
			public ushort Magic;
			public byte MajorLinkerVersion;
			public byte MinorLinkerVersion;
			public uint SizeOfCode;
			public uint SizeOfInitializedData;
			public uint SizeOfUninitializedData;
			public uint AddressOfEntryPoint;
			public uint BaseOfCode;
			public uint BaseOfData;
			public uint ImageBase;
			public uint SectionAlignment;
			public uint FileAlignment;
			public ushort MajorOperatingSystemVersion;
			public ushort MinorOperatingSystemVersion;
			public ushort MajorImageVersion;
			public ushort MinorImageVersion;
			public ushort MajorSubsystemVersion;
			public ushort MinorSubsystemVersion;
			public uint Win32VersionValue;
			public uint SizeOfImage;
			public uint SizeOfHeaders;
			public uint CheckSum;
			public ushort Subsystem;
			public ushort DllCharacteristics;
			public uint SizeOfStackReserve;
			public uint SizeOfStackCommit;
			public uint SizeOfHeapReserve;
			public uint SizeOfHeapCommit;
			public uint LoaderFlags;
			public uint NumberOfRvaAndSizes;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public Opera2.IMAGE_DATA_DIRECTORY[] DataDirectory;
		}

		public struct IMAGE_DATA_DIRECTORY
		{
			public uint VirtualAddress;
			public uint Size;
		}

		public struct IMAGE_DOS_HEADER
		{
			public ushort e_magic;
			public ushort e_cblp;
			public ushort e_cp;
			public ushort e_crlc;
			public ushort e_cparhdr;
			public ushort e_minalloc;
			public ushort e_maxalloc;
			public ushort e_ss;
			public ushort e_sp;
			public ushort e_csum;
			public ushort e_ip;
			public ushort e_cs;
			public ushort e_lfarlc;
			public ushort e_ovno;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public ushort[] e_res;
			public ushort e_oemid;
			public ushort e_oeminfo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public ushort[] e_res2;
			public int e_lfanew;
		}

		public struct IMAGE_EXPORT_DIRECTORY
		{
			public uint Characteristics;
			public uint TimeDateStamp;
			public ushort MajorVersion;
			public ushort MinorVersion;
			public uint Name;
			public uint Base;
			public uint NumberOfFunctions;
			public uint NumberOfNames;
			public uint AddressOfFunctions;
			public uint AddressOfNames;
			public uint AddressOfNameOrdinals;
		}

		public struct UINTRESULT
		{
			public uint Value;
		}

		public struct USHORTRESULT
		{
			public ushort Value;
		}
	}
}
