using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using S7.Net;

internal sealed class MainForm : Form
{
    private readonly TextBox serverPath = new TextBox();
    private readonly TextBox ip = new TextBox { Text = "127.0.0.1" };
    private readonly NumericUpDown db = new NumericUpDown { Minimum = 1, Maximum = 65535, Value = 1 };
    private readonly NumericUpDown serverLength = new NumericUpDown { Minimum = 1, Maximum = 65535, Value = 256 };
    private readonly NumericUpDown stopHeartbeatAfterSeconds = new NumericUpDown { Minimum = 0, Maximum = 86400, Value = 0 };
    private readonly NumericUpDown offset = new NumericUpDown { Minimum = 0, Maximum = 65535, Value = 8 };
    private readonly NumericUpDown readLength = new NumericUpDown { Minimum = 1, Maximum = 65535, Value = 8 };
    private readonly Label plcHeartbeatValue = new Label { Text = "PLC寄存器中的值：--", AutoSize = true, Anchor = AnchorStyles.Left };
    private readonly Label interactionValue = new Label { Text = "PLC寄存器中的值：--", AutoSize = true, Anchor = AnchorStyles.Left };
    private readonly Label readPermissionValue = new Label { Text = "PLC寄存器中的值：--", AutoSize = true, Anchor = AnchorStyles.Left };
    private readonly Label saveStatusValue = new Label { Text = "PLC寄存器中的值：--", AutoSize = true, Anchor = AnchorStyles.Left };
    private readonly Label saveFailureValue = new Label { Text = "PLC寄存器中的值：--", AutoSize = true, Anchor = AnchorStyles.Left };
    private readonly Label barcodeValue = new Label { Text = "PLC寄存器中的值：--", AutoSize = true, Anchor = AnchorStyles.Left };
    private readonly Label mesHeartbeatValue = new Label { Text = "PLC寄存器中的值：--", AutoSize = true, Anchor = AnchorStyles.Left };
    private readonly Label barcodeCheckValue = new Label { Text = "检测 --", AutoSize = true, Anchor = AnchorStyles.Left };
    private readonly Timer plcValueTimer = new Timer { Interval = 500 };
    private readonly S7Help s7Help = new S7Help();
    private readonly MESHelp mesHelp = new MESHelp();
    private readonly TextBox log = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both };
    private readonly TextBox plcResultLog = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both };
    private Process serverProcess;
    private StreamWriter heartbeatLogWriter;
    private readonly object heartbeatLogSync = new object();
    private ToolStripMenuItem readTestMenu;
    private bool heartbeatWarningShown;
    private int plcValueReading;

    public MainForm()
    {
        Text = "S7client";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 620);
        Size = new Size(1080, 700);
        BuildLayout();
        BuildMenu();
        serverPath.Text = FindServerExecutable();
        plcValueTimer.Tick += ReadHeartbeatValues;
        plcValueTimer.Start();
        FormClosing += (sender, args) =>
        {
            plcValueTimer.Stop();
            StopHeartbeatMonitor();
            StopBarcodeMonitor();
            StopServer();
        };
        Shown += (sender, args) => StartHeartbeatMonitor(sender, args);
        Shown += (sender, args) => StartBarcodeMonitor(sender, args);
    }

    private void BuildMenu()
    {
        var menu = new MenuStrip();
        var testMenu = new ToolStripMenuItem("测试");
        var stopPlcHeartbeat = new ToolStripMenuItem("PLC心跳停止");
        stopPlcHeartbeat.Click += RunStopPlcHeartbeatTest;
        var barcodeRequest = new ToolStripMenuItem("发起条码请求");
        barcodeRequest.Click += RunBarcodeRequestTest;
        var allowReadTest = new ToolStripMenuItem("允许读取测试");
        allowReadTest.Click += RunAllowReadTest;
        readTestMenu = new ToolStripMenuItem("读取") { Enabled = false };
        readTestMenu.Click += RunReadTest;
        testMenu.DropDownItems.Add(stopPlcHeartbeat);
        testMenu.DropDownItems.Add(barcodeRequest);
        testMenu.DropDownItems.Add(allowReadTest);
        testMenu.DropDownItems.Add(readTestMenu);
        menu.Items.Add(testMenu);
        MainMenuStrip = menu;
        Controls.Add(menu);
        menu.BringToFront();
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), ColumnCount = 1, RowCount = 2 };
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 58));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 42));

        var columns = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        columns.Controls.Add(BuildHostPanel(), 0, 0);
        columns.Controls.Add(BuildPlcPanel(), 1, 0);
        root.Controls.Add(columns, 0, 0);

        var logSplit = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            SplitterDistance = 150,
            FixedPanel = FixedPanel.Panel1
        };
        var communicationLogGroup = new GroupBox { Text = "心跳监控 / 通信日志", Dock = DockStyle.Fill, Padding = new Padding(8) };
        log.Dock = DockStyle.Fill;
        communicationLogGroup.Controls.Add(log);
        logSplit.Panel1.Controls.Add(communicationLogGroup);

        var plcResultLogGroup = new GroupBox { Text = "PLCResult 处理日志", Dock = DockStyle.Fill, Padding = new Padding(8) };
        plcResultLog.Dock = DockStyle.Fill;
        plcResultLogGroup.Controls.Add(plcResultLog);
        logSplit.Panel2.Controls.Add(plcResultLogGroup);
        root.Controls.Add(logSplit, 0, 1);
        Controls.Add(root);
    }

    private Control BuildHostPanel()
    {
        var group = new GroupBox { Text = "上位机", Dock = DockStyle.Fill, Padding = new Padding(8) };
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 4 };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 4; i++) panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

        panel.Controls.Add(new Label { Text = "Server 程序", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        serverPath.Dock = DockStyle.Fill;
        serverPath.ReadOnly = true;
        panel.Controls.Add(serverPath, 1, 0);
        panel.Controls.Add(new Label { Text = "S7端口：102（固定）", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 0);

        panel.Controls.Add(new Label { Text = "Server 控制", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        var start = new Button { Text = "启动 Server", Dock = DockStyle.Fill };
        start.Click += StartServer;
        panel.Controls.Add(start, 1, 1);
        var stop = new Button { Text = "停止 Server", Dock = DockStyle.Fill };
        stop.Click += (sender, args) => StopServer();
        panel.Controls.Add(stop, 2, 1);

        panel.Controls.Add(new Label { Text = "位", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
        panel.Controls.Add(new Label { Text = "MES心跳 DB50.266.7", AutoSize = true, Anchor = AnchorStyles.Left }, 1, 2);
        panel.Controls.Add(mesHeartbeatValue, 2, 2);

        panel.Controls.Add(new Label { Text = "条码检测", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
        panel.Controls.Add(barcodeCheckValue, 1, 3);
        panel.SetColumnSpan(barcodeCheckValue, 2);
        group.Controls.Add(panel);
        return group;
    }

    private Control BuildPlcPanel()
    {
        var group = new GroupBox { Text = "PLC", Dock = DockStyle.Fill, Padding = new Padding(8) };
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 12 };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        for (int i = 0; i < 12; i++) panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

        panel.Controls.Add(new Label { Text = "PLC 地址", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        ip.Dock = DockStyle.Fill;
        panel.Controls.Add(ip, 1, 0);
        panel.Controls.Add(new Label { Text = "S7端口：102（固定）", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 0);

        panel.Controls.Add(new Label { Text = "位", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        panel.Controls.Add(new Label { Text = "PLC心跳 DB50.266.0", AutoSize = true, Anchor = AnchorStyles.Left }, 1, 1);
        panel.Controls.Add(plcHeartbeatValue, 2, 1);

        panel.Controls.Add(new Label { Text = "位", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
        panel.Controls.Add(new Label { Text = "条码检测请求 DB50.266.1", AutoSize = true, Anchor = AnchorStyles.Left }, 1, 2);
        panel.Controls.Add(interactionValue, 2, 2);

        panel.Controls.Add(new Label { Text = "允许读取 DB50.266.4", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
        panel.Controls.Add(new Label { Text = "MES允许取数据", AutoSize = true, Anchor = AnchorStyles.Left }, 1, 3);
        panel.Controls.Add(readPermissionValue, 2, 3);

        panel.Controls.Add(new Label { Text = "保存成功 DB50.266.5", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 4);
        panel.Controls.Add(new Label { Text = "PLCResult 已保存", AutoSize = true, Anchor = AnchorStyles.Left }, 1, 4);
        panel.Controls.Add(saveStatusValue, 2, 4);

        panel.Controls.Add(new Label { Text = "保存失败 DB50.266.6", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 5);
        panel.Controls.Add(new Label { Text = "PLCResult 保存失败", AutoSize = true, Anchor = AnchorStyles.Left }, 1, 5);
        panel.Controls.Add(saveFailureValue, 2, 5);

        panel.Controls.Add(new Label { Text = "条码", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 6);
        panel.Controls.Add(new Label { Text = "DB50.2.0", AutoSize = true, Anchor = AnchorStyles.Left }, 1, 6);
        panel.Controls.Add(barcodeValue, 2, 6);

        panel.Controls.Add(new Label { Text = "PLC 地址块", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 7);
        db.Dock = DockStyle.Left;
        panel.Controls.Add(db, 1, 7);
        panel.Controls.Add(new Label { Text = "Rack 0 / Slot 1", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 7);

        panel.Controls.Add(new Label { Text = "起始偏移", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 8);
        offset.Dock = DockStyle.Left;
        panel.Controls.Add(offset, 1, 8);
        panel.Controls.Add(new Label { Text = "字节", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 8);

        panel.Controls.Add(new Label { Text = "读写长度", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 9);
        readLength.Dock = DockStyle.Left;
        panel.Controls.Add(readLength, 1, 9);
        var test = new Button { Text = "连接并读写验证", Dock = DockStyle.Fill };
        test.Click += RunTest;
        panel.Controls.Add(test, 2, 9);
        panel.SetRowSpan(test, 2);

        panel.Controls.Add(new Label { Text = "停止 PLC 心跳（秒）", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 10);
        stopHeartbeatAfterSeconds.Dock = DockStyle.Left;
        panel.Controls.Add(stopHeartbeatAfterSeconds, 1, 10);
        panel.Controls.Add(new Label { Text = "0 = 不停止", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 10);

        var heartbeatStatus = new Label { Text = "心跳监控：自动运行", AutoSize = true, Anchor = AnchorStyles.Left };
        panel.Controls.Add(heartbeatStatus, 0, 11);
        panel.SetColumnSpan(heartbeatStatus, 3);
        group.Controls.Add(panel);
        return group;
    }

    private void StartServer(object sender, EventArgs args)
    {
        try
        {
            if (serverProcess != null && !serverProcess.HasExited) { WriteLog("Server 已在运行。"); return; }
            if (!File.Exists(serverPath.Text)) throw new FileNotFoundException("找不到 S7ServerSimulator.exe", serverPath.Text);
            serverProcess = Process.Start(new ProcessStartInfo
            {
                FileName = serverPath.Text,
                Arguments = string.Format("--db {0} --length {1} --stop-heartbeat-after-seconds {2}", (int)db.Value, (int)serverLength.Value, (int)stopHeartbeatAfterSeconds.Value),
                WorkingDirectory = Path.GetDirectoryName(serverPath.Text),
                UseShellExecute = true
            });
            WriteLog("已启动 Server，等待其监听 TCP 102。PID=" + serverProcess.Id);
        }
        catch (Exception ex) { WriteLog("启动失败：" + ex.Message); }
    }

    private void RunStopPlcHeartbeatTest(object sender, EventArgs args)
    {
        stopHeartbeatAfterSeconds.Value = 5;
        if (serverProcess != null && !serverProcess.HasExited)
        {
            StopServer();
            System.Threading.Thread.Sleep(200);
        }
        WriteLog("测试：Server 启动后 5 秒停止 PLC 心跳。");
        StartServer(sender, args);
    }

    private void RunBarcodeRequestTest(object sender, EventArgs args)
    {
        string barcode = PromptBarcode();
        if (barcode == null) return;

        try
        {
            string configPath = FindHeartbeatConfig();
            HeartbeatConfig config = s7Help.ReadHeartbeatConfig(configPath);
            PlcConfig target = config.Plc.FirstOrDefault(item => string.Equals(item.Ip, ip.Text.Trim(), StringComparison.OrdinalIgnoreCase)) ?? config.Plc[0];
            if (target.Barcode == null) throw new InvalidOperationException("配置中没有 barcode 地址块。");
            if (target.Interaction == null) throw new InvalidOperationException("配置中没有 interaction 地址块。");

            byte[] barcodeBytes = Encoding.ASCII.GetBytes(barcode);
            if (barcodeBytes.Length > target.Barcode.MaxLength)
                throw new InvalidOperationException("条码长度不能超过 " + target.Barcode.MaxLength + " 字节。");

            using (var plc = new Plc(CpuType.S71200, target.Ip, (short)target.Rack, (short)target.Slot))
            {
                plc.Open();
                byte[] barcodeBlock = new byte[target.Barcode.MaxLength];
                Buffer.BlockCopy(barcodeBytes, 0, barcodeBlock, 0, barcodeBytes.Length);
                s7Help.WriteBlock(plc, target.Barcode.Db, target.Barcode.ByteOffset, barcodeBlock);

                byte[] interactionByte = s7Help.ReadBlock(plc, target.Interaction.Db, target.Interaction.ByteOffset, 1);
                interactionByte[0] = mesHelp.SetBarcodeRequest(interactionByte[0], target.Interaction);
                s7Help.WriteBlock(plc, target.Interaction.Db, target.Interaction.ByteOffset, interactionByte);
                plc.Close();
            }
            WriteLog("条码请求已发送：" + barcode + "，写入 DB" + target.Barcode.Db + "." + target.Barcode.ByteOffset + ".0，交互位已置 True。");
        }
        catch (Exception ex)
        {
            WriteLog("条码请求失败：" + ex.Message);
        }
    }

    private void RunAllowReadTest(object sender, EventArgs args)
    {
        try
        {
            HeartbeatConfig config = s7Help.ReadHeartbeatConfig(FindHeartbeatConfig());
            PlcConfig target = config.Plc.FirstOrDefault(item => string.Equals(item.Ip, ip.Text.Trim(), StringComparison.OrdinalIgnoreCase)) ?? config.Plc[0];
            using (var plc = new Plc(CpuType.S71200, target.Ip, (short)target.Rack, (short)target.Slot))
            {
                plc.Open();
                byte[] bytes = s7Help.ReadBlock(plc, target.ReadPermission.Db, target.ReadPermission.ByteOffset, 1);
                bytes[0] = (byte)(bytes[0] | (1 << target.ReadPermission.Bit));
                s7Help.WriteBlock(plc, target.ReadPermission.Db, target.ReadPermission.ByteOffset, bytes);
                byte[] verifyBytes = s7Help.ReadBlock(plc, target.ReadPermission.Db, target.ReadPermission.ByteOffset, 1);
                bool enabled = (verifyBytes[0] & (1 << target.ReadPermission.Bit)) != 0;
                if (!enabled)
                    throw new InvalidOperationException("PLC 未确认允许保存请求（DB50.266.4=1）。");
                if (readTestMenu != null) readTestMenu.Enabled = true;
            }
            WriteLog("允许保存测试已执行并读回确认：DB50.266.4=1，读取菜单已启用。");
        }
        catch (Exception ex)
        {
            WriteLog("允许读取测试失败：" + ex.Message);
        }
    }

    private void RunReadTest(object sender, EventArgs args)
    {
        try
        {
            TestInput input = PromptTestInput();
            if (input == null) return;
            float testValue;
            if (!float.TryParse(input.RealValue, NumberStyles.Float, CultureInfo.InvariantCulture, out testValue))
                throw new InvalidOperationException("测试值必须是有效的 REAL 数值。");
            int detectionResult;
            if (!int.TryParse(input.DetectionResult, NumberStyles.Integer, CultureInfo.InvariantCulture, out detectionResult)
                || detectionResult < 0 || detectionResult > 2)
                throw new InvalidOperationException("检测结果必须输入 0、1 或 2。");

            HeartbeatConfig config = s7Help.ReadHeartbeatConfig(FindHeartbeatConfig());
            PlcConfig target = config.Plc.FirstOrDefault(item => string.Equals(item.Ip, ip.Text.Trim(), StringComparison.OrdinalIgnoreCase)) ?? config.Plc[0];
            if (target.ReadPermission == null)
                throw new InvalidOperationException("配置中没有允许读取地址块。");

            string barcode;
            using (var plc = new Plc(CpuType.S71200, target.Ip, (short)target.Rack, (short)target.Slot))
            {
                plc.Open();
                byte[] permissionBytes = s7Help.ReadBlock(plc, target.ReadPermission.Db, target.ReadPermission.ByteOffset, 1);
                if ((permissionBytes[0] & (1 << target.ReadPermission.Bit)) == 0)
                    throw new InvalidOperationException("PLC 尚未发出允许读取（DB50.266.4=0）。");

                byte[] barcodeBytes = s7Help.ReadBlock(plc, target.Barcode.Db, target.Barcode.ByteOffset, target.Barcode.MaxLength);
                barcode = mesHelp.DecodeBarcode(barcodeBytes, target.Barcode);
            }
            if (string.IsNullOrWhiteSpace(barcode))
                throw new InvalidOperationException("PLC 的 DB50.2.0 没有读取到二维码。");

            byte[] valueBytes = Encoding.ASCII.GetBytes(barcode);
            if (target.Barcode == null || valueBytes.Length > target.Barcode.MaxLength)
                throw new InvalidOperationException("输入值长度超过 DB50.2.0 的配置长度。");
            if (target.TestValue == null || target.TestValue.Length != 4)
                throw new InvalidOperationException("testValue 必须配置为 4 字节 REAL。");

            bool saved;
            using (var plc = new Plc(CpuType.S71200, target.Ip, (short)target.Rack, (short)target.Slot))
            {
                plc.Open();
                byte[] barcodeBlock = new byte[target.Barcode.MaxLength];
                Buffer.BlockCopy(valueBytes, 0, barcodeBlock, 0, valueBytes.Length);
                s7Help.WriteBlock(plc, target.Barcode.Db, target.Barcode.ByteOffset, barcodeBlock);

                byte[] realBytes = BitConverter.GetBytes(testValue);
                if (BitConverter.IsLittleEndian) Array.Reverse(realBytes);
                s7Help.WriteBlock(plc, target.TestValue.Db, target.TestValue.ByteOffset, realBytes);

                byte[] judgementBytes = new byte[target.Judgement.Length];
                judgementBytes[0] = (byte)((detectionResult >> 8) & 0xFF);
                judgementBytes[1] = (byte)(detectionResult & 0xFF);
                s7Help.WriteBlock(plc, target.Judgement.Db, target.Judgement.ByteOffset, judgementBytes);
                byte[] readJudgementBytes = s7Help.ReadBlock(plc, target.Judgement.Db, target.Judgement.ByteOffset, target.Judgement.Length);
                int plcDetectionResult = (readJudgementBytes[0] << 8) | readJudgementBytes[1];

                saved = s7Help.SaveReadTestResult(barcode, testValue, plcDetectionResult, WriteLog);
                byte[] saveStatusBytes = s7Help.ReadBlock(plc, target.SaveStatus.Db, target.SaveStatus.ByteOffset, 1);
                int saveStatusMask = 1 << target.SaveStatus.Bit;
                saveStatusBytes[0] = saved
                    ? (byte)(saveStatusBytes[0] | saveStatusMask)
                    : (byte)(saveStatusBytes[0] & ~saveStatusMask);
                int saveFailureMask = 1 << target.SaveFailure.Bit;
                saveStatusBytes[0] = saved
                    ? (byte)(saveStatusBytes[0] & ~saveFailureMask)
                    : (byte)(saveStatusBytes[0] | saveFailureMask);
                s7Help.WriteBlock(plc, target.SaveStatus.Db, target.SaveStatus.ByteOffset, saveStatusBytes);
            }
            WriteLog(string.Format("读取测试已执行：PLC二维码={0}，检测结果={1}，REAL测试值={2}，tb_PLCResult保存={3}，DB50.266.5={4}，DB50.266.6={5}", barcode, detectionResult, testValue, saved ? "成功" : "失败", saved ? 1 : 0, saved ? 0 : 1));
        }
        catch (Exception ex)
        {
            WriteLog("读取测试失败：" + ex.Message);
            MessageBox.Show(this, ex.Message, "读取测试", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private string PromptBarcode()
    {
        using (var dialog = new Form { Text = "发起条码请求", StartPosition = FormStartPosition.CenterParent, ClientSize = new Size(420, 125), FormBorderStyle = FormBorderStyle.FixedDialog, MinimizeBox = false, MaximizeBox = false })
        {
            var label = new Label { Text = "请输入条码：", AutoSize = true, Location = new Point(15, 18) };
            var input = new TextBox { Location = new Point(15, 45), Width = 390 };
            var ok = new Button { Text = "确定", DialogResult = DialogResult.OK, Location = new Point(245, 82), Width = 75 };
            var cancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Location = new Point(330, 82), Width = 75 };
            dialog.Controls.AddRange(new Control[] { label, input, ok, cancel });
            dialog.AcceptButton = ok;
            dialog.CancelButton = cancel;
            return dialog.ShowDialog(this) == DialogResult.OK ? input.Text.Trim() : null;
        }
    }

    private TestInput PromptTestInput()
    {
        using (var dialog = new Form { Text = "读取测试", StartPosition = FormStartPosition.CenterParent, ClientSize = new Size(440, 215), FormBorderStyle = FormBorderStyle.FixedDialog, MinimizeBox = false, MaximizeBox = false })
        {
            var resultLabel = new Label { Text = "检测结果(0/1/2)：", AutoSize = true, Location = new Point(15, 15) };
            var resultInput = new TextBox { Location = new Point(145, 12), Width = 275, Text = "1" };
            var realLabel = new Label { Text = "请输入测试值(REAL)：", AutoSize = true, Location = new Point(15, 50) };
            var realInput = new TextBox { Location = new Point(145, 47), Width = 275 };
            var ok = new Button { Text = "确定", DialogResult = DialogResult.OK, Location = new Point(250, 155), Width = 80 };
            var cancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Location = new Point(340, 155), Width = 80 };
            dialog.Controls.AddRange(new Control[] { resultLabel, resultInput, realLabel, realInput, ok, cancel });
            dialog.AcceptButton = ok;
            dialog.CancelButton = cancel;
            if (dialog.ShowDialog(this) != DialogResult.OK) return null;
            return new TestInput { RealValue = realInput.Text.Trim(), DetectionResult = resultInput.Text.Trim() };
        }
    }

    private void StopServer()
    {
        try
        {
            if (serverProcess == null || serverProcess.HasExited) return;
            serverProcess.CloseMainWindow();
            if (!serverProcess.WaitForExit(2000)) serverProcess.Kill();
            WriteLog("Server 已停止。");
        }
        catch (Exception ex) { WriteLog("停止失败：" + ex.Message); }
        finally { if (serverProcess != null) serverProcess.Dispose(); serverProcess = null; }
    }

    private void RunTest(object sender, EventArgs args)
    {
        var testArgs = new[] { "--ip", ip.Text.Trim(), "--db", db.Value.ToString(), "--offset", offset.Value.ToString(), "--length", readLength.Value.ToString() };
        var original = Console.Out;
        using (var writer = new StringWriter())
        {
            Console.SetOut(writer);
            Program.RunCli(testArgs);
            Console.SetOut(original);
            WriteLog(writer.ToString().TrimEnd());
        }
    }

    private void ReadHeartbeatValues(object sender, EventArgs args)
    {
        if (System.Threading.Interlocked.Exchange(ref plcValueReading, 1) != 0) return;
        string plcIp = ip.Text.Trim();
        Task.Run(() =>
        {
            string plcValue = "PLC寄存器中的值：--";
            string interaction = "PLC寄存器中的值：--";
            string readPermission = "PLC寄存器中的值：--";
            string saveStatus = "PLC寄存器中的值：--";
            string saveFailure = "PLC寄存器中的值：--";
            string barcode = "PLC寄存器中的值：--";
            string mesValue = "PLC寄存器中的值：--";
            bool readAllowed = false;
            try
            {
                using (var plc = new Plc(CpuType.S71200, plcIp, 0, 1))
                {
                    plc.Open();
                    byte[] bytes = s7Help.ReadBlock(plc, 50, 266, 1);
                    plcValue = "PLC寄存器中的值：" + ((bytes[0] & 0x01) != 0 ? "1" : "0");
                    HeartbeatConfig config = s7Help.ReadHeartbeatConfig(FindHeartbeatConfig());
                    PlcConfig target = config.Plc.FirstOrDefault(item => string.Equals(item.Ip, plcIp, StringComparison.OrdinalIgnoreCase)) ?? config.Plc[0];
                    interaction = "PLC寄存器中的值：" + (target.Interaction != null && mesHelp.IsBarcodeRequest(bytes[0], target.Interaction) ? "1" : "0");
                    readAllowed = target.ReadPermission != null && (bytes[0] & (1 << target.ReadPermission.Bit)) != 0;
                    readPermission = "PLC寄存器中的值：" + (readAllowed ? "1" : "0");
                    saveStatus = "PLC寄存器中的值：" + (target.SaveStatus != null && (bytes[0] & (1 << target.SaveStatus.Bit)) != 0 ? "1" : "0");
                    saveFailure = "PLC寄存器中的值：" + (target.SaveFailure != null && (bytes[0] & (1 << target.SaveFailure.Bit)) != 0 ? "1" : "0");
                    if (target.Barcode != null)
                    {
                        byte[] barcodeBytes = s7Help.ReadBlock(plc, target.Barcode.Db, target.Barcode.ByteOffset, target.Barcode.MaxLength);
                        barcode = "PLC寄存器中的值：" + mesHelp.DecodeBarcode(barcodeBytes, target.Barcode);
                    }

                    // 心跳监控进程运行时由它负责写 MES 心跳；否则由界面轮询负责翻转 DB50.DBX266.7。
                    if (!s7Help.IsHeartbeatMonitoring)
                    {
                        bytes[0] = (byte)(bytes[0] ^ 0x80);
                        s7Help.WriteBlock(plc, 50, 266, bytes);
                        bytes = s7Help.ReadBlock(plc, 50, 266, 1);
                    }
                    mesValue = "PLC寄存器中的值：" + ((bytes[0] & 0x80) != 0 ? "1" : "0");
                    plc.Close();
                }
            }
            catch
            {
                // PLC 未连接时保持 --，避免通信轮询打断界面操作。
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref plcValueReading, 0);
            }

            if (!IsDisposed && IsHandleCreated)
            {
                try { BeginInvoke(new Action<string, string, string, string, string, string, string, bool>(UpdateHeartbeatValues), plcValue, interaction, readPermission, saveStatus, saveFailure, barcode, mesValue, readAllowed); }
                catch (InvalidOperationException) { }
            }
        });
    }

    private void UpdateHeartbeatValues(string plcValue, string interaction, string readPermission, string saveStatus, string saveFailure, string barcode, string mesValue, bool readAllowed)
    {
        if (IsDisposed) return;
        plcHeartbeatValue.Text = plcValue;
        interactionValue.Text = interaction;
        readPermissionValue.Text = readPermission;
        saveStatusValue.Text = saveStatus;
        saveFailureValue.Text = saveFailure;
        barcodeValue.Text = barcode;
        mesHeartbeatValue.Text = mesValue;
        if (readTestMenu != null) readTestMenu.Enabled = readAllowed;
    }

    private void StartHeartbeatMonitor(object sender, EventArgs args)
    {
        try
        {
            if (s7Help.IsHeartbeatMonitoring)
            {
                WriteLog("心跳监控已在运行。");
                return;
            }

            string configPath = FindHeartbeatConfig();
            if (!File.Exists(configPath)) throw new FileNotFoundException("找不到 S7client.json", configPath);

            heartbeatWarningShown = false;
            log.BackColor = SystemColors.Window;
            string logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDirectory);
            string logPath = Path.Combine(logDirectory, "heartbeat-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".log");
            heartbeatLogWriter = new StreamWriter(logPath, false) { AutoFlush = true };

            s7Help.StartHeartbeatMonitor(configPath, WriteHeartbeatLog, WriteHeartbeatLog);
            WriteHeartbeatLog("心跳监控已启动，日志文件：" + logPath);
        }
        catch (Exception ex)
        {
            WriteLog("启动心跳监控失败：" + ex.Message);
            CloseHeartbeatLogWriter();
        }
    }

    private void StartBarcodeMonitor(object sender, EventArgs args)
    {
        try
        {
            if (s7Help.IsBarcodeMonitoring) return;
            string configPath = FindHeartbeatConfig();
            s7Help.StartBarcodeMonitor(configPath, WriteHeartbeatLog, WriteHeartbeatLog);
            WriteHeartbeatLog("条码监控线程已启动。");
        }
        catch (Exception ex)
        {
            WriteLog("启动条码监控失败：" + ex.Message);
        }
    }

    private void StopHeartbeatMonitor()
    {
        try
        {
            if (!s7Help.IsHeartbeatMonitoring) return;
            s7Help.StopHeartbeatMonitor();
            WriteHeartbeatLog("心跳监控已停止。");
        }
        catch (Exception ex) { WriteLog("停止心跳监控失败：" + ex.Message); }
        finally
        {
            CloseHeartbeatLogWriter();
        }
    }

    private void StopBarcodeMonitor()
    {
        try
        {
            if (!s7Help.IsBarcodeMonitoring) return;
            s7Help.StopBarcodeMonitor();
            WriteHeartbeatLog("条码监控线程已停止。");
        }
        catch (Exception ex) { WriteLog("停止条码监控失败：" + ex.Message); }
    }

    private void WriteHeartbeatLog(string message)
    {
        string line = DateTime.Now.ToString("HH:mm:ss") + " " + message;
        lock (heartbeatLogSync)
        {
            if (heartbeatLogWriter != null) heartbeatLogWriter.WriteLine(line);
        }
        WriteLog(message);
        UpdateBarcodeCheckResult(message);
        if (message.IndexOf("STATUS=OK", StringComparison.OrdinalIgnoreCase) >= 0)
            ResetHeartbeatWarning();
        if (message.IndexOf("COMMUNICATION_FAULT", StringComparison.OrdinalIgnoreCase) >= 0)
            ShowHeartbeatWarning(message);
    }

    private void UpdateBarcodeCheckResult(string message)
    {
        const string valueMarker = "BARCODE_REQUEST value=";
        const string acceptedMarker = "BARCODE_REQUEST accepted, partno_id=";
        const string rejectedMarker = "BARCODE_REQUEST rejected, partno_id=";
        string displayText = null;

        int valueIndex = message.IndexOf(valueMarker, StringComparison.OrdinalIgnoreCase);
        if (valueIndex >= 0)
        {
            displayText = "检测 " + message.Substring(valueIndex + valueMarker.Length) + "...";
        }

        int acceptedIndex = message.IndexOf(acceptedMarker, StringComparison.OrdinalIgnoreCase);
        if (acceptedIndex >= 0)
        {
            displayText = "检测 " + ReadBarcodePartNoId(message, acceptedIndex + acceptedMarker.Length) + "成功";
        }

        int rejectedIndex = message.IndexOf(rejectedMarker, StringComparison.OrdinalIgnoreCase);
        if (rejectedIndex >= 0)
        {
            displayText = "检测 " + ReadBarcodePartNoId(message, rejectedIndex + rejectedMarker.Length) + "失败";
        }

        if (displayText == null) return;
        if (InvokeRequired)
        {
            try { BeginInvoke(new Action<string>(SetBarcodeCheckText), displayText); }
            catch (InvalidOperationException) { }
            return;
        }
        SetBarcodeCheckText(displayText);
    }

    private static string ReadBarcodePartNoId(string message, int startIndex)
    {
        int commaIndex = message.IndexOf(',', startIndex);
        return (commaIndex >= 0 ? message.Substring(startIndex, commaIndex - startIndex) : message.Substring(startIndex)).Trim();
    }

    private void SetBarcodeCheckText(string text)
    {
        if (!IsDisposed) barcodeCheckValue.Text = text;
    }

    private void ShowHeartbeatWarning(string message)
    {
        if (InvokeRequired)
        {
            try { BeginInvoke(new Action<string>(ShowHeartbeatWarning), message); }
            catch (InvalidOperationException) { }
            return;
        }
        if (IsDisposed || heartbeatWarningShown) return;
        heartbeatWarningShown = true;
        log.BackColor = Color.MistyRose;
        MessageBox.Show(this, message, "S7 心跳警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void ResetHeartbeatWarning()
    {
        if (InvokeRequired)
        {
            try { BeginInvoke(new Action(ResetHeartbeatWarning)); }
            catch (InvalidOperationException) { }
            return;
        }
        heartbeatWarningShown = false;
        log.BackColor = SystemColors.Window;
    }

    private void CloseHeartbeatLogWriter()
    {
        lock (heartbeatLogSync)
        {
            if (heartbeatLogWriter == null) return;
            heartbeatLogWriter.Dispose();
            heartbeatLogWriter = null;
        }
    }

    private void WriteLog(string message)
    {
        if (IsDisposed) return;
        if (InvokeRequired)
        {
            try { BeginInvoke(new Action<string>(WriteLog), message); }
            catch (InvalidOperationException) { }
            return;
        }
        TextBox targetLog = message.StartsWith("PLC_RESULT_PROCESS", StringComparison.OrdinalIgnoreCase)
            ? plcResultLog
            : log;
        targetLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " " + message + Environment.NewLine);
    }

    private string FindHeartbeatConfig()
    {
        string baseDir = AppContext.BaseDirectory;
        string[] candidates =
        {
            Path.Combine(baseDir, "S7client.json"),
            Path.Combine(baseDir, @"..\..\..\..\S7client\S7client.json")
        };
        return candidates.Select(Path.GetFullPath).FirstOrDefault(File.Exists) ?? candidates[0];
    }

    private static string FindServerExecutable()
    {
        DirectoryInfo directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !string.Equals(directory.Name, "S7Development", StringComparison.OrdinalIgnoreCase))
            directory = directory.Parent;

        if (directory != null)
            return Path.Combine(directory.FullName, "S7ServerSimulator", "bin", "Release", "S7ServerSimulator.exe");

        return Path.Combine(AppContext.BaseDirectory, "S7ServerSimulator.exe");
    }
}

internal sealed class TestInput
{
    public string RealValue { get; set; }
    public string DetectionResult { get; set; }
}
