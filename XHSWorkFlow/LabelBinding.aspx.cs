using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using XHS.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    public partial class LabelBinding : System.Web.UI.Page
    {
        protected string menuname = "客户标签绑定";

        private readonly FactoryBarcodeParser barcodeParser = new FactoryBarcodeParser();

        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ResetPage();
            }
        }

        protected void lnk_view_Click(object sender, EventArgs e)
        {
            // 跳转到绑定记录浏览页面，按项目实际地址修改。
            // Response.Redirect("LabelBindingList.aspx");
        }

        protected void lnkbutton_save_Click(object sender, EventArgs e)
        {
            // TODO：保存客户标签主记录与多条本厂标签绑定明细。
            Label_Message.Text = "请在后台实现最终保存逻辑。";
        }

        protected void lnkbutton_clear_Click(object sender, EventArgs e)
        {
            ResetPage();
        }

        protected void txt_CustomerQRCode_TextChanged(object sender, EventArgs e)
        {
            string rawCode = (txt_CustomerQRCode.Text ?? string.Empty).Trim();
            if (rawCode.Length == 0)
            {
                ShowError("请扫描客户 QRCode。");
                return;
            }

            // TODO：调用通用 LabelParser，根据客户标签规则解析。
            // 示例二维码：P2026061705592/CAQPL/5401113XNY02A03&120&EA&260602
            // 解析后再通过客户物料号查询本厂品号和零件名称。

            ClearFactoryScanArea();
            Label_Message.Text = "客户 QRCode 已接收，请接入 LabelParser 完成解析。";
        }

        protected void txt_FactoryBarcode_TextChanged(
            object sender,
            EventArgs e)
        {
            string rawCode =
                (txt_FactoryBarcode.Text ?? string.Empty).Trim();

            PartInfo partInfo =
                barcodeParser.ParseFactoryBarcode(rawCode, "JHX", "CUSTOMER");

            if (partInfo == null)
            {
                ShowError("条码解析失败，不符合本厂条码规则。");
                return;
            }

            bool isPackagingCode = string.IsNullOrWhiteSpace(partInfo.ProcessOrderNo);
            txt_FactoryCodeType.Text = isPackagingCode ? "包装码" : "工单码";

            if (isPackagingCode)
            {
                PartMaster partmaster = new PartMaster();
                PartMasterInfo partMasterInfo = partmaster.GetPartMasterByJHSPartNo(partInfo.JHSMaterialNo);
                if (partMasterInfo != null)
                {
                    txt_WorkOrderNo.Text = string.Empty;
                    txt_FactoryPartNo.Text = partMasterInfo.JHSPartNo ?? string.Empty;
                    txt_FactoryPartName.Text = partMasterInfo.MaterialName ?? string.Empty;
                    txt_FactoryBatchNo.Text = partInfo.JHSBatchNo ?? string.Empty;
                }
                else
                {
                    txt_WorkOrderNo.Text = string.Empty;
                    txt_FactoryPartNo.Text = partInfo.JHSMaterialNo ?? string.Empty;
                    txt_FactoryPartName.Text = partInfo.MaterialName ?? string.Empty;
                    txt_FactoryBatchNo.Text = partInfo.JHSBatchNo ?? string.Empty;
                }
            }
            else
            {
                txt_WorkOrderNo.Text = partInfo.ProcessOrderNo ?? string.Empty;
                txt_FactoryPartNo.Text = partInfo.JHSMaterialNo ?? string.Empty;
                txt_FactoryPartName.Text = partInfo.MaterialName ?? string.Empty;
                txt_FactoryBatchNo.Text = partInfo.JHSBatchNo ?? string.Empty;
            }

            Label_Message.Text = "本厂条码解析成功。";
        }

        protected void txt_BindQty_TextChanged(object sender, EventArgs e)
        {
            ValidateBindQty();
        }

        protected void btnUseRemainingQty_Click(object sender, EventArgs e)
        {
            decimal remainingQty = ParseDecimal(hidRemainingQty.Value);
            decimal availableQty = ParseDecimal(hidFactoryAvailableQty.Value);
            txt_BindQty.Text = Math.Min(remainingQty, availableQty).ToString("0.####", CultureInfo.InvariantCulture);
            ValidateBindQty();
        }

        protected void btnConfirmBind_Click(object sender, EventArgs e)
        {
            if (!ValidateBindQty())
            {
                return;
            }

            // TODO：将当前本厂标签加入页面暂存明细。
            // 必须再次在服务端校验：
            // 1. 本厂品号等于客户物料映射出的本厂品号。
            // 2. 本次绑定数量 > 0。
            // 3. 本次绑定数量 <= 客户剩余数量。
            // 4. 本次绑定数量 <= 本厂标签剩余可用数量。
            // 5. 同一包装码不得超量重复绑定。

            Label_Message.Text = "请在后台实现加入绑定明细逻辑。";
        }

        protected void gvBindingRecords_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "DeleteBinding", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string bindingDetailId = Convert.ToString(e.CommandArgument, CultureInfo.InvariantCulture);
            // TODO：从页面暂存明细中删除，并重新计算已绑定数量和剩余数量。
            Label_Message.Text = "待删除绑定明细：" + bindingDetailId;
        }

        private bool ValidateBindQty()
        {
            decimal bindQty;
            if (!decimal.TryParse(txt_BindQty.Text, out bindQty) || bindQty <= 0)
            {
                ShowError("本次绑定数量必须大于 0。");
                return false;
            }

            decimal remainingQty = ParseDecimal(hidRemainingQty.Value);
            if (bindQty > remainingQty)
            {
                ShowError(string.Format(CultureInfo.InvariantCulture,
                    "本次绑定数量 {0:0.####} 超过客户剩余数量 {1:0.####}。",
                    bindQty, remainingQty));
                return false;
            }

            decimal availableQty = ParseDecimal(hidFactoryAvailableQty.Value);
            if (bindQty > availableQty)
            {
                ShowError(string.Format(CultureInfo.InvariantCulture,
                    "本次绑定数量 {0:0.####} 超过本厂标签可用数量 {1:0.####}。",
                    bindQty, availableQty));
                return false;
            }

            txt_ValidationStatus.Text = "数量校验通过";
            Label_Message.Text = string.Empty;
            return true;
        }

        private void ResetPage()
        {
            txt_CustomerQRCode.Text = string.Empty;
            txt_CustomerLabelNo.Text = string.Empty;
            txt_SourceCode.Text = string.Empty;
            txt_CustomerMaterialNo.Text = string.Empty;
            txt_ExpectedPartNo.Text = string.Empty;
            txt_PartName.Text = string.Empty;
            txt_CustomerBatchNo.Text = string.Empty;
            txt_CustomerQty.Text = "0";
            txt_BoundQty.Text = "0";
            txt_RemainingQty.Text = "0";
            txt_Unit.Text = string.Empty;

            hidCustomerQty.Value = "0";
            hidBoundQty.Value = "0";
            hidRemainingQty.Value = "0";
            hidFactoryLabelQty.Value = "0";
            hidFactoryAvailableQty.Value = "0";

            Label_SummaryCustomerQty.Text = "0";
            Label_SummaryBoundQty.Text = "0";
            Label_SummaryRemainingQty.Text = "0";
            Label_BindingProgressText.Text = "0 / 0";
            bindingProgressBar.Style["width"] = "0%";
            bindingProgressBar.InnerText = "0%";
            pnlCompleted.Visible = false;

            ClearFactoryScanArea();
            Label_Message.Text = string.Empty;
        }

        private void ClearFactoryScanArea()
        {
            txt_FactoryBarcode.Text = string.Empty;
            txt_FactoryCodeType.Text = string.Empty;
            txt_WorkOrderNo.Text = string.Empty;
            txt_FactoryPartNo.Text = string.Empty;
            txt_FactoryPartName.Text = string.Empty;
            txt_FactoryBatchNo.Text = string.Empty;
            txt_FactoryLabelQty.Text = string.Empty;
            txt_BindQty.Text = string.Empty;
            txt_ValidationStatus.Text = string.Empty;
            hidFactoryLabelQty.Value = "0";
            hidFactoryAvailableQty.Value = "0";
            hidFactoryLabelRaw.Value = string.Empty;
        }

        private void ShowError(string message)
        {
            string escapedMessage = message
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\r", string.Empty)
                .Replace("\n", "\\n");

            ClientScript.RegisterStartupScript(
                GetType(),
                "showMessage",
                "showMessageModal('" + escapedMessage + "');",
                true);
        }

        private static decimal ParseDecimal(string value)
        {
            decimal result;
            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)
                ? result
                : 0M;
        }
    }
}
