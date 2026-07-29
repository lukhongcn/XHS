using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using BLL;
using CheryPortHelp;
using XHS.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>配送单上传页面。扫描 KD 标签和配送单，核对一致后上传。</summary>
    public partial class DeliveryUpload : Page
    {
        protected string menuname = "配送单上传";
        private readonly PackingOperationService packingService = new PackingOperationService();

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

        // 缓存 KD 对应的 PackingId 和原始 KD 码（用于接口校验）
        private long KdPackingId
        {
            get
            {
                object val = ViewState["KdPackingId"];
                return val != null ? (long)val : 0L;
            }
            set { ViewState["KdPackingId"] = value; }
        }

        private string KdRawBarcode
        {
            get { return ViewState["KdRawBarcode"] as string ?? string.Empty; }
            set { ViewState["KdRawBarcode"] = value; }
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

            // 查找该 KD 对应的装箱记录，检查 PackingStage 必须是装箱完成
            PackingOperationResult stateResult = packingService.GetPackingStateByKdQRCode(qrCode);
            if (stateResult.PackingRecord == null)
            {
                ShowMessage("该 KD 标签尚未创建装箱任务，请先在装箱页面中完成装箱。");
                return;
            }

            string stage = stateResult.PackingRecord.PackingStage ?? string.Empty;
            if (stage != PackingStageInfo.装箱完成.Status)
            {
                string stageName = string.IsNullOrWhiteSpace(stage)
                    ? "未开始" : PackingStageInfo.GetStatusName(stage);
                ShowMessage("该 KD 标签的装箱阶段为" + stageName + "，必须为" + PackingStageInfo.装箱完成.StatusName + "后才能上传配送单。");

                return;
            }

            KdInfo = parsedInfo;
            KdPackingId = stateResult.PackingRecord.Id ?? 0L;
            KdRawBarcode = qrCode;
            BindKdInfo(parsedInfo);

            // KD 扫描成功，切到配送单
            rblScanType.SelectedValue = "Packing";

            // 清除旧配送单和核验结果
            ClearDeliveryDisplay();
            txt_DeliveryMatch.Text = string.Empty;

            RecheckMatch();
            ShowMessage("KD 标签扫描成功，装箱已完成，请扫描配送单。");
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

        // ----- 条码解析辅助 -----

        private static string Extract10Value(string barcode)
        {
            return ExtractKeyValue(barcode, "10#");
        }

        private static int? Extract17Quantity(string barcode)
        {
            string val = ExtractKeyValue(barcode, "17#");
            if (val == null) return null;
            int qty;
            return int.TryParse(val, out qty) ? qty : (int?)null;
        }

        private static string ExtractKeyValue(string barcode, string key)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return null;
            int start = barcode.IndexOf(key, StringComparison.Ordinal);
            if (start < 0) return null;
            start += key.Length;
            int end = barcode.IndexOf("$", start, StringComparison.Ordinal);
            if (end < 0) return null;
            return barcode.Substring(start, end - start);
        }

        private enum BarcodeFormat { Single, MinPack, Unknown }

        private static BarcodeFormat DetectBarcodeFormat(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return BarcodeFormat.Unknown;
            bool has10 = barcode.Contains("10#");
            bool has11 = barcode.Contains("11#");
            bool has12 = barcode.Contains("12#");
            bool has17 = barcode.Contains("17#");
            bool endsDY = barcode.TrimEnd().EndsWith("DY");
            if (!has10 || !has11 || !has12) return BarcodeFormat.Unknown;
            if (has17 && endsDY) return BarcodeFormat.MinPack;
            if (!has17 && !endsDY) return BarcodeFormat.Single;
            return BarcodeFormat.Unknown;
        }

        // ----- 接口校验 -----

        /// <summary>按平台规则校验。返回 null 表示通过，否则返回错误消息。</summary>
        private string ValidateUploadRequest(
            string packageBarCode,
            string materialNo,
            int packCount,
            List<PackingScanRecordInfo> scanRecords)
        {
            // 1. 箱内物料数量与包装流水号中 17# 数量一致
            int? qty17 = Extract17Quantity(packageBarCode);
            if (!qty17.HasValue || qty17.Value != packCount)
                return "箱内物料数量与包装流水号中17#数量不一致";

            if (scanRecords == null || scanRecords.Count == 0)
                return null; // 无明细时不校验 2a-2c

            // 2a. 检查全部物料流水号格式统一
            BarcodeFormat? format = null;
            foreach (var r in scanRecords)
            {
                BarcodeFormat fmt = DetectBarcodeFormat(r.QRCode);
                if (fmt == BarcodeFormat.Unknown)
                    return "非标准产品标签，请检查";
                if (format == null)
                    format = fmt;
                else if (format.Value != fmt)
                    return "箱内明细不允许单件、最小包装混装";
            }

            // 2b. 每个物料流水号中 10# 与 materialNo 一致
            foreach (var r in scanRecords)
            {
                string part10 = Extract10Value(r.QRCode);
                if (!string.Equals(materialNo, part10, StringComparison.OrdinalIgnoreCase))
                    return "箱内明细物料与箱码物料不一致";
            }

            // 2c. 箱内明细数量之和与 packCount 一致
            int totalQty;
            if (format == BarcodeFormat.Single)
                totalQty = scanRecords.Count;
            else
            {
                totalQty = 0;
                foreach (var r in scanRecords)
                {
                    int? q = Extract17Quantity(r.QRCode);
                    if (q.HasValue) totalQty += q.Value;
                }
            }
            if (totalQty != packCount)
                return "箱内物料数量与箱内明细数量之和不一致";

            return null;
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
                // 1. 获取装箱扫描明细（仅零件标签）
                List<PackingScanRecordInfo> scanRecords = KdPackingId > 0
                    ? packingService.GetScanRecordsByPackingId(KdPackingId)
                        .FindAll(r => r.QRCodeType == PackingScanRecordQRCodeTypeInfo.MaterialLabel)
                    : new List<PackingScanRecordInfo>();

                string materialNo = KdInfo.PartNo;
                int packCount = KdInfo.Quantity ?? 0;
                string packageBarCode = KdRawBarcode;

                // 2. 接口校验
                string validateError = ValidateUploadRequest(packageBarCode, materialNo, packCount, scanRecords);
                if (validateError != null)
                {
                    txt_DeliveryMatch.Text = validateError;
                    ShowMessage(validateError);
                    return;
                }

                // 3. 组装上传信息
                var uploadInfo = new CheryUploadInfo
                {
                    DeliveryNo = KdInfo.SupplyBatchNo,
                    SxCardSeq = DeliveryInfo.PackingCardNo,
                    MaterialNo = materialNo,
                    MaterialName = string.Empty,
                    PackingCount = packCount.ToString(),
                    PackageType = CheryPortConfig.PackageType,
                    PackageBarCode = packageBarCode,
                    PackageCode = DeliveryInfo.PackageCode,
                    PackageName = string.Empty,
                    PackingDate = DateTime.Now,
                    CheckTime = DateTime.Now,
                    CheckUserName = GetUserName(),
                    PackingDetails = scanRecords.ConvertAll(r => new PackingDetailInfo
                    {
                        materialBarCode = r.QRCode,
                        materialNo = r.MaterialNo,
                        materialName = string.Empty,
                        createTime = (r.ScanTime ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"),
                        createName = r.ScanUser
                    })
                };

                // 4. 构建请求并发送
                CheryCheckRecordRequest request = CheryRequestBuilder.BuildCheckRecordRequest(uploadInfo);
                CheryPostResult postResult = new CheryHttpClient().PostCheckRecordRaw(request);

                if (postResult == null || postResult.Response == null)
                {
                    ShowMessage("平台上传失败：无法连接到海行云平台。");
                    return;
                }

                if (postResult.Response.code != 200)
                {
                    ShowMessage(string.Format("平台返回失败：code={0}, msg={1}",
                        postResult.Response.code, postResult.Response.msg ?? string.Empty));
                    return;
                }

                // 5. 平台成功后写入 ShippingGoods
                ShippingGoodsInfo record = new ShippingGoodsInfo
                {
                    SupplyBatchNo = KdInfo.SupplyBatchNo,
                    PartNo = KdInfo.PartNo,
                    CartonNo = KdInfo.CartonNo,
                    Quantity = packCount,
                    SupplierCode = KdInfo.SupplierCode,
                    PackingCardNo = DeliveryInfo.PackingCardNo,
                    PackageCode = DeliveryInfo.PackageCode,
                    ExSupplyBatchNo = DeliveryInfo.ExSupplyBatchNo,
                    Status = ShippingGoodsStatusInfo.UnPrinted,
                    Creater = GetUserName(),
                    CreatDate = DateTime.Now
                };

                string saveMessage = new ShippingGoods().InsertShippingGoods(
                    new List<ShippingGoodsInfo> { record });

                if (string.IsNullOrWhiteSpace(saveMessage))
                {
                    ShowMessage("上传成功，code=" + postResult.Response.code);
                    BindInitialState();
                    KdInfo = null;
                    DeliveryInfo = null;
                    KdPackingId = 0;
                    KdRawBarcode = null;
                }
                else
                {
                    ShowMessage("平台上传成功，但本地保存失败：" + saveMessage);
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
