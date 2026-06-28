using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.UI;
using BLL;
using ModuleWorkFlow.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>
    /// 出货单上传页面。
    /// </summary>
    public partial class ShippingGoodsUpload : Page
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

            SaveRequestParameters();

            string privateCode = IsUpdateMode() ? "PEDIT" : "PADD";
            if (ModuleWorkFlow.BLL.Private.checkPrivate(this, menuid, privateCode))
            {
                if (Session["userid"] == null)
                {
                    Response.Redirect("login.aspx");
                }
                else
                {
                    lab_UserName.Text = Session["userid"].ToString();
                }
            }

            if (!IsPostBack)
            {
                InitializeUploadMode();
            }
        }

        protected void btn_upload_Click(object sender, EventArgs e)
        {
            UploadShippingGoods();
        }

        protected void lnk_view_Click(object sender, EventArgs e)
        {
            string url = $"ShippingGoodsList.aspx?supplyBatchNo={lab_supplyBatchNo.Text.Trim()}";
            Response.Redirect (url);
        }

        private void UploadShippingGoods()
        {
            if (!FileUploadShippingGoods.HasFile)
            {
                ShowMessage("请选择需要上传的出货单。");
                return;
            }

            string extension = Path.GetExtension(FileUploadShippingGoods.FileName);
            if (!string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(extension, ".xls", StringComparison.OrdinalIgnoreCase))
            {
                ShowMessage("只允许上传 Excel 文件。");
                return;
            }

            string uploadPath = GetUploadPath();
            Directory.CreateDirectory(uploadPath);

            string fileName = string.Format("{0}_{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetFileName(FileUploadShippingGoods.FileName));
            string fullName = Path.Combine(uploadPath, fileName);
            FileUploadShippingGoods.SaveAs(fullName);

            List<string> messageList;
            List<ShippingGoodsInfo> shippingGoodsInfos = new UnRegularTableImport().GetList<ShippingGoodsInfo>(fullName, out messageList);
            if (messageList.Count > 0)
            {
                ShowMessage(string.Join("<br />", messageList.ToArray()));
                return;
            }

            if (shippingGoodsInfos.Count == 0)
            {
                ShowMessage("没有读取到可导入的出货货品数据。");
                return;
            }

            if (IsUpdateMode())
            {
                UpdateShippingGoodsByUpload(shippingGoodsInfos);
                return;
            }

            DateTime uploadDate = DateTime.Now;
            ShippingGoods shippingGoods = new ShippingGoods();
            foreach(ShippingGoodsInfo sgi in shippingGoodsInfos)
            {
                List<ShippingGoodsInfo> duplicateShippingGoodsInfos = shippingGoods.GetShippingGoodsBySupplyBatchNo(sgi.SupplyBatchNo, sgi.CartonNo);
                if (duplicateShippingGoodsInfos.Count > 0)
                {
                    ShowMessage(string.Format("供货批次号“{0}”下的纸箱编号“{1}”已存在，不允许重复上传。", sgi.SupplyBatchNo, sgi.CartonNo));
                    return;
                }

                sgi.Status = ShippingGoodsStatusInfo.UnPrinted;
                sgi.Creater = lab_UserName.Text.Trim();
                sgi.CreatDate = uploadDate;
                lab_supplyBatchNo.Text = sgi.SupplyBatchNo;


            }
            string saveMessage = shippingGoods.InsertShippingGoods(shippingGoodsInfos);
            ShowMessage(string.IsNullOrWhiteSpace(saveMessage)
                ? string.Format("上传成功，已导入 {0} 条出货货品数据。", shippingGoodsInfos.Count)
                : saveMessage);
        }

        private void InitializeUploadMode()
        {
            if (!IsUpdateMode())
            {
                return;
            }

            string supplyBatchNo = GetTargetSupplyBatchNo();
            lab_supplyBatchNo.Text = supplyBatchNo;
            btn_upload.Text = "上传修改";
            Label_Message.Text = string.IsNullOrWhiteSpace(supplyBatchNo)
                ? "当前为上传修改模式，请从列表页选择需要修改的批次后进入。"
                : string.Format("当前为上传修改模式，目标批次：{0}。", supplyBatchNo);
        }

        private void SaveRequestParameters()
        {
            string supplyBatchNo = Request.QueryString["supplyBatchNo"];
            if (!string.IsNullOrWhiteSpace(supplyBatchNo))
            {
                lab_supplyBatchNo.Text = supplyBatchNo.Trim();
            }
        }

        private void UpdateShippingGoodsByUpload(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            string targetSupplyBatchNo = GetTargetSupplyBatchNo();
            if (string.IsNullOrWhiteSpace(targetSupplyBatchNo))
            {
                ShowMessage("上传修改模式缺少供货批次号，请从列表页选择一条记录后进入。");
                return;
            }

            ShippingGoods shippingGoods = new ShippingGoods();
            string saveMessage = shippingGoods.SaveUploadEditShippingGoods(targetSupplyBatchNo, lab_UserName.Text.Trim(), shippingGoodsInfos);
            ShowMessage(string.IsNullOrWhiteSpace(saveMessage)
                ? string.Format("上传修改成功，已处理 {0} 条出货货品数据。", shippingGoodsInfos.Count)
                : saveMessage);
        }

        private bool IsUpdateMode()
        {
            return string.Equals(Request.QueryString["mode"], "update", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(Request.QueryString["func"], "edit", StringComparison.OrdinalIgnoreCase);
        }

        private string GetTargetSupplyBatchNo()
        {
            if (!string.IsNullOrWhiteSpace(lab_supplyBatchNo.Text))
            {
                return lab_supplyBatchNo.Text.Trim();
            }

            string supplyBatchNo = Request.QueryString["supplyBatchNo"];
            return string.IsNullOrWhiteSpace(supplyBatchNo) ? string.Empty : supplyBatchNo.Trim();
        }

        private string GetUploadPath()
        {
            string configuredPath = ConfigurationSettings.AppSettings["ShippingGoodsUploadPath"];
            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                throw new ApplicationException("未配置 ShippingGoodsUploadPath。");
            }

            return configuredPath.StartsWith("~", StringComparison.Ordinal)
                ? Server.MapPath(configuredPath)
                : configuredPath;
        }

        private void ShowMessage(string message)
        {
            Label_Message.Text = message;

            string alertMessage = HttpUtility.HtmlDecode(message)
                .Replace("<br />", "\n")
                .Replace("<br/>", "\n")
                .Replace("<br>", "\n");
            string script = string.Format("showMessageModal('{0}');", HttpUtility.JavaScriptStringEncode(alertMessage));
            ClientScript.RegisterStartupScript(GetType(), "ShippingGoodsUploadMessage", script, true);
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
