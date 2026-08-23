rule file_Client {
    meta:
        description = "Release - file Client.exe | Detects Obfuscated/Confused-Crypted Xan-RAT Client/stub"
        author = "Qurti, Dashell, Miikie, NetZer0"
        reference = "Cold-Xan-RAT research"
        date = "2026-08-22"
        hash1 = "d517a4234d1fad5aecb6f3678b0a22ef980cda5094a904badae16dd4f864e3c2"

    strings:
        $s1 = "BInvokedClient.Extensions.RegistryKeyExtensions+<GetKeyValues>d__15" fullword ascii
        $s2 = "Process already elevated." fullword wide
        $s3 = "lSystem.Resources.ResourceReader, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089#System.Resources.R" fullword ascii
        $s4 = "ExecuteProcess" fullword ascii
        $s5 = "GetKeyloggerLogsDirectory" fullword ascii
        $s6 = "Client.exe" fullword ascii
        $s7 = "GetKeyloggerLogsDirectoryResponse" fullword ascii
        $s8 = "InvokedClient.Logging" fullword ascii
        $s9 = "DoClientReconnect" wide ascii
        $s10 = "DoClientDisconnect" wide ascii
        $s11 = "System.Security.Cryptography.X509Certificates" wide ascii
        $s12 = "Mutex" wide ascii
        $s13 = "SELECT * FROM AntivirusProduct" wide ascii
        $s14 = "client" wide ascii
        $s15 = "Successfully displayed MessageBox" wide ascii
        $s16 = "\n>> New Session created\n" wide ascii
        $s17 = "Visted Website" wide ascii
        $s18 = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_3) AppleWebKit/537.75.14 (KHTML, like Gecko) Version/7.0.3 Safari/7046A194A" wide ascii
        $s19 = "User refused the elevation requests." wide ascii
        $s20 = ", try running client as administrator" wide ascii

        $aes1 = "Aes256" ascii wide
        $aes2 = "InvokedCommon.Cryptography" wide ascii

        $op0 = { 28 13 00 00 06 26 20 00 0c 00 00 28 34 00 00 0a }
        $op1 = { 02 00 05 00 04 81 08 00 8c f4 00 00 01 }
        $op2 = { 14 00 00 06 20 ae 00 00 e4 d2 07 }

    condition:
        uint16(0) == 0x5a4d and
        filesize < 2000KB and
        any of ($aes*) and
        any of ($s2, $s5, $s6, $s9, $s10, $s11, $s14, $s16, $s19, $s20) and
        5 of ($s*) or
        1 of ($op*)
}
