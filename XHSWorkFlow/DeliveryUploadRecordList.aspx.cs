using System;
using System.Collections.Generic;
using System.Web.UI;
using BLL;
using ModuleWorkFlow.BLL;
using XHS.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>按 KD 标签查询配送单平台上传记录。</summary>
    public partial class DeliveryUploadRecordList : Page
    {
        private const string MenuId = "B124";
        protected string menuname = string.Empty;
        private readonly PackingRecord packingRecordService = new PackingRecord();
        private readonly DeliveryUploadRecord uploadRecordService = new DeliveryUploadRecord();

        protected void Page_Load(object sender, EventArgs e)
        {
            menuname = new PartTmenu().findbykey(MenuId).Menuname;
            if (Master is DefaultSub master) { master.Menuname = menuname; }
            if (!Private.checkPrivate(this, MenuId, "PQUERY")) return;
            if (Session["userid"] == null) { Response.Redirect("login.aspx"); return; }
            if (!IsPostBack) LoadLatestPackingRecord();
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            string kdQRCode = (txt_KDQRCode.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(kdQRCode)) { ClearResult(); ShowMessage("请输入KD标签。"); return; }

            PackingRecordInfo record = packingRecordService.GetPackingRecordByKdQRCode(kdQRCode);
            if (record == null || !record.Id.HasValue || record.Id.Value <= 0)
            {
                ClearResult();
                ShowMessage("未找到该KD标签对应的装箱记录。");
                return;
            }

            BindResult(record);
            ShowMessage(string.Empty);
        }

        protected void btn_Clear_Click(object sender, EventArgs e)
        {
            txt_KDQRCode.Text = string.Empty;
            ClearResult();
            ShowMessage(string.Empty);
        }

        protected void lnk_back_Click(object sender, EventArgs e) { Response.Redirect("DeliveryUploadPDA.aspx"); }

        private void LoadLatestPackingRecord()
        {
            PackingRecordInfo record = packingRecordService.GetLatestPackingRecord();
            if (record == null || !record.Id.HasValue || record.Id.Value <= 0)
            {
                ShowMessage("暂无装箱记录。");
                return;
            }

            txt_KDQRCode.Text = record.KDQRCode ?? string.Empty;
            BindResult(record);
        }

        private void BindResult(PackingRecordInfo record)
        {
            BindPackingRecord(record);
            List<DeliveryUploadRecordInfo> records = uploadRecordService.GetUploadRecords(record.Id.Value);
            List<DeliveryUploadRecordView> views = new List<DeliveryUploadRecordView>();
            foreach (DeliveryUploadRecordInfo item in records ?? new List<DeliveryUploadRecordInfo>())
            {
                views.Add(new DeliveryUploadRecordView(item));
            }

            gvUploadRecords.DataSource = views;
            gvUploadRecords.DataBind();
            pnlPackingRecord.Visible = true;
            pnlUploadRecords.Visible = true;
        }

        private void BindPackingRecord(PackingRecordInfo record)
        {
            litSupplyBatchNo.Text = Server.HtmlEncode(record.SupplyBatchNo ?? string.Empty);
            litPartNo.Text = Server.HtmlEncode(record.PartNo ?? string.Empty);
            litCartonNo.Text = Server.HtmlEncode(record.CartonNo ?? string.Empty);
            litKDQRCode.Text = Server.HtmlEncode(record.KDQRCode ?? string.Empty);
            litPlanQty.Text = record.PlanQty.HasValue ? record.PlanQty.Value.ToString() : string.Empty;
            litPackingQty.Text = record.PackingQty.HasValue ? record.PackingQty.Value.ToString() : "0";
            litPackingStage.Text = Server.HtmlEncode(GetPackingStageName(record));
            litCreateUser.Text = Server.HtmlEncode(record.CreateUser ?? string.Empty);
            litCreateTime.Text = record.CreateTime.HasValue ? record.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
        }

        private static string GetPackingStageName(PackingRecordInfo record)
        {
            if (record.Status.HasValue && record.Status.Value == 1) return "已完成";
            if (record.ExceptionStatus.HasValue && record.ExceptionStatus.Value != 0) return "异常锁定";
            return string.IsNullOrWhiteSpace(record.PackingStage) ? "装箱中" : record.PackingStage;
        }

        private void ClearResult()
        {
            pnlPackingRecord.Visible = false;
            pnlUploadRecords.Visible = false;
            gvUploadRecords.DataSource = new List<DeliveryUploadRecordView>();
            gvUploadRecords.DataBind();
        }

        private void ShowMessage(string message) { Label_Message.Text = Server.HtmlEncode(message ?? string.Empty); }

        private sealed class DeliveryUploadRecordView
        {
            private readonly DeliveryUploadRecordInfo record;

            public DeliveryUploadRecordView(DeliveryUploadRecordInfo record) { this.record = record ?? new DeliveryUploadRecordInfo(); }
            public string DeliveryNo { get { return Safe(record.DeliveryNo); } }
            public string PartNo { get { return Safe(record.PartNo); } }
            public string UploadStatusText { get { return GetStatusText(record.UploadStatus); } }
            public string ResponseCodeText { get { return record.ResponseCode.HasValue ? record.ResponseCode.Value.ToString() : string.Empty; } }
            public string ResponseMessage { get { return Safe(record.ResponseMessage); } }
            public string UploadUser { get { return Safe(record.UploadUser); } }
            public string UploadTimeText { get { return FormatDateTime(record.UploadTime); } }
            public string CompletedTimeText { get { return FormatDateTime(record.CompletedTime); } }
            public string ErrorMessage { get { return Safe(record.ErrorMessage); } }

            private static string Safe(string value) { return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim(); }
            private static string FormatDateTime(DateTime? value) { return value.HasValue ? value.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty; }
            private static string GetStatusText(string value)
            {
                if (string.Equals(value, DeliveryUploadStatusInfo.Pending, StringComparison.OrdinalIgnoreCase)) return "待上传";
                if (string.Equals(value, DeliveryUploadStatusInfo.Success, StringComparison.OrdinalIgnoreCase)) return "成功";
                if (string.Equals(value, DeliveryUploadStatusInfo.Failed, StringComparison.OrdinalIgnoreCase)) return "失败";
                return Safe(value);
            }
        }
    }
}
