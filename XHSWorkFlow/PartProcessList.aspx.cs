using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using ModuleWorkFlow.BLL;
using XHS.Model;
using XHS.service.DateCode;

namespace ModuleWorkFlow
{
    /// <summary>
    /// 按客户查询零件工序及二维码的页面。
    /// </summary>
    public partial class PartProcessList : Page
    {
        protected string menuname = "";
        private string customerAbbr = "XWD";

        private string MenuId
        {
            get { return customerAbbr.Equals("SQSD", StringComparison.OrdinalIgnoreCase) ? "B10" : "B09"; }
        }

        protected override void OnInit(EventArgs e)
        {
            Load += Page_Load;
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            customerAbbr = GetCustomerAbbr();

            if (!IsSupportedCustomer(customerAbbr))
            {
                Label_Message.Text = "客户编号无效。";
                return;
            }

            menuname = new PartTmenu().findbykey(MenuId).Menuname;
            if (Master is DefaultSub master)
            {
                master.Menuname = menuname;
            }

            if (Session["userid"] == null)
            {
                Response.Redirect("login.aspx");
                return;
            }

            if (!ModuleWorkFlow.BLL.Private.checkPrivate(this, MenuId, "PQUERY"))
            {
                return;
            }

            if (!IsPostBack)
            {
                BindPartNoOptions();
                BindData();
            }
        }

        protected void lnkbutton_search_Click(object sender, EventArgs e)
        {
            MainDataGrid.CurrentPageIndex = 0;
            BindData();
        }

        protected void MainDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            MainDataGrid.CurrentPageIndex = e.NewPageIndex;
            BindData();
        }

        protected void MainDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item
                && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            Label printDateLabel = e.Item.FindControl("Label_PrintDate") as Label;
            if (printDateLabel == null)
            {
                return;
            }

            string barcode = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PartNo_Id"));
            printDateLabel.Text = HttpUtility.HtmlEncode(DecodePrintDate(barcode));
        }

        private string DecodePrintDate(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
            {
                return string.Empty;
            }

            try
            {
                int dateOffset = GetProductionDateOffset();
                if (dateOffset < 0 || barcode.Length < dateOffset + 3)
                {
                    return string.Empty;
                }

                string dateCode = barcode.Substring(dateOffset, 3);
                string configDirectory = Server.MapPath("~/" + GetFormatterConfigDirectory());
                XinWangDaDateCodeProvider provider = new XinWangDaDateCodeProvider(
                    Path.Combine(configDirectory, "Date.json"));
                DateTime printDate = (DateTime)provider.Decode(dateCode);
                return printDate.ToString("yyyy-MM-dd");
            }
            catch (FormatException)
            {
                return string.Empty;
            }
            catch (ArgumentException)
            {
                return string.Empty;
            }
        }

        private int GetProductionDateOffset()
        {
            string uiPath = Server.MapPath("~/" + GetFormatterConfigDirectory() + "/UI.json");
            if (!File.Exists(uiPath))
            {
                return -1;
            }

            BarcodeUiConfig config = new JavaScriptSerializer()
                .Deserialize<BarcodeUiConfig>(File.ReadAllText(uiPath));
            if (config == null || config.Fields == null)
            {
                return -1;
            }

            int offset = 0;
            foreach (BarcodeUiField field in config.Fields)
            {
                if (field == null || string.IsNullOrWhiteSpace(field.Name))
                {
                    continue;
                }

                if (string.Equals(field.Name, "ProductionDate", StringComparison.OrdinalIgnoreCase))
                {
                    return offset;
                }

                if (field.IncludeInBarcode != false)
                {
                    offset += field.Length;
                }
            }

            return -1;
        }

        private void BindPartNoOptions()
        {
            List<PartMasterInfo> parts = new XHS.BLL.PartMaster()
                .GetPartByCustomerAbbr(customerAbbr);

            DropDownList_PartNo.Items.Clear();
            DropDownList_PartNo.Items.Add(new ListItem("全部", string.Empty));
            foreach (PartMasterInfo part in parts ?? new List<PartMasterInfo>())
            {
                string value = (part.JHSPartNo ?? string.Empty).Trim();
                if (value.Length == 0)
                {
                    continue;
                }

                string text = value;
                if (!string.IsNullOrWhiteSpace(part.MaterialName))
                {
                    text += " - " + part.MaterialName.Trim();
                }

                DropDownList_PartNo.Items.Add(new ListItem(text, value));
            }
        }

        private void BindData()
        {
            try
            {
                DateTime? startDate;
                DateTime? endDate;
                if (!TryReadDateRange(out startDate, out endDate))
                {
                    return;
                }

                List<string> dateCodes = EncodeDateRange(startDate, endDate);
                List<XHSPartPartProcessInfo> rows = new XHS.BLL.XHSPartPartProcess()
                    .GetPartProcesses(
                        customerAbbr,
                        DropDownList_PartNo.SelectedValue,
                        TextBox_Barcode.Text.Trim(),
                        dateCodes);

                MainDataGrid.DataSource = rows;
                MainDataGrid.DataBind();
                Label_Message.Text = string.Format("共查询到 {0} 条零件工序。", rows.Count);
            }
            catch (Exception ex)
            {
                MainDataGrid.DataSource = null;
                MainDataGrid.DataBind();
                Label_Message.Text = ex.Message;
            }
        }

        private bool TryReadDateRange(out DateTime? startDate, out DateTime? endDate)
        {
            startDate = ParseDate(TextBox_StartDate.Text);
            endDate = ParseDate(TextBox_EndDate.Text);
            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            {
                Label_Message.Text = "生产日期起不能晚于生产日期止。";
                return false;
            }

            return true;
        }

        private List<string> EncodeDateRange(DateTime? startDate, DateTime? endDate)
        {
            List<string> result = new List<string>();
            if (!startDate.HasValue && !endDate.HasValue)
            {
                return result;
            }

            DateTime start = startDate ?? endDate.Value;
            DateTime end = endDate ?? startDate.Value;
            if ((end - start).TotalDays > 366)
            {
                throw new InvalidOperationException("生产日期范围不能超过366天。" );
            }

            string configDirectory = Server.MapPath("~/" + GetFormatterConfigDirectory());
            XinWangDaDateCodeProvider provider = new XinWangDaDateCodeProvider(
                Path.Combine(configDirectory, "Date.json"));
            for (DateTime date = start.Date; date <= end.Date; date = date.AddDays(1))
            {
                result.Add(provider.Encode(date));
            }

            return result.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        private string GetCustomerAbbr()
        {
            string value = Request.QueryString["CustomerAbbr"];
            return string.IsNullOrWhiteSpace(value) ? "XWD" : value.Trim().ToUpperInvariant();
        }

        private bool IsSupportedCustomer(string value)
        {
            string path = Server.MapPath("~/Config/BarcodeCustomers.json");
            if (!File.Exists(path))
            {
                return false;
            }

            List<CustomerConfig> customers = new JavaScriptSerializer()
                .Deserialize<List<CustomerConfig>>(File.ReadAllText(path));
            return (customers ?? new List<CustomerConfig>())
                .Any(item => item != null && string.Equals(item.CustomerAbbr, value, StringComparison.OrdinalIgnoreCase));
        }

        private string GetFormatterConfigDirectory()
        {
            return "Config/" + (customerAbbr.Equals("SQSD", StringComparison.OrdinalIgnoreCase) ? "SAIC" : "XWD");
        }

        private static DateTime? ParseDate(string value)
        {
            DateTime result;
            return DateTime.TryParse(value, out result) ? result.Date : (DateTime?)null;
        }

        private sealed class CustomerConfig
        {
            public string CustomerAbbr { get; set; }
        }

        private sealed class BarcodeUiConfig
        {
            public List<BarcodeUiField> Fields { get; set; }
        }

        private sealed class BarcodeUiField
        {
            public string Name { get; set; }
            public int Length { get; set; }
            public bool? IncludeInBarcode { get; set; }
        }
    }
}
