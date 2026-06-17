using System;
using System.Collections.Generic;
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

        protected void lnk_upload_Click(object sender, EventArgs e)
        {
            string url = "ShippingGoodsUpload.aspx";
            Response.Redirect(url);
        }

        protected void MainDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            MainDataGrid.CurrentPageIndex = e.NewPageIndex;
            BindData();
        }

        private void BindData()
        {
            List<ShippingGoodsInfo> shippingGoodsInfos = new ShippingGoods().GetShippingGoods(
                TextBox_PartNo.Text.Trim(),
                TextBox_PartName.Text.Trim(),
                TextBox_SupplyBatchNo.Text.Trim());

            MainDataGrid.DataSource = shippingGoodsInfos;
            MainDataGrid.DataBind();
            Label_Message.Text = string.Format("共查询到 {0} 条出货货品数据。", shippingGoodsInfos.Count);
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
