using System;
using System.Runtime.InteropServices;
using System.Threading;

internal static class Program
{
    private const string Snap7Library = "snap7.dll";
    private const int ServerDbArea = 5;
    private const int HeartbeatDb = 50;
    private const int HeartbeatByteOffset = 266;
    private const byte HeartbeatBitMask = 0x01;
    private const int HeartbeatIntervalMs = 1000;

    [DllImport(Snap7Library, CallingConvention = CallingConvention.Cdecl)] private static extern IntPtr Srv_Create();
    [DllImport(Snap7Library, CallingConvention = CallingConvention.Cdecl)] private static extern int Srv_Destroy(ref IntPtr server);
    [DllImport(Snap7Library, CallingConvention = CallingConvention.Cdecl)] private static extern int Srv_Start(IntPtr server);
    [DllImport(Snap7Library, CallingConvention = CallingConvention.Cdecl)] private static extern int Srv_Stop(IntPtr server);
    [DllImport(Snap7Library, CallingConvention = CallingConvention.Cdecl)] private static extern int Srv_RegisterArea(IntPtr server, int areaCode, ushort index, IntPtr data, int size);
    [DllImport(Snap7Library, CallingConvention = CallingConvention.Cdecl)] private static extern int Srv_UnregisterArea(IntPtr server, int areaCode, ushort index);
    [DllImport(Snap7Library, CallingConvention = CallingConvention.Cdecl)] private static extern int Srv_GetStatus(IntPtr server, out int serverStatus, out int cpuStatus, out int clients);
    [DllImport(Snap7Library, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)] private static extern int Srv_ErrorText(int error, IntPtr message, int textSize);

    private static void Main(string[] args)
    {
        try { Run(args); }
        catch (Exception ex) { Console.Error.WriteLine("SERVER FAILED type={0} message={1}", ex.GetType().FullName, ex.Message); Environment.ExitCode = 1; }
    }

    private static void Run(string[] args)
    {
        int db = ReadInt(args, "--db", 1);
        int length = ReadInt(args, "--length", 256);
        bool freezeHeartbeat = HasFlag(args, "--freeze-heartbeat");
        int stopHeartbeatAfterSeconds = ReadInt(args, "--stop-heartbeat-after-seconds", 0);
        if (db < 1 || db > ushort.MaxValue || length < 1 || stopHeartbeatAfterSeconds < 0) throw new ArgumentException("DB and length must be positive; stop-heartbeat-after-seconds cannot be negative.");

        byte[] buffer = new byte[length];
        byte[] heartbeatBuffer = db == HeartbeatDb ? buffer : new byte[HeartbeatByteOffset + 1];
        if (db == HeartbeatDb && length <= HeartbeatByteOffset) throw new ArgumentException("DB50 length must include byte offset 266.");
        GCHandle pinned = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        GCHandle heartbeatPinned = default(GCHandle);
        IntPtr server = IntPtr.Zero;
        Timer heartbeatTimer = null;
        try
        {
            server = Srv_Create();
            if (server == IntPtr.Zero) throw new InvalidOperationException("Srv_Create returned null.");
            Check(Srv_RegisterArea(server, ServerDbArea, (ushort)db, pinned.AddrOfPinnedObject(), buffer.Length), "Srv_RegisterArea");
            if (db != HeartbeatDb)
            {
                heartbeatPinned = GCHandle.Alloc(heartbeatBuffer, GCHandleType.Pinned);
                Check(Srv_RegisterArea(server, ServerDbArea, HeartbeatDb, heartbeatPinned.AddrOfPinnedObject(), heartbeatBuffer.Length), "Srv_RegisterArea DB51 heartbeat");
            }
            Check(Srv_Start(server), "Srv_Start (TCP 102 may be occupied or require a different service to stop)");
            DateTime heartbeatStartUtc = DateTime.UtcNow;
            if (!freezeHeartbeat)
                heartbeatTimer = new Timer(_ =>
                {
                    if (stopHeartbeatAfterSeconds == 0 || (DateTime.UtcNow - heartbeatStartUtc).TotalSeconds < stopHeartbeatAfterSeconds)
                        ToggleHeartbeat(heartbeatBuffer);
                }, null, HeartbeatIntervalMs, HeartbeatIntervalMs);
            string mode = freezeHeartbeat ? "FROZEN" : (stopHeartbeatAfterSeconds == 0 ? "TOGGLE" : "TOGGLE_THEN_STOP");
            Console.WriteLine("READY db={0} length={1} port=102 heartbeat=DB50.DBX266.0 intervalMs=1000 timeoutMs=5000 mode={2} stopHeartbeatAfterSeconds={3}", db, length, mode, stopHeartbeatAfterSeconds);
            Console.WriteLine("Press Enter to stop.");
            while (!Console.KeyAvailable) { PrintStatus(server); Thread.Sleep(1000); }
            Console.ReadKey(true);
            if (heartbeatTimer != null) heartbeatTimer.Dispose();
            Srv_Stop(server);
            Srv_UnregisterArea(server, ServerDbArea, (ushort)db);
            if (db != HeartbeatDb) Srv_UnregisterArea(server, ServerDbArea, HeartbeatDb);
        }
        finally
        {
            if (heartbeatTimer != null) heartbeatTimer.Dispose();
            if (server != IntPtr.Zero) Srv_Destroy(ref server);
            if (heartbeatPinned.IsAllocated) heartbeatPinned.Free();
            if (pinned.IsAllocated) pinned.Free();
        }
    }

    private static void ToggleHeartbeat(byte[] buffer)
    {
        buffer[HeartbeatByteOffset] ^= HeartbeatBitMask;
    }

    private static void PrintStatus(IntPtr server)
    {
        int s, c, clients;
        if (Srv_GetStatus(server, out s, out c, out clients) == 0) Console.WriteLine("STATUS server={0} cpu={1} clients={2}", s, c, clients);
    }

    private static void Check(int result, string operation)
    {
        if (result == 0) return;
        IntPtr text = Marshal.AllocHGlobal(1024);
        try { Srv_ErrorText(result, text, 1024); throw new InvalidOperationException(operation + " failed: " + Marshal.PtrToStringAnsi(text)); }
        finally { Marshal.FreeHGlobal(text); }
    }

    private static int ReadInt(string[] args, string name, int fallback)
    {
        for (int i = 0; i + 1 < args.Length; i++) if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase)) return int.Parse(args[i + 1]);
        return fallback;
    }

    private static bool HasFlag(string[] args, string name)
    {
        for (int i = 0; i < args.Length; i++)
            if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }
}
