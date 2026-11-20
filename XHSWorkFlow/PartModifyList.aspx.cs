using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

using ModuleWorkFlow.business;
using ModuleWorkFlow.BLL;
using ModuleWorkFlow.Model;
using ModuleWorkFlow.Model.System;
using ModuleWorkFlow.BLL.System;
using UtilityData = Utility.Data;

namespace ModuleWorkFlow
{
    /// <summary>
    /// Summary description for PartModifyList.
    /// </summary>
    public class PartModifyList : System.Web.UI.Page
    {
        //protected ModuleWorkFlow.controls.Style Style;
        protected System.Web.UI.WebControls.DropDownList Dropdownlist_ModuleId;
        protected System.Web.UI.WebControls.DataGrid MainDataGrid;
        protected System.Web.UI.WebControls.DataGrid DetailDatagrid;
        protected System.Web.UI.WebControls.Label lab_totalpagte;
        protected System.Web.UI.WebControls.Label lab_nowpage;
        protected System.Web.UI.WebControls.TextBox txtGoPage;


        protected TextBox txt_partnoid;
        protected System.Web.UI.WebControls.Label Label_Message;
        protected System.Web.UI.WebControls.Label lab_seach_moduleid;
        protected Label lab_type;
        protected string menuid = "F32";
        protected string menuname = "";
        private readonly Dictionary<string, string> customerNameMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private static int MODULEID = 0;
        private static int PARTNO = 1;
        private static int PRIORITY = 2;

        private void Page_Load(object sender, System.EventArgs e)
        {
            PartTmenuInfo menuInfo = new PartTmenu().findbykey(menuid);
            if (menuInfo != null)
            {
                menuname = menuInfo.Menuname;
            }

            if (Request.Params["type"] != null && !Request.Params["type"].Trim().Equals(""))
            {
                lab_type.Text = "design";
            }
            else
            {
                lab_type.Text = "";
            }
            // Put user code to initialize the page here
            if (!this.IsPostBack)
            {
                //舦北
                // ModuleWorkFlow.BLL.Private.checkPrivate(this, menuid, "PQUERY");

                //				Common.DDL_BindData_CustomerList(Dropdownlist_Customer);
                //				Common.DDL_BindData_ModuleList(Dropdownlist_ModuleId,Dropdownlist_Customer.SelectedValue);
                bindModuleid();

                if (Request.Params["moduleid"] != null)
                {
                    //					string customerid = Request.Params["customerid"].ToString();
                    string moduleid = Request.Params["moduleid"].ToString();

                    //					Methods.DropDownListChange(Dropdownlist_Customer,customerid);
                    //					Common.DDL_BindData_ModuleList(Dropdownlist_ModuleId,Dropdownlist_Customer.SelectedValue);
                    bindModuleid();
                    Methods.DropDownListChange(Dropdownlist_ModuleId, moduleid);
                    try
                    {
                        int pageIndex = Convert.ToInt32(Request.Params["pageindex"]);
                        MainDataGrid.CurrentPageIndex = pageIndex;
                    }
                    catch
                    {
                    }
                }

                bindDataByModuleId(Dropdownlist_ModuleId.SelectedValue);

            }
            Label_Message.Text = "";
            string msg = Request.Params["msg"];
            if (msg != null && msg.Length > 0) Label_Message.Text = msg;
        }

        private void bindModuleid()
        {
            string configPath = Server.MapPath("~/Config/BarcodeCustomers.json");
            List<CustomerConfig> customers = new JavaScriptSerializer()
                .Deserialize<List<CustomerConfig>>(File.ReadAllText(configPath));

            Dropdownlist_ModuleId.Items.Clear();
            foreach (CustomerConfig customer in customers ?? new List<CustomerConfig>())
            {
                if (customer == null || string.IsNullOrWhiteSpace(customer.CustomerAbbr))
                {
                    continue;
                }

                Dropdownlist_ModuleId.Items.Add(new ListItem(
                    customer.CustomerName ?? customer.CustomerAbbr,
                    customer.CustomerAbbr));
                customerNameMap[customer.CustomerAbbr] =
                    customer.CustomerName ?? customer.CustomerAbbr;
            }

            if (Dropdownlist_ModuleId.Items.FindByValue("SQSD") != null)
            {
                Dropdownlist_ModuleId.SelectedValue = "SQSD";
            }
        }

        private void bindDataByModuleId(string moduleid)
        {
            Label_Message.Text = "";

            List<XHS.Model.XHSPartPartProcessInfo> processList =
                new XHS.BLL.XHSPartPartProcess().GetPartProcesses(
                    moduleid,
                    "",
                    txt_partnoid.Text.Trim(),
                    null);

            Dictionary<string, PartModifyListRow> rowMap =
                new Dictionary<string, PartModifyListRow>(StringComparer.OrdinalIgnoreCase);
            foreach (XHS.Model.XHSPartPartProcessInfo process in processList)
            {
                string partNoId = Convert.ToString(process.PartNo_Id).Trim();
                if (partNoId.Length == 0)
                {
                    continue;
                }

                PartModifyListRow row;
                if (!rowMap.TryGetValue(partNoId, out row))
                {
                    row = new PartModifyListRow
                    {
                        ModuleId = process.ModuleId,
                        CustomerName = customerNameMap.ContainsKey(process.ModuleId ?? "")
                            ? customerNameMap[process.ModuleId]
                            : process.ModuleId,
                        PartNo = process.PartNo,
                        PartNo_Id = partNoId,
                        Priority = 100,
                        priorityname = "中",
                        ProcessNo = process.ProcessNo
                    };
                    rowMap.Add(partNoId, row);
                }

                string processName = Convert.ToString(process.ProcessName).Trim();
                if (processName.Length == 0)
                {
                    processName = Convert.ToString(process.ProcessId).Trim();
                }
                string processKey = process.ProcessNo.HasValue && process.ProcessNo.Value > 0
                    ? process.ProcessNo.Value.ToString()
                    : Convert.ToString(process.ProcessId).Trim() + "#" + Convert.ToString(process.ProcessOrder).Trim();
                if (processName.Length > 0 && row.ProcessKeys.Add(processKey))
                {
                    if (row.ProcessNameHtml.Length > 0)
                    {
                        row.ProcessNameHtml += "、";
                    }
                    string encodedProcessName = HttpUtility.HtmlEncode(processName);
                    row.ProcessNameHtml += string.Equals(Convert.ToString(process.StatusId).Trim(), "Ready", StringComparison.OrdinalIgnoreCase)
                        ? "<span style=\"color:#0d6efd;\">" + encodedProcessName + "</span>"
                        : encodedProcessName;
                }
            }

            List<PartModifyListRow> allRows = new List<PartModifyListRow>(rowMap.Values);
            allRows.Sort(delegate (PartModifyListRow left, PartModifyListRow right)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(left.PartNo_Id, right.PartNo_Id);
            });

            int pageSize = 10;
            int totalPages = Math.Max(1, (allRows.Count + pageSize - 1) / pageSize);
            if (MainDataGrid.CurrentPageIndex >= totalPages)
            {
                MainDataGrid.CurrentPageIndex = totalPages - 1;
            }

            int startIndex = MainDataGrid.CurrentPageIndex * pageSize;
            List<PartModifyListRow> pageRows = allRows
                .GetRange(startIndex, Math.Min(pageSize, allRows.Count - startIndex));

            lab_totalpagte.Text = totalPages.ToString();
            lab_nowpage.Text = (MainDataGrid.CurrentPageIndex + 1).ToString();
            MainDataGrid.VirtualItemCount = allRows.Count;
            MainDataGrid.PageSize = pageSize;
            MainDataGrid.DataSource = pageRows;
            MainDataGrid.DataBind();
        }

        private sealed class PartModifyListRow
        {
            public string ModuleId { get; set; }
            public string CustomerName { get; set; }
            public string PartNo { get; set; }
            public string PartNo_Id { get; set; }
            public int Priority { get; set; }
            public string priorityname { get; set; }
            public string ProcessNameHtml { get; set; }
            public int? ProcessNo { get; set; }
            public HashSet<string> ProcessKeys { get; private set; }
            public string DataTTid { get; set; }
            public string ParentDataTTid { get; set; }

            public PartModifyListRow()
            {
                ProcessNameHtml = "";
                ProcessKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }




    

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Dropdownlist_ModuleId.SelectedIndexChanged += new System.EventHandler(this.Dropdownlist_ModuleId_SelectedIndexChanged);
            this.MainDataGrid.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.MainDataGrid_ItemCommand);
            this.MainDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.MainDataGrid_PageIndexChanged);
            this.MainDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.MainDataGrid_ItemDataBound);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        private void MainDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            MainDataGrid.CurrentPageIndex = e.NewPageIndex;
            if (lab_seach_moduleid.Text.Trim().Equals(""))
            {
                bindDataByModuleId(Dropdownlist_ModuleId.SelectedValue);
            }
            else
            {
                bindDataByModuleId(lab_seach_moduleid.Text.Trim());
            }
        }

        private void MainDataGrid_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            switch (((LinkButton)e.CommandSource).CommandName)
            {

                case "Detail":
                    string moduleid = e.Item.Cells[0].Text.Trim();
                    string partnoid = e.Item.Cells[1].Text.Trim();
                    bindDataDetail(moduleid, partnoid);
                    break;

                default:
                    // Do nothing.
                    break;

            }
        }

        private void bindDataDetail(string moduleid, string partno)
        {
            DetailDatagrid.DataSource = ModuleWorkFlow.business.PartProcess.getPartIDProcessByProcessViewNew(moduleid, partno);
            DetailDatagrid.DataBind();

            //get hours per day from process table
            Hashtable htProcess = ModuleWorkFlow.business.Process.getProcessHoursPerDay();

            for (int i = 0; i < DetailDatagrid.Items.Count; i++)
            {
                string oldprocess = DetailDatagrid.Items[i].Cells[1].Text.Trim();
                DetailDatagrid.Items[i].Cells[1].Text = Methods.TranslateProcessName(oldprocess);
                int minutes = Convert.ToInt32(DetailDatagrid.Items[i].Cells[4].Text);
                DetailDatagrid.Items[i].Cells[4].Text = Methods.FormatMinuteToDayHourMinute(minutes, Convert.ToInt32(htProcess[oldprocess]));
            }
        }

        private void MainDataGrid_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            Label lab_PartNo_Id = e.Item.FindControl("lab_PartNo_Id") as Label;
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                if (lab_PartNo_Id == null)
                {
                    lab_PartNo_Id = e.Item.FindControl("lab_PartNo_Id_Visible") as Label;
                }

                //if (e.Item.Cells[2].Text.Equals("999-1"))
                if (lab_PartNo_Id != null && lab_PartNo_Id.Text.Equals("999-1"))
                {
                    CheckBox ck = e.Item.FindControl("chk_datagrid") as CheckBox;
                    ck.Enabled = false;
                }

                ////IList partprocesses = new ModuleWorkFlow.BLL.PartProcess().getPartProcessByStatusId(e.Item.Cells[1].Text, e.Item.Cells[2].Text, null);
                //IList partprocesses = new ModuleWorkFlow.BLL.PartProcess().getPartProcessByStatusId(e.Item.Cells[1].Text, lab_PartNo_Id.Text, null);
                //string process = "";
                //ProcessCustomer processCustomer = new ProcessCustomer();
                //Hashtable hProcessCustomer = processCustomer.GetCustomerProcessIdProcess();
                //foreach (PartProcessInfo ppi in partprocesses)
                //{
                //    string strProcess = ppi.ProcessName;
                //    if (ppi.CustomerProcessId != null && !ppi.ProcessId.Equals(ppi.CustomerProcessId))
                //    {
                //        strProcess = (hProcessCustomer[ppi.CustomerProcessId] as ProcessCustomerInfo).CustomerProcessName;
                //    }

                //    if (process.Equals(""))
                //    {
                //        process += strProcess;
                //    }
                //    else
                //    {
                //        process += "/" + strProcess;
                //    }
                //}

                Label dg_lab_DataTTid = e.Item.FindControl("dg_lab_DataTTid") as Label;
                Label dg_lab_ParentDataTTid = e.Item.FindControl("dg_lab_ParentDataTTid") as Label;

                e.Item.Attributes.Add("data-tt-id", dg_lab_DataTTid.Text);
                e.Item.Attributes.Add("data-tt-parent-id", dg_lab_ParentDataTTid.Text);

                Label lab_ModuleId = e.Item.FindControl("lab_ModuleId") as Label;
                Label lab_ProcessNo = e.Item.FindControl("lab_ProcessNo") as Label;
                Literal Label_ProcessName = e.Item.FindControl("Label_ProcessName") as Literal;
                Label dg_lnk_ProcessName = e.Item.FindControl("dg_lnk_ProcessName") as Label;

                if (lab_ProcessNo.Text.Equals("0"))
                {
                    Label_ProcessName.Visible = true;
                    dg_lnk_ProcessName.Visible = false;
                }
                else
                {
                    //Label_ProcessName.Visible = false;
                    //dg_lnk_ProcessName.Visible = true;
                }

               
                //Label_ProcessName.Text = process;
            }


        }

        //		private void Dropdownlist_Customer_SelectedIndexChanged(object sender, System.EventArgs e)
        //		{
        //			MainDataGrid.CurrentPageIndex = 0;
        ////			Common.DDL_BindData_ModuleList(Dropdownlist_ModuleId,Dropdownlist_Customer.SelectedValue);
        ////			//MYD070314-4
        ////			//bindDataByModuleId(Dropdownlist_ModuleId.SelectedValue);
        ////			bindDataByCustomerId(Dropdownlist_Customer.SelectedValue);
        //			bindModuleid();
        //		}

        private void Dropdownlist_ModuleId_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            MainDataGrid.CurrentPageIndex = 0;
            lab_seach_moduleid.Text = "";
            bindDataByModuleId(Dropdownlist_ModuleId.SelectedValue);
        }



        private string[] getKeys()
        {
            string[] key = new string[3];
            key[MODULEID] = null;
            key[PARTNO] = null;
            key[PRIORITY] = null;
            foreach (DataGridItem item in MainDataGrid.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    CheckBox cb = (CheckBox)item.FindControl("chk_datagrid");
                    Label lab_PartNo_Id = item.FindControl("lab_PartNo_Id_Visible") as Label;
                    if (lab_PartNo_Id == null)
                    {
                        lab_PartNo_Id = item.FindControl("lab_PartNo_Id") as Label;
                    }
                    Label lab_ModuleId = item.FindControl("lab_ModuleId") as Label;
                    if (cb.Checked)
                    {
                        key[MODULEID] = lab_ModuleId.Text;
                        //key[PARTNO] = item.Cells[2].Text;
                        key[PARTNO] = lab_PartNo_Id.Text;
                        key[PRIORITY] = item.Cells[4].Text;
                        break;
                    }
                }
            }

            return key;
        }

        protected void lnkbutton_edit_Click(object sender, EventArgs e)
        {
            string[] keys = getKeys();
            if (keys[MODULEID] != null && keys[PARTNO] != null && keys[PRIORITY] != null)
            {
                string version = System.Configuration.ConfigurationSettings.AppSettings["versionfor"];
                if (version == null || !version.Equals("PART"))
                {
                    Response.Redirect("PartModifyEdit.aspx?menuid=F32&moduleid=" + keys[MODULEID] + "&partnoid=" + keys[PARTNO] + "&priority=" + keys[PRIORITY] + "&pageindex=" + MainDataGrid.CurrentPageIndex);
                }
                else
                {
                    Response.Redirect("ProductModifyEdit.aspx?menuid=F32&moduleid=" + keys[MODULEID] + "&partnoid=" + keys[PARTNO] + "&priority=" + keys[PRIORITY] + "&pageindex=" + MainDataGrid.CurrentPageIndex);
                }
            }
            else
            {
                Label_Message.Text = "請選擇零件";
            }
        }

        protected void lnkbutton_view_Click(object sender, EventArgs e)
        {
            string[] keys = getKeys();
            if (keys[MODULEID] != null && keys[PARTNO] != null)
            {
                string moduleid = keys[MODULEID];
                string partnoid = keys[PARTNO];
                bindDataDetail(moduleid, partnoid);
            }
            else
            {
                Label_Message.Text = "請選擇零件";
            }
        }

        protected void lnkbutton_search_Click(object sender, EventArgs e)
        {
            MainDataGrid.CurrentPageIndex = 0;
            lab_seach_moduleid.Text = Dropdownlist_ModuleId.SelectedValue;
            bindDataByModuleId(lab_seach_moduleid.Text.Trim());
        }

        private sealed class CustomerConfig
        {
            public string CustomerName { get; set; }
            public string CustomerAbbr { get; set; }
        }




        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (MainDataGrid.CurrentPageIndex + 1 < Convert.ToInt32(lab_totalpagte.Text.Trim()))
            {
                //MainDataGrid.CurrentPageIndex++;
                MainDataGrid.CurrentPageIndex = Convert.ToInt32(lab_nowpage.Text.Trim());
                if (lab_seach_moduleid.Text.Trim().Equals(""))
                {
                    bindDataByModuleId(Dropdownlist_ModuleId.SelectedValue);
                }
                else
                {
                    bindDataByModuleId(lab_seach_moduleid.Text.Trim());
                }
            }
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (MainDataGrid.CurrentPageIndex>0)
            {
                //MainDataGrid.CurrentPageIndex --;
                int page =0;
                if ((Convert.ToInt32(lab_nowpage.Text.Trim()) - 2) >= 0)
                {
                    page = Convert.ToInt32(lab_nowpage.Text.Trim()) - 2;
                }
                MainDataGrid.CurrentPageIndex =page;
                if (lab_seach_moduleid.Text.Trim().Equals(""))
                {
                    bindDataByModuleId(Dropdownlist_ModuleId.SelectedValue);
                }
                else
                {
                    bindDataByModuleId(lab_seach_moduleid.Text.Trim());
                }
            }
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                if (0 < Convert.ToInt32(txtGoPage.Text) && Convert.ToInt32(txtGoPage.Text) <= Convert.ToInt32(lab_totalpagte.Text))
                {
                    MainDataGrid.CurrentPageIndex = Convert.ToInt32(txtGoPage.Text)-1 ;
                }
                else
                {
                    Label_Message.Text = "請輸入正確頁碼";
                    return;
                }
            }
            catch (Exception ex)
            {
                Label_Message.Text = "請輸入正確頁碼";
                return;
            }

            if (lab_seach_moduleid.Text.Trim().Equals(""))
            {
                bindDataByModuleId(Dropdownlist_ModuleId.SelectedValue);
            }
            else
            {
                bindDataByModuleId(lab_seach_moduleid.Text.Trim());
            }

        }
    }
}
