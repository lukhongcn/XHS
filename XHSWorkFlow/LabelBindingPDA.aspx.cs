using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using XHS.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    public partial class LabelBindingPDA : Page
    {
        private const string ContextKey = "LabelBindingPdaContext";
        private const string FlowName = "光束标签绑定流程";
        private const string FlowCode = "LABEL_BINDING";
        private readonly FactoryBarcodeParser barcodeParser = new FactoryBarcodeParser();

        [Serializable]
        private sealed class PdaBindingRow
        {
            public int SeqNo { get; set; }
            public string CodeType { get; set; }
            public string SteelStampRelation { get; set; }
            public string FactoryBarcode { get; set; }
            public string WorkOrderNo { get; set; }
            public string JHSPartNo { get; set; }
            public string JHSBatchNo { get; set; }
            public decimal LabelQty { get; set; }
            public decimal BindQty { get; set; }
            public string BindQtyText { get { return BindQty <= 0 ? string.Empty : BindQty.ToString("0.####", CultureInfo.InvariantCulture); } }
            public string ScanTimeText { get; set; }
            public string PartName { get; set; }
        }

        [Serializable]
        private sealed class PdaWorkflowScan
        {
            public string StepCode { get; set; }
            public int SeqNo { get; set; }
            public string RawCode { get; set; }
        }

        [Serializable]
        private sealed class PdaContext
        {
            public string BindingTaskId { get; set; }
            public int FlowId { get; set; }
            public List<ScanFlowStepInfo> Steps { get; set; }
            public int CurrentStepIndex { get; set; }
            public string CustomerRaw { get; set; }
            public string CustomerMaterial { get; set; }
            public string ExpectedPart { get; set; }
            public string CustomerBatch { get; set; }
            public string CustomerUnit { get; set; }
            public decimal CustomerQty { get; set; }
            public PartInfo CustomerPartMaster { get; set; }
            public List<PdaBindingRow> Rows { get; set; }
            public List<PdaWorkflowScan> Scans { get; set; }
            public List<string> WorkOrderRaws { get; set; }
            public bool IsCompleted { get; set; }
            public string LastSuccessfulRaw { get; set; }
            public string LastSuccessfulStage { get; set; }
            public DateTime LastSuccessfulAt { get; set; }
        }

        private PdaContext Current
        {
            get
            {
                PdaContext context = Session[ContextKey] as PdaContext;
                if (context == null)
                {
                    context = NewContext();
                    Session[ContextKey] = context;
                }
                return context;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            Load += Page_Load;
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!DefaultPDASub.IsPdaAccessAllowed(Context)) return;
            if (!IsPostBack)
            {
                ResetContext();
                InitializeWorkflow();
                Message(string.Empty, true);
                Focus(txtPdaScan);
            }
            ApplyContextToControls();
            BindRows();
        }

        protected void txtPdaScan_TextChanged(object sender, EventArgs e)
        {
            if (!DefaultPDASub.IsPdaAccessAllowed(Context)) return;
            PdaContext context = Current;
            if (context.IsCompleted)
            {
                txtPdaScan.Text = string.Empty;
                Message("本次绑定已完成，请先点击“清空当前绑定”开始下一次绑定。", false);
                return;
            }
            string raw = NormalizeScan(txtPdaScan.Text);
            txtPdaScan.Text = string.Empty;
            if (raw.Length == 0) { ShowScanFailure("Empty", "请扫描条码。", raw, null); return; }

            try
            {
                ScanFlowStepInfo step = GetCurrentStep(context);
                if (step == null)
                {
                    ShowScanFailure("Workflow", "当前扫描流程步骤无效，请刷新页面后重试。", raw, null);
                }
                else
                {
                    ProcessByRegisteredHandler(raw, step);
                }
            }
            catch (Exception ex)
            {
                Utility.Log.WriteLog("log.txt", "LabelBindingPDA 扫描处理异常：" + ex);
                ShowScanFailure("ServerError", "系统处理扫描时发生异常，请联系管理员。", raw, null);
            }
            finally
            {
                ApplyContextToControls();
                BindRows();
            }
        }

        private void ProcessByRegisteredHandler(string raw, ScanFlowStepInfo step)
        {
            switch (ScanStepHandlerRegistry.Resolve(step))
            {
                case ScanStepHandlerType.Customer:
                    ProcessCustomer(raw, step);
                    return;
                case ScanStepHandlerType.WorkOrder:
                    ProcessWorkOrder(raw, step);
                    return;
                case ScanStepHandlerType.Factory:
                    ProcessFactory(raw, step);
                    return;
                default:
                    ShowScanFailure("Handler", "当前流程步骤没有注册对应的扫描处理器。", raw, null);
                    return;
            }
        }

        protected void btnFailureOk_Click(object sender, EventArgs e)
        {
            if (!DefaultPDASub.IsPdaAccessAllowed(Context)) return;
            pnlFailureModal.Visible = false;
            labFailureMessage.Text = string.Empty;
            txtPdaScan.Text = string.Empty;
            ApplyContextToControls();
            BindRows();
            Focus(txtPdaScan);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            if (!DefaultPDASub.IsPdaAccessAllowed(Context)) return;
            PdaContext context = Current;
            string saveResult = SaveClearBindingRecord(context);
            if (!string.IsNullOrWhiteSpace(saveResult))
            {
                Message("清空操作记录保存失败，当前绑定未清空。" + saveResult, false);
                return;
            }

            ResetContext();
            InitializeWorkflow();
            Message("当前绑定已清空，请扫描客户标签。", true);
            ApplyContextToControls();
            BindRows();
            Focus(txtPdaScan);
        }

        private string SaveClearBindingRecord(PdaContext context)
        {
            if (context == null || context.FlowId <= 0)
            {
                return string.Empty;
            }

            return new ScanFlowScanRecord().SaveRecords(new List<ScanFlowScanRecordInfo>
            {
                new ScanFlowScanRecordInfo
                {
                    FlowId = context.FlowId,
                    FlowCode = FlowCode,
                    BindingTaskId = context.BindingTaskId,
                    StepId = null,
                    StepCode = "CLEAR",
                    SeqNo = 0,
                    ScanContent = "CLEAR_BINDING",
                    Status = "已清除",
                    ScanUser = Convert.ToString(Session["userid"]),
                    DeviceInfo = Request.UserAgent,
                    ClientIp = Request.UserHostAddress,
                    ScanTime = DateTime.Now
                }
            });
        }

        protected void btnEndStep_Click(object sender, EventArgs e)
        {
            if (!DefaultPDASub.IsPdaAccessAllowed(Context)) return;
            PdaContext context = Current;
            ScanFlowStepInfo step = GetCurrentStep(context);
            if (step == null || !step.AllowRepeat || ScanStepHandlerRegistry.Resolve(step) == ScanStepHandlerType.Customer)
            {
                Message("当前步骤不能手动结束。", false);
                return;
            }

            bool isLastStep = context.Steps != null && context.CurrentStepIndex >= context.Steps.Count - 1;
            if (isLastStep && ScanStepHandlerRegistry.Resolve(step) == ScanStepHandlerType.Factory)
            {
                decimal bound = context.Rows.Sum(x => x.BindQty);
                if (context.CustomerPartMaster == null || !context.Rows.Any(x => x.BindQty > 0))
                {
                    Message("请至少扫描一张本厂标签后再完成绑定。", false);
                    return;
                }
                if (bound > context.CustomerQty)
                {
                    ShowScanFailure("OverQuantity", "绑定数量不能大于客户数量。", string.Empty, null);
                    return;
                }
                CompleteBinding();
                return;
            }

            AdvanceAfterStepSuccess(context, step, true);
            Message("当前步骤已结束，请扫描" + GetCurrentStepTitle(context) + "。", true);
            ApplyContextToControls();
            BindRows();
            Focus(txtPdaScan);
        }

        protected void btnCompleteBinding_Click(object sender, EventArgs e)
        {
            if (!DefaultPDASub.IsPdaAccessAllowed(Context)) return;
            PdaContext context = Current;
            decimal bound = context.Rows.Sum(x => x.BindQty);
            if (context.CustomerPartMaster == null || !context.Rows.Any(x => x.BindQty > 0))
            {
                Message("请至少扫描一张本厂标签后再完成绑定。", false);
                return;
            }
            if (bound > context.CustomerQty)
            {
                ShowScanFailure("OverQuantity", "绑定数量不能大于客户数量。", string.Empty, null);
                return;
            }
            CompleteBinding();
        }

        private void ProcessCustomer(string raw, ScanFlowStepInfo step)
        {
            PartInfo part = barcodeParser.ParseCustomerBarcode(raw, step.CustomerId, step.LabelType, step.RuleName);
            if (part == null)
            {
                ShowScanFailure("CustomerFormat", "客户标签格式无效，无法按当前流程规则解析。", raw, null);
                return;
            }
            decimal qty = part.Qty;
            if (qty != decimal.Truncate(qty))
            {
                ShowScanFailure("CustomerQty", "客户标签数量必须为整数，当前绑定表不支持小数数量。", raw, null);
                return;
            }

            PdaContext context = Current;
            PartInfo masterPart = new PartMaster().GetPartMasterByCustomerMaterialNo(part.MaterialNo);
            if (masterPart == null)
            {
                ShowScanFailure("PartMasterNotFound", "客户物料号未维护零件主数据。", raw, part);
                return;
            }

            context.BindingTaskId = Guid.NewGuid().ToString("D");
            context.CustomerRaw = raw;
            context.CustomerMaterial = part.MaterialNo;
            context.CustomerPartMaster = masterPart;
            context.ExpectedPart = masterPart.JHSPartNo;
            context.CustomerUnit = part.Unit ?? string.Empty;
            context.CustomerBatch = part.BatchNo ?? string.Empty;
            context.CustomerQty = qty;
            context.Rows = new List<PdaBindingRow>();
            AddWorkflowScan(context, step, raw, 0);
            MarkSuccess(context, raw);
            AdvanceAfterStepSuccess(context, step);
            Message("客户标签校验成功，请扫描" + GetCurrentStepTitle(context) + "。", true);
        }

        /// <summary>
        /// 工单步骤只确认编码格式和流程规则，不做物料主数据、数量或本厂标签比对。
        /// </summary>
        private void ProcessWorkOrder(string raw, ScanFlowStepInfo step)
        {
            PartInfo part = barcodeParser.ParseWorkOrderBarcode(raw, step.CustomerId, step.LabelType, step.RuleName);
            if (part == null || string.IsNullOrWhiteSpace(part.ProcessOrderNo))
            {
                ShowScanFailure("WorkOrderFormat", "工单条码格式无效，无法按当前流程规则解析。", raw, part);
                return;
            }

            PdaContext context = Current;
            if (context.WorkOrderRaws == null) context.WorkOrderRaws = new List<string>();
            if (context.WorkOrderRaws.Contains(raw, StringComparer.Ordinal))
            {
                ShowScanFailure("DuplicateCurrent", "工单条码已在当前绑定中，不能重复扫描。", raw, part);
                return;
            }

            context.WorkOrderRaws.Add(raw);
            context.Rows.Insert(0, new PdaBindingRow
            {
                SeqNo = 1,
                CodeType = "工单",
                WorkOrderNo = raw,
                ScanTimeText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            });
            ReindexRows();
            AddWorkflowScan(context, step, raw, context.WorkOrderRaws.Count);
            MarkSuccess(context, raw);
            AdvanceAfterStepSuccess(context, step);
            Message("工单扫描成功，请扫描" + GetCurrentStepTitle(context) + "。", true);
        }

        private void ProcessFactory(string raw, ScanFlowStepInfo step)
        {
            string[] customerParts;
            decimal customerQty;
            if (TryParseCustomer(raw, out customerParts, out customerQty))
            {
                ShowScanFailure("WrongStage", "当前步骤不是客户标签，请扫描" + GetCurrentStepTitle(Current) + "。", raw, null);
                return;
            }

            PartInfo part = barcodeParser.ParseFactoryBarcode(raw, step.CustomerId, step.LabelType, step.RuleName);
            if (part == null)
            {
                ShowScanFailure("Parse", "本厂条码解析失败，请重新扫描。", raw, null);
                return;
            }

            string type = string.IsNullOrWhiteSpace(step.StepName) ? "本厂标签" : step.StepName;
            string partNo = part.JHSMaterialNo ?? string.Empty;
            string batchNo = part.JHSBatchNo ?? string.Empty;
            string steelStampRelation = GetSteelStampRelation(partNo);
            decimal labelQty = part.JHSQty;
            if (labelQty <= 0)
            {
                ShowScanFailure("Quantity", "本厂条码没有有效标签数量，不能绑定。", raw, part, type);
                return;
            }
            string expectedFactoryPartNo = Current.CustomerPartMaster == null
                ? string.Empty
                : Current.CustomerPartMaster.JHSPartNo;
            if (!string.Equals((expectedFactoryPartNo ?? string.Empty).Trim(), partNo.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                ShowScanFailure("PartMismatch", "本厂品号与客户物料号不匹配。", raw, part, type);
                return;
            }

            decimal remaining = Current.CustomerQty - Current.Rows.Sum(x => x.BindQty);
            if (labelQty > remaining)
            {
                ShowScanFailure("OverQuantity", string.Format(CultureInfo.InvariantCulture, "本次绑定数量 {0:0.####} 超过客户剩余数量 {1:0.####}。", labelQty, remaining), raw, part, type);
                return;
            }

            PdaBindingRow bindingRow = Current.Rows.FirstOrDefault(x =>
                string.Equals(x.CodeType, "工单", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(x.FactoryBarcode));
            if (bindingRow == null)
            {
                bindingRow = new PdaBindingRow();
                Current.Rows.Insert(0, bindingRow);
            }
            bindingRow.CodeType = type;
            bindingRow.SteelStampRelation = steelStampRelation;
            bindingRow.FactoryBarcode = raw;
            bindingRow.JHSPartNo = partNo;
            bindingRow.JHSBatchNo = batchNo;
            bindingRow.LabelQty = labelQty;
            bindingRow.BindQty = labelQty;
            bindingRow.PartName = part.MaterialName;
            bindingRow.WorkOrderNo = string.Join(", ", Current.WorkOrderRaws ?? new List<string>());
            bindingRow.ScanTimeText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            ReindexRows();
            AddWorkflowScan(Current, step, raw, Current.Rows[0].SeqNo);
            MarkSuccess(Current, raw);
            AdvanceAfterStepSuccess(Current, step);

            decimal bound = Current.Rows.Sum(x => x.BindQty);
            if (bound == Current.CustomerQty)
            {
                CompleteBinding();
            }
            else
            {
                Message("扫描成功，请继续扫描" + GetCurrentStepTitle(Current) + "。", true);
            }
        }

        private void CompleteBinding()
        {
            PdaContext context = Current;
            decimal bound = context.Rows.Sum(x => x.BindQty);
            if (bound > context.CustomerQty)
            {
                ShowScanFailure("OverQuantity", "绑定数量不能大于客户数量。", string.Empty, null);
                return;
            }
            List<ScanFlowScanRecordInfo> records = context.Scans
                .Select(x => BuildScanRecord(x.StepCode, x.SeqNo, x.RawCode))
                .ToList();

            string result = new ScanFlowScanRecord().SaveRecords(records);
            if (!string.IsNullOrWhiteSpace(result))
            {
                ShowScanFailure("Save", result, string.Empty, null);
                return;
            }
            // 保存成功后保留当前流程、零件 Session 和绑定明细；扫描框置灰。
            // 下一次绑定由“清空当前绑定”显式清除当前流程，登录 Session 不受影响。
            context.IsCompleted = true;
            Message("绑定成功，已完成本次客户标签绑定。", true);
            ApplyContextToControls();
            BindRows();
            Focus(txtPdaScan);
        }

        private void ShowScanFailure(string type, string reason, string raw, PartInfo part, string parsedType = null)
        {
            bool saved = new ScanFlowScanRecord().SavePdaScanFailure(new LabelBindingPdaScanFailureInfo
            {
                BindingTaskId = Current.BindingTaskId,
                CustomerRawCode = Current.CustomerRaw,
                ScanRawCode = raw,
                ScanStage = GetCurrentStep(Current) == null ? string.Empty : GetCurrentStep(Current).StepCode,
                FailureType = type,
                FailureReason = reason,
                ParsedBarcodeType = parsedType,
                ParsedPartNo = part == null ? null : part.JHSMaterialNo,
                ParsedBatchNo = part == null ? null : part.JHSBatchNo,
                ParsedQty = part == null ? (decimal?)null : part.JHSQty,
                OperatorName = Convert.ToString(Session["userid"]),
                DeviceInfo = Request.UserAgent,
                ClientIp = Request.UserHostAddress,
                ScanTime = DateTime.Now,
                IsProcessed = false
            });
            string suffix = saved ? "\n失败记录已保存" : "\n失败记录保存失败，请重试";
            labFailureTitle.Text = string.Equals(type, "Save", StringComparison.Ordinal) ? "绑定失败" : "扫描失败";
            labFailureMessage.Text = reason + "\n扫描内容：" + (raw ?? string.Empty) + suffix;
            pnlFailureModal.Visible = true;
            Message(saved ? "扫描失败，等待确认。" : "失败记录保存失败，请重试。", false);
        }

        private void ShowScanFailure(string type, string reason, string raw, PartInfo part)
        {
            ShowScanFailure(type, reason, raw, part, null);
        }

        private void ApplyContextToControls()
        {
            PdaContext context = Current;
            ScanFlowStepInfo step = GetCurrentStep(context);
            if (context.IsCompleted)
            {
                btnEndStep.Visible = false;
                btnCompleteBinding.Visible = false;
                labScanState.Text = "本次绑定已完成，请清空后开始下一次绑定";
                labScanState.CssClass = "pda-state pda-state-success";
                txtPdaScan.Enabled = false;
                hidScanStage.Value = string.Empty;
                return;
            }
            txtPdaScan.Enabled = true;
            bool customer = ScanStepHandlerRegistry.Resolve(step) == ScanStepHandlerType.Customer;
            bool isLastStep = step != null && context.Steps != null && context.CurrentStepIndex >= context.Steps.Count - 1;
            bool factory = ScanStepHandlerRegistry.Resolve(step) == ScanStepHandlerType.Factory;
            btnEndStep.Visible = isLastStep && factory && context.CustomerPartMaster != null && context.Rows.Any(x => x.BindQty > 0);
            btnEndStep.Text = factory && isLastStep ? "完成绑定" : "结束当前步骤";
            btnCompleteBinding.Visible = false;
            string title = GetCurrentStepTitle(context);
            labScanState.Text = step == null ? "流程未加载" : "请扫描" + title;
            labScanState.CssClass = "pda-state " + (customer ? "pda-state-customer" : "pda-state-factory");
            hidScanStage.Value = step == null ? string.Empty : step.StepCode;
            labCustomerRaw.Text = context.CustomerRaw ?? string.Empty;
            labCustomerMaterial.Text = context.CustomerMaterial ?? string.Empty;
            labExpectedPart.Text = context.ExpectedPart ?? string.Empty;
            labCustomerBatch.Text = context.CustomerBatch ?? string.Empty;
            labCustomerUnit.Text = context.CustomerUnit ?? string.Empty;
            hidCustomerQty.Value = context.CustomerQty.ToString(CultureInfo.InvariantCulture);
            hidBoundQty.Value = context.Rows.Sum(x => x.BindQty).ToString(CultureInfo.InvariantCulture);
            hidRemainingQty.Value = Math.Max(0, context.CustomerQty - context.Rows.Sum(x => x.BindQty)).ToString(CultureInfo.InvariantCulture);
        }

        private void BindRows()
        {
            PdaContext context = Current;
            List<PdaBindingRow> rows = context.Rows ?? new List<PdaBindingRow>();
            gvBindingRecords.DataSource = rows;
            gvBindingRecords.DataBind();
            decimal bound = rows.Sum(x => x.BindQty);
            labCustomerQty.Text = context.CustomerQty.ToString("0.####", CultureInfo.InvariantCulture);
            labBoundQty.Text = bound.ToString("0.####", CultureInfo.InvariantCulture);
            labRemainingQty.Text = Math.Max(0, context.CustomerQty - bound).ToString("0.####", CultureInfo.InvariantCulture);
            progressBar.Style["width"] = (context.CustomerQty <= 0 ? 0 : Math.Min(100, bound * 100 / context.CustomerQty)).ToString("0.##", CultureInfo.InvariantCulture) + "%";
        }

        private void ResetContext() { Session[ContextKey] = NewContext(); }
        private static PdaContext NewContext() { return new PdaContext { BindingTaskId = Guid.NewGuid().ToString("D"), Steps = new List<ScanFlowStepInfo>(), Rows = new List<PdaBindingRow>(), Scans = new List<PdaWorkflowScan>(), WorkOrderRaws = new List<string>() }; }
        private bool InitializeWorkflow()
        {
            PdaContext context = Current;
            List<ScanFlowStepInfo> steps = new ScanFlowStep().GetStepsByFlowCode(FlowCode);
            if (steps == null || steps.Count == 0)
            {
                Message("扫描流程没有配置步骤：" + FlowName, false);
                return false;
            }

            context.FlowId = steps[0].FlowId ?? 0;
            context.Steps = steps.OrderBy(x => x.StepNo ?? int.MaxValue).ToList();
            context.CurrentStepIndex = 0;
            return true;
        }
        private static ScanFlowStepInfo GetCurrentStep(PdaContext context)
        {
            return context != null && context.Steps != null && context.CurrentStepIndex >= 0 && context.CurrentStepIndex < context.Steps.Count
                ? context.Steps[context.CurrentStepIndex] : null;
        }
        private static string GetCurrentStepTitle(PdaContext context)
        {
            ScanFlowStepInfo step = GetCurrentStep(context);
            if (step == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(step.StepName)) return step.StepName;
            ScanStepHandlerType handlerType = ScanStepHandlerRegistry.Resolve(step);
            if (handlerType == ScanStepHandlerType.Factory) return "本厂标签";
            if (handlerType == ScanStepHandlerType.WorkOrder) return "上方工单码";
            return step.StepCode;
        }
        private static void MoveToNextStep(PdaContext context)
        {
            if (context == null || context.Steps == null || context.Steps.Count == 0) return;
            context.CurrentStepIndex = Math.Min(context.CurrentStepIndex + 1, context.Steps.Count - 1);
        }
        private void AdvanceAfterStepSuccess(PdaContext context, ScanFlowStepInfo step)
        {
            AdvanceAfterStepSuccess(context, step, false);
        }
        private void AdvanceAfterStepSuccess(PdaContext context, ScanFlowStepInfo step, bool manualEnd)
        {
            if (new ScanFlowStep().IsStepCompleted(step, GetStepEndValue(step == null ? null : step.EndControlId, context), GetStepEndValue(step == null ? null : step.EndCompareControlId, context), manualEnd))
            {
                MoveToNextStep(context);
            }
        }
        private static string GetStepEndValue(string controlId, PdaContext context)
        {
            if (string.IsNullOrWhiteSpace(controlId) || context == null)
            {
                return null;
            }

            if (string.Equals(controlId, "hidBoundQty", StringComparison.OrdinalIgnoreCase))
            {
                return context.Rows.Sum(x => x.BindQty).ToString(CultureInfo.InvariantCulture);
            }

            if (string.Equals(controlId, "hidCustomerQty", StringComparison.OrdinalIgnoreCase))
            {
                return context.CustomerQty.ToString(CultureInfo.InvariantCulture);
            }

            return null;
        }
        private static void AddWorkflowScan(PdaContext context, ScanFlowStepInfo step, string rawCode, int seqNo)
        {
            if (context.Scans == null) context.Scans = new List<PdaWorkflowScan>();
            context.Scans.Add(new PdaWorkflowScan { StepCode = step == null ? string.Empty : step.StepCode, SeqNo = seqNo, RawCode = rawCode });
        }

        private ScanFlowScanRecordInfo BuildScanRecord(string stepCode, int seqNo, string rawCode)
        {
            PdaContext context = Current;
            ScanFlowStepInfo step = context.Steps.FirstOrDefault(x => string.Equals(x.StepCode, stepCode, StringComparison.OrdinalIgnoreCase));
            return new ScanFlowScanRecordInfo
            {
                FlowId = context.FlowId,
                FlowCode = FlowCode,
                BindingTaskId = context.BindingTaskId,
                StepId = step == null ? (int?)null : step.StepId,
                StepCode = stepCode,
                SeqNo = seqNo,
                ScanContent = rawCode,
                Status = "已完成",
                ScanUser = Convert.ToString(Session["userid"]),
                DeviceInfo = Request.UserAgent,
                ClientIp = Request.UserHostAddress,
                ScanTime = DateTime.Now
            };
        }
        private void ReindexRows() { for (int i = 0; i < Current.Rows.Count; i++) Current.Rows[i].SeqNo = i + 1; }
        private static string GetSteelStampRelation(string partNo)
        {
            if (string.IsNullOrWhiteSpace(partNo)) return "未维护";
            PartInfo info = new PartMaster().GetPartMasterByJHSPartNo(partNo);
            return info == null || string.IsNullOrWhiteSpace(info.HasSteelStamp) ? "未维护" : info.HasSteelStamp;
        }
        private static void MarkSuccess(PdaContext context, string raw) { ScanFlowStepInfo step = GetCurrentStep(context); context.LastSuccessfulRaw = raw; context.LastSuccessfulStage = step == null ? string.Empty : step.StepCode; context.LastSuccessfulAt = DateTime.Now; }
        private void Message(string text, bool ok) { labMessage.Text = text; labMessage.CssClass = "pda-status " + (ok ? "pda-ok" : "pda-error"); }
        private void Focus(Control control) { ScriptManager.RegisterStartupScript(this, GetType(), "pdaFocus", "window.setTimeout(function(){var e=document.getElementById('" + control.ClientID + "');if(e){e.focus();try{e.select();}catch(x){}}},50);", true); }
        private static string NormalizeScan(string value) { return (value ?? string.Empty).Trim('\r', '\n', ' ', '\t'); }
        private static decimal ParseDecimal(string value) { decimal result; return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ? result : 0M; }
        private static bool TryParseCustomer(string raw, out string[] parts, out decimal qty)
        {
            qty = 0M;
            string[] sections = (raw ?? string.Empty).Split('/');
            parts = sections.Length > 2 ? sections[2].Split('&') : new string[0];
            return parts.Length >= 2 && decimal.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out qty) && qty > 0;
        }
    }
}
