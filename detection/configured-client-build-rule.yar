/*
   YARA Rule Set
   Author: Qurti, Dashell, Miikie
   Date: 2026-08-21
   Identifier: configured-build
   Reference: Cold-Xan-RAT research
*/

/* Rule Set ----------------------------------------------------------------- */

rule coldxan_rat {
   meta:
      description = "built - file coldxan-rat.exe"
      author = "Qurti, Dashell, Miikie"
      reference = "Cold-Xan-RAT research"
      date = "2026-08-21"
      hash1 = "6568561356a90e9e86af1f619fe16e7f827f7b5d9358b13d0ea94789783fc567"
      score = 75
   strings:
      $x1 = "costura.invokedcommon.dll.compressed|0.0.0.0|InvokedCommon, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null|InvokedCommon." fullword ascii /* score: 39.0 */
      $x2 = "costura.protobuf-net.dll.compressed|2.4.0.0|protobuf-net, Version=2.4.0.0, Culture=neutral, PublicKeyToken=257b51d87d2e4d67|prot" fullword ascii /* score: 35.0 */
      $x3 = "costura.costura.dll.compressed|6.0.0.0|Costura, Version=6.0.0.0, Culture=neutral, PublicKeyToken=9919ef960d84173d|Costura.dll|02" fullword ascii /* score: 35.0 */
      $x4 = "costura.gma.system.mousekeyhook.dll.compressed|5.6.130.0|Gma.System.MouseKeyHook, Version=5.6.130.0, Culture=neutral, PublicKeyT" fullword ascii /* score: 35.0 */
      $x5 = "costura.newtonsoft.json.dll.compressed|13.0.0.0|Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6a" fullword ascii /* score: 35.0 */
      $s6 = "costura.system.diagnostics.diagnosticsource.dll.compressed|4.0.1.0|System.Diagnostics.DiagnosticSource, Version=4.0.1.0, Culture" fullword ascii /* score: 29.0 */
      $s7 = "costura.gma.system.mousekeyhook.dll.compressed" fullword ascii /* score: 24.0 */
      $s8 = "BInvokedClient.Extensions.RegistryKeyExtensions+<GetKeyValues>d__15" fullword ascii /* score: 23.0 */
      $s9 = "Process already elevated." fullword wide /* score: 23.0 */
      $s10 = "lSystem.Resources.ResourceReader, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089#System.Resources.R" fullword ascii /* score: 23.0 */
      $s11 = "costura.invokedcommon.dll.compressed" fullword ascii /* score: 22.0 */
      $s12 = "costura.system.diagnostics.diagnosticsource.dll.compressed" fullword ascii /* score: 21.0 */
      $s13 = "costura.protobuf-net.dll.compressed" fullword ascii /* score: 18.0 */
      $s14 = "GetKeyloggerLogsDirectoryResponse" fullword ascii /* score: 18.0 */
      $s15 = "Client.exe" fullword ascii /* score: 18.0 */
      $s16 = "GetKeyloggerLogsDirectory" fullword ascii /* score: 18.0 */
      $s17 = "costura.newtonsoft.json.dll.compressed" fullword ascii /* score: 18.0 */
      $s18 = "costura.costura.dll.compressed" fullword ascii /* score: 18.0 */
      $s19 = "GetProcessesResponse" fullword ascii /* score: 16.0 */
      $s20 = "costura.costura.pdb.compressed|||Costura.pdb|806F4C19B2D7FD9E3B836269EC07647019A29E95|7960" fullword ascii /* score: 15.0 */

      $op0 = { 28 13 00 00 06 26 20 00 0c 00 00 28 34 00 00 0a }
      $op1 = { 02 00 05 00 00 81 08 00 4c 78 01 00 01 }
      $op2 = { 14 00 00 06 38 ae 00 00 c8 d2 07 }
   condition:
      uint16(0) == 0x5a4d and filesize < 2000KB and (1 of ($x*) and 4 of ($s*)) and all of ($op*)
}
