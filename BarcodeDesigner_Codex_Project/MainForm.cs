using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LabelHelp.Services;
using XHS.Model;
using XHS.Model.BarcodeCreateRule;
using XHS.Model.ZPLLabel;
using XHS.service;
using XHS.service.Barcode;
using XHS.service.Formatter;
using XHS.service.SerialNumber;

namespace BarcodeDesigner
{
    public sealed class MainForm : Form
    {
        private readonly Dictionary<string, Control> fieldControls = new Dictionary<string, Control>(StringComparer.OrdinalIgnoreCase);
        private readonly TableLayoutPanel fieldLayout = new TableLayoutPanel();
        private readonly Label statusLabel = new Label();
        private readonly DataGridView barcodeGrid = new DataGridView();
        private readonly Button generateButton = new Button();
        private readonly XHS.BLL.PrintRecord printRecord = new XHS.BLL.PrintRecord();
        private readonly BarcodeBuilder barcodeBuilder;
        private readonly UserFieldValuesStore userFieldValuesStore = new UserFieldValuesStore();
        private readonly string loginUserName;
        private readonly BarcodeCustomerProfile customerProfile;
        private ICodeFormatter serialNumberFormatter;
        private BarcodeRule currentRule;

        public MainForm(string loginUserName, BarcodeCustomerProfile customerProfile)
        {
            this.loginUserName = string.IsNullOrWhiteSpace(loginUserName) ? Environment.UserName : loginUserName;
            this.customerProfile = customerProfile ?? throw new ArgumentNullException(nameof(customerProfile));
            barcodeBuilder = new BarcodeBuilder(
                GetXwdLabelPrintCount,
                ResolveConfigPath(customerProfile.FormatterConfigDirectory));
            InitializeForm();
            LoadRuleAndBuildUi();
            FormClosed += MainForm_FormClosed;
            Shown += MainForm_Shown;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            UpdateDrawingVersionFromPartMaster();
            UpdateGenerateButtonState();
        }

        private void InitializeForm()
        {
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1300, 750);
            Size = new Size(1600, 900);

            fieldLayout.AutoSize = false;
            fieldLayout.Dock = DockStyle.Fill;
            fieldLayout.Padding = new Padding(24, 20, 24, 0);
            fieldLayout.ColumnCount = 2;
            fieldLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            fieldLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            Controls.Add(fieldLayout);
        }

        private void LoadRuleAndBuildUi()
        {
            try
            {
                string schemaFile = customerProfile.UiJsonName;
                if (string.IsNullOrWhiteSpace(schemaFile))
                {
                    throw new ConfigurationErrorsException(
                        "客户配置缺少 UI JSON 文件名。" );
                }

                string schemaPath = ResolveConfigPath(schemaFile);
                BarcodeRule rule = new BarcodeDesignerJsonRuleLoader().LoadBarcodeRule(schemaPath);
                BuildUi(rule);
            }
            catch (Exception ex)
            {
                Text = "Barcode Designer - 配置加载失败";
                Controls.Add(new Label
                {
                    AutoSize = true,
                    ForeColor = Color.DarkRed,
                    Location = new Point(24, 24),
                    Text = ex.Message
                });
            }
        }

        private void BuildUi(BarcodeRule rule)
        {
            currentRule = rule;
            Text = rule.Ui == null || string.IsNullOrWhiteSpace(rule.Ui.FormTitle)
                ? rule.Description ?? rule.RuleName
                : rule.Ui.FormTitle;

            List<BarcodeFieldRule> fields = rule.Fields
                .OrderBy(field => field.Ui == null ? int.MaxValue : field.Ui.Order)
                .ToList();

            BarcodeFieldRule serialField = fields.FirstOrDefault(field =>
                string.Equals(field.Name, "SerialNo", StringComparison.OrdinalIgnoreCase));
            if (serialField == null)
            {
                throw new InvalidOperationException("界面配置缺少 SerialNo 字段。" );
            }

            serialNumberFormatter = new FormatterFactory(
                    ResolveConfigPath(customerProfile.FormatterConfigDirectory)).Get(
                serialField,
                GetXwdLabelPrintCount);

            fieldLayout.RowCount = fields.Count + 3;
            for (int index = 0; index < fields.Count; index++)
            {
                AddField(fieldLayout, fields[index], index);
            }

            generateButton.AutoSize = true;
            generateButton.Enabled = false;
            generateButton.Text = rule.Ui == null || string.IsNullOrWhiteSpace(rule.Ui.GenerateButtonText)
                ? "生成条码"
                : rule.Ui.GenerateButtonText;
            generateButton.Click += GenerateButton_Click;
            fieldLayout.Controls.Add(generateButton, 1, fields.Count);

            fieldLayout.Controls.Add(new Label { AutoSize = true, Text = "条码结果" }, 0, fields.Count + 1);
            ConfigureBarcodeGrid(fields);
            fieldLayout.Controls.Add(barcodeGrid, 1, fields.Count + 1);

            fieldLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            fieldLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            fieldLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            fieldLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            statusLabel.AutoSize = true;
            statusLabel.ForeColor = Color.DimGray;
            fieldLayout.Controls.Add(statusLabel, 1, fields.Count + 2);
            LoadRememberedValues(rule);
            UpdateDrawingVersionFromPartMaster();
            RefreshSerialNumber();
            UpdateGenerateButtonState();
        }

        private void ConfigureBarcodeGrid(IList<BarcodeFieldRule> fields)
        {
            barcodeGrid.AllowUserToAddRows = false;
            barcodeGrid.AllowUserToDeleteRows = false;
            barcodeGrid.AllowUserToResizeRows = false;
            barcodeGrid.AutoGenerateColumns = false;
            barcodeGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            barcodeGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            barcodeGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            barcodeGrid.Dock = DockStyle.Fill;
            barcodeGrid.MinimumSize = new Size(0, 160);
            barcodeGrid.MultiSelect = false;
            barcodeGrid.ReadOnly = true;
            barcodeGrid.RowHeadersVisible = false;
            barcodeGrid.ScrollBars = ScrollBars.Both;
            barcodeGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            foreach (BarcodeFieldRule field in fields)
            {
                barcodeGrid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = string.IsNullOrWhiteSpace(field.Description) ? field.Name : field.Description,
                    Name = field.Name,
                    DataPropertyName = field.Name,
                    FillWeight = 100,
                    ReadOnly = true
                });
            }

            barcodeGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                HeaderText = "条码",
                Name = "Barcode",
                DataPropertyName = "Barcode",
                Width = 500,
                ReadOnly = true
            });
        }

        private void AddField(TableLayoutPanel layout, BarcodeFieldRule field, int row)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(new Label
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Text = string.IsNullOrWhiteSpace(field.Description) ? field.Name : field.Description
            }, 0, row);

            Control control = CreateFieldControl(field);
            fieldControls[field.Name] = control;
            AttachFieldValueChanged(control);
            layout.Controls.Add(control, 1, row);
        }

        private Control CreateFieldControl(BarcodeFieldRule field)
        {
            if (string.Equals(field.Name, "PartNo", StringComparison.OrdinalIgnoreCase))
            {
                ComboBox comboBox = CreatePartNoComboBox(field, customerProfile.CustomerAbbr);
                comboBox.SelectedValueChanged += PartNoComboBox_SelectedValueChanged;
                comboBox.SelectedIndexChanged += PartNoComboBox_SelectedValueChanged;
                return comboBox;
            }

            if (string.Equals(field.Name, "DrawingVersion", StringComparison.OrdinalIgnoreCase))
            {
                return new TextBox
                {
                    ReadOnly = true,
                    TabStop = false,
                    Width = GetWidth(field, 200)
                };
            }

            string controlName = field.Ui == null ? null : field.Ui.Control;
            bool isDate = string.Equals(field.Source, "Date", StringComparison.OrdinalIgnoreCase)
                || string.Equals(controlName, "DateTimePicker", StringComparison.OrdinalIgnoreCase);

            if (isDate)
            {
                return new DateTimePicker
                {
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = field.Ui == null || string.IsNullOrWhiteSpace(field.Ui.Format)
                        ? "yyyy-MM-dd"
                        : field.Ui.Format,
                    Width = GetWidth(field, 150),
                    Value = DateTime.Today,
                    Enabled = false
                };
            }

            return new TextBox
            {
                ReadOnly = string.Equals(field.Source, "Auto", StringComparison.OrdinalIgnoreCase)
                    || (field.Ui != null && field.Ui.ReadOnly),
                Width = GetWidth(field, 200),
                Text = GetDefaultValue(field)
            };
        }

        private static ComboBox CreatePartNoComboBox(BarcodeFieldRule field, string customerAbbr)
        {
            List<XHS.Model.PartMasterInfo> parts = new XHS.BLL.PartMaster()
                .GetPartByCustomerAbbr(customerAbbr);

            ComboBox comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Width = GetWidth(field, 200),
                DisplayMember = "JHSPartNo",
                ValueMember = "JHSPartNo",
                DataSource = parts
            };
            comboBox.SelectedIndex = -1;
            return comboBox;
        }

        private void PartNoComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateDrawingVersionFromPartMaster();
            if (fieldControls.ContainsKey("SerialNo"))
            {
                RefreshSerialNumber();
            }
        }

        private void UpdateDrawingVersionFromPartMaster()
        {
            Control partNoControl;
            Control drawingVersionControl;
            Control batchNoControl;
            if (!fieldControls.TryGetValue("PartNo", out partNoControl))
            {
                return;
            }

            fieldControls.TryGetValue("DrawingVersion", out drawingVersionControl);
            fieldControls.TryGetValue("BatchNo", out batchNoControl);

            ComboBox partNoComboBox = partNoControl as ComboBox;
            string selectedPartNo = partNoComboBox == null
                ? string.Empty
                : Convert.ToString(partNoComboBox.SelectedValue);
            if (string.IsNullOrWhiteSpace(selectedPartNo) && partNoComboBox != null)
            {
                selectedPartNo = partNoComboBox.Text;
            }

            List<XHS.Model.PartMasterInfo> parts = partNoComboBox == null
                ? null
                : partNoComboBox.DataSource as List<XHS.Model.PartMasterInfo>;
            XHS.Model.PartMasterInfo selectedPart = parts == null
                ? null
                : parts.FirstOrDefault(part => part != null
                    && string.Equals(
                        part.JHSPartNo,
                        selectedPartNo,
                        StringComparison.OrdinalIgnoreCase));

            if (selectedPart == null && partNoComboBox != null)
            {
                selectedPart = partNoComboBox.SelectedItem as XHS.Model.PartMasterInfo;
            }

            if (selectedPart == null && parts != null && partNoComboBox != null)
            {
                selectedPart = parts.FirstOrDefault(part => part != null
                    && string.Equals(
                        part.JHSPartNo,
                        partNoComboBox.Text.Trim(),
                        StringComparison.OrdinalIgnoreCase));
            }

            if (drawingVersionControl != null)
            {
                drawingVersionControl.Text = selectedPart == null
                    ? string.Empty
                    : (selectedPart.Remark ?? string.Empty).Trim();
            }

            if (batchNoControl != null)
            {
                batchNoControl.Text = selectedPart == null
                    ? string.Empty
                    : (selectedPart.BatchNo ?? string.Empty).Trim();
            }
        }

        private static string GetDefaultValue(BarcodeFieldRule field)
        {
            if (field.Ui != null && field.Ui.DefaultValue != null)
            {
                return field.Ui.DefaultValue;
            }

            if (string.Equals(field.Source, "Auto", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(field.Format))
            {
                return new string('0', field.Format.Length);
            }

            return "";
        }

        private static int GetWidth(BarcodeFieldRule field, int defaultWidth)
        {
            return field.Ui == null || field.Ui.Width <= 0 ? defaultWidth : field.Ui.Width;
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            try
            {
                int quantity = GetGenerateQuantity();
                string schemaFile = customerProfile.UiJsonName;
                DataTable result = barcodeBuilder.Build(
                    schemaFile,
                    CollectFieldValues(),
                    quantity);

                barcodeGrid.DataSource = result;
                if (result.Rows.Count > 0)
                {
                    SetFieldText("SerialNo", Convert.ToString(result.Rows[0]["SerialNo"]));
                }

                string zplFile = GenerateZplFile(result);
                PrintZplFile(zplFile);
                SavePrintRecord(result, zplFile);
                RefreshSerialNumber();

                statusLabel.Text = string.Format(
                    "已生成并打印 {0} 条条码，已保存打印记录。",
                    result.Rows.Count);
            }
            catch (Exception ex)
            {
                statusLabel.ForeColor = Color.DarkRed;
                statusLabel.Text = ex.Message;
            }
        }

        private string GenerateZplFile(DataTable barcodeResult)
        {
            string outputFile = ConfigurationManager.AppSettings["ZplOutputFile"];
            if (string.IsNullOrWhiteSpace(outputFile))
            {
                throw new ConfigurationErrorsException("App.config 缺少 ZplOutputFile 配置。");
            }

            string printRulePath = ResolveConfigPath(customerProfile.PrintRuleFile);
            BarcodePrintRule printRule = BarcodePrintRuleLoader.Load(printRulePath);
            string templatePath = ResolveConfigPath(printRule.TemplateFile);
            string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outputFile);
            StringBuilder zpl = new StringBuilder();
            ZplGenerator generator = new ZplGenerator();

            foreach (DataRow row in barcodeResult.Rows)
            {
                List<string> values = BuildPrintValues(row, printRule);

                ZplResult generated = generator.Generate(templatePath, values);
                if (!generated.Success)
                {
                    throw new InvalidOperationException("生成 ZPL 失败：" + generated.ErrorMessage);
                }

                if (zpl.Length > 0)
                {
                    zpl.AppendLine();
                }
                zpl.Append(generated.Zpl);
            }

            string outputDirectory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrWhiteSpace(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            File.WriteAllText(outputPath, zpl.ToString(), Encoding.UTF8);
            return outputPath;
        }

        private static string ResolveConfigPath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ConfigurationErrorsException("标签配置文件名不能为空。");
            }

            return Path.IsPathRooted(fileName)
                ? fileName
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }

        private static List<string> BuildPrintValues(DataRow row, BarcodePrintRule printRule)
        {
            var values = new List<string>();
            foreach (BarcodePrintValueRule valueRule in printRule.Values)
            {
                var value = new StringBuilder();
                foreach (BarcodePrintValuePart part in valueRule.Parts)
                {
                    if (!row.Table.Columns.Contains(part.Field))
                    {
                        throw new InvalidOperationException("打印规则字段不存在：" + part.Field);
                    }

                    string fieldValue = Convert.ToString(row[part.Field]) ?? string.Empty;
                    int start = part.Start ?? 0;
                    if (start > fieldValue.Length)
                    {
                        throw new InvalidOperationException(
                            "打印规则字段截取位置超出范围：" + part.Field);
                    }

                    int length = part.Length ?? fieldValue.Length - start;
                    if (start + length > fieldValue.Length)
                    {
                        throw new InvalidOperationException(
                            "打印规则字段截取长度超出范围：" + part.Field);
                    }

                    value.Append(fieldValue.Substring(start, length));
                }

                values.Add(value.ToString());
            }

            return values;
        }

        private static void PrintZplFile(string zplFile)
        {
            if (!File.Exists(zplFile))
            {
                throw new FileNotFoundException("ZPL 文件不存在。", zplFile);
            }

            string printerName = ConfigurationManager.AppSettings["PrinterName"];
            if (string.IsNullOrWhiteSpace(printerName))
            {
                using (System.Drawing.Printing.PrintDocument printDocument = new System.Drawing.Printing.PrintDocument())
                {
                    printerName = printDocument.PrinterSettings.PrinterName;
                }
            }

            if (string.IsNullOrWhiteSpace(printerName))
            {
                throw new InvalidOperationException("未配置打印机，且 Windows 没有默认打印机。");
            }

            string zpl = File.ReadAllText(zplFile, Encoding.UTF8);
            PrintResult printResult = new ZebraRawPrinter(printerName).Print(zpl);
            if (!printResult.Success)
            {
                throw new InvalidOperationException("打印失败：" + printResult.ErrorMessage);
            }
        }

        private void SavePrintRecord(DataTable barcodeResult, string zplFile)
        {
            if (barcodeResult == null || barcodeResult.Rows.Count == 0)
            {
                throw new InvalidOperationException("没有可保存的条码记录。");
            }

            DateTime now = DateTime.Now;
            string currentUser = loginUserName;
            string partNo = Convert.ToString(barcodeResult.Rows[0]["PartNo"]);
            string batchNo = Convert.ToString(barcodeResult.Rows[0]["BatchNo"]);
            PrintRecordInfo printRecordInfo = new PrintRecordInfo
            {
                SupplyBatchNo = batchNo,
                PartNo = partNo,
                CartonNo = string.Empty,
                MachineId = Environment.MachineName,
                PrintType = customerProfile.CustomerName + "标签",
                LocalPath = zplFile,
                Status = PrintRecordStatusInfo.Completed,
                PrintCount = barcodeResult.Rows.Count,
                PrintUser = currentUser,
                PrintTime = now,
                FirstPrintUser = currentUser,
                FirstPrintTime = now,
                LastPrintUser = currentUser,
                LastPrintTime = now,
                CreateUser = currentUser,
                CreateTime = now,
                UpdateUser = currentUser,
                UpdateTime = now
            };

            string message = printRecord.InsertPrintRecord(new List<PrintRecordInfo> { printRecordInfo });
            if (!string.IsNullOrWhiteSpace(message))
            {
                throw new InvalidOperationException("保存打印记录失败：" + message);
            }
        }

        private void RefreshSerialNumber()
        {
            if (currentRule == null || currentRule.Fields == null)
            {
                return;
            }

            BarcodeFieldRule serialField = currentRule.Fields.FirstOrDefault(field =>
                field != null
                && string.Equals(field.Name, "SerialNo", StringComparison.OrdinalIgnoreCase));
            if (serialField == null)
            {
                return;
            }

            serialNumberFormatter = new FormatterFactory(
                    ResolveConfigPath(customerProfile.FormatterConfigDirectory)).Get(
                serialField,
                GetXwdLabelPrintCount);
            Control partNoControl;
            string partNo = fieldControls.TryGetValue("PartNo", out partNoControl)
                ? GetControlValue(partNoControl).Trim()
                : string.Empty;
            SetFieldText(
                "SerialNo",
                serialNumberFormatter.Encode(new SerialNumberInput(DateTime.Today, partNo)));
        }

        private int GetXwdLabelPrintCount(DateTime printTime, string partNo)
        {
            return printRecord.GetPrintCount(customerProfile.CustomerName + "标签", printTime, partNo);
        }

        private IDictionary<string, object> CollectFieldValues()
        {
            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, Control> item in fieldControls)
            {
                if (string.Equals(item.Key, "SerialNo", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(item.Key, "GenerateQuantity", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                DateTimePicker dateTimePicker = item.Value as DateTimePicker;
                values[item.Key] = dateTimePicker == null
                    ? (object)GetControlValue(item.Value)
                    : dateTimePicker.Value.Date;
            }

            return values;
        }

        private int GetGenerateQuantity()
        {
            Control control;
            if (!fieldControls.TryGetValue("GenerateQuantity", out control))
            {
                throw new InvalidOperationException("界面配置缺少产生数量字段。" );
            }

            int quantity;
            if (!int.TryParse(control.Text, out quantity) || quantity <= 0)
            {
                throw new FormatException("产生数量必须是大于 0 的整数。" );
            }

            return quantity;
        }

        private void SetFieldText(string fieldName, string value)
        {
            Control control;
            if (!fieldControls.TryGetValue(fieldName, out control))
            {
                throw new InvalidOperationException("界面配置缺少字段：" + fieldName);
            }

            control.Text = value;
        }

        private void LoadRememberedValues(BarcodeRule rule)
        {
            IDictionary<string, string> values = userFieldValuesStore.Load(rule.RuleName);
            foreach (BarcodeFieldRule field in rule.Fields.Where(item => item != null && item.RememberLastValue))
            {
                string value;
                if (values.TryGetValue(field.Name, out value))
                {
                    SetControlValue(field.Name, value);
                }
            }
        }

        private void SaveRememberedValues()
        {
            if (currentRule == null)
            {
                return;
            }

            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (BarcodeFieldRule field in currentRule.Fields.Where(item => item != null && item.RememberLastValue))
            {
                Control control;
                if (fieldControls.TryGetValue(field.Name, out control))
                {
                    values[field.Name] = GetControlValue(control);
                }
            }

            userFieldValuesStore.Save(currentRule.RuleName, values);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                SaveRememberedValues();
            }
            catch
            {
                // 保存失败不能阻止窗口关闭。
            }
        }

        private void SetControlValue(string fieldName, string value)
        {
            Control control;
            if (!fieldControls.TryGetValue(fieldName, out control))
            {
                return;
            }

            ComboBox comboBox = control as ComboBox;
            if (comboBox != null)
            {
                comboBox.SelectedValue = value;
                if (comboBox.SelectedIndex < 0)
                {
                    comboBox.Text = value;
                }

                return;
            }

            control.Text = value ?? string.Empty;
        }

        private static string GetControlValue(Control control)
        {
            ComboBox comboBox = control as ComboBox;
            if (comboBox != null && comboBox.SelectedIndex >= 0)
            {
                return Convert.ToString(comboBox.SelectedValue) ?? string.Empty;
            }

            return control.Text ?? string.Empty;
        }

        private void AttachFieldValueChanged(Control control)
        {
            TextBox textBox = control as TextBox;
            if (textBox != null)
            {
                textBox.TextChanged += FieldValueChanged;
                return;
            }

            ComboBox comboBox = control as ComboBox;
            if (comboBox != null)
            {
                comboBox.SelectedValueChanged += FieldValueChanged;
                comboBox.TextChanged += FieldValueChanged;
                return;
            }

            DateTimePicker dateTimePicker = control as DateTimePicker;
            if (dateTimePicker != null)
            {
                dateTimePicker.ValueChanged += FieldValueChanged;
            }
        }

        private void FieldValueChanged(object sender, EventArgs e)
        {
            UpdateGenerateButtonState();
        }

        private void UpdateGenerateButtonState()
        {
            generateButton.Enabled = fieldControls.Count > 0
                && fieldControls.Values.All(HasValue);
        }

        private static bool HasValue(Control control)
        {
            ComboBox comboBox = control as ComboBox;
            if (comboBox != null)
            {
                return comboBox.SelectedIndex >= 0
                    && !string.IsNullOrWhiteSpace(comboBox.Text);
            }

            return !string.IsNullOrWhiteSpace(control.Text);
        }
    }
}
