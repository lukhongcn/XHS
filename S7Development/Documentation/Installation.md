# Installation

1. Install Visual Studio with .NET Framework 4.8 development tools and the desktop C# workload.
2. Obtain the official Snap7 Win64 build from the official repository (the repository currently starts at release 1.4.3).
3. Copy the official Win64 `snap7.dll` to `ThirdParty/Snap7/Win64/snap7.dll` and keep the official LGPL license text in `ThirdParty/Licenses`.
4. Restore and build:

```powershell
dotnet restore S7Development.sln
dotnet build S7Development.sln -c Release
```

5. Start the simulator first:

```powershell
S7ServerSimulator.exe --db 1 --length 256
```

6. In another console run the client:

```powershell
S7TestClient.exe --ip 127.0.0.1 --db 1 --offset 8 --length 8
```

To observe the server-side heartbeat:

```powershell
S7TestClient.exe --ip 127.0.0.1 --db 51 --offset 0 --length 1 --read-only
```

Alternatively start `S7TestClient.exe` without arguments to open the WinForms interface. Select the Server executable if the auto-detected path is not correct, click `启动 Server`, then click `连接并读写验证`. The interface starts and stops only the Server process it created.

Do not stop unrelated services to free TCP 102. If the port is occupied, report the owning process and use a separate test machine or a Snap7 build/API that explicitly supports a different port; the verified Server API here does not.
