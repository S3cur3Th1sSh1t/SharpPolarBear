# SharpPolarBear

This is a weaponized version for one of the Exploits published by SandboxEscaper from here (https://github.com/SandboxEscaper/polarbearrepo). 

Most of the code comes from rasta-mouse CollectorService repository (https://github.com/rasta-mouse/CollectorService). I just changed the CVE-2019-0841-Code from the original SandboxEscaper C++ Code to C# and added some checks.

I have also added the required binaries (schtasks.exe, schedsvc.dll + an exported Windows XP Job File), which are unpacked during runtime and deleted afterwards. So you just need one binary here.

Windows Defender seams to have a heuristic detection for Applications creating a hardlink from C:\windows\system32\tasks to a file in C:\windows\system32\. So with Defender this executable is most likely blocked/detected.

You have to run the executable twice to get a system shell. 

## Legal disclaimer:
Usage of SharpPolarBear for attacking targets without prior mutual consent is illegal. It's the end user's responsibility to obey all applicable local, state and federal laws. Developers assume no liability and are not responsible for any misuse or damage caused by this program. Only use for educational / pentesting purposes.
