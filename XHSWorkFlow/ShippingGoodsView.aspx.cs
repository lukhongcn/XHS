using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using CheryPortHelp;
using ModuleWorkFlow.BLL;
using ModuleWorkFlow.business;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>
    /// 出货货品新增/编辑页面。
    /// </summary>
    public partial class ShippingGoodsView : Page
    {
        protected string menuname = "";
        private const string MenuId = "B01";

        private void Page_Load(object sender, EventArgs e)
        {
            menuname = new PartTmenu().findbykey(MenuId).Menuname;
            if (Master is DefaultSub master)
            {
                master.Menuname = menuname;
            }

            if (!ModuleWorkFlow.BLL.Private.checkPrivate(this, MenuId, "PEDIT"))
            {
                return;
            }

            //if (Session["userid"] == null)
            //{
            //    Response.Redirect("login.aspx");
            //    return;
            //}

            txt_barcode.Attributes["autocomplete"] = "off";
            txt_barcode.Attributes["onkeydown"] = "return shippingGoodsBarcodeKeyDown(event);";

            if (!IsPostBack)
            {
                InitializePage();
                LoadShippingGoods();
            }
        }

        protected void txt_barcode_TextChanged(object sender, EventArgs e)
        {
            ParseBarcode();
        }

        protected void lnk_view_Click(object sender, EventArgs e)
        {
            string url = string.Format("ShippingGoodsList.aspx?supplyBatchNo={0}", txt_SupplyBatchNo.Text.Trim());
            Response.Redirect(url);
        }

        protected void lnkbutton_save_Click(object sender, EventArgs e)
        {
            SaveShippingGoods();
        }

        private void InitializePage()
        {
            txt_SupplierCode.Text = CheryPortConfig.SupplNo;
            ApplyReadOnlyState();
        }

        private void ApplyReadOnlyState()
        {
            ApplyTextBoxReadOnly(txt_SupplierCode);
            ApplyTextBoxReadOnly(txt_PartNo);
            ApplyTextBoxReadOnly(txt_Quantity);
            ApplyTextBoxReadOnly(txt_SupplyBatchNo);
            ApplyTextBoxReadOnly(txt_StackLayerCount);
        }

        private static void ApplyTextBoxReadOnly(TextBox textBox)
        {
            textBox.ReadOnly = true;
            string cssClass = textBox.CssClass ?? string.Empty;
            if (!cssClass.Contains("shipping-goods-readonly"))
            {
                textBox.CssClass = (cssClass + " shipping-goods-readonly").Trim();
            }
        }

        private void LoadShippingGoods()
        {
            string idText = Request.QueryString["id"];
            int id;
            if (!int.TryParse(idText, out id))
            {
                return;
            }

            ShippingGoodsInfo shippingGoodsInfo = new ShippingGoods().GetShippingGoods().Find(item => item != null && item.Id == id);
            if (shippingGoodsInfo == null)
            {
                ShowMessage("未找到对应的出货货品数据。");
                return;
            }

            BindShippingGoodsInfo(shippingGoodsInfo);
        }

        private void BindShippingGoodsInfo(ShippingGoodsInfo shippingGoodsInfo)
        {
            hid_Id.Value = shippingGoodsInfo.Id.HasValue ? shippingGoodsInfo.Id.Value.ToString() : string.Empty;
            hid_CartonNo.Value = SafeValue(shippingGoodsInfo.CartonNo);
            hid_QrCode.Value = SafeValue(shippingGoodsInfo.OutBoxQRCode);
            hid_Status.Value = SafeValue(shippingGoodsInfo.Status);
            hid_PrintCount.Value = shippingGoodsInfo.PrintCount.HasValue ? shippingGoodsInfo.PrintCount.Value.ToString() : string.Empty;
            hid_Creater.Value = SafeValue(shippingGoodsInfo.Creater);
            hid_CreatDate.Value = shippingGoodsInfo.CreatDate.HasValue
                ? shippingGoodsInfo.CreatDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                : string.Empty;

            txt_SupplierCode.Text = CheryPortConfig.SupplNo;
            txt_PartNo.Text = SafeValue(shippingGoodsInfo.PartNo);
            txt_PartChineseName.Text = SafeValue(shippingGoodsInfo.PartChineseName);
            txt_PartEnglishName.Text = SafeValue(shippingGoodsInfo.PartEnglishName);
            txt_Quantity.Text = shippingGoodsInfo.Quantity.HasValue ? shippingGoodsInfo.Quantity.Value.ToString() : string.Empty;
            txt_SupplyBatchNo.Text = SafeValue(shippingGoodsInfo.SupplyBatchNo);
            txt_StackLayerCount.Text = shippingGoodsInfo.StackLayerCount.HasValue ? shippingGoodsInfo.StackLayerCount.Value.ToString() : string.Empty;
            txt_ProductionDate.Text = FormatDateTimeLocal(shippingGoodsInfo.ProductionDate);
            txt_InspectionConfirmDate.Text = FormatDateTimeLocal(shippingGoodsInfo.InspectionConfirmDate);
            txt_BoxCount.Text = string.Empty;
            txt_SingleBoxGrossWeight.Text = shippingGoodsInfo.SingleBoxGrossWeight.HasValue
                ? shippingGoodsInfo.SingleBoxGrossWeight.Value.ToString("0.##")
                : string.Empty;
        }

        private void ParseBarcode()
        {
            string barcode = SafeValue(txt_barcode.Text)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();

            txt_barcode.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(barcode))
            {
                return;
            }

            txt_SupplierCode.Text = CheryPortConfig.SupplNo;
            ClearQrDrivenFields();
            hid_QrCode.Value = barcode;

            var qrCodeService = new XHS.BLL.QRCode();
            ShippingGoodsInfo scannedShippingGoodsInfo = qrCodeService.ParseShippingGoodsInfo(barcode);

            string scanSupplierCode = SafeValue(scannedShippingGoodsInfo.SupplierCode);
            if (!string.IsNullOrWhiteSpace(scanSupplierCode) &&
                !string.Equals(scanSupplierCode.Trim(), txt_SupplierCode.Text.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                ShowMessage(string.Format("扫码供应商代码“{0}”与系统配置“{1}”不一致，页面已按系统配置带出。", scanSupplierCode.Trim(), txt_SupplierCode.Text.Trim()));
            }
            else
            {
                ShowMessage("条码解析完成。");
            }

            txt_PartNo.Text = SafeValue(scannedShippingGoodsInfo.PartNo);
            txt_Quantity.Text = scannedShippingGoodsInfo.Quantity.HasValue ? scannedShippingGoodsInfo.Quantity.Value.ToString() : string.Empty;
            txt_SupplyBatchNo.Text = SafeValue(scannedShippingGoodsInfo.SupplyBatchNo);
            txt_StackLayerCount.Text = scannedShippingGoodsInfo.StackLayerCount.HasValue ? scannedShippingGoodsInfo.StackLayerCount.Value.ToString() : string.Empty;
            txt_ProductionDate.Text = FormatDateTimeLocal(scannedShippingGoodsInfo.ProductionDate);
        }

        private void ClearQrDrivenFields()
        {
            txt_PartNo.Text = string.Empty;
            txt_Quantity.Text = string.Empty;
            txt_SupplyBatchNo.Text = string.Empty;
            txt_StackLayerCount.Text = string.Empty;
            txt_ProductionDate.Text = string.Empty;
            txt_BoxCount.Text = string.Empty;
            hid_QrCode.Value = string.Empty;
        }

        private void SaveShippingGoods()
        {
            string validateMessage;
            if (!ValidateInput(out validateMessage))
            {
                ShowMessage(validateMessage);
                return;
            }

            List<ShippingGoodsInfo> shippingGoodsInfos = BuildShippingGoodsInfo();
            ShippingGoodsInfo firstShippingGoodsInfo = shippingGoodsInfos.Count == 0 ? null : shippingGoodsInfos[0];
            ShippingGoods shippingGoods = new ShippingGoods();
            bool success;

            if (firstShippingGoodsInfo != null && firstShippingGoodsInfo.Id.HasValue)
            {
                ParamterInfo parameterInfo = shippingGoods.UpdateShippingGoods(shippingGoodsInfos);
                IList source = new ArrayList();
                source.Add(parameterInfo);
                success = Common.Save(source);
                ShowMessage(success ? "保存成功。" : "保存失败。");
                return;
            }

            foreach (ShippingGoodsInfo shippingGoodsInfo in shippingGoodsInfos)
            {
                List<ShippingGoodsInfo> duplicateInfos = shippingGoods.GetShippingGoodsBySupplyBatchNo(
                    shippingGoodsInfo.SupplyBatchNo,
                    shippingGoodsInfo.CartonNo);
                if (duplicateInfos.Count > 0)
                {
                    ShowMessage(string.Format("供货批次号“{0}”下的纸箱编号“{1}”已存在。", shippingGoodsInfo.SupplyBatchNo, shippingGoodsInfo.CartonNo));
                    return;
                }
            }

            string saveMessage = shippingGoods.InsertShippingGoods(shippingGoodsInfos);
            if (string.IsNullOrWhiteSpace(saveMessage))
            {
                ShowMessage(string.Format("保存成功，共生成 {0} 条数据。", shippingGoodsInfos.Count));
                hid_Id.Value = string.Empty;
                hid_Status.Value = ShippingGoodsStatusInfo.UnPrinted;
                hid_PrintCount.Value = "0";
                hid_Creater.Value = firstShippingGoodsInfo == null ? string.Empty : firstShippingGoodsInfo.Creater;
                hid_CreatDate.Value = firstShippingGoodsInfo != null && firstShippingGoodsInfo.CreatDate.HasValue
                    ? firstShippingGoodsInfo.CreatDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    : string.Empty;
                return;
            }

            ShowMessage(saveMessage);
        }

        private bool ValidateInput(out string message)
        {
            var missingFields = new List<string>();

            CheckRequiredField(txt_SupplierCode.Text, "供应商代码", missingFields);
            CheckRequiredField(txt_PartNo.Text, "零件编号", missingFields);
            CheckRequiredField(txt_PartChineseName.Text, "零件中文名称", missingFields);
            CheckRequiredField(txt_Quantity.Text, "数量", missingFields);
            CheckRequiredField(txt_SupplyBatchNo.Text, "供货批次号", missingFields);
            CheckRequiredField(txt_StackLayerCount.Text, "码放层数", missingFields);
            CheckRequiredField(txt_ProductionDate.Text, "生产日期", missingFields);
            CheckRequiredField(txt_InspectionConfirmDate.Text, "检验确认日期", missingFields);
            CheckRequiredField(txt_BoxCount.Text, "纸箱数量", missingFields);
            if (missingFields.Count > 0)
            {
                message = "以下字段不能为空：" + Environment.NewLine + string.Join(Environment.NewLine, missingFields.ToArray());
                return false;
            }

            int quantity;
            if (!int.TryParse(txt_Quantity.Text.Trim(), out quantity))
            {
                message = "数量必须为整数。";
                return false;
            }

            int stackLayerCount;
            if (!int.TryParse(txt_StackLayerCount.Text.Trim(), out stackLayerCount))
            {
                message = "码放层数必须为整数。";
                return false;
            }

            int boxCount;
            if (!int.TryParse(txt_BoxCount.Text.Trim(), out boxCount))
            {
                message = "纸箱数量必须为整数。";
                return false;
            }

            DateTime productionDate;
            if (!TryParseDate(txt_ProductionDate.Text.Trim(), out productionDate))
            {
                message = "生产日期格式不正确。";
                return false;
            }

            DateTime inspectionConfirmDate;
            if (!TryParseDate(txt_InspectionConfirmDate.Text.Trim(), out inspectionConfirmDate))
            {
                message = "检验确认日期格式不正确。";
                return false;
            }

            decimal grossWeight;
            if (!string.IsNullOrWhiteSpace(txt_SingleBoxGrossWeight.Text) &&
                !decimal.TryParse(txt_SingleBoxGrossWeight.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out grossWeight) &&
                !decimal.TryParse(txt_SingleBoxGrossWeight.Text.Trim(), out grossWeight))
            {
                message = "单箱毛重格式不正确。";
                return false;
            }

            if (!string.Equals(txt_SupplierCode.Text.Trim(), CheryPortConfig.SupplNo, StringComparison.OrdinalIgnoreCase))
            {
                message = "供应商代码必须与系统配置一致。";
                return false;
            }

            message = string.Empty;
            return true;
        }

        private List<ShippingGoodsInfo> BuildShippingGoodsInfo()
        {
            int id;
            int boxCount;
            int printCount;
            decimal singleBoxGrossWeight;
            DateTime creatDate;
            DateTime productionDate;
            DateTime inspectionConfirmDate;

            string userName = Session["userid"] == null ? string.Empty : Session["userid"].ToString().Trim();
            DateTime.TryParse(hid_CreatDate.Value, out creatDate);
            int.TryParse(txt_BoxCount.Text.Trim(), out boxCount);
            TryParseDate(txt_ProductionDate.Text.Trim(), out productionDate);
            TryParseDate(txt_InspectionConfirmDate.Text.Trim(), out inspectionConfirmDate);

            var templateShippingGoodsInfo = new ShippingGoodsInfo
            {
                SupplierCode = txt_SupplierCode.Text.Trim(),
                PartNo = txt_PartNo.Text.Trim(),
                PartChineseName = txt_PartChineseName.Text.Trim(),
                PartEnglishName = SafeValue(txt_PartEnglishName.Text),
                Quantity = Convert.ToInt32(txt_Quantity.Text.Trim()),
                SupplyBatchNo = txt_SupplyBatchNo.Text.Trim(),
                StackLayerCount = Convert.ToInt32(txt_StackLayerCount.Text.Trim()),
                ProductionDate = productionDate,
                InspectionConfirmDate = inspectionConfirmDate,
                CartonNo = SafeValue(hid_CartonNo.Value),
                QrCode = string.Empty,
                OutBoxQRCode = SafeValue(hid_QrCode.Value),
                Status = string.IsNullOrWhiteSpace(hid_Status.Value) ? ShippingGoodsStatusInfo.UnPrinted : hid_Status.Value.Trim(),
                PrintCount = int.TryParse(hid_PrintCount.Value, out printCount) ? printCount : 0,
                Creater = string.IsNullOrWhiteSpace(hid_Creater.Value) ? userName : hid_Creater.Value.Trim(),
                CreatDate = creatDate == DateTime.MinValue ? DateTime.Now : creatDate
            };

            if (int.TryParse(hid_Id.Value, out id))
            {
                templateShippingGoodsInfo.Id = id;
            }

            if (!string.IsNullOrWhiteSpace(txt_SingleBoxGrossWeight.Text))
            {
                if (decimal.TryParse(txt_SingleBoxGrossWeight.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out singleBoxGrossWeight) ||
                    decimal.TryParse(txt_SingleBoxGrossWeight.Text.Trim(), out singleBoxGrossWeight))
                {
                    templateShippingGoodsInfo.SingleBoxGrossWeight = singleBoxGrossWeight;
                }
            }

            if (templateShippingGoodsInfo.Id.HasValue)
            {
                return new List<ShippingGoodsInfo> { templateShippingGoodsInfo };
            }

            var shippingGoodsInfos = new List<ShippingGoodsInfo>();
            for (int i = 1; i <= boxCount; i++)
            {
                shippingGoodsInfos.Add(new ShippingGoodsInfo
                {
                    SupplierCode = templateShippingGoodsInfo.SupplierCode,
                    PartNo = templateShippingGoodsInfo.PartNo,
                    PartChineseName = templateShippingGoodsInfo.PartChineseName,
                    PartEnglishName = templateShippingGoodsInfo.PartEnglishName,
                    Quantity = templateShippingGoodsInfo.Quantity,
                    SupplyBatchNo = templateShippingGoodsInfo.SupplyBatchNo,
                    StackLayerCount = templateShippingGoodsInfo.StackLayerCount,
                    ProductionDate = templateShippingGoodsInfo.ProductionDate,
                    InspectionConfirmDate = templateShippingGoodsInfo.InspectionConfirmDate,
                    CartonNo = txt_BoxCount.Text.Trim() + "-" + i.ToString(),
                    QrCode = templateShippingGoodsInfo.QrCode,
                    OutBoxQRCode = templateShippingGoodsInfo.OutBoxQRCode,
                    Status = templateShippingGoodsInfo.Status,
                    PrintCount = templateShippingGoodsInfo.PrintCount,
                    Creater = templateShippingGoodsInfo.Creater,
                    CreatDate = templateShippingGoodsInfo.CreatDate,
                    SingleBoxGrossWeight = templateShippingGoodsInfo.SingleBoxGrossWeight
                });
            }

            return shippingGoodsInfos;
        }

        private static void CheckRequiredField(string value, string fieldName, List<string> missingFields)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                missingFields.Add(fieldName);
            }
        }

        private void ShowMessage(string message)
        {
            Label_Message.Text = SafeValue(message);
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static bool TryParseDate(string value, out DateTime dateValue)
        {
            return DateTime.TryParse(value, out dateValue) ||
                   DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue) ||
                   DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue) ||
                   DateTime.TryParseExact(value, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue);
        }

        private static string FormatDateTimeLocal(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy-MM-ddTHH:mm") : string.Empty;
        }

        #region Web Form Designer generated code
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            Load += new EventHandler(Page_Load);
        }
        #endregion
    }
}









