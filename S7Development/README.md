# S7 Development Environment

This directory contains the independent Snap7 Server simulator and S7.NetPlus test client.

Stage 1 and Stage 2 intentionally contain no MES code, SQL Server access, or business protocol.

## Components

- `S7ServerSimulator`: native Snap7 Server API host. It registers generic DB byte arrays and serves real S7 TCP traffic.
- `S7client`: S7.NetPlus client used to connect, write, read, and verify raw bytes.

The client heartbeat configuration is JSON-based. Each PLC is identified only by its IP and can have its own rack, slot and DB settings. The default mapping is `DB50.DBX266.0` for the PLC heartbeat and `DB50.DBX266.7` for the MES heartbeat. If the PLC heartbeat remains unchanged for more than 5 seconds, that PLC is reported as `COMMUNICATION_FAULT`.

The simulator uses Snap7's fixed ISO-on-TCP port 102. Snap7 Server does not expose a configurable listener port through the verified API. It also registers `DB50` as a server-side heartbeat area and toggles `DB50.DBX266.0` between `false` and `true` every second. A heartbeat monitor should treat an unchanged value for more than 5 seconds as a communication fault.

## Build and run

```powershell
dotnet restore S7Development.sln
dotnet build S7Development.sln -c Release
dotnet run --project S7ServerSimulator -c Release -- --db 1 --length 256
dotnet run --project S7client -c Release -- --ip 127.0.0.1 --db 1 --offset 8 --length 8
dotnet run --project S7client -c Release -- --config S7client/S7client.json --monitor
```

To simulate a stopped PLC heartbeat, start the simulator with `--freeze-heartbeat`, then start the client with `--monitor --popup`. After 5 seconds, the MES client displays a communication alarm dialog.

```powershell
dotnet run --project S7ServerSimulator -c Release -- --db 50 --length 267 --freeze-heartbeat
dotnet run --project S7client -c Release -- --config S7client/S7client.json --monitor --popup
```

Read the server heartbeat without writing it:

```powershell
dotnet run --project S7client -c Release -- --ip 127.0.0.1 --db 50 --offset 266 --length 1 --read-only
```

Without command-line arguments, `S7client.exe` opens the WinForms test interface. Use `启动 Server` to launch the independent `S7ServerSimulator.exe`, then use `连接并读写验证` to run the S7.NetPlus test.

The client writes `10 20 30 40 50 60 70 80`, reads it back, and exits non-zero if the bytes do not match.

## Dependency record

| Component | Version | License | Source |
|---|---:|---|---|
| Snap7 | 1.4.3 (official GitHub Win64 build) | LGPL-3.0-or-later | https://github.com/davenardella/snap7 |
| S7.NetPlus | 0.20.0 | MIT | https://github.com/S7NetPlus/s7netplus |

The corresponding license texts are kept under `ThirdParty/Licenses`.
