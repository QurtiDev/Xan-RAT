

using System;
using System.Runtime.InteropServices;


namespace Plugin.Helper.Stealer
{
	public static class InternalStructs
	{
		public const int CCH_RM_MAX_APP_NAME = 255;
		public const int CCH_RM_MAX_SVC_NAME = 63;

		public struct ParentProcessUtilities
		{
			internal IntPtr Reserved1;
			internal IntPtr PebBaseAddress;
			internal IntPtr Reserved2_0;
			internal IntPtr Reserved2_1;
			internal IntPtr UniqueProcessId;
			internal IntPtr InheritedFromUniqueProcessId;
		}

		public struct SYSTEM_HANDLE_INFORMATION
		{
			public uint NumberOfHandles;
			public IntPtr Handles;
		}

		public struct SYSTEM_HANDLE_TABLE_ENTRY_INFO
		{
			public ushort UniqueProcessId;
			public ushort CreatorBackTraceIndex;
			public byte ObjectTypeIndex;
			public byte HandleAttributes;
			public ushort HandleValue;
			public IntPtr pAddress;
			public uint dwGrantedAccess;
		}

		public struct UintResult
		{
			public uint Value;
		}

		public enum PROCESSINFOCLASS
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

		public enum SYSTEM_INFORMATION_CLASS
		{
			SystemBasicInformation,
			SystemProcessorInformation,
			SystemPerformanceInformation,
			SystemTimeOfDayInformation,
			SystemPathInformation,
			SystemProcessInformation,
			SystemCallCountInformation,
			SystemDeviceInformation,
			SystemProcessorPerformanceInformation,
			SystemFlagsInformation,
			SystemCallTimeInformation,
			SystemModuleInformation,
			SystemLocksInformation,
			SystemStackTraceInformation,
			SystemPagedPoolInformation,
			SystemNonPagedPoolInformation,
			SystemHandleInformation,
			SystemObjectInformation,
			SystemPageFileInformation,
			SystemVdmInstemulInformation,
			SystemVdmBopInformation,
			SystemFileCacheInformation,
			SystemPoolTagInformation,
			SystemInterruptInformation,
			SystemDpcBehaviorInformation,
			SystemFullMemoryInformation,
			SystemLoadGdiDriverInformation,
			SystemUnloadGdiDriverInformation,
			SystemTimeAdjustmentInformation,
			SystemSummaryMemoryInformation,
			SystemMirrorMemoryInformation,
			SystemPerformanceTraceInformation,
			SystemObsolete0,
			SystemExceptionInformation,
			SystemCrashDumpStateInformation,
			SystemKernelDebuggerInformation,
			SystemContextSwitchInformation,
			SystemRegistryQuotaInformation,
			SystemExtendServiceTableInformation,
			SystemPrioritySeperation,
			SystemVerifierAddDriverInformation,
			SystemVerifierRemoveDriverInformation,
			SystemProcessorIdleInformation,
			SystemLegacyDriverInformation,
			SystemCurrentTimeZoneInformation,
			SystemLookasideInformation,
			SystemTimeSlipNotification,
			SystemSessionCreate,
			SystemSessionDetach,
			SystemSessionInformation,
			SystemRangeStartInformation,
			SystemVerifierInformation,
			SystemVerifierThunkExtend,
			SystemSessionProcessInformation,
			SystemLoadGdiDriverInSystemSpace,
			SystemNumaProcessorMap,
			SystemPrefetcherInformation,
			SystemExtendedProcessInformation,
			SystemRecommendedSharedDataAlignment,
			SystemComPlusPackage,
			SystemNumaAvailableMemory,
			SystemProcessorPowerInformation,
			SystemEmulationBasicInformation,
			SystemEmulationProcessorInformation,
			SystemExtendedHandleInformation,
			SystemLostDelayedWriteInformation,
			SystemBigPoolInformation,
			SystemSessionPoolTagInformation,
			SystemSessionMappedViewInformation,
			SystemHotpatchInformation,
			SystemObjectSecurityMode,
			SystemWatchdogTimerHandler,
			SystemWatchdogTimerInformation,
			SystemLogicalProcessorInformation,
			SystemWow64SharedInformationObsolete,
			SystemRegisterFirmwareTableInformationHandler,
			SystemFirmwareTableInformation,
			SystemModuleInformationEx,
			SystemVerifierTriageInformation,
			SystemSuperfetchInformation,
			SystemMemoryListInformation,
			SystemFileCacheInformationEx,
			SystemThreadPriorityClientIdInformation,
			SystemProcessorIdleCycleTimeInformation,
			SystemVerifierCancellationInformation,
			SystemProcessorPowerInformationEx,
			SystemRefTraceInformation,
			SystemSpecialPoolInformation,
			SystemProcessIdInformation,
			SystemErrorPortInformation,
			SystemBootEnvironmentInformation,
			SystemHypervisorInformation,
			SystemVerifierInformationEx,
			SystemTimeZoneInformation,
			SystemImageFileExecutionOptionsInformation,
			SystemCoverageInformation,
			SystemPrefetchPatchInformation,
			SystemVerifierFaultsInformation,
			SystemSystemPartitionInformation,
			SystemSystemDiskInformation,
			SystemProcessorPerformanceDistribution,
			SystemNumaProximityNodeInformation,
			SystemDynamicTimeZoneInformation,
			SystemCodeIntegrityInformation,
			SystemProcessorMicrocodeUpdateInformation,
			SystemProcessorBrandString,
			SystemVirtualAddressInformation,
			SystemLogicalProcessorAndGroupInformation,
			SystemProcessorCycleTimeInformation,
			SystemStoreInformation,
			SystemRegistryAppendString,
			SystemAitSamplingValue,
			SystemVhdBootInformation,
			SystemCpuQuotaInformation,
			SystemNativeBasicInformation,
			SystemErrorPortTimeouts,
			SystemLowPriorityIoInformation,
			SystemBootEntropyInformation,
			SystemVerifierCountersInformation,
			SystemPagedPoolInformationEx,
			SystemSystemPtesInformationEx,
			SystemNodeDistanceInformation,
			SystemAcpiAuditInformation,
			SystemBasicPerformanceInformation,
			SystemQueryPerformanceCounterInformation,
			SystemSessionBigPoolInformation,
			SystemBootGraphicsInformation,
			SystemScrubPhysicalMemoryInformation,
			SystemBadPageInformation,
			SystemProcessorProfileControlArea,
			SystemCombinePhysicalMemoryInformation,
			SystemEntropyInterruptTimingInformation,
			SystemConsoleInformation,
			SystemPlatformBinaryInformation,
			SystemPolicyInformation,
			SystemHypervisorProcessorCountInformation,
			SystemDeviceDataInformation,
			SystemDeviceDataEnumerationInformation,
			SystemMemoryTopologyInformation,
			SystemMemoryChannelInformation,
			SystemBootLogoInformation,
			SystemProcessorPerformanceInformationEx,
			SystemCriticalProcessErrorLogInformation,
			SystemSecureBootPolicyInformation,
			SystemPageFileInformationEx,
			SystemSecureBootInformation,
			SystemEntropyInterruptTimingRawInformation,
			SystemPortableWorkspaceEfiLauncherInformation,
			SystemFullProcessInformation,
			SystemKernelDebuggerInformationEx,
			SystemBootMetadataInformation,
			SystemSoftRebootInformation,
			SystemElamCertificateInformation,
			SystemOfflineDumpConfigInformation,
			SystemProcessorFeaturesInformation,
			SystemRegistryReconciliationInformation,
			SystemEdidInformation,
			SystemManufacturingInformation,
			SystemEnergyEstimationConfigInformation,
			SystemHypervisorDetailInformation,
			SystemProcessorCycleStatsInformation,
			SystemVmGenerationCountInformation,
			SystemTrustedPlatformModuleInformation,
			SystemKernelDebuggerFlags,
			SystemCodeIntegrityPolicyInformation,
			SystemIsolatedUserModeInformation,
			SystemHardwareSecurityTestInterfaceResultsInformation,
			SystemSingleModuleInformation,
			SystemAllowedCpuSetsInformation,
			SystemDmaProtectionInformation,
			SystemInterruptCpuSetsInformation,
			SystemSecureBootPolicyFullInformation,
			SystemCodeIntegrityPolicyFullInformation,
			SystemAffinitizedInterruptProcessorInformation,
			SystemRootSiloInformation,
			SystemCpuSetInformation,
			SystemCpuSetTagInformation,
			SystemWin32WerStartCallout,
			SystemSecureKernelProfileInformation,
			SystemCodeIntegrityPlatformManifestInformation,
			SystemInterruptSteeringInformation,
			SystemSuppportedProcessorArchitectures,
			SystemMemoryUsageInformation,
			SystemCodeIntegrityCertificateInformation,
			SystemPhysicalMemoryInformation,
			SystemControlFlowTransition,
			SystemKernelDebuggingAllowed,
			SystemActivityModerationExeState,
			SystemActivityModerationUserSettings,
			SystemCodeIntegrityPoliciesFullInformation,
			SystemCodeIntegrityUnlockInformation,
			SystemIntegrityQuotaInformation,
			SystemFlushInformation,
			SystemProcessorIdleMaskInformation,
			SystemSecureDumpEncryptionInformation,
			SystemWriteConstraintInformation,
			SystemKernelVaShadowInformation,
			SystemHypervisorSharedPageInformation,
			SystemFirmwareBootPerformanceInformation,
			SystemCodeIntegrityVerificationInformation,
			SystemFirmwarePartitionInformation,
			SystemSpeculationControlInformation,
			SystemDmaGuardPolicyInformation,
			SystemEnclaveLaunchControlInformation,
			SystemWorkloadAllowedCpuSetsInformation,
			SystemCodeIntegrityUnlockModeInformation,
			SystemLeapSecondInformation,
			SystemFlags2Information,
			SystemSecurityModelInformation,
			SystemCodeIntegritySyntheticCacheInformation,
			MaxSystemInfoClass,
		}

		public struct UINTRESULT
		{
			public uint Value;
		}

		public struct USHORTRESULT
		{
			public ushort Value;
		}

		public struct ULONGRESULT
		{
			public ulong Value;
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

		public struct PROCESS_BASIC_INFORMATION
		{
			public int ExitStatus;
			public IntPtr PebBaseAddress;
			public UIntPtr AffinityMask;
			public uint BasePriority;
			public UIntPtr UniqueProcessId;
			public UIntPtr InheritedFromUniqueProcessId;
		}

		public enum FileType
		{
			FILE_TYPE_UNKNOWN = 0,
			FILE_TYPE_DISK = 1,
			FILE_TYPE_CHAR = 2,
			FILE_TYPE_PIPE = 3,
			FILE_TYPE_REMOTE = 32768, // 0x00008000
		}

		public enum SECItemType
		{
			siBuffer,
			siClearDataBuffer,
			siCipherDataBuffer,
			siDERCertBuffer,
			siEncodedCertBuffer,
			siDERNameBuffer,
			siEncodedNameBuffer,
			siAsciiNameString,
			siAsciiString,
			siDEROID,
			siUnsignedInteger,
			siUTCTime,
			siGeneralizedTime,
			siVisibleString,
			siUTF8String,
			siBMPString,
		}

		public struct SECItem
		{
			public InternalStructs.SECItemType type;
			public IntPtr dataPtr;
			public uint len;
		}

		public enum SECStatus
		{
			SECWouldBlock = -2, // 0xFFFFFFFE
			SECFailure = -1, // 0xFFFFFFFF
			SECSuccess = 0,
		}

		public enum PRBool
		{
			PR_FALSE,
			PR_TRUE,
		}

		public struct SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX
		{
			public IntPtr Object;
			public UIntPtr UniqueProcessId;
			public UIntPtr HandleValue;
			public uint GrantedAccess;
			public ushort CreatorBackTraceIndex;
			public ushort ObjectTypeIndex;
			public uint HandleAttributes;
			public uint Reserved;
		}

		public struct FILETIME
		{
			public uint dwLowDateTime;
			public uint dwHighDateTime;
		}

		public struct RM_UNIQUE_PROCESS
		{
			public uint dwProcessId;
			public InternalStructs.FILETIME ProcessStartTime;
		}

		public enum RM_APP_TYPE
		{
			RmUnknownApp = 0,
			RmMainWindow = 1,
			RmOtherWindow = 2,
			RmService = 3,
			RmExplorer = 4,
			RmConsole = 5,
			RmCritical = 1000, // 0x000003E8
		}

		public enum RM_REBOOT_REASON : uint
		{
			RmRebootReasonNone = 0,
			RmRebootReasonPermissionDenied = 1,
			RmRebootReasonSessionMismatch = 2,
			RmRebootReasonCriticalProcess = 4,
			RmRebootReasonCriticalService = 8,
			RmRebootReasonDetectedSelf = 9,
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct RM_PROCESS_INFO
		{
			public InternalStructs.RM_UNIQUE_PROCESS Process;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string strAppName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string strServiceShortName;
			public InternalStructs.RM_APP_TYPE ApplicationType;
			public uint AppStatus;
			public uint TSSessionId;
			[MarshalAs(UnmanagedType.Bool)]
			public bool bRestartable;
		}

		public enum CRED_TYPE
		{
			GENERIC = 1,
			DOMAIN_PASSWORD = 2,
			DOMAIN_CERTIFICATE = 3,
			DOMAIN_VISIBLE_PASSWORD = 4,
			MAXIMUM = 5,
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CREDENTIALW
		{
			public int flags;
			public int type;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string targetName;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string comment;
			public InternalStructs.FILETIME lastWritten;
			public int credentialBlobSize;
			public IntPtr credentialBlob;
			public int persist;
			public int attributeCount;
			public IntPtr credAttribute;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string targetAlias;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string userName;
		}
	}
}
