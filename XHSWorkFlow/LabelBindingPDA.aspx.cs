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
        private const string CustomerStage = "客户标签";
        private const string FactoryStage = "本厂条码";
        private readonly FactoryBarcodeParser barcodeParser = new FactoryBarcodeParser();

        [Serializable]
        private sealed class PdaBindingRow
        {
            public int SeqNo { get; set; }
            public string CodeType { get; set; }
            public string SteelStampRelation { get; set; }
            public string FactoryBarcode { get; set; }
            public string JHSPartNo { get; set; }
            public string JHSBatchNo { get; set; }
            public decimal LabelQty { get; set; }
            public decimal BindQty { get; set; }
            public string BindQtyText { get { return BindQty.ToString("0.####", CultureInfo.InvariantCulture); } }
            public string ScanTimeText { get; set; }
            public string PartName { get; set; }
        }

        [Serializable]
        private sealed class PdaContext
        {
            public string BindingTaskId { get; set; }
            public string Stage { get; set; }
            public string CustomerRaw { get; set; }
            public string CustomerMaterial { get; set; }
            public string ExpectedPart { get; set; }
            public string CustomerBatch { get; set; }
            public string CustomerUnit { get; set; }
            public decimal CustomerQty { get; set; }
            public List<PdaBindingRow> Rows { get; set; }
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
            if (!IsPostBack)
            {
                ResetContext();
                Message(string.Empty, true);
                Focus(txtPdaScan);
            }
            ApplyContextToControls();
            BindRows();
        }

        protected void txtPdaScan_TextChanged(object sender, EventArgs e)
        {
            PdaContext context = Current;
            string raw = NormalizeScan(txtPdaScan.Text);
            txtPdaScan.Text = string.Empty;
            if (raw.Length == 0) { ShowScanFailure("Empty", "请扫描条码。", raw, null); return; }

            if (IsFastDuplicateSuccess(context, raw)) return;
            try
            {
                if (string.Equals(context.Stage, CustomerStage, StringComparison.Ordinal))
                {
                    ProcessCustomer(raw);
                }
                else
                {
                    ProcessFactory(raw);
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

        protected void btnFailureOk_Click(object sender, EventArgs e)
        {
            pnlFailureModal.Visible = false;
            labFailureMessage.Text = string.Empty;
            txtPdaScan.Text = string.Empty;
            ApplyContextToControls();
            BindRows();
            Focus(txtPdaScan);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ResetContext();
            Message("当前绑定已清空，请扫描客户标签。", true);
            ApplyContextToControls();
            BindRows();
            Focus(txtPdaScan);
        }

        private void ProcessCustomer(string raw)
        {
            string[] parts;
            decimal qty;
            if (!TryParseCustomer(raw, out parts, out qty))
            {
                ShowScanFailure("CustomerFormat", "客户标签格式无效，无法读取客户数量。", raw, null);
                return;
            }
            if (qty != decimal.Truncate(qty))
            {
                ShowScanFailure("CustomerQty", "客户标签数量必须为整数，当前绑定表不支持小数数量。", raw, null);
                return;
            }

            PdaContext context = Current;
            context.BindingTaskId = Guid.NewGuid().ToString("D");
            context.CustomerRaw = raw;
            context.CustomerMaterial = parts[0];
            context.ExpectedPart = parts[0];
            context.CustomerUnit = parts.Length > 2 ? parts[2] : string.Empty;
            context.CustomerBatch = parts.Length > 3 ? parts[3] : string.Empty;
            context.CustomerQty = qty;
            context.Rows = new List<PdaBindingRow>();
            context.Stage = FactoryStage;
            MarkSuccess(context, raw);
            Message("客户标签校验成功，请扫描本厂条码。", true);
        }

        private void ProcessFactory(string raw)
        {
            string[] customerParts;
            decimal customerQty;
            if (TryParseCustomer(raw, out customerParts, out customerQty))
            {
                ShowScanFailure("WrongStage", "当前绑定尚未完成，请先继续扫描本厂条码。", raw, null);
                return;
            }

            PartInfo part = barcodeParser.ParseFactoryBarcode(raw, "JHX", "CUSTOMER");
            if (part == null)
            {
                ShowScanFailure("Parse", "本厂条码解析失败，请重新扫描。", raw, null);
                return;
            }

            string type = string.IsNullOrWhiteSpace(part.ProcessOrderNo) ? "包装码" : "工单码";
            string partNo = part.JHSMaterialNo ?? string.Empty;
            string batchNo = part.JHSBatchNo ?? string.Empty;
            string steelStampRelation = GetSteelStampRelation(partNo);
            decimal labelQty = part.JHSQty;
            if (labelQty <= 0)
            {
                ShowScanFailure("Quantity", "本厂条码没有有效标签数量，不能绑定。", raw, part, type);
                return;
            }
            if (!string.Equals((Current.ExpectedPart ?? string.Empty).Trim(), partNo.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                ShowScanFailure("PartMismatch", "本厂品号与客户物料号不匹配。", raw, part, type);
                return;
            }
            if (Current.Rows.Any(x => string.Equals(x.FactoryBarcode, raw, StringComparison.Ordinal)))
            {
                ShowScanFailure("DuplicateCurrent", "本厂条码已在当前绑定中，不能重复扫描。", raw, part, type);
                return;
            }
            if (new LabelBindingBiz().IsFactoryBarcodeBound(raw))
            {
                ShowScanFailure("DuplicateExisting", "本厂条码已经绑定过其他客户标签。", raw, part, type);
                return;
            }

            decimal remaining = Current.CustomerQty - Current.Rows.Sum(x => x.BindQty);
            if (labelQty > remaining)
            {
                ShowScanFailure("OverQuantity", string.Format(CultureInfo.InvariantCulture, "本次绑定数量 {0:0.####} 超过客户剩余数量 {1:0.####}。", labelQty, remaining), raw, part, type);
                return;
            }

            Current.Rows.Insert(0, new PdaBindingRow
            {
                SeqNo = 1,
                CodeType = type,
                SteelStampRelation = steelStampRelation,
                FactoryBarcode = raw,
                JHSPartNo = partNo,
                JHSBatchNo = batchNo,
                LabelQty = labelQty,
                BindQty = labelQty,
                PartName = part.MaterialName,
                ScanTimeText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            });
            ReindexRows();
            MarkSuccess(Current, raw);

            decimal bound = Current.Rows.Sum(x => x.BindQty);
            if (bound == Current.CustomerQty)
            {
                CompleteBinding();
            }
            else
            {
                Message("本厂条码绑定成功，请继续扫描本厂条码。", true);
            }
        }

        private void CompleteBinding()
        {
            PdaContext context = Current;
            LabelBindingCustomerInfo customer = new LabelBindingCustomerInfo
            {
                CustomerMaterialNo = context.CustomerMaterial,
                ExpectedPartNo = context.ExpectedPart,
                CustomerBatchNo = context.CustomerBatch,
                CustomerUnit = context.CustomerUnit,
                CustomerQty = Convert.ToInt32(context.CustomerQty),
                RawCode = context.CustomerRaw,
                Status = "已完成",
                Creater = Convert.ToString(Session["userid"]),
                CreatDate = DateTime.Now
            };
            List<LabelBindingFactoryInfo> details = context.Rows.Select(x => new LabelBindingFactoryInfo
            {
                SeqNo = x.SeqNo, FactoryBarcode = x.FactoryBarcode, JHSPartNo = x.JHSPartNo, PartName = x.PartName,
                JHSBatchNo = x.JHSBatchNo, LabelQty = x.LabelQty, BindQty = x.BindQty, Creater = customer.Creater, CreatDate = DateTime.Now
            }).ToList();
            string result = new LabelBindingBiz().SaveCustomerBinding(customer, details);
            if (!string.IsNullOrWhiteSpace(result))
            {
                ShowScanFailure("Save", result, string.Empty, null);
                return;
            }
            ResetContext();
            Message("绑定成功，已完成本次客户标签绑定。", true);
            Focus(txtPdaScan);
        }

        private void ShowScanFailure(string type, string reason, string raw, PartInfo part, string parsedType = null)
        {
            bool saved = new LabelBindingBiz().SavePdaScanFailure(new LabelBindingPdaScanFailureInfo
            {
                BindingTaskId = Current.BindingTaskId,
                CustomerRawCode = Current.CustomerRaw,
                ScanRawCode = raw,
                ScanStage = Current.Stage,
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
            bool customer = string.Equals(context.Stage, CustomerStage, StringComparison.Ordinal);
            labScanState.Text = customer ? "请扫描客户标签" : "请扫描本厂条码";
            labScanState.CssClass = "pda-state " + (customer ? "pda-state-customer" : "pda-state-factory");
            hidScanStage.Value = context.Stage;
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
            gvBindingRecords.DataSource = context.Rows;
            gvBindingRecords.DataBind();
            decimal bound = context.Rows.Sum(x => x.BindQty);
            labCustomerQty.Text = context.CustomerQty.ToString("0.####", CultureInfo.InvariantCulture);
            labBoundQty.Text = bound.ToString("0.####", CultureInfo.InvariantCulture);
            labRemainingQty.Text = Math.Max(0, context.CustomerQty - bound).ToString("0.####", CultureInfo.InvariantCulture);
            progressBar.Style["width"] = (context.CustomerQty <= 0 ? 0 : Math.Min(100, bound * 100 / context.CustomerQty)).ToString("0.##", CultureInfo.InvariantCulture) + "%";
        }

        private void ResetContext() { Session[ContextKey] = NewContext(); }
        private static PdaContext NewContext() { return new PdaContext { BindingTaskId = Guid.NewGuid().ToString("D"), Stage = CustomerStage, Rows = new List<PdaBindingRow>() }; }
        private void ReindexRows() { for (int i = 0; i < Current.Rows.Count; i++) Current.Rows[i].SeqNo = i + 1; }
        private static string GetSteelStampRelation(string partNo)
        {
            if (string.IsNullOrWhiteSpace(partNo)) return "未维护";
            PartMasterInfo info = new PartMaster().GetPartMasterByJHSPartNo(partNo);
            return info == null || string.IsNullOrWhiteSpace(info.HasSteelStamp) ? "未维护" : info.HasSteelStamp;
        }
        private static void MarkSuccess(PdaContext context, string raw) { context.LastSuccessfulRaw = raw; context.LastSuccessfulStage = context.Stage; context.LastSuccessfulAt = DateTime.Now; }
        private static bool IsFastDuplicateSuccess(PdaContext context, string raw) { return string.Equals(context.LastSuccessfulRaw, raw, StringComparison.Ordinal) && string.Equals(context.LastSuccessfulStage, context.Stage, StringComparison.Ordinal) && (DateTime.Now - context.LastSuccessfulAt).TotalSeconds < 2; }
        private void Message(string text, bool ok) { labMessage.Text = text; labMessage.CssClass = "pda-status " + (ok ? "pda-ok" : "pda-error"); }
        private void Focus(Control control) { ClientScript.RegisterStartupScript(GetType(), "pdaFocus", "window.setTimeout(function(){var e=document.getElementById('" + control.ClientID + "');if(e){e.focus();try{e.select();}catch(x){}}},50);", true); }
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
