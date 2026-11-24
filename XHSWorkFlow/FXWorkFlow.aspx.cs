using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ModuleWorkFlow.Model;
using XHS.Model;

namespace ModuleWorkFlow
{
    public partial class FXWorkFlow : Page
    {
        private const string ProcessId = "QMJCPD";
        private const string RecheckProcessId = "QMJC";
        private const string ReadyStatusId = "Ready";
        private const string ImplementationStatusId = "Implementation";
        private const string PendingStatusId = "Pending";
        private const string RecheckActionCode = "RECHECK";
        private const string ScrapActionCode = "SCRAP";
        private const string PackActionCode = "PACK";
        private const string GridRowsViewStateKey = "FX.GridRows";

        [Serializable]
        private sealed class FxGridRow
        {
            public int RowNumber { get; set; }
            public string PartNoId { get; set; }
            public int? ProcessNo { get; set; }
            public string ReplaceQRCode { get; set; }
            public string ActionCode { get; set; }
            public string ActionText { get; set; }
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

            List<FxGridRow> existingRows = CaptureGridRows();
            if (ContainsQRCode(existingRows, code))
            {
                GridRows = existingRows;
                hidScannedCode.Value = string.Empty;
                pnlActionModal.Visible = false;
                labScanState.Text = "QRCode 已扫入";
                ShowMessage("该 QRCode 已扫入，请勿重复扫描。", false);
                txtQRCode.Focus();
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

        private static bool ContainsQRCode(
            IEnumerable<FxGridRow> rows,
            string code)
        {
            foreach (FxGridRow row in rows ?? new List<FxGridRow>())
            {
                if (row != null && string.Equals(
                    NormalizeScan(row.PartNoId),
                    code,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            List<FxGridRow> rows = CaptureGridRows();
            if (rows.Count == 0)
            {
                ShowMessage("没有需要保存的资料。", false);
                return;
            }

            try
            {
                IList batchSource = new ArrayList();
                XHS.BLL.PLCResult plcResult = new XHS.BLL.PLCResult();
                foreach (FxGridRow row in rows)
                {
                    if (row == null || !row.ProcessNo.HasValue)
                    {
                        throw new InvalidOperationException("待保存资料缺少 ProcessNo。");
                    }

                    List<PartPartProcessInfo> rowProcesses =
                        new XHS.BLL.XHSPartPartProcess()
                            .GetPartProcessInfoListFromProcessNo(
                                NormalizeScan(row.PartNoId), row.ProcessNo.Value);
                    bool shouldProcess;
                    switch (row.ActionCode)
                    {
                        case RecheckActionCode:
                            shouldProcess = ProcessRecheckSource(
                                row, rowProcesses);
                            batchSource.Add(new XHS.BLL.XHSPartPartProcess().UpdatePartProcessFromModifyEdit(rowProcesses));

                            if (NormalizeScan(row.ReplaceQRCode).Length > 0)
                            {
                                batchSource.Add(new XHS.BLL.XHSPartPartProcess()
                                    .UpdatePartNoId(
                                        NormalizeScan(row.PartNoId),
                                        NormalizeScan(row.ReplaceQRCode)));
                            }
                            break;
                        case ScrapActionCode:
                            shouldProcess = ProcessScrapSource(row, rowProcesses);
                            if (shouldProcess)
                            {
                                batchSource.Add(new XHS.BLL.XHSPartPartProcess()
                                    .updateDiscount(row.PartNoId));
                            }
                            break;
                        case PackActionCode:
                            List<PartPartProcessInfo> packSource =
                                new List<PartPartProcessInfo>();
                            HashSet<int> packSourceProcessNos =
                                new HashSet<int>();
                            shouldProcess = ProcessPackSource(
                                row,
                                rowProcesses,
                                packSource,
                                packSourceProcessNos);
                            if (shouldProcess)
                            {
                                batchSource.Add(new XHS.BLL.XHSPartPartProcess()
                                    .UpdatePartProcessFromModifyEdit(packSource));
                            }
                            break;
                        default:
                            throw new InvalidOperationException(
                                "ProcessNo " + row.ProcessNo.Value + " 当前只支持重检、报废、装箱操作。");
                    }
                    if (!shouldProcess)
                    {
                        continue;
                    }
                    batchSource.Add(plcResult.UpdateDealtMethod(
                        row.ProcessNo.Value,
                        row.ActionText));
                }

                ModuleWorkFlow.BLL.Schedule schedule = new ModuleWorkFlow.BLL.Schedule();
                if (!schedule.SaveSchedule(batchSource))
                {
                    throw new InvalidOperationException("没有更新到工序资料。");
                }

                GridRows = new List<FxGridRow>();
                BindGrid(GridRows);
                pnlActionModal.Visible = false;
                hidScannedCode.Value = string.Empty;
                labScanState.Text = "保存完成，请扫描下一个 QRCode";
                ShowMessage("保存成功。", true);
                txtQRCode.Focus();
            }
            catch (Exception ex)
            {
                GridRows = rows;
                BindGrid(rows);
                labScanState.Text = "保存失败";
                ShowMessage(ex.Message, false);
            }
        }

        private static bool ProcessRecheckSource(
            FxGridRow row,
            List<PartPartProcessInfo> processes)
        {
            if (processes == null || processes.Count == 0)
                throw new InvalidOperationException("未找到重新检测的工序资料。");

            PartPartProcessInfo current = processes.Find(item =>
                item != null && item.ProcessNo == row.ProcessNo.Value);
            if (current == null)
                throw new InvalidOperationException(
                    "未找到 ProcessNo " + row.ProcessNo.Value + " 的完整工序资料。");

            string originalPartNoId = NormalizeScan(row.PartNoId);
            string replacementQRCode = NormalizeScan(row.ReplaceQRCode);
            if (originalPartNoId.Length == 0)
                throw new InvalidOperationException("原 PartNo_Id 不能为空。");
            PartPartProcessInfo insertPartProcessInfo = new PartPartProcessInfo
            {
                PartNo_Id = originalPartNoId,
                QRCode = replacementQRCode,
                ProcessId = RecheckProcessId,
                StatusId = ReadyStatusId
            };

            current.StatusId = ImplementationStatusId;

            PartPartProcessInfo originalNext = processes
                .Find(item => item != null && item.ProcessOrder > current.ProcessOrder);
            foreach (PartPartProcessInfo process in processes
                .FindAll(item => item != null && item.ProcessOrder > current.ProcessOrder)
                .OrderByDescending(item => item.ProcessOrder)
                .ThenByDescending(item => item.ProcessNo))
            {
                process.ProcessOrder += 1;
                if (ReferenceEquals(process, originalNext))
                    process.StatusId = PendingStatusId;
            }

            PartPartProcessInfo recheck = CloneProcess(originalNext ?? current);
            recheck.ProcessNo = 0;
            recheck.ProcessId = insertPartProcessInfo.ProcessId;
            recheck.ProcessOrder = current.ProcessOrder + 1;
            recheck.StatusId = insertPartProcessInfo.StatusId;
            processes.Add(recheck);
            return true;
        }

        private static PartPartProcessInfo CloneProcess(PartPartProcessInfo source)
        {
            return new PartPartProcessInfo
            {
                ModuleId = source.ModuleId,
                PartNo = source.PartNo,
                PartNo_Id = source.PartNo_Id,
                ProcessId = source.ProcessId,
                ProcessNeedMinutes = source.ProcessNeedMinutes,
                Priority = source.Priority,
                StartDate = source.StartDate,
                SelfStartDate = source.SelfStartDate,
                SelfEndDate = source.SelfEndDate,
                isConfirm = source.isConfirm,
                ProcessComment = source.ProcessComment,
                ProcessMachineId = source.ProcessMachineId,
                IsUnnormal = source.IsUnnormal,
                UnNormalReasonsId = source.UnNormalReasonsId,
                DepartMentId = source.DepartMentId,
                NeedProduct = source.NeedProduct,
                CustomerProcessId = source.CustomerProcessId,
                RelationPartNo = source.RelationPartNo,
                RelationPartNoId = source.RelationPartNoId,
                SameProcessNo = source.SameProcessNo,
                Ispcnc = source.Ispcnc
            };
        }

        private static bool ProcessPackSource(
            FxGridRow row,
            List<PartPartProcessInfo> processes,
            ICollection<PartPartProcessInfo> source,
            ISet<int> sourceProcessNos)
        {
            PartPartProcessInfo currentProcess = processes.Find(item =>
                item != null && item.ProcessNo == row.ProcessNo.Value);
            if (currentProcess == null)
                return false;

            PartPartProcessInfo nextProcess = processes
                .FindAll(item => item != null && item.ProcessOrder > currentProcess.ProcessOrder)
                .OrderBy(item => item.ProcessOrder)
                .ThenBy(item => item.ProcessNo)
                .FirstOrDefault();
            if (nextProcess == null)
                return false;

            currentProcess.StatusId = "Implementation";
            nextProcess.StatusId = ReadyStatusId;
            AddUpdateSource(source, sourceProcessNos, currentProcess);
            AddUpdateSource(source, sourceProcessNos, nextProcess);
            return true;
        }

        private static bool ProcessScrapSource(
            FxGridRow row,
            List<PartPartProcessInfo> processes)
        {
            PartPartProcessInfo currentProcess = processes.Find(item =>
                item != null && item.ProcessNo == row.ProcessNo.Value);
            if (currentProcess == null)
                throw new InvalidOperationException("未找到待报废的工序资料。");
            if (processes.Count == 0)
                throw new InvalidOperationException("未找到待报废的工序资料。");
            return true;
        }

        private static void AddUpdateSource(
            ICollection<PartPartProcessInfo> source,
            ISet<int> sourceProcessNos,
            PartPartProcessInfo process)
        {
            if (process.ProcessNo > 0 && !sourceProcessNos.Add(process.ProcessNo))
            {
                throw new InvalidOperationException(
                    "ProcessNo " + process.ProcessNo + " 在本次保存中重复。");
            }
            source.Add(process);
        }

        private void AppendRecords(
            IList<XHSPartPartProcessInfo> processes,
            string actionCode)
        {
            List<FxGridRow> rows = CaptureGridRows();
            foreach (XHSPartPartProcessInfo process
                in processes ?? new List<XHSPartPartProcessInfo>())
            {
                string partNoId = NormalizeScan(process == null ? null : process.PartNo_Id);
                if (partNoId.Length == 0 || ContainsQRCode(rows, partNoId)) continue;

                rows.Add(new FxGridRow
                {
                    RowNumber = rows.Count + 1,
                    PartNoId = partNoId,
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
                if (action != null)
                {
                    rows[index].ActionCode = action.SelectedValue;
                    rows[index].ActionText = action.SelectedItem == null
                        ? string.Empty
                        : action.SelectedItem.Text;
                }
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
