using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Xml;
using S7.Net;
using XHS.BLL;
using XHS.Model;

/// <summary>
/// S7 配置辅助类。一个配置文件可以包含多台 PLC，每台 PLC 都使用自己的 IP 地址连接。
/// </summary>
public sealed class S7Help
{
    private const string PlcResultAbnormalCode = "0";
    private const string PlcResultQualifiedCode = "1";
    private const string PlcResultUnqualifiedCode = "2";

    private readonly object monitorSync = new object();
    private readonly MESHelp mesHelp = new MESHelp();
    private readonly XHSPartPartProcess partPartProcess = new XHSPartPartProcess();
    private readonly BLL.ScanFlowStep scanFlowStep = new BLL.ScanFlowStep();
    private readonly BLL.ScanFlowScanRecord scanFlowScanRecord = new BLL.ScanFlowScanRecord();
    private readonly XHS.BLL.PLCResultProcessService plcResultProcessService = new XHS.BLL.PLCResultProcessService();
    private readonly PLCResult plcResult = new PLCResult();
    private readonly Dictionary<string, HeartbeatState> heartbeatStates = new Dictionary<string, HeartbeatState>(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> faultNotifications = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private Thread monitorThread;
    private ManualResetEventSlim monitorStopEvent;
    private Thread barcodeThread;
    private ManualResetEventSlim barcodeStopEvent;
    private Thread plcResultThread;
    private ManualResetEventSlim plcResultStopEvent;
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
            plcResultStopEvent = new ManualResetEventSlim(false);
            plcResultThread = new Thread(() => PLCResultLoop(messageReceived, faultReceived))
            {
                IsBackground = true,
                Name = "PLC Result Processor"
            };
            plcResultThread.Start();
        }
    }

    public void StopBarcodeMonitor()
    {
        Thread thread;
        ManualResetEventSlim stopEvent;
        Thread resultThread;
        ManualResetEventSlim resultStopEvent;
        lock (monitorSync)
        {
            thread = barcodeThread;
            stopEvent = barcodeStopEvent;
            resultThread = plcResultThread;
            resultStopEvent = plcResultStopEvent;
            if (stopEvent != null) stopEvent.Set();
            if (resultStopEvent != null) resultStopEvent.Set();
        }
        if (thread != null && thread.IsAlive) thread.Join(3000);
        if (resultThread != null && resultThread.IsAlive) resultThread.Join(3000);
        lock (monitorSync)
        {
            if (barcodeThread == thread) barcodeThread = null;
            if (barcodeStopEvent == stopEvent) barcodeStopEvent = null;
            if (plcResultThread == resultThread) plcResultThread = null;
            if (plcResultStopEvent == resultStopEvent) plcResultStopEvent = null;
        }
        if (stopEvent != null) stopEvent.Dispose();
        if (resultStopEvent != null) resultStopEvent.Dispose();
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
            {
                CheckBarcodeRequest(target, messageReceived, faultReceived);
            }
            stopEvent.Wait(Math.Max(100, config.PollIntervalMs));
        }
    }

    private void PLCResultLoop(Action<string> messageReceived, Action<string> faultReceived)
    {
        ManualResetEventSlim stopEvent;
        lock (monitorSync) stopEvent = plcResultStopEvent;
        while (!stopEvent.IsSet)
        {
            try
            {
                string message = plcResultProcessService.ProcessLatestUndealt();
                if (!string.IsNullOrWhiteSpace(message)) Publish(messageReceived, "PLC_RESULT_PROCESS " + message);
            }
            catch (Exception ex)
            {
                Publish(faultReceived, "PLC_RESULT_PROCESS error=" + ex.Message);
            }
            stopEvent.Wait(GetPLCResultProcessIntervalMs());
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
                        GetScanFlowCode(),
                        "Ready");
                bool isReady = processes != null
                    && processes.Count > 0;
                SaveScanHistory(barcode, target.Ip, isReady, messageReceived);
                byte[] judgementBytes = ReadBlock(plc, target.Judgement.Db, target.Judgement.ByteOffset, target.Judgement.Length);
                int plcResultCode = (judgementBytes[0] << 8) | judgementBytes[1];
                if (!IsValidPlcResultCode(plcResultCode)) plcResultCode = int.Parse(PlcResultAbnormalCode, CultureInfo.InvariantCulture);
                XHSPartPartProcessInfo partProcessInfo = isReady ? processes[0] : null;
                PLCResultInfo plcResultInfo = new PLCResultInfo
                {
                    ProcessNo = partProcessInfo == null || !partProcessInfo.ProcessNo.HasValue ? 0 : partProcessInfo.ProcessNo.Value,
                    PartProcessId = partProcessInfo == null || !partProcessInfo.Id.HasValue ? 0 : partProcessInfo.Id.Value,
                    CreateDate = DateTime.Now,
                    ResultValue = testValue,
                    Result = plcResultCode
                };
                bool databaseSaved = plcResult.InsertPLCResult(plcResultInfo);
                bool localSaved = false;
                if (!databaseSaved) localSaved = SavePLCResultLocally(barcode, plcResultInfo);
                bool saved = databaseSaved;
                Publish(messageReceived, string.Format(
                    "PLC={0} BARCODE_REQUEST barcode={1}, testValue={2}, result={3}, tb_PLCResult insert={4}, local PLCResult.csv={5}",
                    target.Ip,
                    barcode,
                    testValue,
                    plcResultCode,
                    databaseSaved ? "OK" : "FAILED",
                    localSaved ? "OK" : "NOT_USED"));
                WriteSaveFeedback(plc, target, saved, messageReceived);
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
                resultBytes[0] = IsPlcResultCode(plcResultCode, PlcResultQualifiedCode)
                    ? (byte)(resultBytes[0] | resultMask)
                    : (byte)(resultBytes[0] & ~resultMask);
                resultBytes[0] = IsPlcResultCode(plcResultCode, PlcResultUnqualifiedCode)
                    ? (byte)(resultBytes[0] | failureMask)
                    : (byte)(resultBytes[0] & ~failureMask);
                WriteBlock(plc, target.PartProcessResult.Db, target.PartProcessResult.ByteOffset, resultBytes);

                interactionBytes[0] = isReady
                    ? mesHelp.SetBarcodeRequest(interactionBytes[0], target.Interaction)
                    : mesHelp.ClearBarcodeRequest(interactionBytes[0], target.Interaction);
                WriteBlock(plc, target.Interaction.Db, target.Interaction.ByteOffset, interactionBytes);
                if (isReady) handledBarcodeRequests.Add(requestKey);

                Publish(messageReceived, string.Format(
                    "PLC={0} BARCODE_REQUEST handled, DB{1}.{2}.{3}={4}, DB{5}.{6}.{7}={8}, DB{9}.{10}.{11}={12}",
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
                    isReady ? 0 : 1));
                barcodeFaultNotifications.Remove(target.Ip);
            }
        }
        catch (Exception ex)
        {
            string message = string.Format("PLC={0} BARCODE_COMMUNICATION_FAULT error={1}", target.Ip, ex.Message);
            if (barcodeFaultNotifications.Add(target.Ip)) Publish(faultReceived, message);
        }
    }

    private void SaveScanHistory(string barcode, string deviceInfo, bool isReady, Action<string> messageReceived)
    {
        string flowCode = GetScanFlowCode();
        string stepCode = GetScanStepCode();
        List<ScanFlowStepInfo> steps = scanFlowStep.GetStepsByFlowCode(flowCode);
        ScanFlowStepInfo step = null;
        if (steps != null)
        {
            foreach (ScanFlowStepInfo item in steps)
            {
                if (item != null && string.Equals(item.StepCode, stepCode, StringComparison.OrdinalIgnoreCase))
                {
                    step = item;
                    break;
                }
            }
        }

        if (step == null || !step.FlowId.HasValue || !step.StepId.HasValue)
        {
            Publish(messageReceived, string.Format(
                "PLC={0} BARCODE_HISTORY skipped, flowCode={1}, stepCode={2} not found",
                deviceInfo, flowCode, stepCode));
            return;
        }

        ScanFlowScanRecordInfo record = new ScanFlowScanRecordInfo
        {
            FlowId = step.FlowId,
            FlowCode = flowCode,
            BindingTaskId = barcode,
            StepId = step.StepId,
            StepCode = step.StepCode,
            SeqNo = step.StepNo,
            ScanContent = barcode,
            Status = isReady ? "已完成" : "未通过",
            DeviceInfo = deviceInfo,
            ScanTime = DateTime.Now
        };
        string saveMessage = scanFlowScanRecord.SaveRecords(new List<ScanFlowScanRecordInfo> { record });
        Publish(messageReceived, string.Format(
            "PLC={0} BARCODE_HISTORY {1}, barcode={2}, status={3}",
            deviceInfo,
            string.IsNullOrWhiteSpace(saveMessage) ? "saved" : "failed: " + saveMessage,
            barcode,
            record.Status));
    }

    private static string GetScanFlowCode()
    {
        return GetAppSetting("ScanFlowCode");
    }

    private static string GetScanStepCode()
    {
        return GetAppSetting("ScanStepCode");
    }

    private static string GetAppSetting(string key)
    {
        string configPath = Path.Combine(AppContext.BaseDirectory, "S7client.exe.config");
        if (!File.Exists(configPath)) configPath = Path.Combine(AppContext.BaseDirectory, "App.config");
        if (!File.Exists(configPath)) return null;

        XmlDocument document = new XmlDocument();
        document.Load(configPath);
        XmlNode node = document.SelectSingleNode("/configuration/appSettings/add[@key='" + key + "']");
        XmlElement element = node as XmlElement;
        return element == null ? null : element.GetAttribute("value");
    }

    private static int GetPLCResultProcessIntervalMs()
    {
        int interval;
        return int.TryParse(GetAppSetting("PLCResultProcessIntervalMs"), out interval) && interval >= 100
            ? interval
            : 5000;
    }

    private static bool IsValidPlcResultCode(int result)
    {
        return IsPlcResultCode(result, PlcResultAbnormalCode)
            || IsPlcResultCode(result, PlcResultQualifiedCode)
            || IsPlcResultCode(result, PlcResultUnqualifiedCode);
    }

    private static bool IsPlcResultCode(int result, string code)
    {
        return string.Equals(result.ToString(CultureInfo.InvariantCulture), code, StringComparison.Ordinal);
    }

    private void WriteSaveFeedback(Plc plc, PlcConfig target, bool saved, Action<string> messageReceived)
    {
        if (target.SaveStatus == null || target.SaveFailure == null) return;

        byte[] statusBytes = ReadBlock(plc, target.SaveStatus.Db, target.SaveStatus.ByteOffset, 1);
        int saveStatusMask = 1 << target.SaveStatus.Bit;
        statusBytes[0] = saved
            ? (byte)(statusBytes[0] | saveStatusMask)
            : (byte)(statusBytes[0] & ~saveStatusMask);
        int saveFailureMask = 1 << target.SaveFailure.Bit;
        statusBytes[0] = saved
            ? (byte)(statusBytes[0] & ~saveFailureMask)
            : (byte)(statusBytes[0] | saveFailureMask);
        WriteBlock(plc, target.SaveStatus.Db, target.SaveStatus.ByteOffset, statusBytes);

        if (target.ReadPermission != null)
        {
            byte[] permissionBytes = ReadBlock(plc, target.ReadPermission.Db, target.ReadPermission.ByteOffset, 1);
            int requestMask = 1 << target.ReadPermission.Bit;
            permissionBytes[0] = (byte)(permissionBytes[0] & ~requestMask);
            WriteBlock(plc, target.ReadPermission.Db, target.ReadPermission.ByteOffset, permissionBytes);
        }

        Publish(messageReceived, string.Format(
            "PLC={0} BARCODE_REQUEST result feedback, save={1}",
            target.Ip,
            saved ? 1 : 0));
    }

    private void CheckSaveRequest(PlcConfig target, Action<string> messageReceived, Action<string> faultReceived)
    {
        if (target.ReadPermission == null || target.Barcode == null || target.TestValue == null || target.Judgement == null
            || target.SaveStatus == null || target.SaveFailure == null) return;

        try
        {
            using (var plc = new Plc(CpuType.S71200, target.Ip, (short)target.Rack, (short)target.Slot))
            {
                plc.Open();
                byte[] permissionBytes = ReadBlock(plc, target.ReadPermission.Db, target.ReadPermission.ByteOffset, 1);
                int requestMask = 1 << target.ReadPermission.Bit;
                if ((permissionBytes[0] & requestMask) == 0)
                {
                    byte[] feedbackBytes = ReadBlock(plc, target.SaveStatus.Db, target.SaveStatus.ByteOffset, 1);
                    int feedbackSaveStatusMask = 1 << target.SaveStatus.Bit;
                    int feedbackSaveFailureMask = 1 << target.SaveFailure.Bit;
                    byte clearedFeedback = (byte)(feedbackBytes[0] & ~feedbackSaveStatusMask & ~feedbackSaveFailureMask);
                    if (clearedFeedback != feedbackBytes[0])
                    {
                        feedbackBytes[0] = clearedFeedback;
                        WriteBlock(plc, target.SaveStatus.Db, target.SaveStatus.ByteOffset, feedbackBytes);
                        Publish(messageReceived, string.Format(
                            "PLC={0} SAVE_REQUEST feedback cleared, DB{1}.{2}.{3}=0, DB{4}.{5}.{6}=0",
                            target.Ip,
                            target.SaveStatus.Db, target.SaveStatus.ByteOffset, target.SaveStatus.Bit,
                            target.SaveFailure.Db, target.SaveFailure.ByteOffset, target.SaveFailure.Bit));
                    }
                    return;
                }

                byte[] barcodeBytes = ReadBlock(plc, target.Barcode.Db, target.Barcode.ByteOffset, target.Barcode.MaxLength);
                string barcode = mesHelp.DecodeBarcode(barcodeBytes, target.Barcode);
                if (string.IsNullOrWhiteSpace(barcode))
                    throw new InvalidOperationException("PLC 存盘请求中的 DB50.2.0 没有二维码。");

                byte[] realBytes = ReadBlock(plc, target.TestValue.Db, target.TestValue.ByteOffset, target.TestValue.Length);
                double testValue = ReadS7Real(realBytes);
                byte[] judgementBytes = ReadBlock(plc, target.Judgement.Db, target.Judgement.ByteOffset, target.Judgement.Length);
                int result = (judgementBytes[0] << 8) | judgementBytes[1];
                if (!IsValidPlcResultCode(result)) result = int.Parse(PlcResultAbnormalCode, CultureInfo.InvariantCulture);

                List<XHSPartPartProcessInfo> processes = partPartProcess.GetPartProcessesByPartNoIdAndProcessIdAndStatusId(
                    barcode, GetScanFlowCode(), "Ready");
                XHSPartPartProcessInfo partProcessInfo = processes != null && processes.Count > 0 ? processes[0] : null;
                PLCResultInfo info = new PLCResultInfo
                {
                    ProcessNo = partProcessInfo == null || !partProcessInfo.ProcessNo.HasValue ? 0 : partProcessInfo.ProcessNo.Value,
                    PartProcessId = partProcessInfo == null || !partProcessInfo.Id.HasValue ? 0 : partProcessInfo.Id.Value,
                    CreateDate = DateTime.Now,
                    ResultValue = testValue,
                    Result = result
                };

                bool databaseSaved = plcResult.InsertPLCResult(info);
                bool localSaved = false;
                if (!databaseSaved) localSaved = SavePLCResultLocally(barcode, info);
                bool saved = databaseSaved;
                Publish(messageReceived, string.Format(
                    "PLC={0} SAVE_REQUEST barcode={1}, tb_PLCResult insert={2}, local PLCResult.csv={3}",
                    target.Ip, barcode, databaseSaved ? "OK" : "FAILED", localSaved ? "OK" : "NOT_USED"));

                byte[] statusBytes = ReadBlock(plc, target.SaveStatus.Db, target.SaveStatus.ByteOffset, 1);
                int saveStatusMask = 1 << target.SaveStatus.Bit;
                statusBytes[0] = saved
                    ? (byte)(statusBytes[0] | saveStatusMask)
                    : (byte)(statusBytes[0] & ~saveStatusMask);
                int saveFailureMask = 1 << target.SaveFailure.Bit;
                statusBytes[0] = saved
                    ? (byte)(statusBytes[0] & ~saveFailureMask)
                    : (byte)(statusBytes[0] | saveFailureMask);
                WriteBlock(plc, target.SaveStatus.Db, target.SaveStatus.ByteOffset, statusBytes);
                permissionBytes[0] = (byte)(permissionBytes[0] & ~requestMask);
                WriteBlock(plc, target.ReadPermission.Db, target.ReadPermission.ByteOffset, permissionBytes);
                Publish(messageReceived, string.Format(
                    "PLC={0} SAVE_REQUEST handled, DB{1}.{2}.{3}={4}, DB{5}.{6}.{7}={8}, DB{9}.{10}.{11}=0",
                    target.Ip,
                    target.SaveStatus.Db, target.SaveStatus.ByteOffset, target.SaveStatus.Bit, saved ? 1 : 0,
                    target.SaveFailure.Db, target.SaveFailure.ByteOffset, target.SaveFailure.Bit, saved ? 0 : 1,
                    target.ReadPermission.Db, target.ReadPermission.ByteOffset, target.ReadPermission.Bit));
                barcodeFaultNotifications.Remove(target.Ip);
            }
        }
        catch (Exception ex)
        {
            string message = string.Format("PLC={0} SAVE_REQUEST_COMMUNICATION_FAULT error={1}", target.Ip, ex.Message);
            if (barcodeFaultNotifications.Add(target.Ip)) Publish(faultReceived, message);
            Publish(messageReceived, message);
        }
    }

    public bool SaveReadTestResult(string barcode, double testValue, int result, Action<string> messageReceived)
    {
        XHSPartPartProcessInfo partProcessInfo = null;
        try
        {
            List<XHSPartPartProcessInfo> processes = partPartProcess.GetPartProcessesByPartNoIdAndProcessIdAndStatusId(
                barcode,
                GetScanFlowCode(),
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
        bool databaseSaved = plcResult.InsertPLCResult(info);
        bool localSaved = false;
        if (!databaseSaved) localSaved = SavePLCResultLocally(barcode, info);
        bool saved = databaseSaved;
        Publish(messageReceived, string.Format(
            "READ_TEST tb_PLCResult insert={0}, local PLCResult.csv={1}",
            databaseSaved ? "OK" : "FAILED",
            localSaved ? "OK" : "NOT_USED"));
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
            string filePath = Path.Combine(AppContext.BaseDirectory, "PLCResult.csv");
            lock (localResultFileSync)
            {
                bool writeHeader = !File.Exists(filePath) || new FileInfo(filePath).Length == 0;
                using (var writer = new StreamWriter(filePath, true, new UTF8Encoding(true)))
                {
                    if (writeHeader)
                        writer.WriteLine("CreateDate,Barcode,ProcessNo,PartProcessId,ResultValue,Result");

                    writer.WriteLine(string.Join(",", new[]
                    {
                        EscapeCsv(info.CreateDate ?? DateTime.Now),
                        EscapeCsv(barcode),
                        EscapeCsv(info.ProcessNo.HasValue ? info.ProcessNo.Value.ToString(CultureInfo.InvariantCulture) : string.Empty),
                        EscapeCsv(info.PartProcessId.HasValue ? info.PartProcessId.Value.ToString(CultureInfo.InvariantCulture) : string.Empty),
                        EscapeCsv(info.ResultValue.HasValue ? info.ResultValue.Value.ToString(CultureInfo.InvariantCulture) : string.Empty),
                        EscapeCsv(info.Result.ToString(CultureInfo.InvariantCulture))
                    }));
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string EscapeCsv(object value)
    {
        string text = value == null ? string.Empty : Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        return "\"" + text.Replace("\"", "\"\"") + "\"";
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
