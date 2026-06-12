using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.UI;
using BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>
    /// 出货单上传页面。
    /// </summary>
    public partial class ShippingGoodsUpload : Page
    {
        protected string menuname = "上传出货单";

        private void Page_Load(object sender, EventArgs e)
        {
            if (Master is DefaultSub master)
            {
                master.Menuname = menuname;
            }
        }

        protected void btn_upload_Click(object sender, EventArgs e)
        {
            UploadShippingGoods();
        }

        protected void lnkbutton_upload_Click(object sender, EventArgs e)
        {
            UploadShippingGoods();
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

            string saveMessage = new ShippingGoods().InsertShippingGoods(shippingGoodsInfos);
            ShowMessage(string.IsNullOrWhiteSpace(saveMessage)
                ? string.Format("上传成功，已导入 {0} 条出货货品数据。", shippingGoodsInfos.Count)
                : saveMessage);
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
