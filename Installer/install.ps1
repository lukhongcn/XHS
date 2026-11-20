param([switch]$Elevated)

$ErrorActionPreference = 'Stop'

$principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    $scriptArgs = @('-NoLogo', '-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', ('"{0}"' -f $PSCommandPath), '-Elevated')
    $elevatedProcess = Start-Process -FilePath 'powershell.exe' -ArgumentList $scriptArgs -Verb RunAs -Wait -PassThru
    exit $elevatedProcess.ExitCode
}

$appName = '欣旺达标签打印'
$installDir = 'C:\XHS\BarcodeDesigner'
$databaseConnectionString = 'server=192.168.1.71;Pooling=false;database=XHS;uid=sa;pwd=ss#12345'
$payloadZip = Join-Path $PSScriptRoot 'BarcodeDesignerPayload.zip'
$extractDir = Join-Path $env:TEMP ('BarcodeDesignerPayload_' + [Guid]::NewGuid().ToString('N'))
$configName = "$appName.exe.config"
$script:progressForm = $null
$script:progressBar = $null
$script:statusLabel = $null

function Set-InstallProgress([int]$percent, [string]$status) {
    if ($null -eq $script:progressForm) { return }
    $script:progressBar.Value = [Math]::Max(0, [Math]::Min(100, $percent))
    $script:statusLabel.Text = $status
    $script:progressForm.Refresh()
    [System.Windows.Forms.Application]::DoEvents()
}

function Show-InstallProgress {
    $script:progressForm = New-Object System.Windows.Forms.Form
    $script:progressForm.Text = "$appName 安装程序"
    $script:progressForm.Size = New-Object System.Drawing.Size(490, 155)
    $script:progressForm.StartPosition = 'CenterScreen'
    $script:progressForm.FormBorderStyle = 'FixedDialog'
    $script:progressForm.ControlBox = $false
    $script:progressForm.TopMost = $true

    $script:statusLabel = New-Object System.Windows.Forms.Label
    $script:statusLabel.Location = New-Object System.Drawing.Point(18, 18)
    $script:statusLabel.Size = New-Object System.Drawing.Size(440, 24)
    $script:statusLabel.Text = '正在准备安装...'
    $script:progressForm.Controls.Add($script:statusLabel)

    $script:progressBar = New-Object System.Windows.Forms.ProgressBar
    $script:progressBar.Location = New-Object System.Drawing.Point(18, 54)
    $script:progressBar.Size = New-Object System.Drawing.Size(440, 25)
    $script:progressBar.Minimum = 0
    $script:progressBar.Maximum = 100
    $script:progressForm.Controls.Add($script:progressBar)
    $script:progressForm.Show()
    Set-InstallProgress 3 '正在准备安装...'
}

function Set-DatabaseConnection([string]$exePath) {
    Add-Type -AssemblyName System.Configuration

    $configPath = "$exePath.config"
    if (-not (Test-Path -LiteralPath $configPath)) {
        throw "找不到程序配置文件：$configPath"
    }

    $fileMap = New-Object System.Configuration.ExeConfigurationFileMap
    $fileMap.ExeConfigFilename = $configPath
    $config = [System.Configuration.ConfigurationManager]::OpenMappedExeConfiguration(
        $fileMap,
        [System.Configuration.ConfigurationUserLevel]::None)
    $section = $config.GetSection('appSettings')

    if ($null -eq $section) {
        throw '配置文件中没有 appSettings 节。'
    }

    if ($section.SectionInformation.IsProtected) {
        $section.SectionInformation.UnprotectSection()
    }

    if ($null -eq $section.Settings['MsSQLConnString']) {
        $section.Settings.Add('MsSQLConnString', $databaseConnectionString)
    }
    else {
        $section.Settings['MsSQLConnString'].Value = $databaseConnectionString
    }

    if (-not $section.SectionInformation.IsProtected) {
        $section.SectionInformation.ProtectSection('DataProtectionConfigurationProvider')
    }

    $section.SectionInformation.ForceSave = $true
    $config.Save([System.Configuration.ConfigurationSaveMode]::Full)
}

function Merge-AppSettings([string]$targetExePath, [string]$templateExePath) {
    Add-Type -AssemblyName System.Configuration

    $targetMap = New-Object System.Configuration.ExeConfigurationFileMap
    $targetMap.ExeConfigFilename = "$targetExePath.config"
    $targetConfig = [System.Configuration.ConfigurationManager]::OpenMappedExeConfiguration(
        $targetMap,
        [System.Configuration.ConfigurationUserLevel]::None)

    $templateMap = New-Object System.Configuration.ExeConfigurationFileMap
    $templateMap.ExeConfigFilename = "$templateExePath.config"
    $templateConfig = [System.Configuration.ConfigurationManager]::OpenMappedExeConfiguration(
        $templateMap,
        [System.Configuration.ConfigurationUserLevel]::None)

    $targetSection = $targetConfig.GetSection('appSettings')
    $templateSection = $templateConfig.GetSection('appSettings')
    if ($null -eq $targetSection -or $null -eq $templateSection) {
        throw '目标配置或安装包配置中没有 appSettings 节。'
    }

    foreach ($key in $templateSection.Settings.AllKeys) {
        # 数据库连接由目标电脑维护，升级时不能覆盖。
        if ($key -ieq 'MsSQLConnString') {
            continue
        }

        $templateValue = $templateSection.Settings[$key].Value
        if ($null -eq $targetSection.Settings[$key]) {
            $targetSection.Settings.Add($key, $templateValue)
        }
        else {
            $targetSection.Settings[$key].Value = $templateValue
        }
    }

    if (-not $targetSection.SectionInformation.IsProtected) {
        $targetSection.SectionInformation.ProtectSection('DataProtectionConfigurationProvider')
    }
    $targetSection.SectionInformation.ForceSave = $true
    $targetConfig.Save([System.Configuration.ConfigurationSaveMode]::Full)
}

try {
    Add-Type -AssemblyName System.Windows.Forms
    Add-Type -AssemblyName System.Drawing
    Show-InstallProgress

    if (-not (Test-Path -LiteralPath $payloadZip)) {
        throw "找不到安装内容：$payloadZip"
    }

    Set-InstallProgress 8 '正在检查程序并准备安装目录...'
    $running = Get-Process -Name '欣旺达标签打印' -ErrorAction SilentlyContinue
    if ($running) {
        $running | Stop-Process -Force
    }

    New-Item -ItemType Directory -Path $extractDir -Force | Out-Null
    Set-InstallProgress 12 '正在解压安装文件...'
    Expand-Archive -LiteralPath $payloadZip -DestinationPath $extractDir -Force
    New-Item -ItemType Directory -Path $installDir -Force | Out-Null
    $targetExePath = Join-Path $installDir "$appName.exe"
    $hadExistingConfig = Test-Path -LiteralPath "$targetExePath.config"

    $files = @(Get-ChildItem -LiteralPath $extractDir -File -Recurse)
    $fileCount = [Math]::Max(1, $files.Count)
    $copiedCount = 0
    Set-InstallProgress 20 '正在复制程序文件...'
    foreach ($file in $files) {
        $relativePath = $file.FullName.Substring($extractDir.TrimEnd('\').Length).TrimStart('\')
        $destination = Join-Path $installDir $relativePath
        if ($file.Name -ieq $configName -and (Test-Path -LiteralPath $destination)) {
            $copiedCount++
            continue
        }

        $destinationDirectory = Split-Path -Parent $destination
        if (-not (Test-Path -LiteralPath $destinationDirectory)) {
            New-Item -ItemType Directory -Path $destinationDirectory -Force | Out-Null
        }
        Copy-Item -LiteralPath $file.FullName -Destination $destination -Force
        $copiedCount++
        $copyPercent = 20 + [int](55 * $copiedCount / $fileCount)
        Set-InstallProgress $copyPercent "正在复制程序文件... ($copiedCount/$fileCount)"
    }

    if ($hadExistingConfig) {
        Set-InstallProgress 80 '正在保留并更新本机配置...'
        Merge-AppSettings $targetExePath (Join-Path $extractDir "$appName.exe")
    }

    Set-InstallProgress 88 '正在生成本机加密数据库配置...'
    Set-DatabaseConnection $targetExePath

    Set-InstallProgress 96 '正在创建桌面快捷方式...'
    $shell = New-Object -ComObject WScript.Shell
    $desktop = [Environment]::GetFolderPath('Desktop')
    $shortcut = $shell.CreateShortcut((Join-Path $desktop "$appName.lnk"))
    $shortcut.TargetPath = Join-Path $installDir "$appName.exe"
    $shortcut.WorkingDirectory = $installDir
    $shortcut.Save()

    Set-InstallProgress 100 '安装完成。'
    Start-Sleep -Milliseconds 500
    $script:progressForm.Close()
    [System.Windows.Forms.MessageBox]::Show(
        "安装完成。程序位置：`n$installDir",
        $appName,
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Information) | Out-Null
}
catch {
    if ($null -ne $script:progressForm) { $script:progressForm.Close() }
    Add-Type -AssemblyName System.Windows.Forms
    [System.Windows.Forms.MessageBox]::Show(
        "安装失败：`n$($_.Exception.Message)",
        $appName,
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Error) | Out-Null
    exit 1
}
finally {
    if ($null -ne $script:progressForm -and -not $script:progressForm.IsDisposed) { $script:progressForm.Close() }
    if (Test-Path -LiteralPath $extractDir) {
        Remove-Item -LiteralPath $extractDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}
