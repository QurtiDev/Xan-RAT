rule file_Client {
    meta:
        description = "Release - file Client.exe"
        author = "Qurti, Dashell, Miikie, NetZer0"
        reference = "Cold-Xan-RAT research"
        date = "2026-08-22"
        hash1 = "d517a4234d1fad5aecb6f3678b0a22ef980cda5094a904badae16dd4f864e3c2"

    strings:
        $s6 = "costura.system.diagnostics.diagnosticsource.dll.compressed|4.0.1.0|System.Diagnostics.DiagnosticSource, Version=4.0.1.0, Culture" fullword ascii
        $s7 = "costura.gma.system.mousekeyhook.dll.compressed" fullword ascii
        $s8 = "BInvokedClient.Extensions.RegistryKeyExtensions+<GetKeyValues>d__15" fullword ascii
        $s9 = "Process already elevated." fullword wide
        $s10 = "lSystem.Resources.ResourceReader, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089#System.Resources.R" fullword ascii
        $s11 = "costura.invokedcommon.dll.compressed" fullword ascii
        $s12 = "ExecuteProcess" fullword ascii
        $s13 = "costura.system.diagnostics.diagnosticsource.dll.compressed" fullword ascii
        $s14 = "costura.newtonsoft.json.dll.compressed" fullword ascii
        $s15 = "GetKeyloggerLogsDirectory" fullword ascii
        $s16 = "costura.costura.dll.compressed" fullword ascii
        $s17 = "costura.protobuf-net.dll.compressed" fullword ascii
        $s18 = "Client.exe" fullword ascii
        $s19 = "GetKeyloggerLogsDirectoryResponse" fullword ascii
        $s20 = "InvokedClient.Logging" fullword ascii
        $s21 = "DoClientReconnect" wide ascii
        $s22 = "DoClientDisconnect" wide ascii
        $s23 = "System.Security.Cryptography.X509Certificates" wide ascii
        $s24 = "Mutex" wide ascii
        $s25 = "SELECT * FROM AntivirusProduct" wide ascii
        $s26 = "client" wide ascii
        $s27 = "Successfully displayed MessageBox" wide ascii
        $s28 = "\n>> New Session created\n" wide ascii
        $s29 = "Visted Website" wide ascii
        $s30 = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_3) AppleWebKit/537.75.14 (KHTML, like Gecko) Version/7.0.3 Safari/7046A194A" wide ascii
        $s31 = "User refused the elevation requests." wide ascii
        $s32 = "Process already elevated." wide ascii
        $s33 = ", try running client as administrator" wide ascii
        

        $op0 = { 28 13 00 00 06 26 20 00 0c 00 00 28 34 00 00 0a }
        $op1 = { 02 00 05 00 04 81 08 00 8c f4 00 00 01 }
        $op2 = { 14 00 00 06 20 ae 00 00 e4 d2 07 }

    condition:
        uint16(0) == 0x5a4d and
        filesize < 2000KB and
        (12 of ($s*) or any of ($op*))
}
