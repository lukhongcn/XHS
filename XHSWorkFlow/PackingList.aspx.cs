using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using ModuleWorkFlow.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>装箱列表页面，按箱浏览装箱记录并查看扫描明细。</summary>
    public partial class PackingList : Page
    {
        private const string MenuId = "B12";
        protected string menuname = string.Empty;
        private readonly XHS.BLL.PackingRecord packingRecordBll = new XHS.BLL.PackingRecord();

        private void Page_Load(object sender, EventArgs e)
        {
            menuname = new PartTmenu().findbykey(MenuId).Menuname;
            if (Master is DefaultSub master)
            {
                master.Menuname = menuname;
            }

            if (!Private.checkPrivate(this, MenuId, "PQUERY"))
            {
                return;
            }

            if (!IsPostBack)
            {
                BindData();
            }
        }

        protected void lnkbutton_search_Click(object sender, EventArgs e)
        {
            MainDataGrid.CurrentPageIndex = 0;
            BindData();
        }

        protected void lnkbutton_edit_Click(object sender, EventArgs e)
        {
            string kdQRCode = SafeValue(TextBox_KDQRCode.Text);
            if (string.IsNullOrWhiteSpace(kdQRCode))
            {
                ShowMessage("请输入KD码后再进入编辑页面。");
                return;
            }

            PackingRecordInfo record = packingRecordBll.GetPackingRecordByKdQRCode(kdQRCode);
            if (record == null)
            {
                ShowMessage("未找到对应的装箱记录，请确认KD码是否完整。");
                return;
            }

            if (string.Equals(record.PackingStage, PackingStageInfo.上传完成.Status, StringComparison.OrdinalIgnoreCase))
            {
                ShowMessage("该装箱记录已经上传，不允许重新装箱。");
                return;
            }

            if (!record.TaskId.HasValue)
            {
                ShowMessage("该装箱记录没有有效任务编号，无法进入编辑页面。");
                return;
            }

            Response.Redirect("Packing.aspx?taskId=" + record.TaskId.Value, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void MainDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            MainDataGrid.CurrentPageIndex = e.NewPageIndex;
            BindData();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static PackingDetailResponse GetPackingDetail(long packingId)
        {
            HttpContext context = HttpContext.Current;
            PackingList page = context == null ? null : context.Handler as PackingList;
            if (context == null || context.Session["userid"] == null || page == null || !Private.checkPrivate(page, MenuId, "PQUERY"))
            {
                return PackingDetailResponse.Fail("当前用户没有装箱浏览权限，请重新登录或联系管理员。");
            }

            if (packingId <= 0)
            {
                return PackingDetailResponse.Fail("装箱记录编号无效。");
            }

            var recordBll = new XHS.BLL.PackingRecord();
            PackingRecordInfo record = recordBll.GetPackingRecordById(packingId);
            if (record == null)
            {
                return PackingDetailResponse.Fail("装箱记录不存在或已被删除。");
            }

            var scanBll = new XHS.BLL.PackingScanRecord();
            List<PackingScanRecordInfo> scanRecords = scanBll.GetExPackingScanRecords(record.SupplyBatchNo, record.PartNo, record.CartonNo);
            return PackingDetailResponse.From(record, scanRecords);
        }

        protected string GetShortCode(object value)
        {
            string code = Convert.ToString(value);
            if (string.IsNullOrEmpty(code) || code.Length <= 24)
            {
                return code;
            }

            return code.Substring(0, 20) + "...";
        }

        private void BindData()
        {
            List<PackingRecordInfo> records = packingRecordBll.SearchPackingRecords(
                TextBox_KDQRCode.Text,
                TextBox_PartNo.Text,
                TextBox_PartCode.Text);
            MainDataGrid.DataSource = records;
            MainDataGrid.DataBind();
            Label_Message.Text = records.Count == 0 ? "暂无符合条件的装箱记录。" : string.Empty;
        }

        private void ShowMessage(string message)
        {
            string safe = string.IsNullOrWhiteSpace(message) ? string.Empty : message.Trim();
            Label_Message.Text = safe;
            ClientScript.RegisterStartupScript(GetType(), "PackingListMessage",
                string.Format("showMessageModal('{0}');", HttpUtility.JavaScriptStringEncode(safe)), true);
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            Load += new EventHandler(Page_Load);
        }

        public sealed class PackingDetailResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public PackingDetailRecord Record { get; set; }
            public List<PackingDetailScanRecord> ScanRecords { get; set; }

            public static PackingDetailResponse Fail(string message)
            {
                return new PackingDetailResponse { Success = false, Message = message, ScanRecords = new List<PackingDetailScanRecord>() };
            }

            public static PackingDetailResponse From(PackingRecordInfo record, List<PackingScanRecordInfo> scanRecords)
            {
                var response = new PackingDetailResponse
                {
                    Success = true,
                    Record = new PackingDetailRecord
                    {
                        KDQRCode = record.KDQRCode,
                        CartonNo = record.CartonNo,
                        PartNo = record.PartNo,
                        SupplyBatchNo = record.SupplyBatchNo,
                        PlanQty = record.PlanQty,
                        PackingQty = record.PackingQty
                    },
                    ScanRecords = new List<PackingDetailScanRecord>()
                };
                foreach (PackingScanRecordInfo item in scanRecords ?? new List<PackingScanRecordInfo>())
                {
                    response.ScanRecords.Add(new PackingDetailScanRecord
                    {
                        QRCodeType = item.QRCodeType,
                        QRCode = item.QRCode,
                        MaterialNo = item.MaterialNo,
                        Qty = item.Qty,
                        ScanUser = item.ScanUser,
                        ScanTime = item.ScanTime.HasValue ? item.ScanTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty
                    });
                }
                return response;
            }
        }

        public sealed class PackingDetailRecord
        {
            public string KDQRCode { get; set; }
            public string CartonNo { get; set; }
            public string PartNo { get; set; }
            public string SupplyBatchNo { get; set; }
            public int? PlanQty { get; set; }
            public int? PackingQty { get; set; }
        }

        public sealed class PackingDetailScanRecord
        {
            public string QRCodeType { get; set; }
            public string QRCode { get; set; }
            public string MaterialNo { get; set; }
            public int? Qty { get; set; }
            public string ScanUser { get; set; }
            public string ScanTime { get; set; }
        }
    }
}
