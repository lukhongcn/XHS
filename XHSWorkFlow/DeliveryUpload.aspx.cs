using System;
using System.Web;
using System.Web.UI;
using BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>配送单上传页面。扫描 KD 标签和配送单，核对一致后上传。</summary>
    public partial class DeliveryUpload : Page
    {
        protected string menuname = "配送单上传";

        // 缓存 KD 解析结果
        private ShippingGoodsInfo KdInfo
        {
            get { return ViewState["KdInfo"] as ShippingGoodsInfo; }
            set { ViewState["KdInfo"] = value; }
        }

        // 缓存配送单解析结果
        private ShippingGoodsInfo DeliveryInfo
        {
            get { return ViewState["DeliveryInfo"] as ShippingGoodsInfo; }
            set { ViewState["DeliveryInfo"] = value; }
        }

        private void Page_Load(object sender, EventArgs e)
        {
            if (Master is DefaultSub master) { master.Menuname = menuname; }
            txt_ScanQRCode.Attributes["autocomplete"] = "off";
            txt_ScanQRCode.Attributes["onkeydown"] = "return deliveryScanKeyDown(event);";
            if (!IsPostBack) { BindInitialState(); }
        }

        protected void lnk_view_Click(object sender, EventArgs e) { Response.Redirect("DeliveryUpload.aspx"); }
        protected void txt_ScanQRCode_TextChanged(object sender, EventArgs e) { ProcessScan(); }
        protected void lnkbutton_upload_Click(object sender, EventArgs e) { UploadDelivery(); }

        private void BindInitialState()
        {
            ClearKdDisplay();
            ClearDeliveryDisplay();
            txt_DeliveryMatch.Text = string.Empty;
            lnkbutton_upload.Enabled = false;
        }

        private void ProcessScan()
        {
            string qrCode = SafeValue(txt_ScanQRCode.Text);
            txt_ScanQRCode.Text = string.Empty;
            if (string.IsNullOrWhiteSpace(qrCode)) { return; }

            try
            {
                if (rblScanType.SelectedValue == "KD")
                {
                    ScanKd(qrCode);
                }
                else
                {
                    ScanDelivery(qrCode);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("操作失败：" + ex.Message);
            }
        }

        private void ScanKd(string qrCode)
        {
            ShippingGoodsInfo parsedInfo = new FactoryBarcodeParser()
                .ParseShippingGoodsBarcode(qrCode, "XHSFZKD");
            if (parsedInfo == null ||
                string.IsNullOrWhiteSpace(parsedInfo.PartNo) ||
                string.IsNullOrWhiteSpace(parsedInfo.SupplyBatchNo) ||
                string.IsNullOrWhiteSpace(parsedInfo.CartonNo) ||
                !parsedInfo.Quantity.HasValue || parsedInfo.Quantity.Value <= 0)
            {
                ShowMessage("KD 标签解析失败，缺少零件号、供货批次号、箱号或数量。");
                return;
            }

            KdInfo = parsedInfo;
            BindKdInfo(parsedInfo);

            // KD 扫描成功后自动切到配送单
            rblScanType.SelectedValue = "Packing";

            // 清除旧配送单和核验结果
            ClearDeliveryDisplay();
            txt_DeliveryMatch.Text = string.Empty;

            RecheckMatch();
            ShowMessage("KD 标签扫描成功，请扫描配送单。");
        }

        private void ScanDelivery(string qrCode)
        {
            if (KdInfo == null)
            {
                ShowMessage("请先扫描 KD 标签。");
                return;
            }

            ShippingGoodsInfo parsedInfo = new FactoryBarcodeParser()
                .ParseShippingGoodsBarcode(qrCode, "XHSFZPS");
            if (parsedInfo == null ||
                string.IsNullOrWhiteSpace(parsedInfo.PartNo) ||
                string.IsNullOrWhiteSpace(parsedInfo.SupplyBatchNo) ||
                !parsedInfo.Quantity.HasValue)
            {
                ShowMessage("配送单解析失败，缺少零件号、供货批次号或数量。");
                return;
            }

            DeliveryInfo = parsedInfo;
            BindDeliveryInfo(parsedInfo);

            RecheckMatch();
        }

        private void RecheckMatch()
        {
            if (KdInfo == null || DeliveryInfo == null)
            {
                lnkbutton_upload.Enabled = false;
                return;
            }

            // 核对零件编号
            if (!string.Equals(KdInfo.PartNo, DeliveryInfo.PartNo, StringComparison.OrdinalIgnoreCase))
            {
                txt_DeliveryMatch.Text = string.Format("零件不匹配：KD={0}，配送单={1}", KdInfo.PartNo, DeliveryInfo.PartNo);
                lnkbutton_upload.Enabled = false;
                return;
            }

            // 核对供货批次号
            if (!string.Equals(KdInfo.SupplyBatchNo, DeliveryInfo.SupplyBatchNo, StringComparison.OrdinalIgnoreCase))
            {
                txt_DeliveryMatch.Text = string.Format("批号不匹配：KD={0}，配送单={1}", KdInfo.SupplyBatchNo, DeliveryInfo.SupplyBatchNo);
                lnkbutton_upload.Enabled = false;
                return;
            }

            // 核对数量
            if (KdInfo.Quantity.Value != DeliveryInfo.Quantity.Value)
            {
                txt_DeliveryMatch.Text = string.Format("数量不匹配：KD={0}，配送单={1}", KdInfo.Quantity.Value, DeliveryInfo.Quantity.Value);
                lnkbutton_upload.Enabled = false;
                return;
            }

            txt_DeliveryMatch.Text = "核验通过，零件/批号/数量一致。";
            lnkbutton_upload.Enabled = true;
        }

        private void UploadDelivery()
        {
            if (KdInfo == null || DeliveryInfo == null)
            {
                ShowMessage("请先扫描 KD 标签和配送单。");
                return;
            }

            if (!lnkbutton_upload.Enabled)
            {
                ShowMessage("核验未通过，无法上传。");
                return;
            }

            try
            {
                // 写入 ShippingGoods
                ShippingGoodsInfo record = new ShippingGoodsInfo
                {
                    SupplyBatchNo = KdInfo.SupplyBatchNo,
                    PartNo = KdInfo.PartNo,
                    CartonNo = KdInfo.CartonNo,
                    Quantity = KdInfo.Quantity.Value,
                    SupplierCode = KdInfo.SupplierCode,
                    PackingCardNo = DeliveryInfo.PackingCardNo,
                    PackageCode = DeliveryInfo.PackageCode,
                    ExSupplyBatchNo = DeliveryInfo.ExSupplyBatchNo,
                    Status = ShippingGoodsStatusInfo.UnPrinted,
                    Creater = GetUserName(),
                    CreatDate = DateTime.Now
                };

                string saveMessage = new ShippingGoods().InsertShippingGoods(
                    new System.Collections.Generic.List<ShippingGoodsInfo> { record });

                if (string.IsNullOrWhiteSpace(saveMessage))
                {
                    ShowMessage("上传成功。");
                    BindInitialState();
                    KdInfo = null;
                    DeliveryInfo = null;
                }
                else
                {
                    ShowMessage(saveMessage);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("上传失败：" + ex.Message);
            }
        }

        private void BindKdInfo(ShippingGoodsInfo info)
        {
            txt_KdSupplyBatchNo.Text = SafeValue(info.SupplyBatchNo);
            txt_KdPartNo.Text = SafeValue(info.PartNo);
            txt_KdCartonNo.Text = SafeValue(info.CartonNo);
            txt_KdQty.Text = info.Quantity.HasValue ? info.Quantity.Value.ToString() : string.Empty;
        }

        private void ClearKdDisplay()
        {
            txt_KdSupplyBatchNo.Text = string.Empty;
            txt_KdPartNo.Text = string.Empty;
            txt_KdCartonNo.Text = string.Empty;
            txt_KdQty.Text = string.Empty;
        }

        private void BindDeliveryInfo(ShippingGoodsInfo info)
        {
            txt_DeliverySupplyBatchNo.Text = SafeValue(info.SupplyBatchNo);
            txt_DeliveryPartNo.Text = SafeValue(info.PartNo);
            txt_DeliveryQty.Text = info.Quantity.HasValue ? info.Quantity.Value.ToString() : string.Empty;
            txt_DeliveryCardNo.Text = SafeValue(info.PackingCardNo);
        }

        private void ClearDeliveryDisplay()
        {
            txt_DeliverySupplyBatchNo.Text = string.Empty;
            txt_DeliveryPartNo.Text = string.Empty;
            txt_DeliveryQty.Text = string.Empty;
            txt_DeliveryCardNo.Text = string.Empty;
        }

        private string GetUserName() { return "admin"; }

        private void ShowMessage(string message)
        {
            Label_Message.Text = SafeValue(message);
            ClientScript.RegisterStartupScript(
                GetType(),
                "DeliveryUploadMessage",
                string.Format("showMessageModal('{0}');", HttpUtility.JavaScriptStringEncode(SafeValue(message))),
                true);
        }

        private static string SafeValue(string value) { return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim(); }

        protected override void OnInit(EventArgs e) { InitializeComponent(); base.OnInit(e); }
        private void InitializeComponent() { Load += new EventHandler(Page_Load); }
    }
}
