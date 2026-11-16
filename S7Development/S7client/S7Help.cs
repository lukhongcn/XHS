using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Globalization;
using S7.Net;
using XHS.BLL;
using XHS.Model;

/// <summary>
/// S7 配置辅助类。一个配置文件可以包含多台 PLC，每台 PLC 都使用自己的 IP 地址连接。
/// </summary>
public sealed class S7Help
{
    private readonly object monitorSync = new object();
    private readonly MESHelp mesHelp = new MESHelp();
    private readonly XHSPartPartProcess partPartProcess = new XHSPartPartProcess();
    private readonly PLCResult plcResult = new PLCResult();
    private readonly Dictionary<string, HeartbeatState> heartbeatStates = new Dictionary<string, HeartbeatState>(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> faultNotifications = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private Thread monitorThread;
    private ManualResetEventSlim monitorStopEvent;
    private Thread barcodeThread;
    private ManualResetEventSlim barcodeStopEvent;
    private readonly HashSet<string> barcodeFaultNotifications = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> handledBarcodeRequests = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private readonly object localResultFileSync = new object();

    public bool IsHeartbeatMonitoring
    {
        get { lock (monitorSync) return monitorThread != null && monitorThread.IsAlive; }
    }

    public bool IsBarcodeMonitoring
    {
        get { lock (monitorSync) return barcodeThread != null && barcodeThread.IsAlive; }
    }

    public byte[] ReadBlock(Plc plc, int db, int byteOffset, int length)
    {
        if (plc == null) throw new ArgumentNullException(nameof(plc));
        if (length < 1) throw new ArgumentOutOfRangeException(nameof(length));
        return plc.ReadBytes(DataType.DataBlock, db, byteOffset, length);
    }

    public void WriteBlock(Plc plc, int db, int byteOffset, byte[] data)
    {
        if (plc == null) throw new ArgumentNullException(nameof(plc));
        if (data == null || data.Length == 0) throw new ArgumentException("写入数据不能为空。", nameof(data));
        plc.WriteBytes(DataType.DataBlock, db, byteOffset, data);
    }

    public byte[] ReadBlock(PlcConfig target, int length)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        using (var plc = new Plc(CpuType.S71200, target.Ip, (short)target.Rack, (short)target.Slot))
        {
            plc.Open();
            return ReadBlock(plc, target.Db, target.ByteOffset, length);
        }
    }

    public void WriteBlock(PlcConfig target, byte[] data)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        using (var plc = new Plc(CpuType.S71200, target.Ip, (short)target.Rack, (short)target.Slot))
        {
            plc.Open();
            WriteBlock(plc, target.Db, target.ByteOffset, data);
        }
    }

    public void StartHeartbeatMonitor(string configPath, Action<string> messageReceived, Action<string> faultReceived)
    {
        HeartbeatConfig config = ReadHeartbeatConfig(configPath);
        lock (monitorSync)
        {
            if (monitorThread != null && monitorThread.IsAlive) return;
            faultNotifications.Clear();
            monitorStopEvent = new ManualResetEventSlim(false);
            monitorThread = new Thread(() => MonitorLoop(config, messageReceived, faultReceived))
            {
                IsBackground = true,
                Name = "S7 Heartbeat Monitor"
            };
            monitorThread.Start();
        }
    }

    public void StopHeartbeatMonitor()
    {
        Thread thread;
        ManualResetEventSlim stopEvent;
        lock (monitorSync)
        {
            thread = monitorThread;
            stopEvent = monitorStopEvent;
            if (stopEvent != null) stopEvent.Set();
        }

        if (thread != null && thread.IsAlive) thread.Join(3000);

        lock (monitorSync)
        {
            if (monitorThread == thread) monitorThread = null;
            if (monitorStopEvent == stopEvent) monitorStopEvent = null;
        }
        if (stopEvent != null) stopEvent.Dispose();
    }

    public void StartBarcodeMonitor(string configPath, Action<string> messageReceived, Action<string> faultReceived)
    {
        HeartbeatConfig config = ReadHeartbeatConfig(configPath);
        lock (monitorSync)
        {
            if (barcodeThread != null && barcodeThread.IsAlive) return;
            barcodeFaultNotifications.Clear();
            barcodeStopEvent = new ManualResetEventSlim(false);
            barcodeThread = new Thread(() => BarcodeLoop(config, messageReceived, faultReceived))
            {
                IsBackground = true,
                Name = "S7 Barcode Monitor"
            };
            barcodeThread.Start();
        }
    }

    public void StopBarcodeMonitor()
    {
        Thread thread;
        ManualResetEventSlim stopEvent;
        lock (monitorSync)
        {
            thread = barcodeThread;
            stopEvent = barcodeStopEvent;
            if (stopEvent != null) stopEvent.Set();
        }
        if (thread != null && thread.IsAlive) thread.Join(3000);
        lock (monitorSync)
        {
            if (barcodeThread == thread) barcodeThread = null;
            if (barcodeStopEvent == stopEvent) barcodeStopEvent = null;
        }
        if (stopEvent != null) stopEvent.Dispose();
    }

    public void CheckHeartbeats(string configPath, Action<string> messageReceived, Action<string> faultReceived)
    {
        HeartbeatConfig config = ReadHeartbeatConfig(configPath);
        CheckAllHeartbeats(config, messageReceived, faultReceived);
    }

    private void MonitorLoop(HeartbeatConfig config, Action<string> messageReceived, Action<string> faultReceived)
    {
        ManualResetEventSlim stopEvent;
        lock (monitorSync) stopEvent = monitorStopEvent;
        while (!stopEvent.IsSet)
        {
            CheckAllHeartbeats(config, messageReceived, faultReceived);
            stopEvent.Wait(Math.Max(100, config.PollIntervalMs));
        }
    }

    private void BarcodeLoop(HeartbeatConfig config, Action<string> messageReceived, Action<string> faultReceived)
    {
        ManualResetEventSlim stopEvent;
        lock (monitorSync) stopEvent = barcodeStopEvent;
        while (!stopEvent.IsSet)
        {
            foreach (PlcConfig target in config.Plc)
                CheckBarcodeRequest(target, messageReceived, faultReceived);
            stopEvent.Wait(Math.Max(100, config.PollIntervalMs));
        }
    }

    private void CheckBarcodeRequest(PlcConfig target, Action<string> messageReceived, Action<string> faultReceived)
    {
        if (target.Interaction == null || target.Barcode == null || target.PartProcessResult == null || target.PartProcessFailure == null) return;
        try
        {
            using (var plc = new Plc(CpuType.S71200, target.Ip, (short)target.Rack, (short)target.Slot))
            {
                plc.Open();
                byte[] interactionBytes = ReadBlock(plc, target.Interaction.Db, target.Interaction.ByteOffset, 1);
                if (!mesHelp.IsBarcodeRequest(interactionBytes[0], target.Interaction))
                {
                    string requestPrefix = target.Ip + "|";
                    handledBarcodeRequests.RemoveWhere(key => key.StartsWith(requestPrefix, StringComparison.OrdinalIgnoreCase));
                    barcodeFaultNotifications.Remove(target.Ip);
                    return;
                }

                byte[] barcodeBytes = ReadBlock(plc, target.Barcode.Db, target.Barcode.ByteOffset, target.Barcode.MaxLength);
                string barcode = mesHelp.DecodeBarcode(barcodeBytes, target.Barcode);
                byte[] realBytes = ReadBlock(plc, target.TestValue.Db, target.TestValue.ByteOffset, target.TestValue.Length);
                double testValue = ReadS7Real(realBytes);
                string requestKey = target.Ip + "|" + barcode;
                if (handledBarcodeRequests.Contains(requestKey)) return;
                Publish(messageReceived, string.Format("PLC={0} BARCODE_REQUEST value={1}", target.Ip, barcode));

                List<XHSPartPartProcessInfo> processes =
                    partPartProcess.GetPartProcessesByPartNoIdAndProcessIdAndStatusId(
                        barcode,
                        "QMJC",
                        "Ready");
                bool isReady = processes != null
                    && processes.Count > 0;
                byte[] judgementBytes = ReadBlock(plc, target.Judgement.Db, target.Judgement.ByteOffset, target.Judgement.Length);
                int plcResultCode = (judgementBytes[0] << 8) | judgementBytes[1];
                if (plcResultCode < 0 || plcResultCode > 2) plcResultCode = 0;
                if (!isReady)
                {
                    Publish(messageReceived, string.Format(
                        "PLC={0} BARCODE_REQUEST rejected, partno_id={1}, statusid={2}",
                        target.Ip,
                        barcode,
                        processes == null || processes.Count == 0 ? "空" : processes[0].StatusId));
                }
                else
                {
                    Publish(messageReceived, string.Format(
                        "PLC={0} BARCODE_REQUEST accepted, partno_id={1}, statusid=Ready",
                        target.Ip, barcode));
                }

                byte[] resultBytes = ReadBlock(plc, target.PartProcessResult.Db, target.PartProcessResult.ByteOffset, 1);
                int resultMask = 1 << target.PartProcessResult.Bit;
                int failureMask = 1 << target.PartProcessFailure.Bit;
                resultBytes[0] = plcResultCode == 1
                    ? (byte)(resultBytes[0] | resultMask)
                    : (byte)(resultBytes[0] & ~resultMask);
                resultBytes[0] = plcResultCode == 2
                    ? (byte)(resultBytes[0] | failureMask)
                    : (byte)(resultBytes[0] & ~failureMask);
                WriteBlock(plc, target.PartProcessResult.Db, target.PartProcessResult.ByteOffset, resultBytes);

                interactionBytes[0] = isReady
                    ? mesHelp.SetBarcodeRequest(interactionBytes[0], target.Interaction)
                    : mesHelp.ClearBarcodeRequest(interactionBytes[0], target.Interaction);
                WriteBlock(plc, target.Interaction.Db, target.Interaction.ByteOffset, interactionBytes);
                if (isReady) handledBarcodeRequests.Add(requestKey);

                XHSPartPartProcessInfo partProcessInfo = isReady ? processes[0] : null;
                PLCResultInfo plcResultInfo = new PLCResultInfo
                {
                    ProcessNo = partProcessInfo == null || !partProcessInfo.ProcessNo.HasValue ? 0 : partProcessInfo.ProcessNo.Value,
                    PartProcessId = partProcessInfo == null || !partProcessInfo.Id.HasValue ? 0 : partProcessInfo.Id.Value,
                    CreateDate = DateTime.Now,
                    ResultValue = testValue,
                    Result = plcResultCode
                };
                bool saved = plcResult.InsertPLCResult(plcResultInfo);
                if (!saved)
                {
                    saved = SavePLCResultLocally(barcode, plcResultInfo);
                }

                byte[] saveStatusBytes = ReadBlock(plc, target.SaveStatus.Db, target.SaveStatus.ByteOffset, 1);
                int saveStatusMask = 1 << target.SaveStatus.Bit;
                saveStatusBytes[0] = saved
                    ? (byte)(saveStatusBytes[0] | saveStatusMask)
                    : (byte)(saveStatusBytes[0] & ~saveStatusMask);
                int saveFailureMask = 1 << target.SaveFailure.Bit;
                saveStatusBytes[0] = saved
                    ? (byte)(saveStatusBytes[0] & ~saveFailureMask)
                    : (byte)(saveStatusBytes[0] | saveFailureMask);
                WriteBlock(plc, target.SaveStatus.Db, target.SaveStatus.ByteOffset, saveStatusBytes);
                Publish(messageReceived, string.Format(
                    "PLC={0} BARCODE_REQUEST handled, DB{1}.{2}.{3}={4}, DB{5}.{6}.{7}={8}, DB{9}.{10}.{11}={12}, DB{13}.{14}.{15}={16}, DB{17}.{18}.{19}={20}",
                    target.Ip,
                    target.PartProcessResult.Db,
                    target.PartProcessResult.ByteOffset,
                    target.PartProcessResult.Bit,
                    isReady ? 1 : 0,
                    target.Interaction.Db,
                    target.Interaction.ByteOffset,
                    target.Interaction.Bit,
                    isReady ? 1 : 0,
                    target.PartProcessFailure.Db,
                    target.PartProcessFailure.ByteOffset,
                    target.PartProcessFailure.Bit,
                    isReady ? 0 : 1,
                    target.SaveStatus.Db,
                    target.SaveStatus.ByteOffset,
                    target.SaveStatus.Bit,
                    saved ? 1 : 0,
                    target.SaveFailure.Db,
                    target.SaveFailure.ByteOffset,
                    target.SaveFailure.Bit,
                    saved ? 0 : 1));
                barcodeFaultNotifications.Remove(target.Ip);
            }
        }
        catch (Exception ex)
        {
            string message = string.Format("PLC={0} BARCODE_COMMUNICATION_FAULT error={1}", target.Ip, ex.Message);
            if (barcodeFaultNotifications.Add(target.Ip)) Publish(faultReceived, message);
        }
    }

    public bool SaveReadTestResult(string barcode, double testValue, int result, Action<string> messageReceived)
    {
        XHSPartPartProcessInfo partProcessInfo = null;
        try
        {
            List<XHSPartPartProcessInfo> processes = partPartProcess.GetPartProcessesByPartNoIdAndProcessIdAndStatusId(
                barcode,
                "QMJC",
                "Ready");
            if (processes != null && processes.Count > 0) partProcessInfo = processes[0];
        }
        catch (Exception ex)
        {
            Publish(messageReceived, "READ_TEST 查询工序失败：" + ex.Message);
        }

        PLCResultInfo info = new PLCResultInfo
        {
            ProcessNo = partProcessInfo == null || !partProcessInfo.ProcessNo.HasValue ? 0 : partProcessInfo.ProcessNo.Value,
            PartProcessId = partProcessInfo == null || !partProcessInfo.Id.HasValue ? 0 : partProcessInfo.Id.Value,
            CreateDate = DateTime.Now,
            ResultValue = testValue,
            Result = result
        };
        bool saved = plcResult.InsertPLCResult(info);
        if (!saved) saved = SavePLCResultLocally(barcode, info);
        Publish(messageReceived, string.Format(
            "READ_TEST barcode={0}, processno={1}, partprocessid={2}, result={3}, save={4}",
            barcode,
            info.ProcessNo,
            info.PartProcessId,
            result,
            saved ? "OK" : "FAILED"));
        return saved;
    }

    private void CheckAllHeartbeats(HeartbeatConfig config, Action<string> messageReceived, Action<string> faultReceived)
    {
        foreach (PlcConfig target in config.Plc)
            CheckHeartbeat(target, config.HeartbeatTimeoutSeconds, messageReceived, faultReceived);
    }

    private void CheckHeartbeat(PlcConfig target, int timeoutSeconds, Action<string> messageReceived, Action<string> faultReceived)
    {
        try
        {
            using (var plc = new Plc(CpuType.S71200, target.Ip, (short)target.Rack, (short)target.Slot))
            {
                plc.Open();
                byte[] bytes = ReadBlock(plc, target.Db, target.ByteOffset, 1);
                bool plcHeartbeat = (bytes[0] & (1 << target.PlcHeartbeatBit)) != 0;
                bool mesHeartbeat = (bytes[0] & (1 << target.MesHeartbeatBit)) != 0;

                HeartbeatState state;
                if (!heartbeatStates.TryGetValue(target.Ip, out state) || state.PlcValue != plcHeartbeat)
                {
                    state = new HeartbeatState { PlcValue = plcHeartbeat, LastChangeUtc = DateTime.UtcNow };
                    heartbeatStates[target.Ip] = state;
                }
                bool timeout = (DateTime.UtcNow - state.LastChangeUtc).TotalSeconds > timeoutSeconds;
                bool nextMesHeartbeat = !mesHeartbeat;
                int mesHeartbeatMask = 1 << target.MesHeartbeatBit;
                bytes[0] = nextMesHeartbeat
                    ? (byte)(bytes[0] | mesHeartbeatMask)
                    : (byte)(bytes[0] & ~mesHeartbeatMask);
                WriteBlock(plc, target.Db, target.ByteOffset, bytes);
                byte[] verifyBytes = ReadBlock(plc, target.Db, target.ByteOffset, 1);
                bool mesHeartbeatAfterWrite = (verifyBytes[0] & mesHeartbeatMask) != 0;

                Publish(messageReceived, string.Format(
                    "PLC={0} PLCHeartbeat={1} MESBefore={2} MESAfterWrite={3} MESWrite={4} STATUS={5}",
                    target.Ip, plcHeartbeat, mesHeartbeat, mesHeartbeatAfterWrite,
                    mesHeartbeatAfterWrite == nextMesHeartbeat ? "OK" : "FAILED",
                    timeout ? "COMMUNICATION_FAULT" : "OK"));
                if (timeout)
                {
                    if (faultNotifications.Add(target.Ip))
                        Publish(faultReceived, string.Format("PLC={0} STATUS=COMMUNICATION_FAULT 心跳超过 {1} 秒未变化。", target.Ip, timeoutSeconds));
                }
                else
                {
                    faultNotifications.Remove(target.Ip);
                }
            }
        }
        catch (Exception ex)
        {
            string message = string.Format("PLC={0} STATUS=COMMUNICATION_FAULT error={1}", target.Ip, ex.Message);
            if (faultNotifications.Add(target.Ip)) Publish(faultReceived, message);
            Publish(messageReceived, message);
        }
    }

    private static void Publish(Action<string> callback, string message)
    {
        if (callback != null) callback(message);
    }

    private static double ReadS7Real(byte[] data)
    {
        if (data == null || data.Length != 4) throw new ArgumentException("S7 REAL 必须是 4 字节。", nameof(data));
        if (BitConverter.IsLittleEndian) Array.Reverse(data);
        return BitConverter.ToSingle(data, 0);
    }

    private bool SavePLCResultLocally(string barcode, PLCResultInfo info)
    {
        try
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "PLCResult.txt");
            string line = string.Format(
                CultureInfo.InvariantCulture,
                "{0:O}\tbarcode={1}\tprocessno={2}\tpartprocessid={3}\tresultvalue={4}\tresult={5}{6}",
                info.CreateDate ?? DateTime.Now,
                barcode ?? string.Empty,
                info.ProcessNo.HasValue ? info.ProcessNo.Value.ToString(CultureInfo.InvariantCulture) : string.Empty,
                info.PartProcessId.HasValue ? info.PartProcessId.Value.ToString(CultureInfo.InvariantCulture) : string.Empty,
                info.ResultValue.HasValue ? info.ResultValue.Value.ToString(CultureInfo.InvariantCulture) : string.Empty,
                info.Result,
                Environment.NewLine);
            lock (localResultFileSync)
            {
                File.AppendAllText(filePath, line, Encoding.UTF8);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public HeartbeatConfig ReadHeartbeatConfig(string configPath)
    {
        if (string.IsNullOrWhiteSpace(configPath))
            throw new ArgumentException("心跳配置文件路径不能为空。", nameof(configPath));

        using (var stream = File.OpenRead(configPath))
        {
            var serializer = new DataContractJsonSerializer(typeof(HeartbeatConfig));
            var config = (HeartbeatConfig)serializer.ReadObject(stream);
            if (config == null || config.Plc == null || config.Plc.Length == 0)
                throw new System.IO.InvalidDataException("JSON 中没有配置 Plc。");

            foreach (PlcConfig plc in config.Plc)
            {
                if (plc == null || string.IsNullOrWhiteSpace(plc.Ip))
                    throw new System.IO.InvalidDataException("每台 PLC 的心跳配置都必须包含 ip 地址。");
                if (plc.Interaction == null || plc.ReadPermission == null || plc.Judgement == null || plc.TestValue == null || plc.SaveStatus == null || plc.SaveFailure == null || plc.Barcode == null)
                    throw new System.IO.InvalidDataException("每台 PLC 都必须配置 interaction、readPermission、judgement、testValue、saveStatus、saveFailure 和 barcode 地址块。");
            }

            return config;
        }
    }
}

internal sealed class HeartbeatState
{
    public bool PlcValue { get; set; }
    public DateTime LastChangeUtc { get; set; }
}

[DataContract]
public sealed class HeartbeatConfig
{
    [DataMember(Name = "pollIntervalMs")] public int PollIntervalMs { get; set; } = 1000;
    [DataMember(Name = "heartbeatTimeoutSeconds")] public int HeartbeatTimeoutSeconds { get; set; } = 5;
    [DataMember(Name = "plc")] public PlcConfig[] Plc { get; set; }
}

[DataContract]
public sealed class PlcConfig
{
    [DataMember(Name = "ip")] public string Ip { get; set; }
    [DataMember(Name = "rack")] public int Rack { get; set; }
    [DataMember(Name = "slot")] public int Slot { get; set; }
    [DataMember(Name = "db")] public int Db { get; set; } = 50;
    [DataMember(Name = "byteOffset")] public int ByteOffset { get; set; } = 266;
    [DataMember(Name = "plcHeartbeatBit")] public int PlcHeartbeatBit { get; set; } = 0;
    [DataMember(Name = "mesHeartbeatBit")] public int MesHeartbeatBit { get; set; } = 7;
    [DataMember(Name = "interaction")] public PlcBitConfig Interaction { get; set; }
    [DataMember(Name = "partProcessResult")] public PlcBitConfig PartProcessResult { get; set; }
    [DataMember(Name = "partProcessFailure")] public PlcBitConfig PartProcessFailure { get; set; }
    [DataMember(Name = "readPermission")] public PlcBitConfig ReadPermission { get; set; }
    [DataMember(Name = "judgement")] public UIntConfig Judgement { get; set; }
    [DataMember(Name = "testValue")] public RealConfig TestValue { get; set; }
    [DataMember(Name = "saveStatus")] public PlcBitConfig SaveStatus { get; set; }
    [DataMember(Name = "saveFailure")] public PlcBitConfig SaveFailure { get; set; }
    [DataMember(Name = "barcode")] public BarcodeConfig Barcode { get; set; }
}

[DataContract]
public sealed class PlcBitConfig
{
    [DataMember(Name = "name")] public string Name { get; set; }
    [DataMember(Name = "db")] public int Db { get; set; }
    [DataMember(Name = "byteOffset")] public int ByteOffset { get; set; }
    [DataMember(Name = "bit")] public int Bit { get; set; }
    [DataMember(Name = "initialValue")] public bool InitialValue { get; set; }
    [DataMember(Name = "description")] public string Description { get; set; }
}

[DataContract]
public sealed class BarcodeConfig
{
    [DataMember(Name = "name")] public string Name { get; set; }
    [DataMember(Name = "db")] public int Db { get; set; }
    [DataMember(Name = "byteOffset")] public int ByteOffset { get; set; }
    [DataMember(Name = "maxLength")] public int MaxLength { get; set; } = 64;
    [DataMember(Name = "encoding")] public string Encoding { get; set; } = "ASCII";
    [DataMember(Name = "description")] public string Description { get; set; }
}

[DataContract]
public sealed class RealConfig
{
    [DataMember(Name = "name")] public string Name { get; set; }
    [DataMember(Name = "db")] public int Db { get; set; }
    [DataMember(Name = "byteOffset")] public int ByteOffset { get; set; }
    [DataMember(Name = "length")] public int Length { get; set; } = 4;
    [DataMember(Name = "description")] public string Description { get; set; }
}

[DataContract]
public sealed class UIntConfig
{
    [DataMember(Name = "name")] public string Name { get; set; }
    [DataMember(Name = "db")] public int Db { get; set; }
    [DataMember(Name = "byteOffset")] public int ByteOffset { get; set; }
    [DataMember(Name = "length")] public int Length { get; set; } = 2;
    [DataMember(Name = "description")] public string Description { get; set; }
}
