rule XanQuasar
{
    meta:
        author = "Actuallymilk, Qurti"
        description = "Quasar-based Cold Xan RAT detection rules"
        mitre_attack_id = "S0262" // Quasar ID
        mitre_tactic = "TA0011,TA0006,TA0009,TA0007"
        mitre_technique = "T1095,T1056.001,T1125,T1555.003,T1059.003,T1547.001,T1614,T1016,T1082"


    strings:
        $plugin1 = "PluginRemoteDesktop" ascii
        $plugin2 = "PluginHVNC" ascii
        $plugin3 = "PluginRemoteWebcam" ascii
        $plugin4 = "PluginStealer" ascii
        $plugin5 = "PluginPasswordRecovery" ascii
        $plugin6 = "PluginSurvival" ascii
        $geo1 = "InvokedClient.IpGeoLocation" ascii
        $geo2 = "GeoInformationRetriever" ascii
        $geo3 = "GeoInformationFactory" ascii
        $geo4 = "GetGeoInformation" ascii
        $geo5 = "https://ipwho.is/" ascii
        $geo6 = "https://api.ipify.org/" ascii
        $remote_desktop1 = "PluginRemoteDesktopHandler" ascii
        $remote_desktop2 = "GetDesktopOldGfxEng" ascii
        $remote_desktop3 = "SetRDStatus" ascii
        $remote_desktop4 = "RemoteDesktopStatus" ascii
        $hvnc1 = "PluginHVNCHandler" ascii
        $hvnc2 = "DoInputEventHVNC" ascii
        $hvnc3 = "DoNewProcessHVNC" ascii
        $hvnc4 = "StartNewProfileChrome" ascii
        $webcam1 = "PluginWebcamHandler" ascii
        $webcam2 = "GetWebcamImageResponse" ascii
        $webcam3 = "DoWebcamStop" ascii
        $webcam4 = "VideoCaptureDevice" ascii
        $stealer1 = "PluginStealerHandler" ascii
        $stealer2 = "GetStealerLogsResponse" ascii
        $stealer3 = "DiscordCanary" ascii
        $stealer4 = "dQw4w9WgXcQ:" ascii
        $stealer5 = "GetStealerLogs" ascii
        $stealer6 = "https://discord.com/api/v9/users/@me" ascii
        $password1 = "PluginPasswordRecoveryHandler" ascii
        $password2 = "ChromePassReader" ascii
        $password3 = "FirefoxPassReader" ascii
        $password4 = "FileZillaPassReader" ascii
        $password5 = "WinScpPassReader" ascii
        $survival1 = "ResetConfig.xml" ascii
        $survival2 = "XRSBackupData" ascii
        $survival3 = "DoSurvivialInstall" ascii
        $survival4 = "NewSurvivalLog" ascii
        $survival5 = "BasicReset_AfterImageApply" ascii
        $survival6 = "FactoryReset_AfterImageApply" ascii
        $survival7 = "%TARGETOSDRIVE%\\Recovery\\OEM\\" ascii
        $uninstall = "Uninstalling... good bye :-(" ascii
        $random1 = "DONT CLOSE THIS WINDOW!" ascii
		$random2 = "ping -n 10 localhost > nul" ascii
		$random3 = "del /a /q /f" ascii
		$registry1 = "RegistryHandler" ascii
        $registry2 = "DoLoadRegistryKey" ascii
        $registry3 = "DoCreateRegistryKey" ascii
        $registry4 = "DoDeleteRegistryValue" ascii
        $registry5 = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run" ascii
        $registry6 = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce" ascii
        $registry7 = "InvokedClient.Registry" ascii
        $registry8 = "InvokedServer.Registry" ascii
        $registry9 = "RegistryEditor" ascii
        $registry10 = "RegistrySeeker" ascii
        $registry11 = "RegValueHelper" ascii
        $registry12 = "Cannot create key: Error writing to the registry" ascii
        $registry13 = "HKEY_LOCAL_MACHINE" ascii
        
        $hex1 = { 28 13 00 00 06 26 }
        $hex2 = { 28 ?? ?? ?? 0A 26 ?? ?? ?? 0A }
    condition:
        uint16(0) == 0x5A4D and any of them
}

