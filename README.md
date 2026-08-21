# 🐀 Xan-RAT also known as Cold Xan RAT
Contains Xan RAT, a [Quasar-based](https://github.com/quasar/Quasar) remote access trojan's full src code and detection rules, along with short analysis of it's capabilities etc



## Special Thanks to

[Milk](https://github.com/actuallymilk) - Helping with writing YARA rules for this!! </br>
[NZ0](https://github.com/Net-Zer0) - Guidance on writing YARA rules & helping in a lot of areas related to the project </br>
[Miikie](https://github.com/miikie) - Helping with writing YARA rules! </br>
[Dashell](https://github.com/DashellF) - Helping with writing YARA rules!!  </br>



> [!NOTE]
> 
> I'm not the original developer of the Cold Xan RAT; I've simply assembled this into a compilable stage with some plugins so it can be tested in an authorized enviroment and properly ruled for research purposes, although some code may be changed and modified a bit from the original due to issues/bugs that it had.
> 

> [!CAUTION]
>### Disclaimer 
>This repository and everything it contains is provided solely for educational and research purposes.  
>YOU ARE responsible for complying with all applicable laws and regulations in your jurisdiction. 
>The author(s) of this repository assume no liability for ANY misuse, damages, or legal consequences resulting from the use of this software or anything this repository contains!
>OBTAIN WRITTEN PERMISSION TO USE! This is intended for ONLY AUTHORIZED USE
>
>Warning that the repo includes third-party libraries in lib. If you want to rebuild and obtain them yourself, you can obviously do so. This should NEVER be run in a production environment-only in a testing environment.

> [!TIP]
> Detection rules we've written for this malware can be found in the following places in this repository
>
> [cold-xan-ruleset]()
> [configured Xan client rules]()
> [base-quasar-ruleset]()
>
>Note that these are WIP, and still being actively refined and worked on, feel free to modify as needed!!

</br>

## Contributing
Contributions are welcome, especially in the following areas:

- Improving detection rules YARA, Sigma(We're already planning to add but you can also do so), etc.) and reducing false positives

- Adding support for new RAT families or variants of this, either public or prviate

- Enhancing build scripts(the raw dlls should be pulled from their respective build repos not hardcoded, for now it's fine) and documentation of usage for research

- Reporting issues or bugs in the code of the repository (Will add known bugs section soonish)




### Capabilities & New Ideas we're planning to add + contributions are also appreciated

Updated decryptor logic, proper bypasses

More targets to take
VPN details
Game app details
API keys
etc


Stealthier persistence mechanisms


### To Build & Setup


Open Developer PowerShell for Visual Studio then run the following:
```
msbuild HQ.sln 
  /t:Rebuild 
  /p:Configuration=Release 
  /p:Platform="Any CPU" 
  /p:RunPostBuildEvent=Always 
  /m:1
  /v:minimal
```



Once done files will be built into ~/bin/Release

In there you will find HQ.exe, which acts as the C2 management software
![](https://github.com/QurtiDev/Xan-RAT/blob/main/assets/c2management.png)

Run it, and generate your Quasar cert
![](https://github.com/QurtiDev/Xan-RAT/blob/main/assets/certpic.png)

Now you have the C2 for Cold Xan RAT working nicely!

![](https://github.com/QurtiDev/Xan-RAT/blob/main/assets/c2panel.png)





![](https://github.com/QurtiDev/Xan-RAT/blob/main/assets/clientbuilder.png)

![](https://github.com/QurtiDev/Xan-RAT/blob/main/assets/modules.png)


### Client manager
![](https://github.com/QurtiDev/Xan-RAT/blob/main/assets/clientmanager.png)


### HVNC 
![](https://github.com/QurtiDev/Xan-RAT/blob/main/assets/hvnc.png)


![](https://github.com/QurtiDev/Xan-RAT/blob/main/assets/hvncbrowser.png)


### RDP
![](https://github.com/QurtiDev/Xan-RAT/blob/main/assets/rdp.png)

### Remote Shell
![](https://github.com/QurtiDev/Xan-RAT/blob/main/assets/rshell.png)

### System Backdoor
![](https://github.com/QurtiDev/Xan-RAT/blob/main/assets/sbackdoor.png)


### Remote Exec
![](https://github.com/QurtiDev/Xan-RAT/blob/main/assets/remotexec.png)
