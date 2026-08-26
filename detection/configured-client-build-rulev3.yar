rule file_Client {
    meta:
        description = "Release - file Client.exe | Detects Obfuscated/Confused-Crypted Xan-RAT Client/stub"
        author = "Qurti, Dashell, Miikie, NetZer0"
        reference = "Cold-Xan-RAT research"
        date = "2026-08-26"
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
        $s21 = "This program cannot be run in DOS mode." ascii
        $s22 = "costura.costura.dll.compressed|6.0.0.0|Costura, Version=6.0.0.0, Culture=neutral, PublicKeyToken=9919ef960d84173d|Costura.dll|028E9832F421F11F9497C610F1734E0F3D868037|5120" fullword ascii
        $s23 = "costura.gma.system.mousekeyhook.dll.compressed|5.6.130.0|Gma.System.MouseKeyHook, Version=5.6.130.0, Culture=neutral, PublicKeyToken=null|Gma.System.MouseKeyHook.dll|1325E8DD76180A165117E04DA4EE4A020E996880|57344" fullword ascii
        $s24 = "costura.invokedcommon.dll.compressed|0.0.0.0|InvokedCommon, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null|InvokedCommon.dll|E587E022869D572377757C0AF376DCDB422A68C8|96256" fullword ascii
        $s25 = "costura.newtonsoft.json.dll.compressed|13.0.0.0|Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed|newtonsoft.json.dll|1E76E6099570EDE620B76ED47CF8D03A936D49F8|711952" fullword ascii
        $s26 = "costura.protobuf-net.dll.compressed|2.4.0.0|protobuf-net, Version=2.4.0.0, Culture=neutral, PublicKeyToken=257b51d87d2e4d67|protobuf-net.dll|FAA645B92E3DE7037C23E99DD2101EF3DA5756E5|289280" fullword ascii
        $s27 = "costura.system.diagnostics.diagnosticsource.dll.compressed|4.0.1.0|System.Diagnostics.DiagnosticSource, Version=4.0.1.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51|system.diagnostics.diagnosticsource.dll|85DC92EDD4B0049ED9049E075C4DEF8A3D64E43B|35760" fullword ascii

        $xml1 = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" ascii wide
        $xml2 = "<assembly xmlns=\"urn:schemas-microsoft-com:asm.v1\" manifestVersion=\"1.0\">" ascii wide
        $xml3 = "<assemblyIdentity version=\"1.0.0.0\" name=\"MyApplication.app\"/>" ascii wide
        $xml4 = "<requestedPrivileges xmlns=\"urn:schemas-microsoft-com:asm.v3\">" ascii wide
        $xml5 = "<requestedExecutionLevel level=\"asInvoker\" uiAccess=\"false\"/>" ascii wide


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
        (10 of ($s*) or
        1 of ($op*) or
        all of ($xml*))
}
