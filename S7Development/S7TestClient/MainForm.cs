using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

internal sealed class MainForm : Form
{
    private readonly TextBox serverPath = new TextBox();
    private readonly TextBox ip = new TextBox { Text = "127.0.0.1" };
    private readonly NumericUpDown db = new NumericUpDown { Minimum = 1, Maximum = 65535, Value = 1 };
    private readonly NumericUpDown serverLength = new NumericUpDown { Minimum = 1, Maximum = 65535, Value = 256 };
    private readonly NumericUpDown offset = new NumericUpDown { Minimum = 0, Maximum = 65535, Value = 8 };
    private readonly NumericUpDown readLength = new NumericUpDown { Minimum = 1, Maximum = 65535, Value = 8 };
    private readonly TextBox log = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both };
    private Process serverProcess;

    public MainForm()
    {
        Text = "S7 Test Client";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(760, 520);
        Size = new Size(900, 620);
        BuildLayout();
        serverPath.Text = FindServerExecutable();
        FormClosing += (sender, args) => StopServer();
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), ColumnCount = 3, RowCount = 8 };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(new Label { Text = "Server 程序", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        root.Controls.Add(serverPath, 1, 0);
        var browse = new Button { Text = "浏览...", Dock = DockStyle.Fill };
        browse.Click += BrowseServer;
        root.Controls.Add(browse, 2, 0);

        root.Controls.Add(new Label { Text = "PLC IP", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        root.Controls.Add(ip, 1, 1);
        var start = new Button { Text = "启动 Server", Dock = DockStyle.Fill };
        start.Click += StartServer;
        root.Controls.Add(start, 2, 1);

        root.Controls.Add(new Label { Text = "DB 编号", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
        root.Controls.Add(db, 1, 2);
        var stop = new Button { Text = "停止 Server", Dock = DockStyle.Fill };
        stop.Click += (sender, args) => StopServer();
        root.Controls.Add(stop, 2, 2);

        root.Controls.Add(new Label { Text = "Server 数据长度", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
        root.Controls.Add(serverLength, 1, 3);
        root.Controls.Add(new Label { Text = "TCP 102", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 3);

        root.Controls.Add(new Label { Text = "起始偏移", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 4);
        root.Controls.Add(offset, 1, 4);
        root.Controls.Add(new Label { Text = "Rack 0 / Slot 1", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 4);

        root.Controls.Add(new Label { Text = "读写长度", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 5);
        root.Controls.Add(readLength, 1, 5);
        var test = new Button { Text = "连接并读写验证", Dock = DockStyle.Fill };
        test.Click += RunTest;
        root.Controls.Add(test, 2, 5);

        root.Controls.Add(new Label { Text = "通信日志", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 6);
        root.SetColumnSpan(log, 3);
        root.Controls.Add(log, 0, 7);
        root.SetColumnSpan(serverPath, 1);
        Controls.Add(root);
    }

    private void BrowseServer(object sender, EventArgs args)
    {
        using (var dialog = new OpenFileDialog { Filter = "Server executable|S7ServerSimulator.exe|Executable|*.exe" })
        {
            if (dialog.ShowDialog(this) == DialogResult.OK) serverPath.Text = dialog.FileName;
        }
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
                Arguments = string.Format("--db {0} --length {1}", (int)db.Value, (int)serverLength.Value),
                WorkingDirectory = Path.GetDirectoryName(serverPath.Text),
                UseShellExecute = true
            });
            WriteLog("已启动 Server，等待其监听 TCP 102。PID=" + serverProcess.Id);
        }
        catch (Exception ex) { WriteLog("启动失败：" + ex.Message); }
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

    private void WriteLog(string message) { log.AppendText(DateTime.Now.ToString("HH:mm:ss") + " " + message + Environment.NewLine); }

    private static string FindServerExecutable()
    {
        string baseDir = AppContext.BaseDirectory;
        string[] candidates =
        {
            Path.Combine(baseDir, @"..\..\..\..\S7ServerSimulator\bin\Release\net48x64heartbeat\S7ServerSimulator.exe"),
            Path.Combine(baseDir, @"..\..\..\..\S7ServerSimulator\bin\Release\net48x64new\S7ServerSimulator.exe"),
            Path.Combine(baseDir, @"..\..\..\..\S7ServerSimulator\bin\Release\net48\S7ServerSimulator.exe")
        };
        return candidates.Select(Path.GetFullPath).FirstOrDefault(File.Exists) ?? candidates[0];
    }
}
