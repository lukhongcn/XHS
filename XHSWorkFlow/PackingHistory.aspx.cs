using System;
using System.Collections.Generic;
using System.Web.UI;
using BLL;
using ModuleWorkFlow.BLL;
using XHS.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>按 KD 标签查询装箱任务及扫描明细。</summary>
    public partial class PackingHistory : Page
    {
        private const string MenuId = "B122";
        protected string menuname = string.Empty;
        private readonly PackingRecord packingRecordService = new PackingRecord();
        private readonly PackingOperationService packingService = new PackingOperationService();

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

            BindPackingRecord(record);
            List<PackingScanRecordInfo> scanRecords = packingService.GetScanRecordsByPackingId(record.Id.Value);
            gvScanRecords.DataSource = scanRecords ?? new List<PackingScanRecordInfo>();
            gvScanRecords.DataBind();
            pnlPackingRecord.Visible = true;
            pnlScanRecords.Visible = true;
            ShowMessage(string.Empty);
        }

        protected void btn_Clear_Click(object sender, EventArgs e)
        {
            txt_KDQRCode.Text = string.Empty;
            ClearResult();
            ShowMessage(string.Empty);
        }

        protected void lnk_back_Click(object sender, EventArgs e) { Response.Redirect("Packing.aspx"); }

        private void LoadLatestPackingRecord()
        {
            PackingRecordInfo record = packingRecordService.GetLatestPackingRecord();
            if (record == null || !record.Id.HasValue || record.Id.Value <= 0)
            {
                ShowMessage("暂无装箱记录。");
                return;
            }

            txt_KDQRCode.Text = record.KDQRCode ?? string.Empty;
            BindPackingRecord(record);
            List<PackingScanRecordInfo> scanRecords = packingService.GetScanRecordsByPackingId(record.Id.Value);
            gvScanRecords.DataSource = scanRecords ?? new List<PackingScanRecordInfo>();
            gvScanRecords.DataBind();
            pnlPackingRecord.Visible = true;
            pnlScanRecords.Visible = true;
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
            pnlScanRecords.Visible = false;
            gvScanRecords.DataSource = new List<PackingScanRecordInfo>();
            gvScanRecords.DataBind();
        }

        private void ShowMessage(string message) { Label_Message.Text = Server.HtmlEncode(message ?? string.Empty); }
    }
}
