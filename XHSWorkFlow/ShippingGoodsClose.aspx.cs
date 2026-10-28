using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using ModuleWorkFlow.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>
    /// 出货货品结案页面。
    /// </summary>
    public partial class ShippingGoodsClose : Page
    {
        private const string MenuId = "B06";
        protected string menuname = "";

        private void Page_Load(object sender, EventArgs e)
        {
            menuname = new PartTmenu().findbykey(MenuId).Menuname;
            if (Master is DefaultSub master)
            {
                master.Menuname = menuname;
            }

            if (!Private.checkPrivate(this, MenuId, "PEDIT"))
            {
                return;
            }

            if (Session["userid"] == null)
            {
                Response.Redirect("login.aspx");
                return;
            }

            InitializeScanTextBoxes();

            if (!IsPostBack)
            {
                BindData(new List<ShippingGoodsInfo>(), "请先扫描批次、零件编号和配送单号。", false);
            }
        }

        protected void TextBox_SupplyBatchNo_TextChanged(object sender, EventArgs e)
        {
            MainDataGrid.CurrentPageIndex = 0;
            if (string.IsNullOrWhiteSpace(SafeValue(TextBox_SupplyBatchNo.Text)))
            {
                TextBox_PartNo.Text = string.Empty;
                TextBox_DeliveryNo.Text = string.Empty;
                BindData(new List<ShippingGoodsInfo>(), "请先扫描批次。", false);
                return;
            }

            TextBox_PartNo.Text = string.Empty;
            TextBox_DeliveryNo.Text = string.Empty;
            BindData(new List<ShippingGoodsInfo>(), "请继续扫描零件编号。", false);
            TextBox_PartNo.Focus();
        }

        protected void TextBox_PartNo_TextChanged(object sender, EventArgs e)
        {
            MainDataGrid.CurrentPageIndex = 0;
            TextBox_DeliveryNo.Text = string.Empty;
            BindData(new List<ShippingGoodsInfo>(), "请继续扫描配送单号。", false);
            TextBox_DeliveryNo.Focus();
        }

        protected void TextBox_DeliveryNo_TextChanged(object sender, EventArgs e)
        {
            MainDataGrid.CurrentPageIndex = 0;
            BindCurrentShippingGoods(true);
        }

        protected void lnk_view_Click(object sender, EventArgs e)
        {
            Response.Redirect("ShippingGoodsList.aspx");
        }

        protected void lnkbutton_save_Click(object sender, EventArgs e)
        {
            SaveShippingGoodsClose();
        }

        private void InitializeScanTextBoxes()
        {
            TextBox_SupplyBatchNo.Attributes["autocomplete"] = "off";
            TextBox_PartNo.Attributes["autocomplete"] = "off";
        }

        private void BindCurrentShippingGoods(bool showPopupWhenMissing)
        {
            List<ShippingGoodsInfo> shippingGoodsInfos = GetExactShippingGoods();
            if (shippingGoodsInfos.Count == 0)
            {
                string message = string.Format(
                    "未找到批次“{0}”、零件编号“{1}”与配送单号“{2}”对应的出货货品数据。",
                    SafeValue(TextBox_SupplyBatchNo.Text),
                    SafeValue(TextBox_PartNo.Text),
                    SafeValue(TextBox_DeliveryNo.Text));
                BindData(shippingGoodsInfos, message, showPopupWhenMissing);
                return;
            }

            BindData(
                shippingGoodsInfos,
                string.Format("已带出 {0} 条待结案数据。", shippingGoodsInfos.Count),
                false);
        }

        private List<ShippingGoodsInfo> GetExactShippingGoods()
        {
            string supplyBatchNo = SafeValue(TextBox_SupplyBatchNo.Text);
            string partNo = SafeValue(TextBox_PartNo.Text);
            string deliveryNo = SafeValue(TextBox_DeliveryNo.Text);
            if (string.IsNullOrWhiteSpace(supplyBatchNo) || string.IsNullOrWhiteSpace(partNo) || string.IsNullOrWhiteSpace(deliveryNo))
            {
                return new List<ShippingGoodsInfo>();
            }

            List<ShippingGoodsInfo> shippingGoodsInfos = new ShippingGoods().GetShippingGoods(partNo, string.Empty, supplyBatchNo);
            return shippingGoodsInfos
                .Where(item => item != null &&
                    string.Equals(SafeValue(item.SupplyBatchNo), supplyBatchNo, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(SafeValue(item.PartNo), partNo, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(SafeValue(item.DeliveryNo), deliveryNo, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void BindData(List<ShippingGoodsInfo> shippingGoodsInfos, string message, bool showPopup)
        {
            MainDataGrid.DataKeyField = "Id";
            MainDataGrid.DataSource = shippingGoodsInfos;
            MainDataGrid.DataBind();
            ShowMessage(message, showPopup);
        }

        private void SaveShippingGoodsClose()
        {
            string supplyBatchNo = SafeValue(TextBox_SupplyBatchNo.Text);
            string partNo = SafeValue(TextBox_PartNo.Text);
            string deliveryNo = SafeValue(TextBox_DeliveryNo.Text);
            if (string.IsNullOrWhiteSpace(supplyBatchNo) || string.IsNullOrWhiteSpace(partNo) || string.IsNullOrWhiteSpace(deliveryNo))
            {
                ShowMessage("请先扫描批次、零件编号和配送单号，再执行结案保存。", true);
                return;
            }

            List<ShippingGoodsInfo> shippingGoodsInfos = GetExactShippingGoods();
            if (shippingGoodsInfos.Count == 0)
            {
                ShowMessage(
                    string.Format("未找到批次“{0}”、零件编号“{1}”与配送单号“{2}”对应的数据，无法结案。", supplyBatchNo, partNo, deliveryNo),
                    true);
                BindData(new List<ShippingGoodsInfo>(), Label_Message.Text, false);
                return;
            }

            string currentUser = SafeValue(Session["userid"] == null ? string.Empty : Session["userid"].ToString());
            string saveMessage = new ShippingGoods().UpdateShippingGoodsClose(
                supplyBatchNo,
                partNo,
                deliveryNo,
                currentUser,
                DateTime.Now);
            if (!string.IsNullOrWhiteSpace(saveMessage))
            {
                ShowMessage(saveMessage, true);
                return;
            }

            BindCurrentShippingGoods(false);
            ShowMessage(string.Format("结案成功，共更新 {0} 条数据。", shippingGoodsInfos.Count), false);
        }

        private void ShowMessage(string message, bool showPopup)
        {
            string safeMessage = SafeValue(message);
            Label_Message.Text = safeMessage;

            if (!showPopup)
            {
                return;
            }

            string script = string.Format(
                "showMessageModal('{0}');",
                HttpUtility.JavaScriptStringEncode(safeMessage));
            ClientScript.RegisterStartupScript(GetType(), "ShippingGoodsCloseMessage", script, true);
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public static string[] GetSupplyBatchNoSuggestions(string prefixText, int count)
        {
            string prefix = SafeValue(prefixText);
            List<ShippingGoodsInfo> shippingGoodsInfos = new ShippingGoods().GetShippingGoods(string.Empty, string.Empty, prefix);

            return shippingGoodsInfos
                .Where(item => item != null && !string.IsNullOrWhiteSpace(item.SupplyBatchNo))
                .Select(item => item.SupplyBatchNo.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item)
                .Take(count > 0 ? count : 12)
                .ToArray();
        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public static string[] GetPartNoSuggestions(string prefixText, int count, string contextKey)
        {
            string prefix = SafeValue(prefixText);
            string supplyBatchNo = SafeValue(contextKey);
            List<ShippingGoodsInfo> shippingGoodsInfos = new ShippingGoods().GetShippingGoods(prefix, string.Empty, supplyBatchNo);

            return shippingGoodsInfos
                .Where(item => item != null && !string.IsNullOrWhiteSpace(item.PartNo))
                .Select(item => item.PartNo.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item)
                .Take(count > 0 ? count : 12)
                .ToArray();
        }

        #region Web Form Designer generated code
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            Load += new EventHandler(Page_Load);
        }
        #endregion
    }
}
