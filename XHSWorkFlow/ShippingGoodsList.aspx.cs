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
    /// 出货货品浏览页面。
    /// </summary>
    public partial class ShippingGoodsList : Page
    {
        protected string menuname = "";
        private string menuid = "B01";
        private void Page_Load(object sender, EventArgs e)
        {
            menuname = new PartTmenu().findbykey(menuid).Menuname;
            if (Master is DefaultSub master)
            {
                master.Menuname = menuname;
            }
            if (ModuleWorkFlow.BLL.Private.checkPrivate(this, menuid, "PQUERY"))
            {

                if (!IsPostBack)
                {
                    string supplyBatchNo = Request.QueryString["supplyBatchNo"];
                    if (!string.IsNullOrWhiteSpace(supplyBatchNo))
                    {
                        TextBox_SupplyBatchNo.Text = supplyBatchNo.Trim();
                    }

                    BindData();
                }
            }
        }

        private void Search()
        {
            MainDataGrid.CurrentPageIndex = 0;
            BindData();
        }

        protected void lnkbutton_search_Click(object sender, EventArgs e)
        {
            Search();
        }

        protected void lnkbutton_delete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox_SupplyBatchNo.Text) || string.IsNullOrWhiteSpace(TextBox_PartNo.Text))
            {
                ShowMessage("请输入供货批次号和零件编号后再删除。");
                return;
            }

            List<ShippingGoodsInfo> shippingGoodsInfos = new ShippingGoods().GetShippingGoods(
                TextBox_PartNo.Text.Trim(),
                string.Empty,
                TextBox_SupplyBatchNo.Text.Trim());
            string message = new ShippingGoods().SaveDeleteShippingGoods(
                TextBox_SupplyBatchNo.Text.Trim(),
                TextBox_PartNo.Text.Trim());
            if (string.IsNullOrWhiteSpace(message))
            {
                BindData();
                ShowMessage(string.Format("已删除批次“{0}”下零件编号“{1}”的 {2} 条出货货品数据。", TextBox_SupplyBatchNo.Text.Trim(), TextBox_PartNo.Text.Trim(), shippingGoodsInfos.Count));
                return;
            }

            ShowMessage(message);
        }

        protected void DropDownList_CloseStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            Search();
        }

        protected void lnk_upload_Click(object sender, EventArgs e)
        {
            string url = "ShippingGoodsUpload.aspx";
            Response.Redirect(url);
        }

        protected void lnkbutton_edit_Click(object sender, EventArgs e)
        {
            ShippingGoodsInfo shippingGoodsInfo;
            if (!TryGetSingleSelectedShippingGoods(out shippingGoodsInfo))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(shippingGoodsInfo.SupplyBatchNo))
            {
                Label_Message.Text = "所选出货货品数据缺少供货批次号，无法执行编辑。";
                return;
            }

            string url = string.Format(
                "ShippingGoodsView.aspx?func=edit&id={0}&supplyBatchNo={1}",
                shippingGoodsInfo.Id,
                HttpUtility.UrlEncode(shippingGoodsInfo.SupplyBatchNo.Trim()));
            Response.Redirect(url);
        }

        protected void lnkbutton_upload_edit_Click(object sender, EventArgs e)
        {
            ShippingGoodsInfo shippingGoodsInfo;
            if (!TryGetSingleSelectedShippingGoods(out shippingGoodsInfo))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(shippingGoodsInfo.SupplyBatchNo))
            {
                Label_Message.Text = "所选出货货品数据缺少供货批次号，无法执行上传修改。";
                return;
            }

            string url = string.Format(
                "ShippingGoodsUpload.aspx?func=edit&supplyBatchNo={0}",
                HttpUtility.UrlEncode(shippingGoodsInfo.SupplyBatchNo.Trim()));
            Response.Redirect(url);
        }

        protected void MainDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            MainDataGrid.CurrentPageIndex = e.NewPageIndex;
            BindData();
        }

        protected void MainDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            ShippingGoodsInfo shippingGoodsInfo = e.Item.DataItem as ShippingGoodsInfo;
            if (shippingGoodsInfo == null)
            {
                return;
            }

            HyperLink hyperLinkPrintCount = e.Item.FindControl("HyperLink_PrintCount") as HyperLink;
            Label labelPrintCount = e.Item.FindControl("Label_PrintCount") as Label;
            if (hyperLinkPrintCount == null || labelPrintCount == null)
            {
                return;
            }

            string printCountText = shippingGoodsInfo.PrintCount.HasValue
                ? shippingGoodsInfo.PrintCount.Value.ToString()
                : "0";

            if (!shippingGoodsInfo.PrintCount.HasValue || shippingGoodsInfo.PrintCount.Value <= 0)
            {
                hyperLinkPrintCount.Visible = false;
                labelPrintCount.Text = printCountText;
                labelPrintCount.Visible = true;
                return;
            }

            string url = string.Format(
                "api/print/record-detail.ashx?supplyBatchNo={0}&partNo={1}&cartonNo={2}",
                HttpUtility.UrlEncode((shippingGoodsInfo.SupplyBatchNo ?? string.Empty).Trim()),
                HttpUtility.UrlEncode((shippingGoodsInfo.PartNo ?? string.Empty).Trim()),
                HttpUtility.UrlEncode((shippingGoodsInfo.CartonNo ?? string.Empty).Trim()));

            hyperLinkPrintCount.Text = printCountText;
            hyperLinkPrintCount.NavigateUrl = "javascript:void(0);";
            hyperLinkPrintCount.Attributes["class"] = "print-record-link";
            hyperLinkPrintCount.Attributes["onclick"] = string.Format("return openPrintRecordModal('{0}');", url);
            labelPrintCount.Visible = false;
        }

        private void BindData()
        {
            List<ShippingGoodsInfo> shippingGoodsInfos = new ShippingGoods().GetShippingGoods(
                TextBox_PartNo.Text.Trim(),
                TextBox_PartName.Text.Trim(),
                TextBox_SupplyBatchNo.Text.Trim(),
                DropDownList_CloseStatus.SelectedValue);

            MainDataGrid.DataKeyField = "Id";
            MainDataGrid.DataSource = shippingGoodsInfos;
            MainDataGrid.DataBind();
            Label_Message.Text = string.Format("共查询到 {0} 条出货货品数据。", shippingGoodsInfos.Count);
        }

        private bool TryGetSingleSelectedShippingGoods(out ShippingGoodsInfo shippingGoodsInfo)
        {
            shippingGoodsInfo = null;
            List<ShippingGoodsInfo> selectedShippingGoodsInfos = GetSelectedShippingGoods();
            int selectedCount = selectedShippingGoodsInfos.Count;

            if (selectedCount == 0)
            {
                ShowMessage("请选择一条出货货品数据。");
                return false;
            }

            if (selectedCount > 1)
            {
                ShowMessage("只能选择一条出货货品数据。");
                return false;
            }

            shippingGoodsInfo = selectedShippingGoodsInfos[0];
            return true;
        }

        private List<ShippingGoodsInfo> GetSelectedShippingGoods()
        {
            List<ShippingGoodsInfo> shippingGoodsInfos = new ShippingGoods().GetShippingGoods(
                TextBox_PartNo.Text.Trim(),
                TextBox_PartName.Text.Trim(),
                TextBox_SupplyBatchNo.Text.Trim(),
                DropDownList_CloseStatus.SelectedValue);
            List<ShippingGoodsInfo> selectedShippingGoodsInfos = new List<ShippingGoodsInfo>();

            foreach (DataGridItem item in MainDataGrid.Items)
            {
                if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
                {
                    continue;
                }

                CheckBox checkBox = item.FindControl("chk_datagrid") as CheckBox;
                if (checkBox == null || !checkBox.Checked)
                {
                    continue;
                }

                object dataKey = MainDataGrid.DataKeys[item.ItemIndex];
                if (dataKey == null)
                {
                    continue;
                }

                int selectedId;
                if (!int.TryParse(dataKey.ToString(), out selectedId))
                {
                    continue;
                }

                ShippingGoodsInfo selectedShippingGoodsInfo = shippingGoodsInfos.FirstOrDefault(current => current != null && current.Id == selectedId);
                if (selectedShippingGoodsInfo != null)
                {
                    selectedShippingGoodsInfos.Add(selectedShippingGoodsInfo);
                }
            }

            return selectedShippingGoodsInfos;
        }

        private void ShowMessage(string message)
        {
            Label_Message.Text = message;

            string script = string.Format(
                "showMessageModal('{0}');",
                HttpUtility.JavaScriptStringEncode(message ?? string.Empty));
            ClientScript.RegisterStartupScript(GetType(), "ShippingGoodsListMessage", script, true);
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
