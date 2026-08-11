using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using BLL;
using XHS.BLL;
using XHS.Model;
using ModuleWorkFlow.BLL;

namespace ModuleWorkFlow
{
    /// <summary>装箱扫描页面。</summary>
    public partial class Packing : Page
    {
        private const string MenuId = "B12";
        protected string menuname = "";
        private readonly PackingOperationService packingService = new PackingOperationService();
        private string currentPackingStage = string.Empty;
        private bool currentPackingLocked;
        private long currentApprovedRepackScanId;

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
            txt_ScanQRCode.Attributes["onkeydown"] = "return packingScanKeyDown(event);";
            txt_RepackQRCode.Attributes["autocomplete"] = "off";
            txt_RepackQRCode.Attributes["onkeydown"] = "return packingRepackKeyDown(event);";
            if (!IsPostBack) { LoadPackingRecord(); }
        }

        protected void lnk_view_Click(object sender, EventArgs e) { Response.Redirect("Packing.aspx"); }
        protected void txt_ScanQRCode_TextChanged(object sender, EventArgs e) { ProcessScan(); }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static PackingPageStatusResponse GetPackingPageStatus(string taskId, string lockToken)
        {
            HttpContext context = HttpContext.Current;
            Packing page = context == null ? null : context.Handler as Packing;
            if (context == null || context.Session["userid"] == null || page == null || !Private.checkPrivate(page, MenuId, "PEDIT"))
            {
                return PackingPageStatusResponse.Fail("当前用户没有装箱编辑权限，请重新登录或联系管理员。");
            }

            Guid parsedTaskId;
            if (!Guid.TryParse(SafeValue(taskId), out parsedTaskId))
            {
                return PackingPageStatusResponse.Fail("装箱任务编号无效。");
            }

            PackingOperationService service = new PackingOperationService();
            PackingOperationResult result = service.GetPackingState(parsedTaskId);
            PackingRecordInfo record = result.PackingRecord;
            if (record == null || !record.Id.HasValue)
            {
                return PackingPageStatusResponse.Fail("装箱任务不存在。");
            }

            bool isLocked = record.ExceptionStatus.HasValue && record.ExceptionStatus.Value != 0;
            bool tokenChanged = !string.Equals(SafeValue(record.LockToken), SafeValue(lockToken), StringComparison.Ordinal);
            return PackingPageStatusResponse.Ok(
                isLocked,
                !isLocked || tokenChanged,
                service.GetApprovedRepackTargetScanId(record.Id.Value));
        }

        protected void gvScanRecords_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "Repack", StringComparison.OrdinalIgnoreCase)) return;
            long scanRecordId;
            if (!long.TryParse(SafeValue(Convert.ToString(e.CommandArgument)), out scanRecordId) || scanRecordId <= 0)
            {
                ShowMessage("重新装箱目标明细无效。");
                return;
            }

            long packingId = GetRequiredPackingId();
            if (packingId <= 0) return;

            PackingOperationResult state = packingService.GetPackingState(GetTaskId());
            if (state.PackingRecord == null)
            {
                ShowMessage("装箱任务不存在，请重新进入页面。");
                return;
            }

            BindPackingRecord(state.PackingRecord);
            currentApprovedRepackScanId = packingService.GetApprovedRepackTargetScanId(packingId);
            if (currentPackingLocked)
            {
                ShowMessage("重新装箱申请已提交，请等待审核。");
                return;
            }
            if (string.Equals(currentPackingStage, PackingStageInfo.上传完成.Status, StringComparison.OrdinalIgnoreCase))
            {
                ShowMessage("该装箱记录已经上传，不允许重新装箱。");
                return;
            }

            if (currentApprovedRepackScanId > 0)
            {
                if (currentApprovedRepackScanId != scanRecordId)
                {
                    ShowMessage("请在本次审核通过的目标明细上执行重新装箱。");
                    return;
                }

                hidRepackScanId.Value = scanRecordId.ToString();
                txt_RepackQRCode.Text = string.Empty;
                ClientScript.RegisterStartupScript(GetType(), "PackingRepackModal", "window.setTimeout(function(){showPackingRepackModal();}, 0);", true);
                return;
            }

            PackingOperationResult result = packingService.RequestRepack(
                packingId, scanRecordId, GetUserName(), Environment.MachineName);
            if (result.PackingRecord != null)
            {
                BindPackingRecord(result.PackingRecord);
                BindScanRecords(result.PackingRecord);
            }
            ShowMessage(result.Message);
        }

        protected void lnkbutton_repack_save_Click(object sender, EventArgs e)
        {
            long packingId = GetRequiredPackingId();
            long scanRecordId;
            string qrCode = SafeValue(txt_RepackQRCode.Text);
            if (packingId <= 0) return;
            if (!long.TryParse(SafeValue(hidRepackScanId.Value), out scanRecordId) || scanRecordId <= 0)
            {
                ShowMessage("重新装箱目标明细无效。");
                return;
            }
            if (string.IsNullOrWhiteSpace(qrCode))
            {
                ShowMessage("请输入或扫描产品二维码。");
                ClientScript.RegisterStartupScript(GetType(), "PackingRepackModal", "window.setTimeout(function(){showPackingRepackModal();}, 0);", true);
                return;
            }

            PackingOperationResult result = packingService.ExecuteApprovedRepack(
                packingId, scanRecordId, SafeValue(hidLockToken.Value), qrCode, GetUserName());
            if (result.PackingRecord != null)
            {
                BindPackingRecord(result.PackingRecord);
                BindScanRecords(result.PackingRecord);
            }
            if (!result.Success)
            {
                hidRepackScanId.Value = scanRecordId.ToString();
                ClientScript.RegisterStartupScript(GetType(), "PackingRepackModal", "window.setTimeout(function(){showPackingRepackModal();}, 0);", true);
            }
            ShowMessage(result.Message);
        }

        private void LoadPackingRecord()
        {
            Guid taskId = GetTaskId();
            if (taskId == Guid.Empty)
            {
                BindWaitingForKdState();
                return;
            }

            PackingOperationResult result = packingService.GetPackingState(taskId);
            if (result.PackingRecord != null)
            {
                BindPackingRecord(result.PackingRecord);
                BindScanRecords(result.PackingRecord);
                if (result.Status == "LOCK")
                {
                    SetLockMessage(result.PackingRecord, result.Message);
                    ShowMessage(result.Message);
                }
                else if (result.Status == "COMPLETED")
                {
                    ShowMessage(result.Message);
                }
            }
            else
            {
                BindWaitingForKdState();
                ShowMessage("装箱任务不存在，请重新扫描 KD 标签。");
            }
        }

        private void BindEmptyScanRecords() { gvScanRecords.DataSource = new List<PackingScanRecordInfo>(); gvScanRecords.DataBind(); }

        private void BindWaitingForKdState()
        {
            BindEmptyScanRecords();
            currentApprovedRepackScanId = 0L;
            hidPackingId.Value = string.Empty;
            hidLockToken.Value = string.Empty;
            hidPackingCompleteMode.Value = string.Empty;
            txt_LockToken.Text = string.Empty;
            txt_Status.Text = "等待 KD 标签";
            txt_ExceptionStatus.Text = "正常";
            txt_Qty.Text = "0";
            txt_ScanQRCode.Enabled = true;
            txt_MaterialNo.Enabled = false;
            txt_Qty.Enabled = false;
            btn_packing_complete.Enabled = false;
            pnlLocked.Style["display"] = "none";
            SetScanTypeState(false);
        }

        private void ProcessScan()
        {
            string qrCode = SafeValue(txt_ScanQRCode.Text);
            txt_ScanQRCode.Text = string.Empty;
            if (string.IsNullOrWhiteSpace(qrCode)) { return; }

            try
            {
                if (GetTaskId() == Guid.Empty && GetPackingId() <= 0 && rblScanType.SelectedValue != "KD")
                {
                    ShowMessage("请先扫描KD标签，确定当前装箱任务。");
                }
                else if (rblScanType.SelectedValue == "KD") { ScanKdQRCode(qrCode); }
                else if (rblScanType.SelectedValue == "Packing") { ScanPackingQRCode(qrCode); }
                else { ScanMaterialQRCode(qrCode); }
            }
            catch (Exception ex)
            {
                LogException(ex);
                ShowMessage("操作失败：" + ex.Message);
            }
        }

        private void ScanKdQRCode(string qrCode)
        {
            ShippingGoodsInfo parsedInfo = new FactoryBarcodeParser().ParseShippingGoodsBarcode(qrCode, "XHSFZKD");
            if (parsedInfo == null ||
                string.IsNullOrWhiteSpace(parsedInfo.PartNo) ||
                string.IsNullOrWhiteSpace(parsedInfo.SupplyBatchNo) ||
                string.IsNullOrWhiteSpace(parsedInfo.CartonNo) ||
                !parsedInfo.Quantity.HasValue || parsedInfo.Quantity.Value <= 0)
            {
                ShowMessage("KD 标签解析失败，缺少零件号、供货批次号、箱号或数量。");
                return;
            }

            txt_SupplyBatchNo.Text = parsedInfo.SupplyBatchNo;
            txt_PartNo.Text = parsedInfo.PartNo;
            txt_CartonNo.Text = parsedInfo.CartonNo;
            txt_PlanQty.Text = parsedInfo.Quantity.Value.ToString();

            // 校验 KD 标签是否在 tb_ShippingGoods 中已存在
            if (!CheckShippingGoodsExists(parsedInfo.SupplyBatchNo, parsedInfo.PartNo, parsedInfo.CartonNo))
            {
                ShowMessage(string.Format("该 KD 标签在出货单中不存在：批次{0}，零件{1}，箱号{2}，不允许建立装箱任务。",
                    parsedInfo.SupplyBatchNo, parsedInfo.PartNo, parsedInfo.CartonNo));
                return;
            }

            int planQty = parsedInfo.Quantity.Value;
            PackingOperationResult result = packingService.OpenOrCreateByKd(
                parsedInfo.SupplyBatchNo,
                parsedInfo.PartNo,
                parsedInfo.CartonNo,
                planQty,
                qrCode,
                GetUserName());

            if (result.PackingRecord != null && result.PackingRecord.TaskId.HasValue)
            {
                // 新创建的任务 → 跳转带上 taskId
                if (result.Message != null && result.Message.Contains("已创建"))
                {
                    RedirectToTask(result.PackingRecord);
                    return;
                }

                // 已有任务 → 进入 HandleResult，由 SetScanTypeState 根据 PackingStage 自动选中
                HandleResult(result);
                return;
            }

            ShowMessage(result.Message);
        }

        private void ScanPackingQRCode(string qrCode)
        {
            long packingId = GetRequiredPackingId();
            if (packingId <= 0) return;

            // 配送单：LabelCodeRule 解码(XHSFZPS) → 核对零件/批号/数量 → 匹配则完成
            PackingOperationResult result = packingService.ScanDeliveryNote(
                packingId,
                SafeValue(hidLockToken.Value),
                qrCode,
                GetUserName());

            HandleResult(result);
        }

        private void ScanMaterialQRCode(string qrCode)
        {
            long packingId = GetRequiredPackingId();
            if (packingId <= 0) return;

            // 解析零件标签二维码
            ShippingGoodsInfo parsedInfo = new FactoryBarcodeParser().ParseShippingGoodsBarcode(qrCode, "XHSFZPart");
            if (parsedInfo == null || string.IsNullOrWhiteSpace(parsedInfo.PartNo))
            {
                ShowMessage("零件标签解析失败，无法识别零件号。");
                return;
            }

            // 每次扫描零件标签，装箱数量自动加 1
            int qty = 1;

            // 自动填入解析到的物料号
            txt_MaterialNo.Text = parsedInfo.PartNo;

            PackingOperationResult result = packingService.ScanMaterialQRCode(
                packingId,
                SafeValue(hidLockToken.Value),
                qrCode,
                parsedInfo.PartNo,
                qty,
                GetUserName());

            HandleResult(result);
        }

        private void HandleResult(PackingOperationResult result)
        {
            // 更新 Token
            if (!string.IsNullOrWhiteSpace(result.NewToken))
            {
                hidLockToken.Value = result.NewToken;
            }

            // 根据状态处理
            if (result.PackingRecord != null)
            {
                BindPackingRecord(result.PackingRecord);
                BindScanRecords(result.PackingRecord);
                if (result.Status == "LOCK")
                {
                    SetLockMessage(result.PackingRecord, result.Message);
                }
            }

            if (result.Status == "LOCK")
            {
                // 异常锁定，冻结页面
                ShowMessage(result.Message);
                return;
            }

            if (result.Status == "STALE")
            {
                // Token 过期，禁止继续操作
                txt_ScanQRCode.Enabled = false;
                    btn_packing_complete.Enabled = false;
                ShowMessage(result.Message);
                return;
            }

            if (result.Status == "COMPLETED")
            {
                ShowMessage(result.Message);
                return;
            }

            ShowMessage(result.Message);
        }

        private Guid GetTaskId()
        {
            Guid taskId;
            return Guid.TryParse(SafeValue(Request.QueryString["taskId"]), out taskId) ? taskId : Guid.Empty;
        }

        private long GetPackingId()
        {
            Guid taskId = GetTaskId();
            if (taskId != Guid.Empty)
            {
                PackingOperationResult result = packingService.GetPackingState(taskId);
                return result.PackingRecord != null && result.PackingRecord.Id.HasValue ? result.PackingRecord.Id.Value : 0L;
            }

            long packingId;
            return long.TryParse(SafeValue(hidPackingId.Value), out packingId) && packingId > 0 ? packingId : 0L;
        }

        private long GetRequiredPackingId()
        {
            long packingId = GetPackingId();
            if (packingId <= 0)
            {
                ShowMessage("请先扫描 KD 标签创建装箱任务。");
            }
            return packingId;
        }

        private void BindPackingRecord(PackingRecordInfo record)
        {
            currentPackingStage = SafeValue(record.PackingStage);
            currentPackingLocked = record.ExceptionStatus.HasValue && record.ExceptionStatus.Value != 0;
            hidPackingId.Value = record.Id.HasValue ? record.Id.Value.ToString() : string.Empty;
            txt_SupplyBatchNo.Text = SafeValue(record.SupplyBatchNo);
            txt_PartNo.Text = SafeValue(record.PartNo);
            txt_CartonNo.Text = SafeValue(record.CartonNo);
            hidLockToken.Value = SafeValue(record.LockToken);
            txt_LockToken.Text = SafeValue(record.LockToken);
            txt_PlanQty.Text = record.PlanQty.HasValue ? record.PlanQty.Value.ToString() : string.Empty;
            txt_Qty.Text = record.PackingQty.HasValue ? record.PackingQty.Value.ToString() : "0";
            txt_Status.Text = !string.IsNullOrWhiteSpace(record.PackingStage)
                ? PackingStageInfo.GetStatusName(record.PackingStage)
                : (record.Status == 1 ? "已完成" : "装箱中");
            txt_ExceptionStatus.Text = record.ExceptionStatus.HasValue && record.ExceptionStatus.Value != 0 ? "异常暂停" : "正常";

            bool locked = currentPackingLocked;
            bool completed = record.Status.HasValue && record.Status.Value == 1;
            pnlLocked.Visible = true;
            pnlLocked.Style["display"] = locked ? "flex !important" : "none";
            Label_LockMessage.Text = locked
                ? string.Format("KD标签：{0}；箱号：{1}；该箱已锁定，等待主管审核解除。", SafeValue(record.KDQRCode), SafeValue(record.CartonNo))
                : string.Empty;
            txt_ScanQRCode.Enabled = !locked && !completed;
            txt_MaterialNo.Enabled = !locked && !completed;
            txt_Qty.Enabled = !locked && !completed;
            SetScanTypeState(locked || completed, record.PackingStage);
            if (locked)
            {
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "PackingStatusPolling",
                    "window.setTimeout(startPackingStatusPolling, 1500);",
                    true);
            }
        }

        private void SetScanTypeState(bool lockedOrCompleted, string packingStage = null)
        {
            var kdItem = rblScanType.Items.FindByValue("KD");
            var packingItem = rblScanType.Items.FindByValue("Packing");
            var materialItem = rblScanType.Items.FindByValue("Material");
            bool hasActiveTask = GetTaskId() != Guid.Empty || GetPackingId() > 0;

            if (lockedOrCompleted)
            {
                // 锁定或已完成：全部禁用
                if (kdItem != null) kdItem.Enabled = false;
                if (packingItem != null) packingItem.Enabled = false;
                if (materialItem != null) materialItem.Enabled = false;
                btn_packing_complete.Enabled = false;
            }
            else if (!hasActiveTask)
            {
                // 无任务：仅 KD 标签启用
                if (kdItem != null) kdItem.Enabled = true;
                if (packingItem != null) packingItem.Enabled = false;
                if (materialItem != null) materialItem.Enabled = false;
                btn_packing_complete.Enabled = false;
                rblScanType.SelectedValue = "KD";
            }
            else if (packingStage == PackingStageInfo.装箱完成.Status)
            {
                // 装箱完成阶段：仅配送单启用
                if (kdItem != null) kdItem.Enabled = false;
                if (packingItem != null) packingItem.Enabled = true;
                if (materialItem != null) materialItem.Enabled = false;
                btn_packing_complete.Enabled = false;
                rblScanType.SelectedValue = "Packing";
            }
            else
            {
                // 装箱中：仅零件标签启用
                if (kdItem != null) kdItem.Enabled = false;
                if (packingItem != null) packingItem.Enabled = false;
                if (materialItem != null) materialItem.Enabled = true;
                btn_packing_complete.Enabled = true;
                rblScanType.SelectedValue = "Material";
            }

            UpdateRadioStyles();
        }

        private void UpdateRadioStyles()
        {
            ClientScript.RegisterStartupScript(
                GetType(),
                "PackingRadioStyles",
                "updatePackingRadioStyles();",
                true);
        }

        protected void btn_packing_complete_Click(object sender, EventArgs e)
        {
            long packingId = GetRequiredPackingId();
            if (packingId <= 0) return;

            PackingOperationResult result = packingService.UpdatePackingStage(
                packingId,
                SafeValue(hidLockToken.Value),
                PackingStageInfo.装箱完成.Status,
                GetUserName());

            HandleResult(result);
        }

        private void RedirectToTask(PackingRecordInfo record)
        {
            if (record == null || !record.TaskId.HasValue)
            {
                ShowMessage("装箱任务没有有效 TaskId。");
                return;
            }

            Response.Redirect("Packing.aspx?taskId=" + record.TaskId.Value, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void SetLockMessage(PackingRecordInfo record, string reason)
        {
            if (record == null)
            {
                return;
            }

            Label_LockMessage.Text = string.Format(
                "KD标签：{0}；箱号：{1}；原因：{2}",
                SafeValue(record.KDQRCode),
                SafeValue(record.CartonNo),
                SafeValue(reason));
        }

        private void BindScanRecords(PackingRecordInfo record)
        {
            currentApprovedRepackScanId = record != null && record.Id.HasValue
                ? packingService.GetApprovedRepackTargetScanId(record.Id.Value)
                : 0L;
            if (currentApprovedRepackScanId > 0)
            {
                ApplyApprovedRepackState();
                hidRepackScanId.Value = currentApprovedRepackScanId.ToString();
            }
            List<PackingScanRecordInfo> records = record != null && record.Id.HasValue
                ? packingService.GetScanRecordsByPackingId(record.Id.Value)
                : new List<PackingScanRecordInfo>();
            gvScanRecords.DataSource = records ?? new List<PackingScanRecordInfo>();
            gvScanRecords.DataBind();
        }

        private void ApplyApprovedRepackState()
        {
            txt_ExceptionStatus.Text = "重新装箱待执行";
            txt_ScanQRCode.Enabled = false;
            txt_MaterialNo.Enabled = false;
            txt_Qty.Enabled = false;
            btn_packing_complete.Enabled = false;
            foreach (System.Web.UI.WebControls.ListItem item in rblScanType.Items)
            {
                item.Enabled = false;
            }
            UpdateRadioStyles();
        }

        protected string GetRepackLinkText(object scanRecordId)
        {
            long rowScanId;
            return long.TryParse(SafeValue(Convert.ToString(scanRecordId)), out rowScanId) && rowScanId == currentApprovedRepackScanId
                ? "重新装箱"
                : "申请";
        }

        protected string GetRepackLinkToolTip(object scanRecordId)
        {
            return GetRepackLinkText(scanRecordId) == "重新装箱" ? "重新装箱" : "申请重新装箱审核";
        }

        protected string GetRepackClientClick(object scanRecordId)
        {
            long rowScanId;
            if (!long.TryParse(SafeValue(Convert.ToString(scanRecordId)), out rowScanId) || rowScanId != currentApprovedRepackScanId)
            {
                return string.Empty;
            }
            return "return openApprovedRepackModal(" + rowScanId + ");";
        }

        protected bool CanUseRepackLink(object scanRecordId)
        {
            if (currentPackingLocked || string.Equals(currentPackingStage, PackingStageInfo.上传完成.Status, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            long rowScanId;
            if (!long.TryParse(SafeValue(Convert.ToString(scanRecordId)), out rowScanId)) return false;
            return currentApprovedRepackScanId <= 0 || currentApprovedRepackScanId == rowScanId;
        }

        protected bool IsMaterialScanRecord(object qrCodeType)
        {
            return string.Equals(SafeValue(Convert.ToString(qrCodeType)), PackingScanRecordQRCodeTypeInfo.MaterialLabel, StringComparison.OrdinalIgnoreCase);
        }

        private static bool CheckShippingGoodsExists(string supplyBatchNo, string partNo, string cartonNo)
        {
            try
            {
                List<ShippingGoodsInfo> list = new ShippingGoods()
                    .GetShippingGoodsByBusinessKey(supplyBatchNo, partNo, cartonNo);
                return list != null && list.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        private string GetUserName() { return SafeValue(Session["userid"] == null ? string.Empty : Session["userid"].ToString()); }

        private void ShowMessage(string message)
        {
            Label_Message.Text = SafeValue(message);
            ClientScript.RegisterStartupScript(GetType(), "PackingMessage", string.Format("showMessageModal('{0}');", HttpUtility.JavaScriptStringEncode(SafeValue(message))), true);
        }

        private static string SafeValue(string value) { return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim(); }

        private static void LogException(Exception ex)
        {
            try
            {
                Utility.Log.WriteLog("Packing.aspx.log", string.Format("Error: {0}\r\nStack: {1}", ex.Message, ex.StackTrace));
            }
            catch { }
        }

        protected override void OnInit(EventArgs e) { InitializeComponent(); base.OnInit(e); }
        private void InitializeComponent() { Load += new EventHandler(Page_Load); }
    }

    /// <summary>装箱页面 AJAX 状态查询结果。</summary>
    public sealed class PackingPageStatusResponse
    {
        public bool Success { get; set; }
        public bool IsLocked { get; set; }
        public bool ShouldReload { get; set; }
        public long ApprovedRepackScanId { get; set; }
        public string Message { get; set; }

        public static PackingPageStatusResponse Ok(bool isLocked, bool shouldReload, long approvedRepackScanId)
        {
            return new PackingPageStatusResponse
            {
                Success = true,
                IsLocked = isLocked,
                ShouldReload = shouldReload,
                ApprovedRepackScanId = approvedRepackScanId,
                Message = string.Empty
            };
        }

        public static PackingPageStatusResponse Fail(string message)
        {
            return new PackingPageStatusResponse
            {
                Success = false,
                IsLocked = true,
                ShouldReload = false,
                ApprovedRepackScanId = 0L,
                Message = message ?? string.Empty
            };
        }
    }
}
