using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace PrintServiceInstaller
{
    public class InstallerForm : Form
    {
        private const string ServiceName = "PrintService";
        private const string DisplayName = "MES Print Service";
        private const string Description = "MES Printing Background Service";
        private const string DefaultInstallPath = @"C:\CheryMES\PrintService";
        private const string ServiceExeName = "PrintService.exe";
        private const string DefaultApiBaseUrl = "http://localhost:55426";
        private const string PendingApiPath = "/api/print/pending-labels";
        private const string CompleteApiPath = "/api/print/complete";
        private const string DefaultMachineId = "PRT-01";
        private const string DefaultUpdatePackageUrl = "http://localhost:55426/printservice/update/PrintServicePackage.zip";
        private static readonly string InstallerLogFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "MESPrintService",
            "logs");
        private static readonly string InstallerSettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "MESPrintService",
            "installer-settings.xml");

        private TextBox txtApiBaseUrl;
        private TextBox txtMachineId;
        private TextBox txtInstallPath;
        private TextBox txtUpdatePackageUrl;
        private Label lblStatus;
        private TextBox txtLog;
        private Button btnInstall;
        private Button btnSaveUrl;
        private Button btnUninstall;
        private Button btnStartService;
        private Button btnStopService;
        private Button btnUpdate;
        private Button btnRefresh;
        private Button btnOpenLogFolder;
        private string _lastUpdatePackageLastModified;
        private string _lastUpdatePackageLength;
        private string _latestUpdatePackageLastModified;
        private string _latestUpdatePackageLength;

        public InstallerForm()
        {
            InitializeComponent();
            LoadCurrentSettings();
            RefreshServiceStatus();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            BeginInvoke(new Action(CheckUpdateOnStartup));
        }

        private void InitializeComponent()
        {
            Text = "MES 打印服务安装程序";
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Microsoft YaHei UI", 9F);
            ClientSize = new Size(720, 540);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Label title = new Label();
            title.Text = "MES 打印服务安装程序";
            title.Font = new Font(Font.FontFamily, 16F, FontStyle.Bold);
            title.Location = new Point(24, 18);
            title.Size = new Size(360, 34);

            lblStatus = new Label();
            lblStatus.Text = "服务状态：检查中";
            lblStatus.Location = new Point(520, 27);
            lblStatus.Size = new Size(170, 24);
            lblStatus.TextAlign = ContentAlignment.MiddleRight;

            Label urlLabel = new Label();
            urlLabel.Text = "接口地址";
            urlLabel.Location = new Point(28, 78);
            urlLabel.Size = new Size(90, 24);

            txtApiBaseUrl = new TextBox();
            txtApiBaseUrl.Location = new Point(120, 76);
            txtApiBaseUrl.Size = new Size(550, 23);

            Label machineLabel = new Label();
            machineLabel.Text = "机器编号";
            machineLabel.Location = new Point(28, 118);
            machineLabel.Size = new Size(90, 24);

            txtMachineId = new TextBox();
            txtMachineId.Location = new Point(120, 116);
            txtMachineId.Size = new Size(200, 23);

            Label pathLabel = new Label();
            pathLabel.Text = "安装目录";
            pathLabel.Location = new Point(28, 158);
            pathLabel.Size = new Size(90, 24);

            txtInstallPath = new TextBox();
            txtInstallPath.Location = new Point(120, 156);
            txtInstallPath.Size = new Size(550, 23);
            txtInstallPath.ReadOnly = true;

            Label updateUrlLabel = new Label();
            updateUrlLabel.Text = "更新包地址";
            updateUrlLabel.Location = new Point(28, 198);
            updateUrlLabel.Size = new Size(90, 24);

            txtUpdatePackageUrl = new TextBox();
            txtUpdatePackageUrl.Location = new Point(120, 196);
            txtUpdatePackageUrl.Size = new Size(550, 23);

            btnInstall = new Button();
            btnInstall.Text = "安装并启动";
            btnInstall.Location = new Point(120, 245);
            btnInstall.Size = new Size(130, 38);
            btnInstall.Click += btnInstall_Click;

            btnSaveUrl = new Button();
            btnSaveUrl.Text = "修改 URL";
            btnSaveUrl.Location = new Point(270, 245);
            btnSaveUrl.Size = new Size(130, 38);
            btnSaveUrl.Click += btnSaveUrl_Click;

            btnUninstall = new Button();
            btnUninstall.Text = "卸载服务";
            btnUninstall.Location = new Point(420, 245);
            btnUninstall.Size = new Size(130, 38);
            btnUninstall.Click += btnUninstall_Click;

            btnStartService = new Button();
            btnStartService.Text = "启动服务";
            btnStartService.Location = new Point(120, 292);
            btnStartService.Size = new Size(130, 30);
            btnStartService.Click += btnStartService_Click;

            btnStopService = new Button();
            btnStopService.Text = "停止服务";
            btnStopService.Location = new Point(270, 292);
            btnStopService.Size = new Size(130, 30);
            btnStopService.Click += btnStopService_Click;

            btnUpdate = new Button();
            btnUpdate.Text = "下载更新";
            btnUpdate.Location = new Point(420, 292);
            btnUpdate.Size = new Size(130, 30);
            btnUpdate.Click += btnUpdate_Click;

            btnRefresh = new Button();
            btnRefresh.Text = "刷新服务状态";
            btnRefresh.Location = new Point(570, 245);
            btnRefresh.Size = new Size(100, 38);
            btnRefresh.Click += delegate { RefreshServiceStatus(); };

            btnOpenLogFolder = new Button();
            btnOpenLogFolder.Text = "打开日志";
            btnOpenLogFolder.Location = new Point(570, 292);
            btnOpenLogFolder.Size = new Size(100, 30);
            btnOpenLogFolder.Click += btnOpenLogFolder_Click;

            txtLog = new TextBox();
            txtLog.Location = new Point(28, 340);
            txtLog.Size = new Size(642, 165);
            txtLog.Multiline = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.ReadOnly = true;

            Controls.Add(title);
            Controls.Add(lblStatus);
            Controls.Add(urlLabel);
            Controls.Add(txtApiBaseUrl);
            Controls.Add(machineLabel);
            Controls.Add(txtMachineId);
            Controls.Add(pathLabel);
            Controls.Add(txtInstallPath);
            Controls.Add(updateUrlLabel);
            Controls.Add(txtUpdatePackageUrl);
            Controls.Add(btnInstall);
            Controls.Add(btnSaveUrl);
            Controls.Add(btnUninstall);
            Controls.Add(btnStartService);
            Controls.Add(btnStopService);
            Controls.Add(btnUpdate);
            Controls.Add(btnRefresh);
            Controls.Add(btnOpenLogFolder);
            Controls.Add(txtLog);
        }

        private void LoadCurrentSettings()
        {
            txtInstallPath.Text = DefaultInstallPath;
            txtApiBaseUrl.Text = DefaultApiBaseUrl;
            txtMachineId.Text = DefaultMachineId;
            txtUpdatePackageUrl.Text = DefaultUpdatePackageUrl;
            LoadInstallerSettings();

            string configPath = GetInstalledConfigPath();
            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ServiceExeName + ".config");
            }

            if (File.Exists(configPath))
            {
                try
                {
                    XmlDocument document = LoadConfig(configPath);
                    txtApiBaseUrl.Text = GetAppSetting(document, "PrintClient.ApiBaseUrl", DefaultApiBaseUrl);
                    txtMachineId.Text = GetAppSetting(document, "PrintClient.MachineId", DefaultMachineId);
                }
                catch (Exception ex)
                {
                    WriteLog("读取配置失败：" + ex.Message);
                }
            }

            SaveInstallerSettings();
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            RunOperation("安装服务", delegate
            {
                ValidateInput();
                SaveInstallerSettings();
                EnsureServicePackageExists();
                CopyServiceFiles();
                SaveInstalledConfig();
                StopAndDeleteService();
                RunSc("create \"" + ServiceName + "\" binPath= \"\\\"" + GetInstalledExePath() + "\\\"\" start= auto DisplayName= \"" + DisplayName + "\"");
                RunSc("description \"" + ServiceName + "\" \"" + Description + "\"");
                RunSc("start \"" + ServiceName + "\"");
                WriteLog("安装完成，服务已启动。");
            });
        }

        private void btnSaveUrl_Click(object sender, EventArgs e)
        {
            RunOperation("修改 URL", delegate
            {
                ValidateInput();
                SaveInstallerSettings();
                string configPath = GetInstalledConfigPath();
                if (!File.Exists(configPath))
                {
                    throw new InvalidOperationException("未找到已安装服务配置，请先安装服务。");
                }

                SaveConfig(configPath);
                RestartServiceIfExists();
                WriteLog("URL 修改完成。");
            });
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要卸载打印服务吗？", "确认卸载", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            RunOperation("卸载服务", delegate
            {
                StopAndDeleteService();
                WriteLog("卸载完成。安装目录文件已保留，方便重新安装。");
            });
        }

        private void btnStartService_Click(object sender, EventArgs e)
        {
            RunOperation("启动服务", delegate
            {
                EnsureServiceInstalled();
                RunSc("start \"" + ServiceName + "\"");
                WriteLog("服务启动命令已执行。");
            });
        }

        private void btnStopService_Click(object sender, EventArgs e)
        {
            RunOperation("停止服务", delegate
            {
                EnsureServiceInstalled();
                RunSc("stop \"" + ServiceName + "\"");
                WaitForServiceStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
                WriteLog("服务停止命令已执行。");
            });
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要下载并更新打印服务吗？更新过程中会自动停止并重启服务。", "确认更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            RunOperation("下载更新", UpdatePrintService);
        }

        private void btnOpenLogFolder_Click(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(InstallerLogFolder);
                Process.Start("explorer.exe", InstallerLogFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开日志目录失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RunOperation(string operationName, Action action)
        {
            SetBusy(true);
            WriteLog("开始：" + operationName);
            try
            {
                action();
                RefreshServiceStatus();
                MessageBox.Show(operationName + "完成。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                WriteLog("失败：" + ex.Message);
                RefreshServiceStatus();
                MessageBox.Show(ex.Message, operationName + "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetBusy(bool busy)
        {
            btnInstall.Enabled = !busy;
            btnSaveUrl.Enabled = !busy;
            btnUninstall.Enabled = !busy;
            btnStartService.Enabled = !busy;
            btnStopService.Enabled = !busy;
            btnUpdate.Enabled = !busy;
            btnRefresh.Enabled = !busy;
            btnOpenLogFolder.Enabled = !busy;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private void ValidateInput()
        {
            string apiBaseUrl = txtApiBaseUrl.Text.Trim();
            if (apiBaseUrl.Length == 0)
            {
                throw new InvalidOperationException("请输入接口地址。");
            }

            Uri uri;
            if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidOperationException("接口地址必须以 http:// 或 https:// 开头。");
            }

            if (txtMachineId.Text.Trim().Length == 0)
            {
                throw new InvalidOperationException("请输入机器编号。");
            }
        }

        private void ValidateUpdatePackageUrl()
        {
            string updatePackageUrl = txtUpdatePackageUrl.Text.Trim();
            if (updatePackageUrl.Length == 0)
            {
                throw new InvalidOperationException("请输入更新包地址。");
            }

            Uri uri;
            if (!Uri.TryCreate(updatePackageUrl, UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidOperationException("更新包地址必须以 http:// 或 https:// 开头。");
            }
        }

        private void CheckUpdateOnStartup()
        {
            try
            {
                if (!ServiceExists() || string.IsNullOrWhiteSpace(txtUpdatePackageUrl.Text))
                {
                    return;
                }

                UpdatePackageInfo packageInfo = GetRemoteUpdatePackageInfo();
                if (packageInfo == null || !IsNewUpdatePackage(packageInfo))
                {
                    return;
                }

                _latestUpdatePackageLastModified = packageInfo.LastModified;
                _latestUpdatePackageLength = packageInfo.ContentLength;

                DialogResult result = MessageBox.Show(
                    "发现新的打印服务更新包，是否立即更新？",
                    "发现更新",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    RunOperation("下载更新", UpdatePrintService);
                }
            }
            catch (Exception ex)
            {
                WriteLog("自动检查更新失败：" + ex.Message);
            }
        }

        private bool IsNewUpdatePackage(UpdatePackageInfo packageInfo)
        {
            if (packageInfo == null)
            {
                return false;
            }

            return !string.Equals(packageInfo.LastModified, _lastUpdatePackageLastModified, StringComparison.OrdinalIgnoreCase) ||
                   !string.Equals(packageInfo.ContentLength, _lastUpdatePackageLength, StringComparison.OrdinalIgnoreCase);
        }

        private UpdatePackageInfo GetRemoteUpdatePackageInfo()
        {
            ValidateUpdatePackageUrl();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(txtUpdatePackageUrl.Text.Trim());
            request.Method = "HEAD";
            request.Timeout = 5000;
            request.ReadWriteTimeout = 5000;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                return new UpdatePackageInfo(
                    response.LastModified == DateTime.MinValue ? string.Empty : response.LastModified.ToUniversalTime().ToString("yyyyMMddHHmmss"),
                    response.ContentLength > 0 ? response.ContentLength.ToString() : string.Empty);
            }
        }

        private void UpdatePrintService()
        {
            ValidateUpdatePackageUrl();
            SaveInstallerSettings();
            EnsureServiceInstalled();
            EnsureInstalledServiceFilesExist();

            UpdatePackageInfo packageInfo = null;
            try
            {
                packageInfo = GetRemoteUpdatePackageInfo();
                if (packageInfo != null)
                {
                    _latestUpdatePackageLastModified = packageInfo.LastModified;
                    _latestUpdatePackageLength = packageInfo.ContentLength;
                }
            }
            catch (Exception ex)
            {
                WriteLog("读取远程更新包信息失败，继续尝试下载：" + ex.Message);
            }

            string tempRoot = CreateTempUpdateFolder();
            string packagePath = Path.Combine(tempRoot, "PrintServicePackage.zip");
            string extractFolder = Path.Combine(tempRoot, "package");
            string backupFolder = DefaultInstallPath.TrimEnd(Path.DirectorySeparatorChar) + "_backup_" + DateTime.Now.ToString("yyyyMMddHHmmss");

            try
            {
                DownloadUpdatePackage(packagePath);
                Directory.CreateDirectory(extractFolder);
                ZipFile.ExtractToDirectory(packagePath, extractFolder);
                string packageContentFolder = ResolvePackageContentFolder(extractFolder);
                ValidateUpdatePackage(packageContentFolder);

                XmlDocument currentConfig = LoadConfig(GetInstalledConfigPath());
                StopInstalledService();
                CopyDirectory(DefaultInstallPath, backupFolder);
                CopyDirectory(packageContentFolder, DefaultInstallPath);
                RestoreSiteConfig(currentConfig, GetInstalledConfigPath());
                RunSc("start \"" + ServiceName + "\"");
                _lastUpdatePackageLastModified = _latestUpdatePackageLastModified;
                _lastUpdatePackageLength = _latestUpdatePackageLength;
                SaveInstallerSettings();
                WriteLog("更新完成。备份目录：" + backupFolder);
            }
            catch
            {
                TryRestoreBackup(backupFolder);
                throw;
            }
            finally
            {
                TryDeleteDirectory(tempRoot);
            }
        }

        private void EnsureServicePackageExists()
        {
            string sourceExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ServiceExeName);
            string sourceConfig = sourceExe + ".config";
            if (!File.Exists(sourceExe))
            {
                throw new FileNotFoundException("安装器旁边没有找到服务程序：" + ServiceExeName);
            }

            if (!File.Exists(sourceConfig))
            {
                throw new FileNotFoundException("安装器旁边没有找到服务配置：" + ServiceExeName + ".config");
            }
        }

        private void CopyServiceFiles()
        {
            string sourceFolder = AppDomain.CurrentDomain.BaseDirectory;
            string targetFolder = DefaultInstallPath;
            Directory.CreateDirectory(targetFolder);
            CopyDirectory(sourceFolder, targetFolder);
            WriteLog("服务文件已复制到：" + targetFolder);
        }

        private void CopyDirectory(string sourceFolder, string targetFolder)
        {
            foreach (string directory in Directory.GetDirectories(sourceFolder, "*", SearchOption.AllDirectories))
            {
                string relativePath = directory.Substring(sourceFolder.Length).TrimStart(Path.DirectorySeparatorChar);
                if (ShouldSkipPath(relativePath))
                {
                    continue;
                }

                Directory.CreateDirectory(Path.Combine(targetFolder, relativePath));
            }

            foreach (string file in Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories))
            {
                string relativePath = file.Substring(sourceFolder.Length).TrimStart(Path.DirectorySeparatorChar);
                if (ShouldSkipPath(relativePath))
                {
                    continue;
                }

                string targetFile = Path.Combine(targetFolder, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                File.Copy(file, targetFile, true);
            }
        }

        private void DownloadUpdatePackage(string packagePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(packagePath));
            WriteLog("开始下载更新包：" + txtUpdatePackageUrl.Text.Trim());
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(txtUpdatePackageUrl.Text.Trim(), packagePath);
            }
            WriteLog("更新包下载完成：" + packagePath);
        }

        private string CreateTempUpdateFolder()
        {
            string tempRoot = Path.Combine(Path.GetTempPath(), "MESPrintServiceUpdate_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempRoot);
            return tempRoot;
        }

        private string ResolvePackageContentFolder(string extractFolder)
        {
            if (File.Exists(Path.Combine(extractFolder, ServiceExeName)))
            {
                return extractFolder;
            }

            string[] directories = Directory.GetDirectories(extractFolder);
            if (directories.Length == 1 && File.Exists(Path.Combine(directories[0], ServiceExeName)))
            {
                return directories[0];
            }

            return extractFolder;
        }

        private void ValidateUpdatePackage(string packageContentFolder)
        {
            string packageExePath = Path.Combine(packageContentFolder, ServiceExeName);
            if (!File.Exists(packageExePath))
            {
                throw new FileNotFoundException("更新包中未找到服务程序：" + ServiceExeName);
            }
        }

        private void EnsureInstalledServiceFilesExist()
        {
            if (!File.Exists(GetInstalledExePath()))
            {
                throw new FileNotFoundException("未找到已安装服务程序：" + GetInstalledExePath());
            }

            if (!File.Exists(GetInstalledConfigPath()))
            {
                throw new FileNotFoundException("未找到已安装服务配置：" + GetInstalledConfigPath());
            }
        }

        private void RestoreSiteConfig(XmlDocument oldConfig, string newConfigPath)
        {
            XmlDocument newConfig = File.Exists(newConfigPath) ? LoadConfig(newConfigPath) : oldConfig;
            CopyAppSetting(oldConfig, newConfig, "PrintClient.ApiBaseUrl");
            CopyAppSetting(oldConfig, newConfig, "PrintClient.PendingApiPath");
            CopyAppSetting(oldConfig, newConfig, "PrintClient.PendingApiUrl");
            CopyAppSetting(oldConfig, newConfig, "PrintClient.CompleteApiUrl");
            CopyAppSetting(oldConfig, newConfig, "PrintClient.MachineId");
            CopyAppSetting(oldConfig, newConfig, "PrintClient.PrinterName");
            CopyAppSetting(oldConfig, newConfig, "PrintClient.PollIntervalSeconds");
            CopyAppSetting(oldConfig, newConfig, "PrintClient.PrintPauseMilliseconds");
            CopyAppSetting(oldConfig, newConfig, "PrintClient.LogFolder");
            SaveConfigDocument(newConfig, newConfigPath);
            WriteLog("现场配置已保留：" + newConfigPath);
        }

        private void CopyAppSetting(XmlDocument source, XmlDocument target, string key)
        {
            XmlElement sourceElement = FindAppSetting(source, key);
            if (sourceElement == null)
            {
                return;
            }

            SetAppSetting(target, key, sourceElement.GetAttribute("value"));
        }

        private void StopInstalledService()
        {
            TryRunSc("stop \"" + ServiceName + "\"");
            WaitForServiceStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
        }

        private void TryRestoreBackup(string backupFolder)
        {
            try
            {
                if (!Directory.Exists(backupFolder))
                {
                    return;
                }

                WriteLog("更新失败，开始恢复备份：" + backupFolder);
                StopInstalledService();
                CopyDirectory(backupFolder, DefaultInstallPath);
                TryRunSc("start \"" + ServiceName + "\"");
                WriteLog("备份恢复完成。");
            }
            catch (Exception ex)
            {
                WriteLog("备份恢复失败：" + ex.Message);
            }
        }

        private void TryDeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch
            {
            }
        }

        private bool ShouldSkipPath(string relativePath)
        {
            if (relativePath.StartsWith("logs\\", StringComparison.OrdinalIgnoreCase) ||
                relativePath.StartsWith("cache\\", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string fileName = Path.GetFileName(relativePath);
            return fileName.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase) ||
                   fileName.EndsWith(".vshost.exe", StringComparison.OrdinalIgnoreCase);
        }

        private void SaveInstalledConfig()
        {
            SaveConfig(GetInstalledConfigPath());
        }

        private void SaveConfig(string configPath)
        {
            XmlDocument document = LoadConfig(configPath);
            SetAppSetting(document, "PrintClient.ApiBaseUrl", NormalizeBaseUrl(txtApiBaseUrl.Text));
            SetAppSetting(document, "PrintClient.PendingApiPath", PendingApiPath);
            SetAppSetting(document, "PrintClient.PendingApiUrl", BuildPendingUrl());
            SetAppSetting(document, "PrintClient.CompleteApiUrl", NormalizeBaseUrl(txtApiBaseUrl.Text) + CompleteApiPath);
            SetAppSetting(document, "PrintClient.MachineId", txtMachineId.Text.Trim());
            SaveConfigDocument(document, configPath);
            WriteLog("配置已保存：" + configPath);
        }

        private void LoadInstallerSettings()
        {
            if (!File.Exists(InstallerSettingsPath))
            {
                return;
            }

            try
            {
                XmlDocument document = LoadConfig(InstallerSettingsPath);
                txtMachineId.Text = GetAppSetting(document, "Installer.MachineId", txtMachineId.Text);
                txtUpdatePackageUrl.Text = GetAppSetting(document, "Installer.UpdatePackageUrl", txtUpdatePackageUrl.Text);
                _lastUpdatePackageLastModified = GetAppSetting(document, "Installer.UpdatePackageLastModified", string.Empty);
                _lastUpdatePackageLength = GetAppSetting(document, "Installer.UpdatePackageLength", string.Empty);
            }
            catch (Exception ex)
            {
                WriteLog("读取安装器配置失败：" + ex.Message);
            }
        }

        private void SaveInstallerSettings()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(InstallerSettingsPath));

                XmlDocument document = new XmlDocument();
                XmlElement configuration = document.CreateElement("configuration");
                document.AppendChild(configuration);
                XmlElement appSettings = document.CreateElement("appSettings");
                configuration.AppendChild(appSettings);

                SetAppSetting(document, "Installer.MachineId", txtMachineId.Text.Trim());
                SetAppSetting(document, "Installer.UpdatePackageUrl", txtUpdatePackageUrl.Text.Trim());
                SetAppSetting(document, "Installer.UpdatePackageLastModified", _lastUpdatePackageLastModified ?? string.Empty);
                SetAppSetting(document, "Installer.UpdatePackageLength", _lastUpdatePackageLength ?? string.Empty);
                SaveConfigDocument(document, InstallerSettingsPath);
            }
            catch (Exception ex)
            {
                WriteLog("保存安装器配置失败：" + ex.Message);
            }
        }

        private string BuildPendingUrl()
        {
            return NormalizeBaseUrl(txtApiBaseUrl.Text) + PendingApiPath + "?machineId=" + Uri.EscapeDataString(txtMachineId.Text.Trim());
        }

        private string NormalizeBaseUrl(string value)
        {
            return value.Trim().TrimEnd('/');
        }

        private XmlDocument LoadConfig(string configPath)
        {
            XmlDocument document = new XmlDocument();
            document.PreserveWhitespace = true;
            document.Load(configPath);
            return document;
        }

        private string GetAppSetting(XmlDocument document, string key, string defaultValue)
        {
            XmlElement element = FindAppSetting(document, key);
            return element == null || string.IsNullOrWhiteSpace(element.GetAttribute("value"))
                ? defaultValue
                : element.GetAttribute("value").Trim();
        }

        private void SetAppSetting(XmlDocument document, string key, string value)
        {
            XmlElement appSettings = document.SelectSingleNode("/configuration/appSettings") as XmlElement;
            if (appSettings == null)
            {
                XmlElement configuration = document.SelectSingleNode("/configuration") as XmlElement;
                if (configuration == null)
                {
                    throw new InvalidOperationException("配置文件缺少 configuration 节点。");
                }

                appSettings = document.CreateElement("appSettings");
                configuration.AppendChild(appSettings);
            }

            XmlElement setting = FindAppSetting(document, key);
            if (setting == null)
            {
                setting = document.CreateElement("add");
                setting.SetAttribute("key", key);
                appSettings.AppendChild(setting);
            }

            setting.SetAttribute("value", value);
        }

        private XmlElement FindAppSetting(XmlDocument document, string key)
        {
            return document.SelectSingleNode("/configuration/appSettings/add[@key='" + key + "']") as XmlElement;
        }

        private void SaveConfigDocument(XmlDocument document, string configPath)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(true);
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(configPath, settings))
            {
                document.Save(writer);
            }
        }

        private void StopAndDeleteService()
        {
            if (!ServiceExists())
            {
                return;
            }

            TryRunSc("stop \"" + ServiceName + "\"");
            WaitForServiceStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
            RunSc("delete \"" + ServiceName + "\"");
            System.Threading.Thread.Sleep(1500);
        }

        private void EnsureServiceInstalled()
        {
            if (!ServiceExists())
            {
                throw new InvalidOperationException("服务未安装，请先安装服务。");
            }
        }

        private void RestartServiceIfExists()
        {
            if (!ServiceExists())
            {
                return;
            }

            TryRunSc("stop \"" + ServiceName + "\"");
            WaitForServiceStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
            RunSc("start \"" + ServiceName + "\"");
        }

        private bool ServiceExists()
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                if (string.Equals(service.ServiceName, ServiceName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void WaitForServiceStatus(ServiceControllerStatus status, TimeSpan timeout)
        {
            try
            {
                using (ServiceController controller = new ServiceController(ServiceName))
                {
                    controller.WaitForStatus(status, timeout);
                }
            }
            catch
            {
            }
        }

        private void RefreshServiceStatus()
        {
            try
            {
                using (ServiceController controller = new ServiceController(ServiceName))
                {
                    lblStatus.Text = "服务状态：" + TranslateStatus(controller.Status);
                }
            }
            catch
            {
                lblStatus.Text = "服务状态：未安装";
            }
        }

        private string TranslateStatus(ServiceControllerStatus status)
        {
            switch (status)
            {
                case ServiceControllerStatus.Running:
                    return "运行中";
                case ServiceControllerStatus.Stopped:
                    return "已停止";
                case ServiceControllerStatus.Paused:
                    return "已暂停";
                case ServiceControllerStatus.StartPending:
                    return "正在启动";
                case ServiceControllerStatus.StopPending:
                    return "正在停止";
                default:
                    return status.ToString();
            }
        }

        private void RunSc(string arguments)
        {
            ProcessResult result = RunProcess("sc.exe", arguments);
            if (result.ExitCode != 0)
            {
                throw new InvalidOperationException(result.Output.Length == 0 ? "服务命令执行失败。" : result.Output);
            }
        }

        private void TryRunSc(string arguments)
        {
            ProcessResult result = RunProcess("sc.exe", arguments);
            if (result.Output.Length > 0)
            {
                WriteLog(result.Output);
            }
        }

        private ProcessResult RunProcess(string fileName, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;
            startInfo.Arguments = arguments;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.StandardOutputEncoding = Encoding.Default;
            startInfo.StandardErrorEncoding = Encoding.Default;

            using (Process process = Process.Start(startInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                string combinedOutput = (output + Environment.NewLine + error).Trim();
                if (combinedOutput.Length > 0)
                {
                    WriteLog(combinedOutput);
                }

                return new ProcessResult(process.ExitCode, combinedOutput);
            }
        }

        private string GetInstalledExePath()
        {
            return Path.Combine(DefaultInstallPath, ServiceExeName);
        }

        private string GetInstalledConfigPath()
        {
            return GetInstalledExePath() + ".config";
        }

        private void WriteLog(string message)
        {
            string logLine = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + message;
            txtLog.AppendText(logLine + Environment.NewLine);
            WriteLogFile(logLine);
        }

        private void WriteLogFile(string logLine)
        {
            try
            {
                Directory.CreateDirectory(InstallerLogFolder);
                string logPath = Path.Combine(InstallerLogFolder, "installer-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                File.AppendAllText(logPath, logLine + Environment.NewLine, new UTF8Encoding(true));
            }
            catch
            {
            }
        }

        private sealed class ProcessResult
        {
            public ProcessResult(int exitCode, string output)
            {
                ExitCode = exitCode;
                Output = output ?? string.Empty;
            }

            public int ExitCode { get; private set; }
            public string Output { get; private set; }
        }

        private sealed class UpdatePackageInfo
        {
            public UpdatePackageInfo(string lastModified, string contentLength)
            {
                LastModified = lastModified ?? string.Empty;
                ContentLength = contentLength ?? string.Empty;
            }

            public string LastModified { get; private set; }
            public string ContentLength { get; private set; }
        }
    }
}
