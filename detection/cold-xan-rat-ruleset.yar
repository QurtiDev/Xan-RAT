/*
   YARA Rule Set
   Author: Qurti, Dashell, Miikie
   Date: 2026-08-20
   Identifier: Build-Files
   Reference: Cold-Xan-RAT research
*/

/* Rule Set ----------------------------------------------------------------- */


rule file_Client {
   meta:
      description = "Release - file Client.exe"
      author = "Qurti, Dashell, Miikie"
      reference = "Cold-Xan-RAT research"
      date = "2026-08-20"
      hash1 = "d517a4234d1fad5aecb6f3678b0a22ef980cda5094a904badae16dd4f864e3c2"
      score = 75
   strings:
      $x1 = "costura.invokedcommon.dll.compressed|0.0.0.0|InvokedCommon, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null|InvokedCommon." fullword ascii /* score: 39.0 */
      $x2 = "costura.newtonsoft.json.dll.compressed|13.0.0.0|Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6a" fullword ascii /* score: 35.0 */
      $x3 = "costura.costura.dll.compressed|6.0.0.0|Costura, Version=6.0.0.0, Culture=neutral, PublicKeyToken=9919ef960d84173d|Costura.dll|02" fullword ascii /* score: 35.0 */
      $x4 = "costura.gma.system.mousekeyhook.dll.compressed|5.6.130.0|Gma.System.MouseKeyHook, Version=5.6.130.0, Culture=neutral, PublicKeyT" fullword ascii /* score: 35.0 */
      $x5 = "costura.protobuf-net.dll.compressed|2.4.0.0|protobuf-net, Version=2.4.0.0, Culture=neutral, PublicKeyToken=257b51d87d2e4d67|prot" fullword ascii /* score: 35.0 */
      $s6 = "costura.system.diagnostics.diagnosticsource.dll.compressed|4.0.1.0|System.Diagnostics.DiagnosticSource, Version=4.0.1.0, Culture" fullword ascii /* score: 29.0 */
      $s7 = "costura.gma.system.mousekeyhook.dll.compressed" fullword ascii /* score: 24.0 */
      $s8 = "BInvokedClient.Extensions.RegistryKeyExtensions+<GetKeyValues>d__15" fullword ascii /* score: 23.0 */
      $s9 = "Process already elevated." fullword wide /* score: 23.0 */
      $s10 = "lSystem.Resources.ResourceReader, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089#System.Resources.R" fullword ascii /* score: 23.0 */
      $s11 = "costura.invokedcommon.dll.compressed" fullword ascii /* score: 22.0 */
      $s12 = "ExecuteProcess" fullword ascii /* score: 22.0 */
      $s13 = "costura.system.diagnostics.diagnosticsource.dll.compressed" fullword ascii /* score: 21.0 */
      $s14 = "costura.newtonsoft.json.dll.compressed" fullword ascii /* score: 18.0 */
      $s15 = "GetKeyloggerLogsDirectory" fullword ascii /* score: 18.0 */
      $s16 = "costura.costura.dll.compressed" fullword ascii /* score: 18.0 */
      $s17 = "costura.protobuf-net.dll.compressed" fullword ascii /* score: 18.0 */
      $s18 = "Client.exe" fullword ascii /* score: 18.0 */
      $s19 = "GetKeyloggerLogsDirectoryResponse" fullword ascii /* score: 18.0 */
      $s20 = "InvokedClient.Logging" fullword ascii /* score: 16.0 */

      $op0 = { 28 13 00 00 06 26 20 00 0c 00 00 28 34 00 00 0a }
      $op1 = { 02 00 05 00 04 81 08 00 8c f4 00 00 01 }
      $op2 = { 14 00 00 06 20 ae 00 00 e4 d2 07 }
   condition:
      uint16(0) == 0x5a4d and filesize < 2000KB and (1 of ($x*) and 4 of ($s*)) and all of ($op*)
}



rule file_HQ {
   meta:
      description = "Release - file HQ.exe"
      author = "Qurti, Dashell, Miikie"
      reference = "Cold-Xan-RAT research"
      date = "2026-08-20"
      hash1 = "0f4d0e93a5271a4fbab3cee708b383065a863077cc45e00b1eaecb23323d00be"
      score = 75
   strings:
      $x1 = "C:\\Windows\\System32\\cmd.exe" fullword wide /* score: 34.0 */
      $s2 = "C:\\Users\\swagkek\\source\\repos\\FXCrypter\\CrypterStub\\obj\\Release\\Stub.pdb" fullword ascii /* score: 29.0 */
      $s3 = "Launch cmd.exe" fullword wide /* score: 29.0 */
      $s4 = "Executables *.exe|*.exe" fullword wide /* score: 27.0 */
      $s5 = "Executable files (*.exe)|*.exe" fullword wide /* score: 27.0 */
      $s6 = "Plugins\\PluginPasswordRecovery.dll" fullword wide /* score: 27.0 */
      $s7 = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe" fullword wide /* score: 27.0 */
      $s8 = "PluginPasswordRecovery.dll" fullword wide /* score: 27.0 */
      $s9 = "System.Security.Permissions.SecurityPermissionAttribute, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934" fullword ascii /* score: 23.0 */
      $s10 = "lSystem.Resources.ResourceReader, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089#System.Resources.R" fullword ascii /* score: 23.0 */
      $s11 = "System.Resources.Extensions.RuntimeResourceSet, System.Resources.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc" fullword ascii /* score: 23.0 */
      $s12 = "fSystem.Drawing.Icon, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'System.Resources.Extensi" fullword ascii /* score: 23.0 */
      $s13 = "UySystem.Windows.Forms.BorderStyle, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" fullword ascii /* score: 23.0 */
      $s14 = "C:\\Program Files\\Mozilla Firefox\\firefox.exe" fullword wide /* score: 22.0 */
      $s15 = "Plugins\\PluginRemoteDesktop.dll" fullword wide /* score: 22.0 */
      $s16 = "PluginSurvival.dll" fullword wide /* score: 22.0 */
      $s17 = "InvokedServer.Forms.FrmRemoteExecution.resources" fullword ascii /* score: 22.0 */
      $s18 = "Executable (*.exe)|*.exe|VBS Script (*.vbs*)|*.vbs*|Batch (*.bat*)|*.bat*|All Files (*.*)|*.*" fullword wide /* score: 22.0 */
      $s19 = "PluginStealer.dll" fullword wide /* score: 22.0 */
      $s20 = "PluginRemoteDesktop.dll" fullword wide /* score: 22.0 */

      $op0 = { 10 01 00 00 38 34 01 00 00 11 09 6f 4d 00 00 0a }
      $op1 = { 02 00 05 00 fc c7 04 00 50 5a 04 00 01 }
      $op2 = { 01 00 00 06 4c 22 09 00 58 49 2f }
   condition:
      uint16(0) == 0x5a4d and filesize < 11000KB and (1 of ($x*) and 4 of ($s*)) and all of ($op*)
}

rule InvokedCommon {
   meta:
      description = "Release - file InvokedCommon.dll"
      author = "Qurti, Dashell, Miikie"
      reference = "Cold-Xan-RAT research"
      date = "2026-08-20"
      hash1 = "9284bf94fcef2ae2fa4a471621a4c6414c8a59f0a56771b37b7e3cbbc619622b"
      score = 75
   strings:
      $s1 = "InvokedCommon.dll" fullword ascii /* score: 26.0 */
      $s2 = "System.Security.Permissions.SecurityPermissionAttribute, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934" fullword ascii /* score: 23.0 */
      $s3 = "GetKeyloggerLogsDirectory" fullword ascii /* score: 18.0 */
      $s4 = "System.Collections.Generic.IEnumerator<InvokedCommon.Models.FileChunk>.get_Current" fullword ascii /* score: 18.0 */
      $s5 = "GetKeyloggerLogsDirectoryResponse" fullword ascii /* score: 18.0 */
      $s6 = "get_PresetProcess" fullword ascii /* score: 16.0 */
      $s7 = "GetLoginsString" fullword ascii /* score: 16.0 */
      $s8 = "GetProcessesResponse" fullword ascii /* score: 16.0 */
      $s9 = "get_processIDs" fullword ascii /* score: 16.0 */
      $s10 = "HasExecutableIdentifier" fullword ascii /* score: 15.0 */
      $s11 = "DoShellExecute" fullword ascii /* score: 14.0 */
      $s12 = "DoShellExecuteResponse" fullword ascii /* score: 14.0 */
      $s13 = "InvokedCommon.Video.Compression" fullword ascii /* score: 14.0 */
      $s14 = "GetPasswords" fullword ascii /* score: 13.0 */
      $s15 = "InvokedCommon.Messages.ReverseProxy" fullword ascii /* score: 13.0 */
      $s16 = "GetPasswordExtensionsString" fullword ascii /* score: 13.0 */
      $s17 = "InvokedCommon.DNS" fullword ascii /* score: 13.0 */
      $s18 = "InvokedCommon.ReverseProxy" fullword ascii /* score: 13.0 */
      $s19 = "GetPasswordsResponse" fullword ascii /* score: 13.0 */
      $s20 = "System.Collections.Generic.IEnumerator<InvokedCommon.Models.FileChunk>.Current" fullword ascii /* score: 13.0 */

      $op0 = { 02 00 05 00 f0 7c 00 00 20 12 01 00 01 }
      $op1 = { 1e 02 28 17 00 00 0a 2a 1b 30 03 00 53 }
      $op2 = { 01 00 00 11 02 28 17 00 00 0a 03 28 18 00 00 0a }
   condition:
      uint16(0) == 0x5a4d and filesize < 300KB and (8 of ($s*)) and all of ($op*)
}

rule PluginHVNC {
   meta:
      description = "Release - file PluginHVNC.dll"
      author = "Qurti, Dashell, Miikie"
      reference = "Cold-Xan-RAT research"
      date = "2026-08-20"
      hash1 = "22a8af6aee817d1e767fc9434a79e463d51572db6b6ce8f58dd79bb7e757b134"
      score = 75
   strings:
      $s1 = "Failed to open target process." fullword wide /* score: 24.0 */
      $s2 = "System.Security.Permissions.SecurityPermissionAttribute, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934" fullword ascii /* score: 23.0 */
      $s3 = "user32.dll base address: " fullword wide /* score: 21.0 */
      $s4 = " not found in the target process." fullword wide /* score: 21.0 */
      $s5 = "C:\\Users\\" fullword wide /* score: 20.0 */
      $s6 = "ProcessManageWritesToExecutableMemory" fullword ascii /* score: 20.0 */
      $s7 = "ProcessLoaderDetour" fullword ascii /* score: 20.0 */
      $s8 = "<GetProcessViaCommandLine>b__0" fullword ascii /* score: 19.0 */
      $s9 = "5Plugin.Helper.Process_Handler+<HandleCloneOpera>d__40" fullword ascii /* score: 19.0 */
      $s10 = "SHCore.dll" fullword ascii /* score: 19.0 */
      $s11 = "=Plugin.Helper.Process_Handler+<GetProcessViaCommandLine>d__45" fullword ascii /* score: 19.0 */
      $s12 = "<GetProcessViaCommandLine>d__45" fullword ascii /* score: 19.0 */
      $s13 = "ProcessCommandLineInformation" fullword ascii /* score: 19.0 */
      $s14 = "GetProcessViaCommandLine" fullword ascii /* score: 19.0 */
      $s15 = "7Plugin.Helper.Process_Handler+<HandleCloneOperaGX>d__41" fullword ascii /* score: 19.0 */
      $s16 = "PluginHVNC.dll" fullword ascii /* score: 19.0 */
      $s17 = "1Plugin.Helper.Process_Handler+<CloneOperaGX>d__34" fullword ascii /* score: 19.0 */
      $s18 = "brave.exe" fullword wide /* score: 18.0 */
      $s19 = "msedge.exe" fullword wide /* score: 18.0 */
      $s20 = "ProcessSubsystemProcess" fullword ascii /* score: 18.0 */

      $op0 = { 02 00 05 00 34 7c 00 00 e0 a9 00 00 01 }
      $op1 = { 01 00 00 11 73 b7 00 00 06 0a 03 06 04 6f 17 00 }
      $op2 = { 10 10 00 03 12 00 00 01 1e 02 28 18 00 00 0a 2a }
   condition:
      uint16(0) == 0x5a4d and filesize < 300KB and (8 of ($s*)) and all of ($op*)
}

rule PluginPasswordRecovery {
   meta:
      description = "Release - file PluginPasswordRecovery.dll"
      author = "Qurti, Dashell, Miikie"
      reference = "Cold-Xan-RAT research"
      date = "2026-08-20"
      hash1 = "3e2134a074b00aaa804b640e6109f9c5cb57d802b74990fca59acfbe46d4eda5"
      score = 75
   strings:
      $s1 = "PluginPasswordRecovery.dll" fullword ascii /* score: 27.0 */
      $s2 = "System.Security.Permissions.SecurityPermissionAttribute, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934" fullword ascii /* score: 23.0 */
      $s3 = "BInvokedClient.Extensions.RegistryKeyExtensions+<GetKeyValues>d__15" fullword ascii /* score: 23.0 */
      $s4 = "get_PotentiallyVulnerablePasswords" fullword ascii /* score: 22.0 */
      $s5 = "<PotentiallyVulnerablePasswords>k__BackingField" fullword ascii /* score: 17.0 */
      $s6 = "set_PotentiallyVulnerablePasswords" fullword ascii /* score: 17.0 */
      $s7 = "potentiallyVulnerablePasswords," fullword ascii /* score: 17.0 */
      $s8 = "Opera Software\\Opera Stable\\Login Data" fullword wide /* score: 16.0 */
      $s9 = "Opera Software\\Opera GX Stable\\Login Data" fullword wide /* score: 16.0 */
      $s10 = "get_DismissedBreachAlertsByLoginGuid" fullword ascii /* score: 16.0 */
      $s11 = "GetPasswords" fullword ascii /* score: 13.0 */
      $s12 = "get_PasswordField" fullword ascii /* score: 13.0 */
      $s13 = "<EncryptedPassword>k__BackingField" fullword ascii /* score: 13.0 */
      $s14 = "get_TimePasswordChanged" fullword ascii /* score: 13.0 */
      $s15 = "get_EncryptedUsername" fullword ascii /* score: 13.0 */
      $s16 = "GetPasswordsResponse" fullword ascii /* score: 13.0 */
      $s17 = "nonSecretPayloadLength" fullword ascii /* score: 12.0 */
      $s18 = "Name dismissedBreachAlertsByLoginGUID" fullword ascii /* score: 11.0 */
      $s19 = "<Logins>k__BackingField" fullword ascii /* score: 11.0 */
      $s20 = "BraveSoftware\\Brave-Browser\\User Data\\Default\\Login Data" fullword wide /* score: 11.0 */

      $op0 = { 02 00 05 00 88 5d 00 00 44 74 00 00 01 }
      $op1 = { 01 00 00 11 73 7d 00 00 06 0a 03 06 04 6f 23 00 }
      $op2 = { 10 10 00 03 13 00 00 01 1e 02 28 24 00 00 0a 2a }
   condition:
      uint16(0) == 0x5a4d and filesize < 200KB and (8 of ($s*)) and all of ($op*)
}

rule PluginRemoteDesktop {
   meta:
      description = "Release - file PluginRemoteDesktop.dll"
      author = "Qurti, Dashell, Miikie"
      reference = "Cold-Xan-RAT research"
      date = "2026-08-20"
      hash1 = "18ea6fbb25e155ab68c2465b4193bc827235445cbf1fb35349878a1c81ef1f1e"
      score = 75
   strings:
      $s1 = "System.Security.Permissions.SecurityPermissionAttribute, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934" fullword ascii /* score: 23.0 */
      $s2 = "PluginRemoteDesktop.dll" fullword ascii /* score: 22.0 */
      $s3 = "<Execute>b__17_0" fullword ascii /* score: 14.0 */
      $s4 = "IMessageProcessor" fullword ascii /* score: 11.0 */
      $s5 = "InvokedCommon.Enums" fullword ascii /* score: 10.0 */
      $s6 = "InvokedCommon.Networking" fullword ascii /* score: 10.0 */
      $s7 = "InvokedCommon.Messages" fullword ascii /* score: 10.0 */
      $s8 = "InvokedCommon.Video.Codecs" fullword ascii /* score: 10.0 */
      $s9 = "InvokedCommon.Video" fullword ascii /* score: 10.0 */
      $s10 = "CanExecuteFrom" fullword ascii /* score: 10.0 */
      $s11 = "KEYEVENTF_KEYDOWN" fullword ascii /* score: 8.0 */
      $s12 = "InvokedCommon" fullword ascii /* score: 7.0 */
      $s13 = ".NETFramework,Version=v4.8" fullword ascii /* score: 6.0 */
      $s14 = ".NET Framework 4.8lY" fullword ascii /* score: 6.0 */
      $s15 = "GetMonitorsResponse" fullword ascii /* score: 5.0 */
      $s16 = "GetForegroundWindowTitle" fullword ascii /* score: 5.0 */
      $s17 = "GetDesktopResponse" fullword ascii /* score: 5.0 */
      $s18 = "get_ImageQuality" fullword ascii /* score: 5.0 */
      $s19 = "get_ShowCursor" fullword ascii /* score: 5.0 */
      $s20 = "GetDesktopResponseOldGfxEng" fullword ascii /* score: 5.0 */

      $op0 = { 02 00 05 00 c8 2e 00 00 7c 2a 00 00 01 }
      $op1 = { 01 00 00 11 73 35 00 00 06 0a 03 06 04 6f 11 00 }
      $op2 = { 10 10 00 03 12 00 00 01 1e 02 28 12 00 00 0a 2a }
   condition:
      uint16(0) == 0x5a4d and filesize < 50KB and (8 of ($s*)) and all of ($op*)
}

rule PluginRemoteWebcam {
   meta:
      description = "Release - file PluginRemoteWebcam.dll"
      author = "Qurti, Dashell, Miikie"
      reference = "Cold-Xan-RAT research"
      date = "2026-08-20"
      hash1 = "617a9413d1524347e1d4661fa4c9d106958fe0575965e7681236c0b2bdb38b72"
      score = 75
   strings:
      $s1 = "System.Security.Permissions.SecurityPermissionAttribute, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934" fullword ascii /* score: 23.0 */
      $s2 = "PluginRemoteWebcam.dll" fullword ascii /* score: 18.0 */
      $s3 = "IMessageProcessor" fullword ascii /* score: 11.0 */
      $s4 = "InvokedCommon.Video" fullword ascii /* score: 10.0 */
      $s5 = "InvokedCommon.Networking" fullword ascii /* score: 10.0 */
      $s6 = "InvokedCommon.Video.Codecs" fullword ascii /* score: 10.0 */
      $s7 = "InvokedCommon.Messages" fullword ascii /* score: 10.0 */
      $s8 = "CanExecuteFrom" fullword ascii /* score: 10.0 */
      $s9 = "InvokedCommon.MessageHandlers" fullword ascii /* score: 10.0 */
      $s10 = "Crossbar configuration is not supported by currently running video source." fullword wide /* score: 9.0 */
      $s11 = "GetMaxAvailableFrameRate" fullword ascii /* score: 8.0 */
      $s12 = "get_FramesReceived" fullword ascii /* score: 8.0 */
      $s13 = "GetFrameRateList" fullword ascii /* score: 8.0 */
      $s14 = "AverageTimePerFrame" fullword ascii /* score: 8.0 */
      $s15 = "GetCurrentActualFrameRate" fullword ascii /* score: 8.0 */
      $s16 = "get_DesiredFrameSize" fullword ascii /* score: 8.0 */
      $s17 = "TemporalCompression" fullword ascii /* score: 7.0 */
      $s18 = "NotificationMessageProcessor" fullword ascii /* score: 7.0 */
      $s19 = "InvokedCommon" fullword ascii /* score: 7.0 */
      $s20 = ".NETFramework,Version=v4.8" fullword ascii /* score: 6.0 */

      $op0 = { 02 00 05 00 a8 4a 00 00 a0 6f 00 00 01 }
      $op1 = { 01 00 00 11 73 0d 00 00 06 0a 03 06 04 6f 19 00 }
      $op2 = { 10 10 00 03 13 00 00 01 1e 02 28 1a 00 00 0a 2a }
   condition:
      uint16(0) == 0x5a4d and filesize < 200KB and (8 of ($s*)) and all of ($op*)
}

rule PluginStealer {
   meta:
      description = "Release - file PluginStealer.dll"
      author = "Qurti, Dashell, Miikie"
      reference = "Cold-Xan-RAT research"
      date = "2026-08-20"
      hash1 = "7a88fb28b0976c582c71d31cff3f40596c71a3f970eaac4140ff86ccb5fc0f07"
      score = 75
   strings:
      $s1 = "System.Security.Permissions.SecurityPermissionAttribute, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934" fullword ascii /* score: 23.0 */
      $s2 = "PluginStealer.dll" fullword ascii /* score: 22.0 */
      $s3 = "https://discord.com/api/v9/users/@me" fullword wide /* score: 21.0 */
      $s4 = "ProcessLoaderDetour" fullword ascii /* score: 20.0 */
      $s5 = "ProcessManageWritesToExecutableMemory" fullword ascii /* score: 20.0 */
      $s6 = "ProcessCommandLineInformation" fullword ascii /* score: 19.0 */
      $s7 = "SystemProcessorPowerInformationEx" fullword ascii /* score: 18.0 */
      $s8 = "ProcessSubsystemProcess" fullword ascii /* score: 18.0 */
      $s9 = "SystemProcessorProfileControlArea" fullword ascii /* score: 18.0 */
      $s10 = "SystemProcessorInformation" fullword ascii /* score: 18.0 */
      $s11 = "SystemProcessorMicrocodeUpdateInformation" fullword ascii /* score: 18.0 */
      $s12 = "SystemProcessorPerformanceInformation" fullword ascii /* score: 18.0 */
      $s13 = "SystemProcessorPowerInformation" fullword ascii /* score: 18.0 */
      $s14 = "SystemProcessorCycleStatsInformation" fullword ascii /* score: 18.0 */
      $s15 = "SystemProcessorIdleInformation" fullword ascii /* score: 18.0 */
      $s16 = "SystemProcessorCycleTimeInformation" fullword ascii /* score: 18.0 */
      $s17 = "SystemProcessIdInformation" fullword ascii /* score: 18.0 */
      $s18 = "SystemProcessorIdleCycleTimeInformation" fullword ascii /* score: 18.0 */
      $s19 = "SystemProcessorPerformanceInformationEx" fullword ascii /* score: 18.0 */
      $s20 = "SystemProcessorFeaturesInformation" fullword ascii /* score: 18.0 */

      $op0 = { 02 00 05 00 80 a6 00 00 84 1e 01 00 01 }
      $op1 = { 01 00 00 11 73 cf 00 00 06 0a 03 06 04 6f 12 00 }
      $op2 = { 10 10 00 03 12 00 00 01 1e 02 28 13 00 00 0a 2a }
   condition:
      uint16(0) == 0x5a4d and filesize < 400KB and (8 of ($s*)) and all of ($op*)
}

rule PluginSurvival {
   meta:
      description = "Release - file PluginSurvival.dll"
      author = "Qurti, Dashell, Miikie"
      reference = "Cold-Xan-RAT research"
      date = "2026-08-20"
      hash1 = "2def6fd438e7bbeace0b1981ab0de1d4d54ff0f8456327e457ae88e0565d783c"
      score = 75
   strings:
      $s1 = " %TARGETOSDRIVE%\\windows\\system32\\config\\SOFTWARE" fullword wide /* score: 25.0 */
      $s2 = "wscript %TARGETOSDRIVE%\\Recovery\\OEM\\" fullword wide /* score: 23.0 */
      $s3 = "System.Security.Permissions.SecurityPermissionAttribute, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934" fullword ascii /* score: 23.0 */
      $s4 = "PluginSurvival.dll" fullword ascii /* score: 22.0 */
      $s5 = "for /F \"tokens=1 delims=\\\" %%A in ('Echo %TARGETOS%') DO SET TARGETOSDRIVE=%%A" fullword wide /* score: 21.0 */
      $s6 = "for /F \"tokens=1,2,3 delims= \" %%A in ('reg query \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\RecoveryEnvironment\" /v TargetOS" wide /* score: 20.0 */
      $s7 = "\\Microsoft\\Windows\\CurrentVersion\\RunOnce /v " fullword wide /* score: 15.0 */
      $s8 = "%TARGETOSDRIVE%\\Recovery\\OEM\\" fullword wide /* score: 14.0 */
      $s9 = "IMessageProcessor" fullword ascii /* score: 11.0 */
      $s10 = "CanExecuteFrom" fullword ascii /* score: 10.0 */
      $s11 = "InvokedCommon.Networking" fullword ascii /* score: 10.0 */
      $s12 = "InvokedCommon.Messages" fullword ascii /* score: 10.0 */
      $s13 = " /t REG_SZ /d \"" fullword wide /* score: 8.0 */
      $s14 = "InvokedCommon" fullword ascii /* score: 7.0 */
      $s15 = "SaveScriptFile" fullword ascii /* score: 6.0 */
      $s16 = "PluginSurvival.MessageHandlers" fullword ascii /* score: 6.0 */
      $s17 = ".NETFramework,Version=v4.8" fullword ascii /* score: 6.0 */
      $s18 = ".NET Framework 4.8" fullword ascii /* score: 6.0 */
      $s19 = "Already Installed!" fullword wide /* score: 6.0 */
      $s20 = "Error backing up current config" fullword wide /* score: 6.0 */

      $op0 = { 02 00 05 00 58 29 00 00 bc 1d 00 00 01 }
      $op1 = { 2a 03 75 16 00 00 01 14 fe 03 2a 0a 17 2a 00 00 }
      $op2 = { 01 00 00 11 04 75 16 00 00 01 0a 06 2d 01 2a 02 }
   condition:
      uint16(0) == 0x5a4d and filesize < 40KB and (8 of ($s*)) and all of ($op*)
}



/* Super Rules ------------------------------------------------------------- */

rule PluginRemoteDesktop_PluginRemoteWebcam_PluginSurvival_super {
   meta:
      description = "Release - from files PluginRemoteDesktop.dll, PluginRemoteWebcam.dll, PluginSurvival.dll"
      author = "Qurti, Dashell, Miikie"
      reference = "Cold-Xan-RAT research"
      date = "2026-08-20"
      hash1 = "18ea6fbb25e155ab68c2465b4193bc827235445cbf1fb35349878a1c81ef1f1e"
      hash2 = "617a9413d1524347e1d4661fa4c9d106958fe0575965e7681236c0b2bdb38b72"
      hash3 = "2def6fd438e7bbeace0b1981ab0de1d4d54ff0f8456327e457ae88e0565d783c"
      score = 75
   strings:
      $s1 = "IMessageProcessor" fullword ascii /* score: 11.0 */
      $s2 = "InvokedCommon.Networking" fullword ascii /* score: 10.0 */
      $s3 = "CanExecuteFrom" fullword ascii /* score: 10.0 */
      $s4 = "InvokedCommon.Messages" fullword ascii /* score: 10.0 */
      $s5 = "InvokedCommon" fullword ascii /* score: 7.0 */
      $s6 = ".NETFramework,Version=v4.8" fullword ascii /* score: 6.0 */
   condition:
      (uint16(0) == 0x5a4d and filesize < 200KB and all of ($s*))
      or (all of them)
}
