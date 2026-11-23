using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using XHS.Model;

namespace ModuleWorkFlow
{
    public partial class FXWorkFlow : Page
    {
        private const string ProcessId = "QMJCPD";
        private const string ReadyStatusId = "Ready";
        private const string GridRowsViewStateKey = "FX.GridRows";

        [Serializable]
        private sealed class FxGridRow
        {
            public int RowNumber { get; set; }
            public string PartNoId { get; set; }
            public int? ProcessNo { get; set; }
            public string ReplaceQRCode { get; set; }
            public string ActionCode { get; set; }
        }

        private List<FxGridRow> GridRows
        {
            get
            {
                return ViewState[GridRowsViewStateKey] as List<FxGridRow>
                    ?? new List<FxGridRow>();
            }
            set
            {
                ViewState[GridRowsViewStateKey] = value;
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            if (!IsLoggedIn())
            {
                RedirectToLogin();
                return;
            }

            if (Master is DefaultPDASub)
            {
                ((DefaultPDASub)Master).PageTitle = "FX PDA";
            }
            base.OnPreInit(e);
        }

        private bool IsLoggedIn()
        {
            return Session != null
                && Session["islogin"] is bool
                && (bool)Session["islogin"]
                && !string.IsNullOrWhiteSpace(Convert.ToString(Session["userid"]));
        }

        private void RedirectToLogin()
        {
            string loginUrl = ResolveUrl("~/login.aspx");
            string returnUrl = Request == null ? string.Empty : Request.RawUrl;
            Response.Redirect(
                loginUrl + "?ReturnUrl=" + Server.UrlEncode(returnUrl),
                false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                GridRows = new List<FxGridRow>();
                BindGrid(GridRows);
                labScanState.Text = "等待扫描 QRCode";
            }
        }

        protected void txtQRCode_TextChanged(object sender, EventArgs e)
        {
            string code = NormalizeScan(txtQRCode.Text);
            txtQRCode.Text = string.Empty;
            if (code.Length == 0)
            {
                ShowMessage("请扫描 QRCode。", false);
                return;
            }

            try
            {
                List<XHSPartPartProcessInfo> processes =
                    new XHS.BLL.XHSPartPartProcess()
                        .GetPartProcessesByPartNoIdAndProcessIdAndStatusId(
                            code,
                            ProcessId,
                            ReadyStatusId);
                if (processes == null || processes.Count == 0)
                {
                    hidScannedCode.Value = string.Empty;
                    pnlActionModal.Visible = false;
                    labScanState.Text = "等待扫描 QRCode";
                    ShowMessage("没有找到对应的工序资料！", false);
                    return;
                }

                hidScannedCode.Value = code;
                labMessage.Visible = false;
                labModalMessage.Text = "请选择处理动作";
                labScanState.Text = "请选择处理动作";
                pnlActionModal.Visible = true;
            }
            catch (Exception ex)
            {
                hidScannedCode.Value = string.Empty;
                pnlActionModal.Visible = false;
                labScanState.Text = "等待扫描 QRCode";
                ShowMessage(ex.Message, false);
            }
        }

        protected void ActionButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;

            string actionCode = button.CommandName;
            string scannedCode = hidScannedCode.Value;
            pnlActionModal.Visible = false;
            try
            {
                List<XHSPartPartProcessInfo> processes =
                    new XHS.BLL.XHSPartPartProcess()
                        .GetPartProcessesByPartNoIdAndProcessIdAndStatusId(
                            scannedCode,
                            ProcessId,
                            ReadyStatusId);
                if (processes == null || processes.Count == 0)
                {
                    ShowMessage("没有找到对应的工序资料！", false);
                    labScanState.Text = "等待扫描 QRCode";
                    hidScannedCode.Value = string.Empty;
                    return;
                }

                AppendRecords(processes, actionCode);
                labMessage.Visible = false;
                labScanState.Text = "动作完成，请扫描下一个 QRCode";
                hidScannedCode.Value = string.Empty;
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, false);
                labScanState.Text = "等待扫描 QRCode";
                hidScannedCode.Value = string.Empty;
            }
        }

        private void AppendRecords(
            IList<XHSPartPartProcessInfo> processes,
            string actionCode)
        {
            List<FxGridRow> rows = CaptureGridRows();
            foreach (XHSPartPartProcessInfo process
                in processes ?? new List<XHSPartPartProcessInfo>())
            {
                rows.Add(new FxGridRow
                {
                    RowNumber = rows.Count + 1,
                    PartNoId = process.PartNo_Id,
                    ProcessNo = process.ProcessNo,
                    ReplaceQRCode = process.RelationPartNoId,
                    ActionCode = actionCode
                });
            }
            GridRows = rows;
            BindGrid(rows);
        }

        private List<FxGridRow> CaptureGridRows()
        {
            List<FxGridRow> rows = GridRows;
            int count = Math.Min(rows.Count, gvRecords.Rows.Count);
            for (int index = 0; index < count; index++)
            {
                GridViewRow gridRow = gvRecords.Rows[index];
                TextBox replaceQRCode = gridRow.FindControl("txtReplaceQRCode") as TextBox;
                DropDownList action = gridRow.FindControl("ddlAction") as DropDownList;
                if (replaceQRCode != null) rows[index].ReplaceQRCode = replaceQRCode.Text;
                if (action != null) rows[index].ActionCode = action.SelectedValue;
                rows[index].RowNumber = index + 1;
            }
            return rows;
        }

        private void BindGrid(IList<FxGridRow> rows)
        {
            gvRecords.DataSource = rows;
            gvRecords.DataBind();
        }

        private void ShowMessage(string message, bool success)
        {
            labMessage.Text = message ?? string.Empty;
            labMessage.Visible = true;
            labMessage.CssClass = success
                ? "workflow-status workflow-ok"
                : "workflow-status workflow-error";
        }

        private static string NormalizeScan(string value)
        {
            return (value ?? string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();
        }
    }
}
