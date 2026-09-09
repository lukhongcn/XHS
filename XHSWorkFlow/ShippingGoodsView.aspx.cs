using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
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

            string privateCode = IsEditMode() ? "PEDIT" : "PADD";
            if (!ModuleWorkFlow.BLL.Private.checkPrivate(this, MenuId, privateCode))
            {
                return;
            }

            if (Session["userid"] == null)
            {
                Response.Redirect("login.aspx");
                return;
            }

            txt_barcode.Attributes["autocomplete"] = "off";
            txt_barcode.Attributes["onkeydown"] = "return shippingGoodsBarcodeKeyDown(event);";
            ApplyAutoDeliveryNoState();

            if (!IsPostBack)
            {
                InitializePage();
                LoadShippingGoods();
            }

            ApplyPrintedEditModeState();
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

        protected void chk_AutoDeliveryNo_CheckedChanged(object sender, EventArgs e)
        {
            ApplyAutoDeliveryNoState();
        }

        private void InitializePage()
        {
            txt_SupplierCode.Text = CheryPortConfig.SupplNo;
            txt_StackLayerCount.Text = "1";
            string todayAtMidnight = FormatDateTimeLocal(DateTime.Today);
            txt_ProductionDate.Text = todayAtMidnight;
            txt_InspectionConfirmDate.Text = todayAtMidnight;
            ApplyBarcodeDrivenReadOnlyState();
            ApplyEditModeState();
        }

        private void ApplyBarcodeDrivenReadOnlyState()
        {
            SetTextBoxReadOnly(txt_SupplierCode, true);
            SetTextBoxReadOnly(txt_PartNo, true);
            SetTextBoxReadOnly(txt_Quantity, true);
            SetTextBoxReadOnly(txt_SupplyBatchNo, true);
            SetTextBoxReadOnly(txt_StackLayerCount, false);
            SetTextBoxReadOnly(txt_ProductionDate, false);
        }

        private void ApplyAutoDeliveryNoState()
        {
            SetTextBoxReadOnly(txt_DeliveryNo, chk_AutoDeliveryNo.Checked);
        }

        private static void SetTextBoxReadOnly(TextBox textBox, bool isReadOnly)
        {
            textBox.ReadOnly = isReadOnly;
            string cssClass = (textBox.CssClass ?? string.Empty).Trim();
            const string readOnlyClass = "shipping-goods-readonly";
            bool hasReadOnlyClass = cssClass.Contains(readOnlyClass);
            if (isReadOnly)
            {
                if (!hasReadOnlyClass)
                {
                    textBox.CssClass = (cssClass + " " + readOnlyClass).Trim();
                }
                return;
            }

            if (hasReadOnlyClass)
            {
                textBox.CssClass = cssClass.Replace(readOnlyClass, string.Empty).Trim();
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

        private bool IsEditMode()
        {
            if (string.Equals(Request.QueryString["func"], "edit", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            int id;
            return int.TryParse(Request.QueryString["id"], out id) && id > 0;
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
            txt_DeliveryNo.Text = SafeValue(shippingGoodsInfo.DeliveryNo);
            txt_OrderQuantity.Text = GetExistingOrderQuantityText(shippingGoodsInfo);
            txt_StackLayerCount.Text = shippingGoodsInfo.StackLayerCount.HasValue ? shippingGoodsInfo.StackLayerCount.Value.ToString() : string.Empty;
            txt_ProductionDate.Text = FormatDateTimeLocal(shippingGoodsInfo.ProductionDate);
            txt_InspectionConfirmDate.Text = FormatDateTimeLocal(shippingGoodsInfo.InspectionConfirmDate);
            txt_BoxCount.Text = GetBoxCountTextFromCartonNo(shippingGoodsInfo.CartonNo);
            txt_SingleBoxGrossWeight.Text = shippingGoodsInfo.SingleBoxGrossWeight.HasValue
                ? shippingGoodsInfo.SingleBoxGrossWeight.Value.ToString("0.##")
                : string.Empty;
        }

        private static string GetExistingOrderQuantityText(ShippingGoodsInfo shippingGoodsInfo)
        {
            if (shippingGoodsInfo == null ||
                string.IsNullOrWhiteSpace(shippingGoodsInfo.PartNo) ||
                string.IsNullOrWhiteSpace(shippingGoodsInfo.SupplyBatchNo) ||
                string.IsNullOrWhiteSpace(shippingGoodsInfo.DeliveryNo))
            {
                return shippingGoodsInfo != null && shippingGoodsInfo.Quantity.HasValue
                    ? shippingGoodsInfo.Quantity.Value.ToString()
                    : string.Empty;
            }

            List<ShippingGoodsInfo> existingInfos = new ShippingGoods().GetShippingGoods(
                shippingGoodsInfo.PartNo,
                string.Empty,
                shippingGoodsInfo.SupplyBatchNo);
            int orderQuantity = existingInfos == null
                ? 0
                : existingInfos
                    .Where(item => item != null &&
                        string.Equals((item.DeliveryNo ?? string.Empty).Trim(), shippingGoodsInfo.DeliveryNo.Trim(), StringComparison.OrdinalIgnoreCase))
                    .Sum(item => item.Quantity ?? 0);

            return orderQuantity > 0 ? orderQuantity.ToString() : string.Empty;
        }

        private void ApplyEditModeState()
        {
            if (!IsEditMode())
            {
                return;
            }

            txt_BoxCount.Text = GetBoxCountTextFromCartonNo(hid_CartonNo.Value);
            SetTextBoxReadOnly(txt_BoxCount, false);
        }

        private void ParseBarcode()
        {
            string originalProductionDateText = txt_ProductionDate.Text;
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
            txt_PartChineseName.Text = string.Empty;
            txt_SingleBoxGrossWeight.Text = string.Empty;
            ShippingGoodsInfo existingPartInfo = FindExistingPartInfo(scannedShippingGoodsInfo.PartNo);
            if (existingPartInfo != null)
            {
                txt_PartChineseName.Text = SafeValue(existingPartInfo.PartChineseName);
                txt_SingleBoxGrossWeight.Text = existingPartInfo.SingleBoxGrossWeight.HasValue
                    ? existingPartInfo.SingleBoxGrossWeight.Value.ToString("0.##", CultureInfo.InvariantCulture)
                    : string.Empty;
            }
            txt_Quantity.Text = scannedShippingGoodsInfo.Quantity.HasValue ? scannedShippingGoodsInfo.Quantity.Value.ToString() : string.Empty;
            txt_SupplyBatchNo.Text = SafeValue(scannedShippingGoodsInfo.SupplyBatchNo);
            txt_DeliveryNo.Text = SafeValue(scannedShippingGoodsInfo.DeliveryNo);
            txt_StackLayerCount.Text = scannedShippingGoodsInfo.StackLayerCount.HasValue ? scannedShippingGoodsInfo.StackLayerCount.Value.ToString() : "1";
            txt_ProductionDate.Text = scannedShippingGoodsInfo.ProductionDate.HasValue
                ? FormatDateTimeLocal(scannedShippingGoodsInfo.ProductionDate)
                : originalProductionDateText;
        }

        private void ClearQrDrivenFields()
        {
            txt_PartNo.Text = string.Empty;
            txt_Quantity.Text = string.Empty;
            txt_SupplyBatchNo.Text = string.Empty;
            txt_DeliveryNo.Text = string.Empty;
            txt_StackLayerCount.Text = "1";
            txt_ProductionDate.Text = string.Empty;
            txt_BoxCount.Text = string.Empty;
            hid_QrCode.Value = string.Empty;
            ApplyBarcodeDrivenReadOnlyState();
            ApplyEditModeState();
        }

        private bool IsPrintedEditMode()
        {
            int printCount;
            return IsEditMode() &&
                ((int.TryParse(hid_PrintCount.Value, out printCount) && printCount > 0) ||
                 string.Equals(hid_Status.Value, ShippingGoodsStatusInfo.Printed, StringComparison.OrdinalIgnoreCase));
        }

        private void ApplyPrintedEditModeState()
        {
            if (!IsPrintedEditMode())
            {
                return;
            }

            TextBox[] readOnlyTextBoxes =
            {
                txt_barcode, txt_SupplierCode, txt_PartNo, txt_PartChineseName, txt_PartEnglishName,
                txt_Quantity, txt_OrderQuantity, txt_SupplyBatchNo, txt_StackLayerCount,
                txt_ProductionDate, txt_InspectionConfirmDate, txt_BoxCount, txt_SingleBoxGrossWeight
            };
            foreach (TextBox textBox in readOnlyTextBoxes)
            {
                SetTextBoxReadOnly(textBox, true);
            }

            SetTextBoxReadOnly(txt_DeliveryNo, false);
            chk_AutoDeliveryNo.Checked = false;
            chk_AutoDeliveryNo.Enabled = false;
        }

        private static ShippingGoodsInfo FindExistingPartInfo(string partNo)
        {
            string normalizedPartNo = SafeValue(partNo);
            if (string.IsNullOrWhiteSpace(normalizedPartNo))
            {
                return null;
            }

            List<ShippingGoodsInfo> candidates = new ShippingGoods()
                .GetShippingGoods(normalizedPartNo, string.Empty, string.Empty);

            return candidates == null
                ? null
                : candidates.Find(item => item != null &&
                    string.Equals(SafeValue(item.PartNo), normalizedPartNo, StringComparison.OrdinalIgnoreCase));
        }

        private void SaveShippingGoods()
        {
            if (IsPrintedEditMode())
            {
                if (string.IsNullOrWhiteSpace(txt_DeliveryNo.Text))
                {
                    ShowMessage("配送单号不能为空。");
                    return;
                }

                string printedSaveMessage = new ShippingGoods().UpdatePrintedDeliveryNo(
                    txt_SupplyBatchNo.Text.Trim(), txt_PartNo.Text.Trim(), hid_CartonNo.Value,
                    GetOriginalDeliveryNo(), txt_DeliveryNo.Text.Trim());
                ShowMessage(string.IsNullOrWhiteSpace(printedSaveMessage) ? "配送单号保存成功。" : printedSaveMessage);
                return;
            }

            if (chk_AutoDeliveryNo.Checked && !IsEditMode())
            {
                try
                {
                    txt_DeliveryNo.Text = new ShippingGoods().GetNextAutoDeliveryNo(DateTime.Now);
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message);
                    return;
                }
            }

            string validateMessage;
            if (!ValidateInput(out validateMessage))
            {
                ShowMessage(validateMessage);
                return;
            }

            ShippingGoods shippingGoods = new ShippingGoods();
            int orderQuantity;
            int boxQuantity;
            int boxCount;
            string quantityMessage;
            if (!TryGetGenerationQuantities(out orderQuantity, out boxQuantity, out boxCount, out quantityMessage))
            {
                ShowMessage(quantityMessage);
                return;
            }

            if (IsEditMode())
            {
                List<ShippingGoodsInfo> editShippingGoodsInfos = shippingGoods.BuildNewShippingGoodsInfos(
                    BuildShippingGoodsTemplate(null), orderQuantity, boxQuantity, boxCount);
                string editSaveMessage = shippingGoods.SaveViewEditShippingGoods(
                    txt_SupplyBatchNo.Text.Trim(),
                    txt_PartNo.Text.Trim(),
                    editShippingGoodsInfos);
                ShowMessage(string.IsNullOrWhiteSpace(editSaveMessage)
                    ? string.Format("保存成功，共生成 {0} 条数据。", editShippingGoodsInfos.Count)
                    : editSaveMessage);
                return;
            }

            List<ShippingGoodsInfo> shippingGoodsInfos = shippingGoods.BuildNewShippingGoodsInfos(
                BuildShippingGoodsTemplate(null), orderQuantity, boxQuantity, boxCount);
            ShippingGoodsInfo firstShippingGoodsInfo = shippingGoodsInfos.Count == 0 ? null : shippingGoodsInfos[0];
            ShippingGoodsInfo duplicateKeyInfo = shippingGoodsInfos.Count == 0 ? null : shippingGoodsInfos[0];
            if (duplicateKeyInfo != null)
            {
                List<ShippingGoodsInfo> duplicateInfos = shippingGoods.GetShippingGoods(
                    duplicateKeyInfo.PartNo,
                    string.Empty,
                    duplicateKeyInfo.SupplyBatchNo);
                bool isDuplicate = duplicateInfos.Any(existingInfo =>
                    existingInfo != null &&
                    string.Equals((existingInfo.PartNo ?? string.Empty).Trim(), (duplicateKeyInfo.PartNo ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals((existingInfo.SupplyBatchNo ?? string.Empty).Trim(), (duplicateKeyInfo.SupplyBatchNo ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals((existingInfo.DeliveryNo ?? string.Empty).Trim(), (duplicateKeyInfo.DeliveryNo ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase));
                if (isDuplicate)
                {
                    ShowMessage(string.Format("零件编号“{0}”、配送单号“{1}”、供货批次号“{2}”的出货货品已存在。", duplicateKeyInfo.PartNo, duplicateKeyInfo.DeliveryNo, duplicateKeyInfo.SupplyBatchNo));
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
            CheckRequiredField(txt_OrderQuantity.Text, "订单数量", missingFields);
            CheckRequiredField(txt_SupplyBatchNo.Text, "供货批次号", missingFields);
            CheckRequiredField(txt_DeliveryNo.Text, "配送单号", missingFields);
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

            int orderQuantity;
            if (!int.TryParse(txt_OrderQuantity.Text.Trim(), out orderQuantity) || orderQuantity <= 0)
            {
                message = "订单数量必须为大于 0 的整数。";
                return false;
            }

            int stackLayerCount;
            if (!int.TryParse(txt_StackLayerCount.Text.Trim(), out stackLayerCount))
            {
                message = "码放层数必须为整数。";
                return false;
            }

            int boxCount;
            string boxCountMessage;
            if (!TryGetBoxCount(out boxCount, out boxCountMessage))
            {
                message = boxCountMessage;
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

        private string GetOriginalDeliveryNo()
        {
            int id;
            if (!int.TryParse(hid_Id.Value, out id))
            {
                return txt_DeliveryNo.Text.Trim();
            }

            ShippingGoodsInfo info = new ShippingGoods().GetShippingGoods().Find(item => item != null && item.Id == id);
            return info == null ? txt_DeliveryNo.Text.Trim() : SafeValue(info.DeliveryNo);
        }

        private bool TryGetGenerationQuantities(
            out int orderQuantity,
            out int boxQuantity,
            out int boxCount,
            out string message)
        {
            orderQuantity = 0;
            boxQuantity = 0;
            boxCount = 0;
            message = string.Empty;

            if (!int.TryParse(txt_OrderQuantity.Text.Trim(), out orderQuantity) || orderQuantity <= 0)
            {
                message = "订单数量必须为大于 0 的整数。";
                return false;
            }

            if (!int.TryParse(txt_Quantity.Text.Trim(), out boxQuantity) || boxQuantity <= 0)
            {
                message = "数量必须为大于 0 的整数。";
                return false;
            }

            if (!TryGetBoxCount(out boxCount, out message))
            {
                return false;
            }

            if (boxCount > 1 && orderQuantity <= boxQuantity * (boxCount - 1))
            {
                message = "订单数量不足以生成指定的纸箱数量。";
                return false;
            }

            return true;
        }

        private ShippingGoodsInfo GetCurrentShippingGoodsForEdit()
        {
            int id;
            if (!int.TryParse(hid_Id.Value, out id))
            {
                return null;
            }

            return new ShippingGoods().GetShippingGoods().Find(item => item != null && item.Id == id);
        }

        private List<ShippingGoodsInfo> BuildEditShippingGoodsInfos(ShippingGoodsInfo currentShippingGoodsInfo)
        {
            int boxCount;
            string boxCountMessage;
            if (!TryGetBoxCount(out boxCount, out boxCountMessage))
            {
                throw new ApplicationException(boxCountMessage);
            }

            ShippingGoodsInfo shippingGoodsInfo = BuildShippingGoodsTemplate(currentShippingGoodsInfo);
            shippingGoodsInfo.Id = currentShippingGoodsInfo.Id;
            shippingGoodsInfo.CartonNo = BuildEditedCartonNo(currentShippingGoodsInfo.CartonNo, boxCount);
            shippingGoodsInfo.Status = SafeValue(currentShippingGoodsInfo.Status);
            shippingGoodsInfo.PrintCount = currentShippingGoodsInfo.PrintCount;
            shippingGoodsInfo.Creater = SafeValue(currentShippingGoodsInfo.Creater);
            shippingGoodsInfo.CreatDate = currentShippingGoodsInfo.CreatDate;
            return new List<ShippingGoodsInfo> { shippingGoodsInfo };
        }

        private ShippingGoodsInfo BuildShippingGoodsTemplate(ShippingGoodsInfo currentShippingGoodsInfo)
        {
            int printCount;
            decimal singleBoxGrossWeight;
            DateTime productionDate;
            DateTime inspectionConfirmDate;

            string userName = Session["userid"] == null ? string.Empty : Session["userid"].ToString().Trim();
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
                DeliveryNo = txt_DeliveryNo.Text.Trim(),
                StackLayerCount = Convert.ToInt32(txt_StackLayerCount.Text.Trim()),
                ProductionDate = productionDate,
                InspectionConfirmDate = inspectionConfirmDate,
                CartonNo = SafeValue(hid_CartonNo.Value),
                QrCode = string.Empty,
                OutBoxQRCode = SafeValue(hid_QrCode.Value),
                Status = currentShippingGoodsInfo == null
                    ? (string.IsNullOrWhiteSpace(hid_Status.Value) ? ShippingGoodsStatusInfo.UnPrinted : hid_Status.Value.Trim())
                    : SafeValue(currentShippingGoodsInfo.Status),
                PrintCount = currentShippingGoodsInfo == null
                    ? (int.TryParse(hid_PrintCount.Value, out printCount) ? printCount : 0)
                    : currentShippingGoodsInfo.PrintCount,
                Creater = currentShippingGoodsInfo == null ? userName : SafeValue(currentShippingGoodsInfo.Creater),
                CreatDate = currentShippingGoodsInfo == null ? DateTime.Now : currentShippingGoodsInfo.CreatDate
            };

            if (!string.IsNullOrWhiteSpace(txt_SingleBoxGrossWeight.Text))
            {
                if (decimal.TryParse(txt_SingleBoxGrossWeight.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out singleBoxGrossWeight) ||
                    decimal.TryParse(txt_SingleBoxGrossWeight.Text.Trim(), out singleBoxGrossWeight))
                {
                    templateShippingGoodsInfo.SingleBoxGrossWeight = singleBoxGrossWeight;
                }
            }

            return templateShippingGoodsInfo;
        }

        private bool TryGetBoxCount(out int boxCount, out string message)
        {
            boxCount = 0;
            message = string.Empty;
            if (!int.TryParse(txt_BoxCount.Text.Trim(), out boxCount) || boxCount <= 0)
            {
                message = "纸箱数量必须为大于 0 的整数。";
                return false;
            }

            return true;
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

            string script = string.Format(
                "showMessageModal('{0}');",
                HttpUtility.JavaScriptStringEncode(SafeValue(message)));
            ClientScript.RegisterStartupScript(GetType(), "ShippingGoodsViewMessage", script, true);
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static string GetBoxCountTextFromCartonNo(string cartonNo)
        {
            int boxCount;
            string safeCartonNo = SafeValue(cartonNo);
            int separatorIndex = safeCartonNo.IndexOf('-');
            if (separatorIndex <= 0)
            {
                return string.Empty;
            }

            string prefix = safeCartonNo.Substring(0, separatorIndex).Trim();
            return int.TryParse(prefix, out boxCount) && boxCount > 0
                ? boxCount.ToString()
                : string.Empty;
        }

        private static string BuildEditedCartonNo(string cartonNo, int boxCount)
        {
            string safeCartonNo = SafeValue(cartonNo);
            int separatorIndex = safeCartonNo.IndexOf('-');
            if (separatorIndex <= 0)
            {
                return boxCount.ToString();
            }

            return boxCount.ToString() + safeCartonNo.Substring(separatorIndex);
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









