# Environment Check

Date: 2026-11-01

| Check | Result |
|---|---|
| OS | Windows 11-compatible build 10.0.26300, x64 host |
| .NET Framework reference assemblies | 4.8 present |
| .NET SDK | 9.0.302, MSBuild 17.14.13 |
| Visual Studio | `vswhere.exe` was not available on PATH; installation/version still needs confirmation in Visual Studio Installer |
| NuGet | Official NuGet restore succeeded for S7netplus 0.20.0 |
| TCP 102 before test | No listener detected |
| Snap7 API | Official C# example verified: `Srv_Create`, `Srv_RegisterArea`, `Srv_Start`, `Srv_GetStatus`, `Srv_Stop` |
| S7.NetPlus | Official repository/NuGet 0.20.0; .NET Framework 4.5.2+ support includes net48 |

Snap7 Server listens on its fixed ISO-on-TCP port 102 through the verified API; it does not provide a configurable listener-port API. No Windows firewall, service, or network setting was changed.

## Architecture note

The first runnable validation used a temporary 32-bit `snap7.dll` and therefore ran both the simulator and client as x86. The final validation uses the official Win64 DLL from the official Snap7 repository. DLL SHA-256: `A9D8C3395056C40006DFAE0056B90BAF4F3A2B51AC642F28D1D1117E9BA426A2`. The DLL, simulator, and client were all verified as PE machine `0x8664`; managed projects target .NET Framework 4.8.
