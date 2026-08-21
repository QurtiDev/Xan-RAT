# Xan-RAT also known as Cold Xan RAT
Contains Xan RAT, a [Quasar-based](https://github.com/quasar/Quasar) remote access trojan's full src code and detection rules, along with short analysis of it's capabilities etc



## Special Thanks to

[Milk](https://github.com/actuallymilk) - Helping with writing YARA rules for this!! </br>
[NZ0](https://github.com/Net-Zer0) - Guidance on writing YARA rules, helping in a lot of areas related to the project </br>
[Miikie](https://github.com/miikie) - Helping with writing YARA rules as well! </br>
[Dashell](https://github.com/DashellF) - Helping with writing YARA rules!!  </br>



> [!NOTE]
> 
> I'm not the original developer of the Cold Xan RAT; I've simply assembled this into a compilable stage with some plugins so it can be tested in an authorized enviroment and properly ruled for research purposes, some code may be changed and modified a bit from original due to issues/bugs that it had.
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


## Contributing
Contributions are welcome, especially in the following areas:

Improving detection rules YARA, Sigma(We're already planning to add but you can also do so), etc.) and reducing false positives

Adding support for new RAT families or variants of this, either public or prviate

Enhancing build scripts(the raw dlls should be pulled from their respective build repos not hardcoded, for now it's fine) and documentation of usage for research

Reporting issues or bugs in the code of the repository (Will add known bugs section soonish)




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
<img width="1087" height="616" alt="image" src="https://github.com/user-attachments/assets/9b628cfe-2552-40ee-9202-49ef3e9dbe98" />

Run it, and generate your Quasar cert
<img width="500" height="342" alt="image" src="https://github.com/user-attachments/assets/221df52c-ef7e-460b-85dd-d3cde536231d" />

Now you have the C2 for Cold Xan RAT working nicely!

<img width="839" height="465" alt="image" src="https://github.com/user-attachments/assets/9d8ff5ef-f538-4b92-8b99-8d477c82430a" />





<img width="534" height="477" alt="image" src="https://github.com/user-attachments/assets/f3340205-6129-4849-a8aa-451d86be0a40" />

<img width="895" height="480" alt="image" src="https://github.com/user-attachments/assets/fc90d22e-2597-470b-a925-2310e0f880a7" />


### Client manager
<img width="1438" height="852" alt="image" src="https://github.com/user-attachments/assets/b90e6a9c-3dcc-4e0a-9926-132061a1f0da" />


### HVNC 
<img width="1438" height="852" alt="image" src="https://github.com/user-attachments/assets/b3d22bdb-f91f-47c3-b3c5-d3f1a9eac4a9" />


<img width="1077" height="677" alt="image" src="https://github.com/user-attachments/assets/d2e97073-9d6d-4701-86ff-efd005c3a9b8" />


### RDP
<img width="1439" height="851" alt="image" src="https://github.com/user-attachments/assets/4beeff95-1ef1-494e-8db3-c9618037e1a7" />

### Remote Shell
<img width="1439" height="853" alt="image" src="https://github.com/user-attachments/assets/69547f82-33f9-452c-ae99-91c65de42be6" />

### System Backdoor
<img width="365" height="365" alt="image" src="https://github.com/user-attachments/assets/c01bc3da-42ef-4338-a759-b9149331868b" />


### Remote Exec
<img width="505" height="523" alt="image" src="https://github.com/user-attachments/assets/2586d08c-4bfc-44da-81f4-d1118028d985" />
