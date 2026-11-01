# Stage 2 Communication Test

## Executed test

- Server: official Snap7 Win64 DLL, Snap7 Server API, DB1, 256 bytes, TCP 102.
- Client: S7.NetPlus 0.20.0, CPU type S71200, rack 0, slot 1.
- Address: `127.0.0.1`.
- Operation: write 8 bytes at offset 8, then read the same 8 bytes.

Observed output:

```text
CONNECT ip=127.0.0.1 status=OK
WRITE db=1 offset=8 length=8 hex=10 20 30 40 50 60 70 80
READ  db=1 offset=8 length=8 hex=10 20 30 40 50 60 70 80
VERIFY SUCCESS elapsedMs=172
```

This is real S7 TCP traffic between separate processes. The client does not share the server buffer or call server business objects.

## Server heartbeat test

The Server writes `DB51`, byte offset `0`, directly in its registered buffer. The value toggles between `0` and `1` every 1000 ms. Two client read-only checks separated by 1.1 seconds returned:

```text
READ db=51 offset=0 length=1 hex=01
READ db=51 offset=0 length=1 hex=00
```

## Negative test observed

When the server was not listening, the client reported a connection failure to `127.0.0.1:102` with the underlying Windows socket error “actively refused”.

## Remaining before final x64/UI stage

- The x64 write/read test is complete. The next step is the WinForms server/client test UI and its configurable generic DB controls.
