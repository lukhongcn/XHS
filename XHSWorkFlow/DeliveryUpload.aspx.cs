using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using BLL;
using CheryPortHelp;
using ModuleWorkFlow.BLL;
using XHS.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>配送单上传页面。扫描 KD 标签和配送单，核对一致后上传。</summary>
    public partial class DeliveryUpload : Page
    {
        private const string MenuId = "B121";
        protected string menuname = "";
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

        // 缓存 ShippingGoods.PackageName（ClearDeliveryDisplay 后再赋值）
        private string PackageNameDefault
        {
            get { return ViewState["PackageNameDefault"] as string ?? string.Empty; }
            set { ViewState["PackageNameDefault"] = value; }
        }

        // KD 对应的零件中文名称
        private string KdPartName
        {
            get { return ViewState["KdPartName"] as string ?? string.Empty; }
            set { ViewState["KdPartName"] = value; }
        }

        private void Page_Load(object sender, EventArgs e)
        {
            menuname = new PartTmenu().findbykey(MenuId).Menuname;
            if (Master is DefaultSub master) { master.Menuname = menuname; }

            if (!Private.checkPrivate(this, MenuId, "PEDIT"))
            {
                return;
            }

            if (Session["userid"] == null)
            {
                Response.Redirect("login.aspx");
                return;
            }

            txt_ScanQRCode.Attributes["autocomplete"] = "off";
            txt_ScanQRCode.Attributes["onkeydown"] = "return deliveryScanKeyDown(event);";
            if (!IsPostBack) { BindInitialState(); }
        }

        protected void lnk_view_Click(object sender, EventArgs e) { Response.Redirect("DeliveryUpload.aspx"); }
        protected void txt_ScanQRCode_TextChanged(object sender, EventArgs e) { ProcessScan(); }
        protected void txt_SxCardSeq_TextChanged(object sender, EventArgs e) { RecheckMatch(); }
        protected void btn_upload_Click(object sender, EventArgs e) { UploadDelivery(); }

        private void BindInitialState()
        {
            ClearKdDisplay();
            ClearDeliveryDisplay();
            txt_DeliveryMatch.Text = string.Empty;
            btn_upload.Enabled = false;
            SetDeliveryRadioState(false);
        }

        /// <summary>hasKd: KD 扫描成功后切到配送单，否则仅 KD 可选。</summary>
        private void SetDeliveryRadioState(bool hasKd)
        {
            var kdItem = rblScanType.Items.FindByValue("KD");
            var packingItem = rblScanType.Items.FindByValue("Packing");
            if (kdItem != null) kdItem.Enabled = !hasKd;
            if (packingItem != null) packingItem.Enabled = hasKd;
            rblScanType.SelectedValue = hasKd ? "Packing" : "KD";
            UpdateRadioStyles();
        }

        private void UpdateRadioStyles()
        {
            ClientScript.RegisterStartupScript(
                GetType(),
                "DeliveryRadioStyles",
                "updateDeliveryRadioStyles();",
                true);
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

            // 1. 从 tb_ShippingGoods 查找零件名称和装箱阶段
            KdPartName = string.Empty;
            string shippingPackingStage = null;
            try
            {
                List<ShippingGoodsInfo> existList = new ShippingGoods()
                    .GetShippingGoodsByBusinessKey(parsedInfo.SupplyBatchNo, parsedInfo.PartNo, parsedInfo.CartonNo);
                if (existList != null && existList.Count > 0)
                {
                    KdPartName = existList[0].PartChineseName ?? string.Empty;
                    shippingPackingStage = existList[0].PackingStage;
                    PackageNameDefault = existList[0].PackageName ?? string.Empty;
                }
                else
                {
                    ShowMessage("该 KD 标签在出货单中不存在，不允许上传配送单。");
                    return;
                }
            }
            catch
            {
                ShowMessage("查询出货单失败，请重试。");
                return;
            }

            // 2. 校验 PackingStage 必须是已完成
            string stage = shippingPackingStage ?? string.Empty;
            if (stage != PackingStageInfo.已完成.Status)
            {
                string stageName = string.IsNullOrWhiteSpace(stage)
                    ? "未开始" : PackingStageInfo.GetStatusName(stage);
                ShowMessage("该 KD 标签的装箱阶段为" + stageName + "，必须为" + PackingStageInfo.已完成.StatusName + "后才能上传配送单。");
                return;
            }

            // 3. 获取 PackingId（供上传时查扫描明细）
            KdPackingId = 0L;
            try
            {
                PackingOperationResult stateResult = packingService.GetPackingStateByKdQRCode(qrCode);
                if (stateResult.PackingRecord != null)
                {
                    KdPackingId = stateResult.PackingRecord.Id ?? 0L;
                }
            }
            catch { }

            KdInfo = parsedInfo;
            KdRawBarcode = qrCode;
            BindKdInfo(parsedInfo);

            // KD 扫描成功，KD 标签灰色禁用，配送单启用
            SetDeliveryRadioState(true);

            // 清除旧配送单和核验结果
            ClearDeliveryDisplay();
            txt_DeliveryPackageName.Text = PackageNameDefault;
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
                btn_upload.Enabled = false;
                return;
            }

            // 核对零件编号
            if (!string.Equals(KdInfo.PartNo, DeliveryInfo.PartNo, StringComparison.OrdinalIgnoreCase))
            {
                txt_DeliveryMatch.Text = string.Format("零件不匹配：KD={0}，配送单={1}", KdInfo.PartNo, DeliveryInfo.PartNo);
                btn_upload.Enabled = false;
                return;
            }

            // 核对供货批次号
            if (!string.Equals(KdInfo.SupplyBatchNo, DeliveryInfo.SupplyBatchNo, StringComparison.OrdinalIgnoreCase))
            {
                txt_DeliveryMatch.Text = string.Format("批号不匹配：KD={0}，配送单={1}", KdInfo.SupplyBatchNo, DeliveryInfo.SupplyBatchNo);
                btn_upload.Enabled = false;
                return;
            }

            // 核对数量
            if (KdInfo.Quantity.Value != DeliveryInfo.Quantity.Value)
            {
                txt_DeliveryMatch.Text = string.Format("数量不匹配：KD={0}，配送单={1}", KdInfo.Quantity.Value, DeliveryInfo.Quantity.Value);
                btn_upload.Enabled = false;
                return;
            }

            txt_DeliveryMatch.Text = "核验通过，零件/批号/数量一致，请填入必填字段后上传。";
            btn_upload.Enabled = true;
        }


        private void UploadDelivery()
        {
            if (KdInfo == null || DeliveryInfo == null)
            {
                ShowMessage("请先扫描 KD 标签和配送单。");
                return;
            }

            // 校验必填字段
            if (string.IsNullOrWhiteSpace(txt_SxCardSeq.Text))
            {
                ShowMessage("请填写随箱卡流水号。");
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_DeliveryPackageName.Text))
            {
                ShowMessage("请填写包装名称。");
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_DeliveryNo.Text))
            {
                ShowMessage("请填写配送单号。");
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_DeliveryPackageCode.Text))
            {
                ShowMessage("请填写包装单编号。");
                return;
            }

            try
            {
                string materialNo = KdInfo.PartNo;
                int packCount = KdInfo.Quantity ?? 0;
                string packageBarCode = KdRawBarcode;

                if (KdPackingId <= 0)
                {
                    ShowMessage("KD码对应的装箱记录不存在，无法上传。");
                    return;
                }

                // 1. 按零件编号获取扫描明细（仅零件标签）
                List<PackingScanRecordInfo> scanRecords = new PackingScanRecord()
                    .GetExPackingScanRecordsByPartNo(materialNo)
                    .FindAll(r => r.QRCodeType == PackingScanRecordQRCodeTypeInfo.MaterialLabel && r.PackingId == KdPackingId);

                // 2. 组装上传信息
                var uploadInfo = new CheryUploadInfo
                {
                    DeliveryNo = SafeValue(txt_DeliveryNo.Text),
                    SxCardSeq = SafeValue(txt_SxCardSeq.Text),
                    MaterialNo = materialNo,
                    MaterialName = SafeValue(txt_KdPartName.Text),
                    PackingCount = packCount.ToString(),
                    PackageType = CheryPortConfig.PackageType,
                    operateType = "1",
                    PackageBarCode = packageBarCode,
                    PackageCode = SafeValue(txt_DeliveryPackageCode.Text),
                    PackageName = SafeValue(txt_DeliveryPackageName.Text),
                    PackingDate = DateTime.Now,
                    CheckTime = DateTime.Now,
                    CheckUserName = GetUserName(),
                    PackingDetails = scanRecords.ConvertAll(r => new PackingDetailInfo
                    {
                        materialBarCode = r.QRCode,
                        materialNo = r.MaterialNo,
                        materialName = SafeValue(txt_KdPartName.Text),
                        createTime = (r.ScanTime ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"),
                        createName = r.ScanUser
                    })
                };

                // 4. 构建请求并发送
                CheryCheckRecordRequest request = CheryRequestBuilder.BuildCheckRecordRequest(uploadInfo);

                // 汇出请求 JSON 到本地 Log 目录（在 HTTP 调用之前保存，确保无论平台是否可达都能留下记录）
                WriteUploadJsonLog(materialNo, CheryHttpClient.SerializeRequest(request));

                CheryPostResult postResult = new CheryHttpClient().PostCheckRecordRaw(request);

                if (postResult == null || postResult.Response == null)
                {
                    ShowMessage("平台上传失败：无法连接到海行云平台。");
                    return;
                }

                string respMsg = string.Format("code={0}, msg={1}",
                    postResult.Response.code, postResult.Response.msg ?? string.Empty);

                // 5. 平台返回 200 时才更新 PackingStage 为上传完成
                if (postResult.Response.code == 200)
                {
                    PackingOperationResult uploadResult = packingService.MarkPackingUploaded(KdPackingId, GetUserName());
                    if (!uploadResult.Success)
                    {
                        ShowMessage("平台上传成功，但状态更新失败：" + uploadResult.Message);
                        return;
                    }

                    respMsg += "，状态已更新为" + ShippingGoodsStatusInfo.Uploaded;
                }

                ShowMessage("平台返回：" + respMsg);
                BindInitialState();
                KdInfo = null;
                DeliveryInfo = null;
                KdPackingId = 0;
                KdRawBarcode = null;
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
            txt_KdPartName.Text = KdPartName;
        }

        private void ClearKdDisplay()
        {
            txt_KdSupplyBatchNo.Text = string.Empty;
            txt_KdPartNo.Text = string.Empty;
            txt_KdCartonNo.Text = string.Empty;
            txt_KdQty.Text = string.Empty;
            txt_KdPartName.Text = string.Empty;
        }

        private void BindDeliveryInfo(ShippingGoodsInfo info)
        {
            txt_DeliverySupplyBatchNo.Text = SafeValue(info.SupplyBatchNo);
            txt_DeliveryPartNo.Text = SafeValue(info.PartNo);
            txt_DeliveryQty.Text = info.Quantity.HasValue ? info.Quantity.Value.ToString() : string.Empty;
            txt_SxCardSeq.Text = SafeValue(info.PackingCardNo);
            txt_DeliveryPackageCode.Text = SafeValue(info.PackageCode);
            // txt_DeliveryPackageName 在 ClearDeliveryDisplay 后赋值
            // txt_DeliveryNo 手工填写，不自动带出
        }

        private void ClearDeliveryDisplay()
        {
            txt_DeliverySupplyBatchNo.Text = string.Empty;
            txt_DeliveryPartNo.Text = string.Empty;
            txt_DeliveryQty.Text = string.Empty;
            txt_DeliveryPackageCode.Text = string.Empty;
            txt_DeliveryPackageName.Text = string.Empty;
            txt_DeliveryNo.Text = string.Empty;
            txt_SxCardSeq.Text = string.Empty;
        }

        private string GetUserName() { return SafeValue(Session["userid"] == null ? string.Empty : Session["userid"].ToString()); }

        private void ShowMessage(string message)
        {
            Label_Message.Text = SafeValue(message);
            ClientScript.RegisterStartupScript(
                GetType(),
                "DeliveryUploadMessage",
                string.Format("showMessageModal('{0}');", HttpUtility.JavaScriptStringEncode(SafeValue(message))),
                true);
        }

        private void WriteUploadJsonLog(string partNo, string requestJson)
        {
            if (string.IsNullOrWhiteSpace(requestJson)) return;

            string safePartNo = string.IsNullOrWhiteSpace(partNo) ? "UnknownPart" : partNo.Trim();
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                safePartNo = safePartNo.Replace(invalidChar.ToString(), string.Empty);

            string logsFolder = Server.MapPath("~/Log");
            Directory.CreateDirectory(logsFolder);

            string fileName = safePartNo + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".json";
            File.WriteAllText(Path.Combine(logsFolder, fileName), requestJson);
        }

        private static string SafeValue(string value) { return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim(); }

        protected override void OnInit(EventArgs e) { InitializeComponent(); base.OnInit(e); }
        private void InitializeComponent() { Load += new EventHandler(Page_Load); }
    }
}
