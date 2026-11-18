using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Web.Script.Serialization;

using Utility;
using ModuleWorkFlow.business;
using ModuleWorkFlow.Model;
using ModuleWorkFlow.BLL;

using ModuleWorkFlow.Model.System;
using ModuleWorkFlow.BLL.System;
using System.Collections.Generic;
namespace ModuleWorkFlow
{
    /// <summary>
    /// Summary description for PartModifyEdit.
    /// </summary>
    public class PartModifyEdit : System.Web.UI.Page
    {
        //protected ModuleWorkFlow.controls.Style Style;
        protected System.Web.UI.WebControls.DropDownList ListBox_CustomerList;
        protected System.Web.UI.WebControls.Label Label_ModuleId;
        protected System.Web.UI.WebControls.Label Label_CustomerName;
        protected System.Web.UI.WebControls.Label Label_PartNo;
        protected System.Web.UI.WebControls.Label Label_PartNoId;
        protected System.Web.UI.WebControls.CheckBoxList CheckBoxList_Process;
        protected System.Web.UI.WebControls.DataGrid MainDataGrid;
        protected System.Web.UI.WebControls.Button Button_AddEdit;
        protected System.Web.UI.WebControls.Label Label_Message;
        protected System.Web.UI.WebControls.Label Label_HiddenSelectRow;
        protected System.Web.UI.WebControls.Label Label_DeleteProcessNo;
        protected System.Web.UI.WebControls.Label Label_DisableMaxOrder;
        protected System.Web.UI.WebControls.DropDownList ListBox_Priority;
        protected System.Web.UI.WebControls.Label Label_HiddenPriority;
        protected Label Label_PageIndex;
        protected System.Web.UI.WebControls.TextBox txt_startDate;
        protected System.Web.UI.WebControls.TextBox txt_endDate;
        protected System.Web.UI.WebControls.Label lab_zuli;
        protected System.Web.UI.WebControls.LinkButton lnkbutton_editrelation;
        private string menuid = "F32";
        protected string menuname = "";
        int isProject;

        private void Page_Load(object sender, System.EventArgs e)
        {

            //權限控制

            if (Request.Params["menuid"] != null)
            {
                menuid = Request.Params["menuid"];
            }

            PartTmenu menu = new PartTmenu();
            PartTmenuInfo mi = menu.findbykey(menuid);

            if (mi != null)
            {
                menuname = mi.Menuname;

                if (this.Master != null && this.Master is DefaultSub)
                {
                    DefaultSub master = (DefaultSub)this.Master;
                    master.Menuname = mi.Menuname;
                }
            }

            //ModuleWorkFlow.BLL.Private.checkPrivate(this, menuid, "PEDIT", Request.Params["moduleid"], Request.Params["partnoid"]);
            if (!this.IsPostBack)
            {
               

                // ModuleWorkFlow.BLL.Private.checkPrivate(this, menuid, "PEDIT");
                Label_HiddenSelectRow.Text = "-1";
                Label_Message.Text = "";
                string moduleid = Request.Params["moduleid"];
                string partnoid = Request.Params["partnoid"];
                string priority = Request.Params["priority"];
                Label_ModuleId.Text = moduleid;
                Label_CustomerName.Text = GetCustomerName(moduleid);
                Label_PartNoId.Text = Request.Params["partnoid"];
                Label_PageIndex.Text = Request.Params["pageindex"];
                Label_HiddenPriority.Text = priority;
                BindDataPriority();
                ModuleWorkFlow.business.Methods.DropDownListChange(ListBox_Priority, priority);
                //BindDataPartStartDate(moduleid);
                BindDataProcessAll();
                //Button_AddEdit.Text = lang.TXT_EDIT;
                dataBindHandly(moduleid, partnoid);
            }
        }

        private string GetCustomerName(string customerAbbr)
        {
            string configPath = Server.MapPath("~/Config/BarcodeCustomers.json");
            if (!File.Exists(configPath))
            {
                return customerAbbr;
            }

            List<CustomerConfig> customers = new JavaScriptSerializer()
                .Deserialize<List<CustomerConfig>>(File.ReadAllText(configPath));
            foreach (CustomerConfig customer in customers ?? new List<CustomerConfig>())
            {
                if (customer != null && string.Equals(customer.CustomerAbbr, customerAbbr, StringComparison.OrdinalIgnoreCase))
                {
                    return string.IsNullOrWhiteSpace(customer.CustomerName)
                        ? customerAbbr
                        : customer.CustomerName;
                }
            }

            return customerAbbr;
        }

        private sealed class CustomerConfig
        {
            public string CustomerName { get; set; }
            public string CustomerAbbr { get; set; }
        }

        private void BindDataProcessAll()
        {
            ModuleWorkFlow.BLL.Process process = new ModuleWorkFlow.BLL.Process();
            CheckBoxList_Process.DataTextField = "CustomerProcessName";
            CheckBoxList_Process.DataValueField = "CustomerProcessId";

            CheckBoxList_Process.DataSource = process.getProcessInfoExceptDesign();

            CheckBoxList_Process.DataBind();
            CheckBoxList_Process.RepeatDirection = RepeatDirection.Vertical;
            CheckBoxList_Process.RepeatColumns = 2;
        }

        //		private void BindDataCutomer()
        //		{
        //			ModuleWorkFlow.BLL.Customer customer = new ModuleWorkFlow.BLL.Customer();
        //			ListBox_CustomerList.DataTextField = "ChineseName";
        //			ListBox_CustomerList.DataValueField = "Id";
        //			ListBox_CustomerList.DataSource = customer.GetCustomer();
        //			ListBox_CustomerList.DataBind();
        //		}

        private void BindDataPriority()
        {
            ListBox_Priority.DataTextField = "priorityname";
            ListBox_Priority.DataValueField = "priorityid";
            ListBox_Priority.DataSource = Priority.getPriorityView();
            ListBox_Priority.DataBind();
        }

        private void BindDataPartStartDate(string moduleid)
        {
        }

        private void dataBindHandly(string moduleid, string partnoid)
        {
            List<XHS.Model.XHSPartPartProcessInfo> processList =
                new XHS.BLL.XHSPartPartProcess().GetPartProcessesByPartNoId(partnoid);
            processList = processList.FindAll(delegate (XHS.Model.XHSPartPartProcessInfo process)
            {
                return string.Equals(process.ModuleId, moduleid, StringComparison.OrdinalIgnoreCase);
            });

            DataTable dt = BuildEditDataTable(processList);
            if (dt.Rows.Count > 0)
            {
                Label_PartNo.Text = Convert.ToString(dt.Rows[0]["partno"]);
            }

            MainDataGrid.DataSource = dt;
            MainDataGrid.DataBind();


            //for (int i = 0; i < MainDataGrid.Items.Count; i++)
            //{
            //    Label label_statusid = MainDataGrid.Items[i].FindControl("Label_StatusID") as Label;
            //    ColorConverter cc = new ColorConverter();
            //    MainDataGrid.Items[i].BackColor = (Color)cc.ConvertFromString(Methods.TranslateStatusColor(label_statusid.Text));
            //    label_statusid.Text = Methods.TranslateStatusName(label_statusid.Text);
            //}

            //int maxdisable = -1;
            //string s = "";
            //for(int i=0;i<dt.Rows.Count;i++)
            //{
            //    if (!Convert.IsDBNull(dt.Rows[i]["factstartdate"]))
            //    {
            //        maxdisable = i;
            //        s += Convert.ToString(dt.Rows[i]["processno"]) + ",";
            //    }
            //}
            //Label_DisableMaxOrder.Text = s;


            //get hours per day from process table
            Hashtable htProcessOld = new ModuleWorkFlow.BLL.Process().GetProcessIdProcess();
            Hashtable htProcess = new Hashtable();
            foreach (string key in htProcessOld.Keys)
            {
                htProcess.Add(key.ToUpper(), htProcessOld[key]);
            }
            for (int i = 0; i < MainDataGrid.Items.Count; i++)
            {
                Label label_processid = MainDataGrid.Items[i].FindControl("Label_ProcessID") as Label;
                Label Label_CustomerProcessID = MainDataGrid.Items[i].FindControl("Label_CustomerProcessID") as Label;
                DropDownList DDLDay = MainDataGrid.Items[i].FindControl("DropDownList_Day") as DropDownList;
                DropDownList DDLHour = MainDataGrid.Items[i].FindControl("DropDownList_Hour") as DropDownList;
                DropDownList DDLMinute = MainDataGrid.Items[i].FindControl("DropDownList_Minute") as DropDownList;
                TextBox txt_startDate = MainDataGrid.Items[i].FindControl("txt_startDate") as TextBox;
                DropDownList dpl_startHour = MainDataGrid.Items[i].FindControl("dpl_startHour") as DropDownList;
                TextBox txt_endDate = MainDataGrid.Items[i].FindControl("txt_endDate") as TextBox;
                DropDownList dpl_endHour = MainDataGrid.Items[i].FindControl("dpl_endHour") as DropDownList;
                CheckBox cbl_isunnormal = MainDataGrid.Items[i].FindControl("cbl_isunnormal") as CheckBox;//whd080901 增加是否返修
                CheckBox chk_Ispcnc = MainDataGrid.Items[i].FindControl("chk_Ispcnc") as CheckBox;// 程式
                Label dg_lab_relationpartno = MainDataGrid.Items[i].FindControl("dg_lab_relationpartno") as Label;
                Label dg_lab_relationpartnoId = MainDataGrid.Items[i].FindControl("dg_lab_relationpartnoId") as Label;
                if (dg_lab_relationpartno.Text.IndexOf(Label_PartNoId.Text.Trim()) > -1)
                {
                    chk_Ispcnc.Checked = true;
                }
                string isPart = System.Configuration.ConfigurationSettings.AppSettings["IsPart"];
                string isPartOnly = System.Configuration.ConfigurationSettings.AppSettings["PartNoOnly"];
                if (isPart != null && Convert.ToBoolean(isPart) && isPartOnly != null && Convert.ToBoolean(isPartOnly))
                    cbl_isunnormal.Enabled = false;

                //init TextBox_NeedTime input box and DropDownList_UnitSelect
                ModuleWorkFlow.business.Methods.DropDownListInit(DDLDay, 0, Setting.DAYS);
                ModuleWorkFlow.business.Methods.DropDownListInit(DDLHour, 0, Setting.HOURS - 1);
                ModuleWorkFlow.business.Methods.EveryMinuteDropDownListInit(DDLMinute);
                ModuleWorkFlow.business.Methods.DropDownListInit(dpl_startHour, 0, Setting.HOURS - 1);
                ModuleWorkFlow.business.Methods.DropDownListInit(dpl_endHour, 0, Setting.HOURS - 1);

                //retieved all data
                int hoursperday = 24;
                if (htProcess[Label_CustomerProcessID.Text.Trim().ToUpper()] != null)
                {
                    hoursperday = (htProcess[Label_CustomerProcessID.Text.Trim().ToUpper()] as ModuleWorkFlow.Model.ProcessInfo).HoursPerDay;
                }
                int processneedminutes = Convert.ToInt32(dt.Rows[i][dt.Columns["ProcessNeedMinutes"]].ToString());
                string oldDay = Convert.ToString((int)(processneedminutes / (60 * hoursperday)));
                string oldHour = Convert.ToString((int)(processneedminutes % (60 * hoursperday) / 60));
                string oldMintes = Convert.ToString((int)(processneedminutes % (60 * hoursperday) % 60));
                string oldStartHour = "";
                string oldEndHour = "";
                if (!dt.Rows[i][dt.Columns["selfStartDate"]].ToString().Equals(""))
                {
                    string oldSelfStartDate = Convert.ToString(((DateTime)(dt.Rows[i][dt.Columns["selfStartDate"]])).ToShortDateString());
                    txt_startDate.Text = oldSelfStartDate;
                    oldStartHour = Convert.ToString(((DateTime)(dt.Rows[i][dt.Columns["selfStartDate"]])).Hour.ToString());
                }
                if (!dt.Rows[i][dt.Columns["selfEndDate"]].ToString().Equals(""))
                {
                    string oldSelfEndDate = Convert.ToString(((DateTime)(dt.Rows[i][dt.Columns["selfEndDate"]])).ToShortDateString());
                    txt_endDate.Text = oldSelfEndDate;
                    oldEndHour = Convert.ToString(((DateTime)(dt.Rows[i][dt.Columns["selfEndDate"]])).Hour.ToString());
                }
                if (Convert.ToInt32(dt.Rows[i][dt.Columns["isUnnormal"]]) == 0)
                {
                    cbl_isunnormal.Checked = false;
                }
                else
                {
                    cbl_isunnormal.Checked = true;
                }
                if ((dt.Rows[i][dt.Columns["pricetype"]]).ToString().Equals("ZULI"))
                {
                    lab_zuli.Text = Convert.ToString(dt.Rows[i][dt.Columns["processno"]]);
                }

                TextBox txt_processComment = MainDataGrid.Items[i].FindControl("txt_processComment") as TextBox;
                DropDownList dpl_processMachineid = MainDataGrid.Items[i].FindControl("dpl_processMachineid") as DropDownList;
                txt_processComment.Text = (dt.Rows[i][dt.Columns["processComment"]]).ToString();

                ModuleWorkFlow.BLL.Machine machine = new ModuleWorkFlow.BLL.Machine();
                try
                {
                    dpl_processMachineid.DataSource = machine.GetAllMachineList(label_processid.Text.Trim());
                    dpl_processMachineid.DataValueField = "Id";
                    dpl_processMachineid.DataTextField = "Id";
                    dpl_processMachineid.DataBind();
                    dpl_processMachineid.Items.Insert(0, "");
                }
                catch
                {
                    dpl_processMachineid.Items.Clear();
                    dpl_processMachineid.Items.Insert(0, "");
                }
                try
                {
                    dpl_processMachineid.SelectedValue = (dt.Rows[i][dt.Columns["processMachineid"]]).ToString();
                }
                catch { }

                DropDownList dpl_unnormalReasons = MainDataGrid.Items[i].FindControl("dpl_unnormalReasons") as DropDownList;

                ModuleWorkFlow.BLL.System.unNormalReasons unr = new ModuleWorkFlow.BLL.System.unNormalReasons();
                dpl_unnormalReasons.DataSource = unr.GetunNormalReasonsInfobyType("unNormal");
                dpl_unnormalReasons.DataTextField = "unnormalReasons";
                dpl_unnormalReasons.DataValueField = "Id";
                dpl_unnormalReasons.DataBind();
                dpl_unnormalReasons.Items.Insert(0, "");
                try
                {
                    dpl_unnormalReasons.SelectedValue = "0";
                    dpl_unnormalReasons.SelectedValue = (dt.Rows[i][dt.Columns["unNormalReasonsId"]]).ToString();
                }
                catch { }

                DropDownList dpl_DepartMent = MainDataGrid.Items[i].FindControl("dpl_DepartMent") as DropDownList;
                ModuleWorkFlow.BLL.DepartMent dm = new DepartMent();
                dpl_DepartMent.DataSource = dm.GetAllDepartmentbyIsFix();
                dpl_DepartMent.DataTextField = "DepartName";
                dpl_DepartMent.DataValueField = "DepartMentId";
                dpl_DepartMent.DataBind();
                dpl_DepartMent.Items.Insert(0, "");

                try
                {
                    dpl_DepartMent.SelectedValue = "0";
                    dpl_DepartMent.SelectedValue = (dt.Rows[i][dt.Columns["departMentId"]]).ToString();
                }
                catch { }

                DropDownList dpl_Supply = MainDataGrid.Items[i].FindControl("dpl_Supply") as DropDownList;
                dpl_Supply.Items.Clear();
                dpl_Supply.Items.Insert(0, "");

                try
                {
                    dpl_Supply.SelectedValue = "0";
                    dpl_Supply.SelectedValue = (dt.Rows[i][dt.Columns["supplyId"]]).ToString();
                }
                catch { }


                ModuleWorkFlow.business.Methods.DropDownListChange(DDLDay, oldDay);
                ModuleWorkFlow.business.Methods.DropDownListChange(DDLHour, oldHour);
                ModuleWorkFlow.business.Methods.DropDownListChange(DDLMinute, oldMintes);
                ModuleWorkFlow.business.Methods.DropDownListChange(dpl_startHour, oldStartHour);
                ModuleWorkFlow.business.Methods.DropDownListChange(dpl_endHour, oldEndHour);

            }

            disableProductedProcess();
            formatDataGrid();
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
            this.CheckBoxList_Process.SelectedIndexChanged += new System.EventHandler(this.CheckBoxList_Process_SelectedIndexChanged);
            this.MainDataGrid.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.MainDataGrid_ItemCommand);
            this.MainDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.MainDataGrid_DeleteCommand);
            //this.Button_AddEdit.Click += new System.EventHandler(this.Button_AddEdit_Click);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        private void MainDataGrid_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {

            int index = e.Item.ItemIndex;
            switch (((Button)e.CommandSource).CommandName)
            {
                case "Upper":
                    if (index > 0)
                    {
                        modifyProcessOrder(index, index - 1);
                        disableProductedProcess();
                    }
                    break;

                case "Lower":
                    if (index < MainDataGrid.Items.Count - 1)
                    {
                        modifyProcessOrder(index, index + 1);
                        disableProductedProcess();
                    }
                    break;


                case "Select":
                    for (int i = 0; i < MainDataGrid.Items.Count; i++)
                    {
                        MainDataGrid.Items[i].BackColor = Color.White;
                    }
                    e.Item.BackColor = Setting.SELECTCOLOR;
                    Label_HiddenSelectRow.Text = Convert.ToString(e.Item.ItemIndex);
                    disableProductedProcess();
                    break;
                case "Delete":
                    //MYD070316-5
                    Label Label_processno = MainDataGrid.Items[index].FindControl("Label_processno") as Label;
                    string processno = Label_processno.Text;
                    Label_DeleteProcessNo.Text = Label_DeleteProcessNo.Text + "," + processno;
                    //if (index < MainDataGrid.Items.Count - 1)
                    //{
                    //    modifyProcessOrder(index, index + 1);
                    //}
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }

        private void CheckBoxList_Process_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string processid = CheckBoxList_Process.SelectedValue;
            bool hasExisted = false;

            if (!hasExisted)
            {
                AddDataSource();
            }

            //clear CheckBoxList_Process all selected item
            for (int i = 0; i < CheckBoxList_Process.Items.Count; i++)
            {
                CheckBoxList_Process.Items[i].Selected = false;
            }
        }

        private void AddDataSource() //whd080901
        {
            BLL.Process process = new ModuleWorkFlow.BLL.Process();

            int selectRow = Convert.ToInt32(Label_HiddenSelectRow.Text);
            if (selectRow > MainDataGrid.Items.Count - 1)
            {
                selectRow = -1;
                Label_HiddenSelectRow.Text = "-1";
            }
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add(new DataColumn("ListOrder", typeof(string)));
            dt.Columns.Add(new DataColumn("ProcessId", typeof(string)));
            dt.Columns.Add(new DataColumn("CustomerProcessId", typeof(string)));
            dt.Columns.Add(new DataColumn("CustomerProcessName", typeof(string)));
            dt.Columns.Add(new DataColumn("Day", typeof(string)));
            dt.Columns.Add(new DataColumn("Hour", typeof(string)));
            dt.Columns.Add(new DataColumn("Minute", typeof(string)));
            dt.Columns.Add(new DataColumn("ProcessNo", typeof(string)));
            dt.Columns.Add(new DataColumn("SelfStartDate", typeof(string)));
            dt.Columns.Add(new DataColumn("StartHour", typeof(string)));
            dt.Columns.Add(new DataColumn("SelfEndDate", typeof(string)));
            dt.Columns.Add(new DataColumn("EndHour", typeof(string)));
            dt.Columns.Add(new DataColumn("FactStartDate", typeof(string)));
            dt.Columns.Add(new DataColumn("FactEndDate", typeof(string)));
            dt.Columns.Add(new DataColumn("IsUnnormal", typeof(int)));//whd080901 增加是否返修
            dt.Columns.Add(new DataColumn("StatusID", typeof(string)));
            dt.Columns.Add(new DataColumn("pricetype", typeof(string)));
            dt.Columns.Add(new DataColumn("processComment", typeof(string)));
            dt.Columns.Add(new DataColumn("processMachineid", typeof(string)));
            dt.Columns.Add(new DataColumn("unNormalReasonsId", typeof(string)));
            dt.Columns.Add(new DataColumn("departMentId", typeof(string)));
            dt.Columns.Add(new DataColumn("supplyId", typeof(string)));

            dt.Columns.Add(new DataColumn("chk_Ispcnc", typeof(int)));//程式
            dt.Columns.Add(new DataColumn("RelationPartNo", typeof(string)));//程式
            dt.Columns.Add(new DataColumn("RelationPartNoId", typeof(string)));//程式

            ProcessCustomer processCustomer = new ProcessCustomer();
            for (int i = 0; i < MainDataGrid.Items.Count; i++)
            {
                if (selectRow != -1 && selectRow == i)
                {
                    //new a record
                    dr = dt.NewRow();

                    dr["ListOrder"] = Convert.ToString(MainDataGrid.Items.Count + 1);
                    ModuleWorkFlow.Model.ProcessInfo pi = processCustomer.GetProcessInfoBykey(CheckBoxList_Process.SelectedValue, new ModuleWorkFlow.BLL.Process());
                    dr["ProcessId"] = pi.ProcessId;
                    dr["CustomerProcessId"] = CheckBoxList_Process.SelectedValue;
                    dr["CustomerProcessName"] = CheckBoxList_Process.SelectedItem.Text.Trim();
                    dr["Day"] = "0";
                    dr["Hour"] = "0";
                    dr["Minute"] = "0";
                    dr["ProcessNo"] = "0";
                    dr["SelfStartDate"] = "";
                    dr["StartHour"] = "0";
                    dr["SelfEndDate"] = "";
                    dr["EndHour"] = "0";
                    dr["FactStartDate"] = "";
                    dr["FactEndDate"] = "";
                    dr["IsUnnormal"] = 1;
                    dr["StatusID"] = "Pending";
                    dr["pricetype"] = "";
                    dr["processComment"] = "";
                    dr["processMachineid"] = "";
                    dr["unNormalReasonsId"] = "";
                    dr["departMentId"] = "";
                    dr["supplyId"] = "";
                    if (string.IsNullOrEmpty(pi.Comment))
                    {
                        dr["chk_Ispcnc"] = 0;
                    }
                    else
                    {
                        dr["chk_Ispcnc"] = 1;
                    }

                    dr["RelationPartNo"] = "";
                    dr["RelationPartNoId"] = "";


                    dt.Rows.Add(dr);
                    ModuleWorkFlow.Model.ProcessInfo processInfo = process.GetProcessInfoById(CheckBoxList_Process.SelectedValue);
                    if (processInfo == null)
                    {
                        processInfo = process.GetProcessInfoByCustomerProcessId(CheckBoxList_Process.SelectedValue);
                    }
                    if (processInfo.PriceType != null && processInfo.PriceType.Equals("ZULI"))
                    {
                        lab_zuli.Text = dr[6].ToString();
                        dr[15] = "ZULI";
                    }

                }
                Label Label_OrderNo = MainDataGrid.Items[i].FindControl("Label_OrderNo") as Label;
                Label label_processid = MainDataGrid.Items[i].FindControl("Label_ProcessID") as Label;
                Label label_customerProcessid = MainDataGrid.Items[i].FindControl("Label_CustomerProcessID") as Label;
                Label label_customerProcessname = MainDataGrid.Items[i].FindControl("Label_CustomerProcessName") as Label;
                DropDownList dropDownList_Day = MainDataGrid.Items[i].FindControl("DropDownList_Day") as DropDownList;
                DropDownList dropDownList_Hour = MainDataGrid.Items[i].FindControl("DropDownList_Hour") as DropDownList;
                DropDownList dropDownList_Minute = MainDataGrid.Items[i].FindControl("DropDownList_Minute") as DropDownList;
                TextBox txt_startDate = MainDataGrid.Items[i].FindControl("txt_startDate") as TextBox;
                DropDownList dpl_startHour = MainDataGrid.Items[i].FindControl("dpl_startHour") as DropDownList;
                TextBox txt_endDate = MainDataGrid.Items[i].FindControl("txt_endDate") as TextBox;
                DropDownList dpl_endHour = MainDataGrid.Items[i].FindControl("dpl_endHour") as DropDownList;
                Label lab_factstartDate = MainDataGrid.Items[i].FindControl("Label_FactStartDate") as Label;
                Label lab_factendDate = MainDataGrid.Items[i].FindControl("Label_FactEndDate") as Label;
                CheckBox cbl_isunnormal = MainDataGrid.Items[i].FindControl("cbl_isunnormal") as CheckBox;
                //cbl_isunnormal.Checked = true;
                //cbl_isunnormal.Enabled = false;

                CheckBox chk_Ispcnc = MainDataGrid.Items[i].FindControl("chk_Ispcnc") as CheckBox;//程式
                Label dg_lab_relationpartno = MainDataGrid.Items[i].FindControl("dg_lab_relationpartno") as Label;
                Label dg_lab_relationpartnoId = MainDataGrid.Items[i].FindControl("dg_lab_relationpartnoId") as Label;
                Label lab_StatusID = MainDataGrid.Items[i].FindControl("dg_lab_StatusID") as Label;
                Label lab_pricetype = MainDataGrid.Items[i].FindControl("dg_lab_pricetype") as Label;
                TextBox txt_processComment = MainDataGrid.Items[i].FindControl("txt_processComment") as TextBox;
                DropDownList dpl_processMachineid = MainDataGrid.Items[i].FindControl("dpl_processMachineid") as DropDownList;
                string processno = (MainDataGrid.Items[i].FindControl("Label_processno") as Label).Text;
                DropDownList dpl_unnormalReasons = MainDataGrid.Items[i].FindControl("dpl_unnormalReasons") as DropDownList;
                DropDownList dpl_DepartMent = MainDataGrid.Items[i].FindControl("dpl_DepartMent") as DropDownList;
                DropDownList dpl_Supply = MainDataGrid.Items[i].FindControl("dpl_Supply") as DropDownList;

                dr = dt.NewRow();

                dr["ListOrder"] = Label_OrderNo.Text.Trim();
                dr["ProcessId"] = label_processid.Text.Trim();
                dr["CustomerProcessId"] = label_customerProcessid.Text.Trim();
                dr["CustomerProcessName"] = label_customerProcessname.Text.Trim();
                dr["Day"] = dropDownList_Day.SelectedValue;
                dr["Hour"] = dropDownList_Hour.SelectedValue;
                dr["Minute"] = dropDownList_Minute.SelectedValue;
                dr["ProcessNo"] = processno;
                dr["SelfStartDate"] = txt_startDate.Text.Trim();
                dr["StartHour"] = dpl_startHour.SelectedValue;
                dr["SelfEndDate"] = txt_endDate.Text.Trim();
                dr["EndHour"] = dpl_endHour.SelectedValue;
                dr["FactStartDate"] = lab_factstartDate.Text;
                dr["FactEndDate"] = lab_factendDate.Text;
                if (cbl_isunnormal.Checked)//whd080901 增加是否返修
                {
                    dr["IsUnnormal"] = 1;
                }
                else
                {
                    dr["IsUnnormal"] = 0;
                }


                dr["chk_Ispcnc"] = Convert.ToInt32(chk_Ispcnc.Checked);

                dr["RelationPartNo"] = dg_lab_relationpartno.Text;
                dr["RelationPartNoId"] = dg_lab_relationpartnoId.Text;

                dr["StatusID"] = lab_StatusID.Text.Trim();
                dr["pricetype"] = lab_pricetype.Text;
                dr["processComment"] = txt_processComment.Text.Trim();
                dr["processMachineid"] = dpl_processMachineid.SelectedValue;
                dr["unNormalReasonsId"] = dpl_unnormalReasons.SelectedValue;
                dr["departMentId"] = dpl_DepartMent.SelectedValue;
                dr["supplyId"] = dpl_Supply.SelectedValue;

                dt.Rows.Add(dr);
                if (process.GetProcessInfoById(label_processid.Text).PriceType.Equals("ZULI"))
                {
                    lab_zuli.Text = dr[6].ToString();
                    dr["pricetype"] = "ZULI";
                }

            }

            if (selectRow == -1)
            {
                //new a record
                dr = dt.NewRow();

                dr["ListOrder"] = Convert.ToString(MainDataGrid.Items.Count + 1);
                ModuleWorkFlow.Model.ProcessInfo pi = processCustomer.GetProcessInfoBykey(CheckBoxList_Process.SelectedValue, new ModuleWorkFlow.BLL.Process());
                dr["ProcessId"] = pi.ProcessId;
                dr["CustomerProcessId"] = CheckBoxList_Process.SelectedValue;
                dr["CustomerProcessName"] = CheckBoxList_Process.SelectedItem.Text.Trim();
                dr["Day"] = "0";
                dr["Hour"] = "0";
                dr["Minute"] = "0";
                dr["ProcessNo"] = "0";
                dr["SelfStartDate"] = "";
                dr["StartHour"] = "0";
                dr["SelfEndDate"] = "";
                dr["EndHour"] = "0";
                dr["FactStartDate"] = "";
                dr["FactEndDate"] = "";
                dr["IsUnnormal"] = 1;
                dr["StatusID"] = "Pending";
                dr["pricetype"] = "";
                dr["processComment"] = "";
                dr["processMachineid"] = "";
                dr["unNormalReasonsId"] = "";
                dr["departMentId"] = "";
                dr["supplyId"] = "";

                //新增數據,表tb_process,comment字段有值，則程序勾選
                if (string.IsNullOrEmpty(pi.Comment))
                {
                    dr["chk_Ispcnc"] = 0;
                }
                else
                {
                    dr["chk_Ispcnc"] = 1;
                }

                dr["RelationPartNo"] = "";
                dt.Rows.Add(dr);
                if (process.GetProcessInfoByCustomerProcessId(CheckBoxList_Process.SelectedValue).PriceType.Equals("ZULI"))
                {
                    lab_zuli.Text = dr["pricetype"].ToString();
                    dr["pricetype"] = "ZULI";
                }

            }

            DataView dv = new DataView(dt);

            MainDataGrid.DataSource = dv;
            MainDataGrid.DataBind();

            //init TextBox_NeedTime input box and DropDownList_UnitSelect
            for (int i = 0; i < MainDataGrid.Items.Count; i++)
            {
                if (i == selectRow) MainDataGrid.Items[i].BackColor = Setting.SELECTCOLOR;
                Label Label_OrderNo = MainDataGrid.Items[i].FindControl("Label_OrderNo") as Label;
                DropDownList DDLDay = MainDataGrid.Items[i].FindControl("DropDownList_Day") as DropDownList;
                DropDownList DDLHour = MainDataGrid.Items[i].FindControl("DropDownList_Hour") as DropDownList;
                DropDownList DDLMinute = MainDataGrid.Items[i].FindControl("DropDownList_Minute") as DropDownList;
                TextBox txt_startDate = MainDataGrid.Items[i].FindControl("txt_startDate") as TextBox;
                DropDownList dpl_startHour = MainDataGrid.Items[i].FindControl("dpl_startHour") as DropDownList;
                TextBox txt_endDate = MainDataGrid.Items[i].FindControl("txt_endDate") as TextBox;
                DropDownList dpl_endHour = MainDataGrid.Items[i].FindControl("dpl_endHour") as DropDownList;
                Label lab_factstartDate = MainDataGrid.Items[i].FindControl("Label_FactStartDate") as Label;
                Label lab_factendDate = MainDataGrid.Items[i].FindControl("Label_FactEndDate") as Label;
                CheckBox cbl_isunnormal = MainDataGrid.Items[i].FindControl("cbl_isunnormal") as CheckBox;//whd080901 增加是否返修
                Label dg_lab_Ispcnc = MainDataGrid.Items[i].FindControl("dg_lab_Ispcnc") as Label;
                CheckBox chk_Ispcnc = MainDataGrid.Items[i].FindControl("chk_Ispcnc") as CheckBox;//程式

                Label dg_lab_relationpartno = MainDataGrid.Items[i].FindControl("dg_lab_relationpartno") as Label;
                Label dg_lab_relationpartnoId = MainDataGrid.Items[i].FindControl("dg_lab_relationpartnoId") as Label;

                string isPart = System.Configuration.ConfigurationSettings.AppSettings["IsPart"];
                string isPartOnly = System.Configuration.ConfigurationSettings.AppSettings["PartNoOnly"];
                if (isPart != null && Convert.ToBoolean(isPart) && isPartOnly != null && Convert.ToBoolean(isPartOnly))
                    cbl_isunnormal.Enabled = false;
                Label lab_StatusID = MainDataGrid.Items[i].FindControl("dg_lab_StatusID") as Label;
                Label lab_pricetype = MainDataGrid.Items[i].FindControl("dg_lab_pricetype") as Label;
                TextBox txt_processComment = MainDataGrid.Items[i].FindControl("txt_processComment") as TextBox;
                DropDownList dpl_processMachineid = MainDataGrid.Items[i].FindControl("dpl_processMachineid") as DropDownList;
                Label label_processid = MainDataGrid.Items[i].FindControl("Label_ProcessID") as Label;
                Label label_customerProcessid = MainDataGrid.Items[i].FindControl("Label_CustomerProcessID") as Label;
                Label label_customerProcessname = MainDataGrid.Items[i].FindControl("Label_CustomerProcessName") as Label;

                Label_OrderNo.Text = Convert.ToString(i + 1);
                ModuleWorkFlow.business.Methods.DropDownListInit(DDLDay, 0, Setting.DAYS);
                ModuleWorkFlow.business.Methods.DropDownListInit(DDLHour, 0, Setting.HOURS - 1);
                ModuleWorkFlow.business.Methods.EveryMinuteDropDownListInit(DDLMinute);
                txt_startDate.Text = "";
                ModuleWorkFlow.business.Methods.DropDownListInit(dpl_startHour, 0, Setting.HOURS - 1);
                txt_endDate.Text = "";
                ModuleWorkFlow.business.Methods.DropDownListInit(dpl_endHour, 0, Setting.HOURS - 1);
                lab_factstartDate.Text = "";
                lab_factendDate.Text = "";
                ModuleWorkFlow.BLL.Machine machine = new ModuleWorkFlow.BLL.Machine();
                try
                {
                    dpl_processMachineid.DataSource = machine.GetAllMachineList(label_processid.Text.Trim());
                    dpl_processMachineid.DataValueField = "Id";
                    dpl_processMachineid.DataTextField = "Id";
                    dpl_processMachineid.DataBind();
                    dpl_processMachineid.Items.Insert(0, "");
                }
                catch
                {
                    dpl_processMachineid.Items.Clear();
                    dpl_processMachineid.Items.Insert(0, "");
                }


                DropDownList dpl_unnormalReasons = MainDataGrid.Items[i].FindControl("dpl_unnormalReasons") as DropDownList;
                ModuleWorkFlow.BLL.System.unNormalReasons unr = new ModuleWorkFlow.BLL.System.unNormalReasons();
                dpl_unnormalReasons.DataSource = unr.GetunNormalReasonsInfoAll();
                dpl_unnormalReasons.DataTextField = "unnormalReasons";
                dpl_unnormalReasons.DataValueField = "Id";
                dpl_unnormalReasons.DataBind();
                dpl_unnormalReasons.Items.Insert(0, "");



                DropDownList dpl_DepartMent = MainDataGrid.Items[i].FindControl("dpl_DepartMent") as DropDownList;
                ModuleWorkFlow.BLL.DepartMent dm = new DepartMent();
                dpl_DepartMent.DataSource = dm.GetAllDepartmentbyIsFix();
                dpl_DepartMent.DataTextField = "DepartName";
                dpl_DepartMent.DataValueField = "DepartMentId";
                dpl_DepartMent.DataBind();
                dpl_DepartMent.Items.Insert(0, "");

                DropDownList dpl_Supply = MainDataGrid.Items[i].FindControl("dpl_Supply") as DropDownList;
                dpl_Supply.Items.Clear();
                dpl_Supply.Items.Insert(0, "");



                cbl_isunnormal.Checked = false;//whd080901 增加是否返修
                lab_StatusID.Text = "Pending";
            }

            //retieved all data
            for (int i = 0; i < MainDataGrid.Items.Count; i++)
            {
                DropDownList dropDownList_Day = MainDataGrid.Items[i].FindControl("DropDownList_Day") as DropDownList;
                DropDownList dropDownList_Hour = MainDataGrid.Items[i].FindControl("DropDownList_Hour") as DropDownList;
                DropDownList dropDownList_Minute = MainDataGrid.Items[i].FindControl("DropDownList_Minute") as DropDownList;
                TextBox txt_startDate = MainDataGrid.Items[i].FindControl("txt_startDate") as TextBox;
                DropDownList dpl_startHour = MainDataGrid.Items[i].FindControl("dpl_startHour") as DropDownList;
                TextBox txt_endDate = MainDataGrid.Items[i].FindControl("txt_endDate") as TextBox;
                DropDownList dpl_endHour = MainDataGrid.Items[i].FindControl("dpl_endHour") as DropDownList;
                Label lab_factstartDate = MainDataGrid.Items[i].FindControl("Label_FactStartDate") as Label;
                Label lab_factendDate = MainDataGrid.Items[i].FindControl("Label_FactEndDate") as Label;
                CheckBox cbl_isunnormal = MainDataGrid.Items[i].FindControl("cbl_isunnormal") as CheckBox;//whd080901 增加是否返修

                CheckBox chk_Ispcnc = MainDataGrid.Items[i].FindControl("chk_Ispcnc") as CheckBox;//程式
                Label dg_lab_relationpartno = MainDataGrid.Items[i].FindControl("dg_lab_relationpartno") as Label;
                Label dg_lab_relationpartnoId = MainDataGrid.Items[i].FindControl("dg_lab_relationpartnoId") as Label;

                Label lab_StatusID = MainDataGrid.Items[i].FindControl("dg_lab_StatusID") as Label;
                Label lab_pricetype = MainDataGrid.Items[i].FindControl("dg_lab_pricetype") as Label;
                TextBox txt_processComment = MainDataGrid.Items[i].FindControl("txt_processComment") as TextBox;
                DropDownList dpl_processMachineid = MainDataGrid.Items[i].FindControl("dpl_processMachineid") as DropDownList;
                Label label_processid = MainDataGrid.Items[i].FindControl("Label_ProcessID") as Label;
                DropDownList dpl_unnormalReasons = MainDataGrid.Items[i].FindControl("dpl_unnormalReasons") as DropDownList;
                DropDownList dpl_DepartMent = MainDataGrid.Items[i].FindControl("dpl_DepartMent") as DropDownList;
                DropDownList dpl_Supply = MainDataGrid.Items[i].FindControl("dpl_Supply") as DropDownList;
                int processno = Convert.ToInt32(((Label)MainDataGrid.Items[i].FindControl("Label_processno")).Text);

                if (processno == 0)
                {
                    if (i == 0)
                    {
                        dt.Rows[i][dt.Columns["StatusID"]] = "Ready";
                        dt.Rows[i + 1][dt.Columns["StatusID"]] = "Pending";
                    }
                    else
                    {
                        string prestatusid = ((Label)MainDataGrid.Items[i - 1].FindControl("dg_lab_StatusID")).Text;
                        if (prestatusid.Equals("XIAODAN"))
                        {
                            dt.Rows[i][dt.Columns["StatusID"]] = "Ready";
                        }

                        if (prestatusid.Equals("Implementation"))
                        {
                            ModuleWorkFlow.Model.ProcessInfo pi = new ModuleWorkFlow.BLL.Process().GetProcessInfoById(label_processid.Text);
                            if (pi.StartDirectly)
                                dt.Rows[i][dt.Columns["StatusID"]] = "Ready";
                        }

                        if (dt.Rows[i][dt.Columns["StatusID"]].Equals("Ready") && i < MainDataGrid.Items.Count - 1)
                        {
                            dt.Rows[i + 1][dt.Columns["StatusID"]] = "Pending";
                        }
                    }

                }



                string oldDay = dt.Rows[i][dt.Columns["Day"]].ToString();
                string oldHour = dt.Rows[i][dt.Columns["Hour"]].ToString();
                string oldMintes = dt.Rows[i][dt.Columns["Minute"]].ToString();
                string oldSelfStartDate = dt.Rows[i][dt.Columns["SelfStartDate"]].ToString();
                string oldStartHour = dt.Rows[i][dt.Columns["StartHour"]].ToString();
                string oldSelfEndDate = dt.Rows[i][dt.Columns["SelfEndDate"]].ToString();
                string oldEndHour = dt.Rows[i][dt.Columns["EndHour"]].ToString();
                int oldisunnormal = Convert.ToInt32(dt.Rows[i][dt.Columns["IsUnnormal"]]);
                string statusid = dt.Rows[i][dt.Columns["StatusID"]].ToString();
                string pricetype = dt.Rows[i][dt.Columns["pricetype"]].ToString();
                string machineid = dt.Rows[i][dt.Columns["processMachineid"]].ToString();
                string processComment = dt.Rows[i][dt.Columns["processComment"]].ToString();
                string oldunnormalReasons = dt.Rows[i][dt.Columns["unNormalReasonsId"]].ToString();
                string oldfactstartdate = dt.Rows[i][dt.Columns["FactStartDate"]].ToString();
                string oldfactenddate = dt.Rows[i][dt.Columns["FactEndDate"]].ToString();
                string oldDepartMent = dt.Rows[i][dt.Columns["departMentId"]].ToString();
                string oldSupply = dt.Rows[i][dt.Columns["supplyId"]].ToString();

                int odlchk_Ispcnc = Convert.ToInt32(dt.Rows[i][dt.Columns["chk_Ispcnc"]]);
                string oldrelationpartno = dt.Rows[i][dt.Columns["RelationPartNo"]].ToString();

                ModuleWorkFlow.business.Methods.DropDownListChange(dropDownList_Day, oldDay);
                ModuleWorkFlow.business.Methods.DropDownListChange(dropDownList_Hour, oldHour);
                ModuleWorkFlow.business.Methods.DropDownListChange(dropDownList_Minute, oldMintes);
                txt_startDate.Text = oldSelfStartDate;
                ModuleWorkFlow.business.Methods.DropDownListChange(dpl_startHour, oldStartHour);
                txt_endDate.Text = oldSelfEndDate;
                ModuleWorkFlow.business.Methods.DropDownListChange(dpl_endHour, oldEndHour);
                lab_factstartDate.Text = oldfactstartdate;
                lab_factendDate.Text = oldfactenddate;

                if (oldisunnormal == 0)//whd080901 
                {
                    cbl_isunnormal.Checked = false;
                }
                //else
                //{
                //    cbl_isunnormal.Checked = true;
                //}

                if (odlchk_Ispcnc == 0)//程式
                {
                    chk_Ispcnc.Checked = false;
                }
                else
                {
                    chk_Ispcnc.Checked = true;
                }

                lab_StatusID.Text = statusid;
                txt_processComment.Text = processComment;
                try
                {
                    dpl_processMachineid.SelectedValue = machineid;
                }
                catch { }
                try
                {
                    dpl_unnormalReasons.SelectedValue = oldunnormalReasons;
                }
                catch { }

                try
                {
                    dpl_DepartMent.SelectedValue = oldDepartMent;
                }
                catch { }
                try
                {
                    dpl_Supply.SelectedValue = oldSupply;
                }
                catch { }



        }

            disableProductedProcess();
            formatDataGrid();
        }

        private DataTable BuildEditDataTable(List<XHS.Model.XHSPartPartProcessInfo> processList)
        {
            DataTable table = new DataTable();
            table.Columns.Add("listorder", typeof(int));
            table.Columns.Add("partno", typeof(string));
            table.Columns.Add("customerProcessid", typeof(string));
            table.Columns.Add("customerprocessname", typeof(string));
            table.Columns.Add("supplyId", typeof(int));
            table.Columns.Add("processid", typeof(string));
            table.Columns.Add("statusid", typeof(string));
            table.Columns.Add("processname", typeof(string));
            table.Columns.Add("factstartdate", typeof(DateTime));
            table.Columns.Add("factenddate", typeof(DateTime));
            table.Columns.Add("processno", typeof(int));
            table.Columns.Add("NeedProduct", typeof(int));
            table.Columns.Add("unNormalReasonsId", typeof(int));
            table.Columns.Add("departMentId", typeof(int));
            table.Columns.Add("processneedminutes", typeof(int));
            table.Columns.Add("totalminutes", typeof(int));
            table.Columns.Add("selfStartDate", typeof(DateTime));
            table.Columns.Add("selfEndDate", typeof(DateTime));
            table.Columns.Add("isUnnormal", typeof(int));
            table.Columns.Add("processComment", typeof(string));
            table.Columns.Add("processMachineid", typeof(string));
            table.Columns.Add("pricetype", typeof(string));
            table.Columns.Add("RelationPartNo", typeof(string));
            table.Columns.Add("RelationPartNoId", typeof(string));

            foreach (XHS.Model.XHSPartPartProcessInfo process in processList)
            {
                DataRow row = table.NewRow();
                row["listorder"] = process.ListOrder.HasValue ? (object)process.ListOrder.Value : DBNull.Value;
                row["partno"] = process.PartNo ?? "";
                row["customerProcessid"] = process.ProcessId ?? "";
                row["customerprocessname"] = process.ProcessName ?? "";
                row["supplyId"] = process.SupplyId.HasValue ? (object)process.SupplyId.Value : DBNull.Value;
                row["processid"] = process.ProcessId ?? "";
                row["statusid"] = process.StatusId ?? "";
                row["processname"] = process.ProcessName ?? "";
                row["factstartdate"] = process.FactStartDate.HasValue ? (object)process.FactStartDate.Value : DBNull.Value;
                row["factenddate"] = process.FactEndDate.HasValue ? (object)process.FactEndDate.Value : DBNull.Value;
                row["processno"] = process.ProcessNo.HasValue ? (object)process.ProcessNo.Value : DBNull.Value;
                row["NeedProduct"] = process.NeedProduct.HasValue ? (object)process.NeedProduct.Value : 0;
                row["unNormalReasonsId"] = process.UnNormalReasonsId.HasValue ? (object)process.UnNormalReasonsId.Value : 0;
                row["departMentId"] = process.DepartmentId.HasValue ? (object)process.DepartmentId.Value : 0;
                row["processneedminutes"] = process.ProcessNeedMinutes.HasValue ? (object)process.ProcessNeedMinutes.Value : 0;
                row["totalminutes"] = process.TotalMinutes.HasValue ? (object)process.TotalMinutes.Value : 0;
                row["selfStartDate"] = process.StartDate.HasValue ? (object)process.StartDate.Value : DBNull.Value;
                row["selfEndDate"] = process.EndDate.HasValue ? (object)process.EndDate.Value : DBNull.Value;
                row["isUnnormal"] = process.IsUnnormal.HasValue ? (object)process.IsUnnormal.Value : 0;
                row["processComment"] = process.ProcessComment ?? "";
                row["processMachineid"] = process.ProcessMachineId ?? "";
                row["pricetype"] = process.PriceType ?? "";
                row["RelationPartNo"] = process.RelationPartNo ?? "";
                row["RelationPartNoId"] = process.RelationPartNoId ?? "";
                table.Rows.Add(row);
            }

            return table;
        }

        private void DeleteDataSource(int index) //080901
        {
            lab_zuli.Text = "";
            int selectRow = Convert.ToInt32(Label_HiddenSelectRow.Text);
            if (selectRow > MainDataGrid.Items.Count - 2)
            {
                selectRow = -1;
                Label_HiddenSelectRow.Text = "-1";
            }
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add(new DataColumn("ListOrder", typeof(string)));
            dt.Columns.Add(new DataColumn("ProcessId", typeof(string)));
            dt.Columns.Add(new DataColumn("CustomerProcessId", typeof(string)));
            dt.Columns.Add(new DataColumn("CustomerProcessName", typeof(string)));
            dt.Columns.Add(new DataColumn("Day", typeof(string)));
            dt.Columns.Add(new DataColumn("Hour", typeof(string)));
            dt.Columns.Add(new DataColumn("Minute", typeof(string)));
            dt.Columns.Add(new DataColumn("ProcessNo", typeof(string)));
            dt.Columns.Add(new DataColumn("SelfStartDate", typeof(string)));
            dt.Columns.Add(new DataColumn("StartHour", typeof(string)));
            dt.Columns.Add(new DataColumn("SelfEndDate", typeof(string)));
            dt.Columns.Add(new DataColumn("EndHour", typeof(string)));
            dt.Columns.Add(new DataColumn("FactStartDate", typeof(string)));
            dt.Columns.Add(new DataColumn("FactEndDate", typeof(string)));
            dt.Columns.Add(new DataColumn("IsUnnormal", typeof(int)));
            dt.Columns.Add(new DataColumn("StatusID", typeof(string)));
            dt.Columns.Add(new DataColumn("pricetype", typeof(string)));
            dt.Columns.Add(new DataColumn("processComment", typeof(string)));
            dt.Columns.Add(new DataColumn("processMachineid", typeof(string)));
            dt.Columns.Add(new DataColumn("unNormalReasonsId", typeof(string)));
            dt.Columns.Add(new DataColumn("departMentId", typeof(string)));
            dt.Columns.Add(new DataColumn("supplyId", typeof(string)));

            dt.Columns.Add(new DataColumn("chk_Ispcnc", typeof(int)));//程式
            dt.Columns.Add(new DataColumn("RelationPartNo", typeof(string)));
            dt.Columns.Add(new DataColumn("RelationPartNoId", typeof(string)));


            string delstatusid = "";
            for (int i = 0; i < MainDataGrid.Items.Count; i++)
            {
                if (index != i)
                {
                    Label Label_OrderNo = MainDataGrid.Items[i].FindControl("Label_OrderNo") as Label;
                    Label label_processid = MainDataGrid.Items[i].FindControl("Label_ProcessID") as Label;
                    Label Label_CustomerProcessID = MainDataGrid.Items[i].FindControl("Label_CustomerProcessID") as Label;
                    Label Label_CustomerProcessName = MainDataGrid.Items[i].FindControl("Label_CustomerProcessName") as Label;
                    DropDownList dropDownList_Day = MainDataGrid.Items[i].FindControl("DropDownList_Day") as DropDownList;
                    DropDownList dropDownList_Hour = MainDataGrid.Items[i].FindControl("DropDownList_Hour") as DropDownList;
                    DropDownList dropDownList_Minute = MainDataGrid.Items[i].FindControl("DropDownList_Minute") as DropDownList;
                    TextBox txt_startDate = MainDataGrid.Items[i].FindControl("txt_startDate") as TextBox;
                    DropDownList dpl_startHour = MainDataGrid.Items[i].FindControl("dpl_startHour") as DropDownList;
                    TextBox txt_endDate = MainDataGrid.Items[i].FindControl("txt_endDate") as TextBox;
                    DropDownList dpl_endHour = MainDataGrid.Items[i].FindControl("dpl_endHour") as DropDownList;
                    Label lab_factstartDate = MainDataGrid.Items[i].FindControl("Label_FactStartDate") as Label;
                    Label lab_factendDate = MainDataGrid.Items[i].FindControl("Label_FactEndDate") as Label;
                    CheckBox cbl_isunnormal = MainDataGrid.Items[i].FindControl("cbl_isunnormal") as CheckBox;//whd080901 增加是否返修

                    CheckBox chk_Ispcnc = MainDataGrid.Items[i].FindControl("chk_Ispcnc") as CheckBox;//程式
                    Label dg_lab_relationpartno = MainDataGrid.Items[i].FindControl("dg_lab_relationpartno") as Label;
                    Label dg_lab_relationpartnoId = MainDataGrid.Items[i].FindControl("dg_lab_relationpartnoId") as Label;

                    Label lab_StatusID = MainDataGrid.Items[i].FindControl("dg_lab_StatusID") as Label;
                    if (i == index + 1)
                        lab_StatusID.Text = delstatusid;
                    Label lab_pricetype = MainDataGrid.Items[i].FindControl("dg_lab_pricetype") as Label;
                    TextBox txt_processComment = MainDataGrid.Items[i].FindControl("txt_processComment") as TextBox;
                    DropDownList dpl_processMachineid = MainDataGrid.Items[i].FindControl("dpl_processMachineid") as DropDownList;
                    string processno = (MainDataGrid.Items[i].FindControl("Label_processno") as Label).Text;
                    DropDownList dpl_unnormalReasons = MainDataGrid.Items[i].FindControl("dpl_unnormalReasons") as DropDownList;
                    DropDownList dpl_DepartMent = MainDataGrid.Items[i].FindControl("dpl_DepartMent") as DropDownList;
                    DropDownList dpl_Supply = MainDataGrid.Items[i].FindControl("dpl_Supply") as DropDownList;


                    dr = dt.NewRow();

                    dr["ListOrder"] = Label_OrderNo.Text.Trim();
                    dr["ProcessId"] = label_processid.Text.Trim();
                    dr["CustomerProcessId"] = Label_CustomerProcessID.Text.Trim();
                    dr["CustomerProcessName"] = Label_CustomerProcessID.Text.Trim();
                    dr["Day"] = dropDownList_Day.SelectedValue;
                    dr["Hour"] = dropDownList_Hour.SelectedValue;
                    dr["Minute"] = dropDownList_Minute.SelectedValue;
                    dr["ProcessNo"] = processno;
                    dr["SelfStartDate"] = txt_startDate.Text.Trim();
                    dr["StartHour"] = dpl_startHour.SelectedValue;
                    dr["SelfEndDate"] = txt_endDate.Text.Trim();
                    dr["EndHour"] = dpl_endHour.SelectedValue;
                    dr["FactStartDate"] = lab_factstartDate.Text;
                    dr["FactEndDate"] = lab_factendDate.Text;
                    if (cbl_isunnormal.Checked)//whd080901 增加是否返修
                    {
                        dr["IsUnnormal"] = 1;
                    }
                    else
                    {
                        dr["IsUnnormal"] = 0;
                    }

                    if (chk_Ispcnc.Checked)//程式
                    {
                        dr["chk_Ispcnc"] = 1;
                    }
                    else
                    {
                        dr["chk_Ispcnc"] = 0;
                    }
                    dr["RelationPartNo"] = dg_lab_relationpartno.Text;
                    dr["RelationPartNoId"] = dg_lab_relationpartnoId.Text;
                    dr["StatusID"] = lab_StatusID.Text;
                    dr["pricetype"] = lab_pricetype.Text;
                    dr["processComment"] = txt_processComment.Text;
                    dr["processMachineid"] = dpl_processMachineid.SelectedValue;
                    if (lab_pricetype.Text.Equals("ZULI"))
                    {
                        lab_zuli.Text = dr["pricetype"].ToString();
                    }
                    dr["unNormalReasonsId"] = dpl_unnormalReasons.SelectedValue;
                    dr["departMentId"] = dpl_DepartMent.SelectedValue;
                    dr["supplyId"] = dpl_Supply.SelectedValue;


                    dt.Rows.Add(dr);
                }
                else
                {
                    delstatusid = ((Label)MainDataGrid.Items[i].FindControl("dg_lab_StatusID")).Text;

                }
            }


            DataView dv = new DataView(dt);

            MainDataGrid.DataSource = dv;
            MainDataGrid.DataBind();

            //init TextBox_NeedTime input box and DropDownList_UnitSelect
            for (int i = 0; i < MainDataGrid.Items.Count; i++)
            {
                if (i == selectRow) MainDataGrid.Items[i].BackColor = Setting.SELECTCOLOR;
                Label Label_OrderNo = MainDataGrid.Items[i].FindControl("Label_OrderNo") as Label;
                DropDownList DDLDay = MainDataGrid.Items[i].FindControl("DropDownList_Day") as DropDownList;
                DropDownList DDLHour = MainDataGrid.Items[i].FindControl("DropDownList_Hour") as DropDownList;
                DropDownList DDLMinute = MainDataGrid.Items[i].FindControl("DropDownList_Minute") as DropDownList;
                DropDownList dpl_startHour = MainDataGrid.Items[i].FindControl("dpl_startHour") as DropDownList;
                DropDownList dpl_endHour = MainDataGrid.Items[i].FindControl("dpl_endHour") as DropDownList;
                Label label_processid = MainDataGrid.Items[i].FindControl("Label_ProcessID") as Label;
                DropDownList dpl_processMachineid = MainDataGrid.Items[i].FindControl("dpl_processMachineid") as DropDownList;
                Label_OrderNo.Text = Convert.ToString(i + 1);
                ModuleWorkFlow.business.Methods.DropDownListInit(DDLDay, 0, Setting.DAYS);
                ModuleWorkFlow.business.Methods.DropDownListInit(DDLHour, 0, Setting.HOURS - 1);
                ModuleWorkFlow.business.Methods.EveryMinuteDropDownListInit(DDLMinute);
                ModuleWorkFlow.business.Methods.DropDownListInit(dpl_startHour, 0, Setting.HOURS - 1);
                ModuleWorkFlow.business.Methods.DropDownListInit(dpl_endHour, 0, Setting.HOURS - 1);
                DropDownList dpl_unnormalReasons = MainDataGrid.Items[i].FindControl("dpl_unnormalReasons") as DropDownList;
                DropDownList dpl_DepartMent = MainDataGrid.Items[i].FindControl("dpl_DepartMent") as DropDownList;
                DropDownList dpl_Supply = MainDataGrid.Items[i].FindControl("dpl_Supply") as DropDownList;


                ModuleWorkFlow.BLL.Machine machine = new ModuleWorkFlow.BLL.Machine();
                try
                {
                    dpl_processMachineid.DataSource = machine.GetAllMachineList(label_processid.Text.Trim());
                    dpl_processMachineid.DataValueField = "Id";
                    dpl_processMachineid.DataTextField = "Id";
                    dpl_processMachineid.DataBind();
                    dpl_processMachineid.Items.Insert(0, "");
                }
                catch
                {
                    dpl_processMachineid.Items.Clear();
                    dpl_processMachineid.Items.Insert(0, "");
                }
                ModuleWorkFlow.BLL.System.unNormalReasons unr = new ModuleWorkFlow.BLL.System.unNormalReasons();
                dpl_unnormalReasons.DataSource = unr.GetunNormalReasonsInfoAll();
                dpl_unnormalReasons.DataTextField = "unnormalReasons";
                dpl_unnormalReasons.DataValueField = "Id";
                dpl_unnormalReasons.DataBind();
                dpl_unnormalReasons.Items.Insert(0, "");



                ModuleWorkFlow.BLL.DepartMent dm = new DepartMent();
                dpl_DepartMent.DataSource = dm.GetAllDepartmentbyIsFix();
                dpl_DepartMent.DataTextField = "DepartName";
                dpl_DepartMent.DataValueField = "DepartMentId";
                dpl_DepartMent.DataBind();
                dpl_DepartMent.Items.Insert(0, "");


                dpl_Supply.Items.Clear();
                dpl_Supply.Items.Insert(0, "");



            }

            //retieved all data
            for (int i = 0; i < MainDataGrid.Items.Count; i++)
            {
                DropDownList dropDownList_Day = MainDataGrid.Items[i].FindControl("DropDownList_Day") as DropDownList;
                DropDownList dropDownList_Hour = MainDataGrid.Items[i].FindControl("DropDownList_Hour") as DropDownList;
                DropDownList dropDownList_Minute = MainDataGrid.Items[i].FindControl("DropDownList_Minute") as DropDownList;
                TextBox txt_startDate = MainDataGrid.Items[i].FindControl("txt_startDate") as TextBox;
                DropDownList dpl_startHour = MainDataGrid.Items[i].FindControl("dpl_startHour") as DropDownList;
                TextBox txt_endDate = MainDataGrid.Items[i].FindControl("txt_endDate") as TextBox;
                DropDownList dpl_endHour = MainDataGrid.Items[i].FindControl("dpl_endHour") as DropDownList;
                CheckBox cbl_isunnormal = MainDataGrid.Items[i].FindControl("cbl_isunnormal") as CheckBox;//whd080901 增加是否返修

                CheckBox chk_Ispcnc = MainDataGrid.Items[i].FindControl("chk_Ispcnc") as CheckBox;//程式
                Label dg_lab_relationpartno = MainDataGrid.Items[i].FindControl("dg_lab_relationpartno") as Label;
                Label dg_lab_relationpartnoId = MainDataGrid.Items[i].FindControl("dg_lab_relationpartnoId") as Label;

                Label lab_pricetype = MainDataGrid.Items[i].FindControl("dg_lab_pricetype") as Label;
                DropDownList dpl_unnormalReasons = MainDataGrid.Items[i].FindControl("dpl_unnormalReasons") as DropDownList;
                DropDownList dpl_DepartMent = MainDataGrid.Items[i].FindControl("dpl_DepartMent") as DropDownList;
                DropDownList dpl_Supply = MainDataGrid.Items[i].FindControl("dpl_Supply") as DropDownList;
                Label lab_factstartDate = MainDataGrid.Items[i].FindControl("Label_FactStartDate") as Label;
                Label lab_factendDate = MainDataGrid.Items[i].FindControl("Label_FactEndDate") as Label;
                TextBox txt_processComment = MainDataGrid.Items[i].FindControl("txt_processComment") as TextBox;
                DropDownList dpl_processMachineid = MainDataGrid.Items[i].FindControl("dpl_processMachineid") as DropDownList;

                string oldDay = dt.Rows[i][dt.Columns["Day"]].ToString();
                string oldHour = dt.Rows[i][dt.Columns["Hour"]].ToString();
                string oldMintes = dt.Rows[i][dt.Columns["Minute"]].ToString();
                string oldSelfStartDate = dt.Rows[i][dt.Columns["SelfStartDate"]].ToString();
                string oldStartHour = dt.Rows[i][dt.Columns["StartHour"]].ToString();
                string oldSelfEndDate = dt.Rows[i][dt.Columns["SelfEndDate"]].ToString();
                string oldEndHour = dt.Rows[i][dt.Columns["EndHour"]].ToString();
                int oldisunnormal = Convert.ToInt32(dt.Rows[i][dt.Columns["IsUnnormal"]]);
                string machineid = dt.Rows[i][dt.Columns["processMachineid"]].ToString();
                string processComment = dt.Rows[i][dt.Columns["processComment"]].ToString();
                string oldunnormalReasonsid = dt.Rows[i][dt.Columns["unNormalReasonsId"]].ToString();
                string oldDepartMentid = dt.Rows[i][dt.Columns["departMentId"]].ToString();
                string oldSupplyId = dt.Rows[i][dt.Columns["supplyId"]].ToString();
                string oldfactstartdate = dt.Rows[i][dt.Columns["FactStartDate"]].ToString();
                string oldfactenddate = dt.Rows[i][dt.Columns["FactEndDate"]].ToString();

                int odlchk_Ispcnc = Convert.ToInt32(dt.Rows[i][dt.Columns["chk_Ispcnc"]]);//程式
                string oldRelationPartNo = dt.Rows[i][dt.Columns["RelationPartNo"]].ToString();

                ModuleWorkFlow.business.Methods.DropDownListChange(dropDownList_Day, oldDay);
                ModuleWorkFlow.business.Methods.DropDownListChange(dropDownList_Hour, oldHour);
                ModuleWorkFlow.business.Methods.DropDownListChange(dropDownList_Minute, oldMintes);
                ModuleWorkFlow.business.Methods.DropDownListChange(dpl_startHour, oldStartHour);
                ModuleWorkFlow.business.Methods.DropDownListChange(dpl_endHour, oldEndHour);
                txt_startDate.Text = oldSelfStartDate;
                txt_endDate.Text = oldSelfEndDate;
                txt_processComment.Text = processComment;
                dpl_processMachineid.SelectedValue = machineid;
                //TextBox txt_processComment = MainDataGrid.Items[i].FindControl("txt_processComment") as TextBox;
                //DropDownList dpl_processMachineid = MainDataGrid.Items[i].FindControl("dpl_processMachineid") as DropDownList;
                lab_factstartDate.Text = oldfactstartdate;
                lab_factendDate.Text = oldfactenddate;

                if (oldisunnormal == 0)//whd080901 
                {
                    cbl_isunnormal.Checked = false;
                }
                else
                {
                    cbl_isunnormal.Checked = true;
                }

                if (odlchk_Ispcnc == 0)//程式
                {
                    chk_Ispcnc.Checked = false;
                }
                else
                {
                    chk_Ispcnc.Checked = true;
                }

                string isPart = System.Configuration.ConfigurationSettings.AppSettings["IsPart"];
                string isPartOnly = System.Configuration.ConfigurationSettings.AppSettings["PartNoOnly"];
                if (isPart != null && Convert.ToBoolean(isPart) && isPartOnly != null && Convert.ToBoolean(isPartOnly))
                    cbl_isunnormal.Enabled = false;
                txt_processComment.Text = processComment;
                try
                {
                    dpl_processMachineid.SelectedValue = machineid;
                }
                catch { }
                try
                {
                    dpl_unnormalReasons.SelectedValue = oldunnormalReasonsid;
                }
                catch { }

                try
                {
                    dpl_DepartMent.SelectedValue = oldDepartMentid;
                }
                catch { }



                try
                {
                    dpl_Supply.SelectedValue = oldSupplyId;
                }
                catch { }

            }

            disableProductedProcess();
            formatDataGrid();
        }

        private void MainDataGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            DeleteDataSource(e.Item.ItemIndex);
        }

        private void Button_AddEdit_Click(object sender, System.EventArgs e)
        {
            if (CheckUserInput() == true)
            {
                Edit();
            }
        }

        private bool CheckUserInput()
        {
            bool val = true;
            if (MainDataGrid.Items.Count == 0)
            {
                val = false;
                Label_Message.Text = Translate.translateString("不允許全部刪除");
            }
            for (int i = 0; i < MainDataGrid.Items.Count; i++)
            {
                DropDownList dropDownList_Day = MainDataGrid.Items[i].FindControl("DropDownList_Day") as DropDownList;
                DropDownList dropDownList_Hour = MainDataGrid.Items[i].FindControl("DropDownList_Hour") as DropDownList;
                DropDownList dropDownList_Minute = MainDataGrid.Items[i].FindControl("DropDownList_Minute") as DropDownList;
                TextBox txt_startDate = MainDataGrid.Items[i].FindControl("txt_startDate") as TextBox;
                TextBox txt_endDate = MainDataGrid.Items[i].FindControl("txt_endDate") as TextBox;
                //限定計劃開始時間和計劃結束時間的格式
                if ((!txt_endDate.Text.Trim().Equals("") && !txt_startDate.Text.Trim().Equals("")))
                {
                    try
                    {
                        DateTime endDate = Convert.ToDateTime(txt_endDate.Text);
                        DateTime startDate = Convert.ToDateTime(txt_startDate.Text);
                        if (startDate.CompareTo(endDate) > 0)
                        {
                            val = false;
                            Label_Message.Text = Lang.TXT_DATELOGICALERROR;
                        }
                    }
                    catch
                    {
                        val = false;
                        Label_Message.Text = Lang.TXT_DATEERROR;
                    }

                }
                else if (txt_endDate.Text.Trim().Equals("") && txt_startDate.Text.Trim().Equals(""))
                {

                }
                else
                {
                    Label_Message.Text = Lang.TXT_TIME;
                    val = false;
                }

            }


            return val;
        }

        private void Edit()//080901
        {
            string msg = save();

            if (msg.Equals(Lang.SAVE_SUCCESS))
            {
                Response.Redirect("PartModifyList.aspx?moduleid=" + Label_ModuleId.Text + "&msg=" + Server.UrlEncode(msg) + "&pageindex=" + Label_PageIndex.Text);
            }
            else
            {
                Label_Message.Text = msg;
            }
        }

        private string save()
        {
            string msg = "";
            //ModuleWorkFlow.BLL.PartProcess pp = new ModuleWorkFlow.BLL.PartProcess();
            Hashtable htProcess = new ModuleWorkFlow.BLL.Process().GetProcessIdProcess();


            List<PartPartProcessInfo> source = new List<PartPartProcessInfo>();
            for (int i = 0; i < MainDataGrid.Items.Count; i++)
            {
                PartPartProcessInfo ppi = new PartPartProcessInfo();
                int processno = 0;
                try
                {
                    Label Label_processno = MainDataGrid.Items[i].FindControl("Label_processno") as Label;
                    processno = Convert.ToInt32(Label_processno.Text);
                }
                catch
                {
                    processno = 0;
                }
                if (processno > 0)
                {
                    Label Currentlab_StatusID = MainDataGrid.Items[i].FindControl("dg_lab_StatusID") as Label;
                    //if (!Currentlab_StatusID.Text.Equals("Ready") && !Currentlab_StatusID.Text.Equals("Pending"))
                    //{
                    string partnoid = Label_PartNoId.Text.Trim();
                    ppi.ModuleId = Label_ModuleId.Text.Trim();
                    ppi.PartNo = Label_PartNo.Text;
                    ppi.PartNo_Id = partnoid;
                    ppi.ProcessNo = processno;
                    ppi.StatusId = Currentlab_StatusID.Text;
                    ppi.ProcessOrder = i + 1;

                    source.Add(ppi);
                    //}
                }
            }
            try
            {
                new ModuleWorkFlow.business.PartProcess().CheckModifyPartProcess(source);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            source.Clear();
            ModuleWorkFlow.BLL.PartPartProcess partPartProcess = new ModuleWorkFlow.BLL.PartPartProcess();
            List<PartPartProcessInfo> existingProcesses = partPartProcess.getListPartProcessInfo(
                Label_ModuleId.Text.Trim(),
                Label_PartNoId.Text.Trim()) ?? new List<PartPartProcessInfo>();
            for (int i = 0; i < MainDataGrid.Items.Count; i++)
            {
                PartPartProcessInfo ppi = new PartPartProcessInfo();
                Label label_processid = MainDataGrid.Items[i].FindControl("Label_ProcessID") as Label;
                Label Label_CustomerProcessID = MainDataGrid.Items[i].FindControl("Label_CustomerProcessID") as Label;
                ModuleWorkFlow.Model.ProcessInfo processinfo = (ModuleWorkFlow.Model.ProcessInfo)htProcess[label_processid.Text];

                DropDownList dropDownList_Day = MainDataGrid.Items[i].FindControl("DropDownList_Day") as DropDownList;
                DropDownList dropDownList_Hour = MainDataGrid.Items[i].FindControl("DropDownList_Hour") as DropDownList;
                DropDownList dropDownList_Minute = MainDataGrid.Items[i].FindControl("DropDownList_Minute") as DropDownList;
                TextBox txt_startDate = MainDataGrid.Items[i].FindControl("txt_startDate") as TextBox;
                DropDownList dpl_startHour = MainDataGrid.Items[i].FindControl("dpl_startHour") as DropDownList;
                TextBox txt_endDate = MainDataGrid.Items[i].FindControl("txt_endDate") as TextBox;
                DropDownList dpl_endHour = MainDataGrid.Items[i].FindControl("dpl_endHour") as DropDownList;
                CheckBox cbl_isunnormal = MainDataGrid.Items[i].FindControl("cbl_isunnormal") as CheckBox;//whd080901 增加是否返修
                TextBox txt_processComment = MainDataGrid.Items[i].FindControl("txt_processComment") as TextBox;
                DropDownList dpl_processMachineid = MainDataGrid.Items[i].FindControl("dpl_processMachineid") as DropDownList;
                DropDownList dpl_unnormalReasons = MainDataGrid.Items[i].FindControl("dpl_unnormalReasons") as DropDownList;
                DropDownList dpl_DepartMent = MainDataGrid.Items[i].FindControl("dpl_DepartMent") as DropDownList;
                DropDownList dpl_Supply = MainDataGrid.Items[i].FindControl("dpl_Supply") as DropDownList;


                CheckBox chk_Ispcnc = MainDataGrid.Items[i].FindControl("chk_Ispcnc") as CheckBox;
                Label dg_lab_relationpartno = MainDataGrid.Items[i].FindControl("dg_lab_relationpartno") as Label;
                Label dg_lab_relationpartnoId = MainDataGrid.Items[i].FindControl("dg_lab_relationpartnoId") as Label;
                ppi.Ispcnc = Convert.ToInt32(chk_Ispcnc.Checked);
                ppi.RelationPartNo = dg_lab_relationpartno.Text.Trim();
                ppi.RelationPartNoId = dg_lab_relationpartnoId.Text.Trim();
                ppi.ProcessId = label_processid.Text.Trim();
                ppi.CustomerProcessId = Label_CustomerProcessID.Text.Trim();
                ppi.ProcessMachineId = dpl_processMachineid.SelectedValue;
                ppi.ProcessComment = txt_processComment.Text;
                int needtime = Convert.ToInt32(dropDownList_Day.SelectedValue) * 60 * processinfo.HoursPerDay +
                    Convert.ToInt32(dropDownList_Hour.SelectedValue) * 60 +
                    Convert.ToInt32(dropDownList_Minute.SelectedValue);

                ppi.ProcessNeedMinutes = needtime;
                ppi.ProcessOrder = i + 1;


                string processno = (MainDataGrid.Items[i].FindControl("Label_processno") as Label).Text;

                string partnoid = Label_PartNoId.Text.Trim();
                ppi.ModuleId = Label_ModuleId.Text.Trim();
                ppi.PartNo = Label_PartNo.Text;
                ppi.PartNo_Id = partnoid;
                Label Currentlab_StatusID = MainDataGrid.Items[i].FindControl("dg_lab_StatusID") as Label;
                ppi.StatusId = Currentlab_StatusID.Text;


                ppi.Priority = Convert.ToInt32(ListBox_Priority.SelectedValue);
                ppi.StartDate = DateTime.Now;
                ppi.ProcessNo = Convert.ToInt32(processno);

                PartPartProcessInfo existingProcess = existingProcesses.Find(m => m.ProcessNo == ppi.ProcessNo);
                if (existingProcess != null)
                {
                    ppi.NeedProduct = existingProcess.NeedProduct;
                }

                if (!txt_startDate.Text.Equals("") && !txt_endDate.Text.Equals(""))
                {
                    ppi.SelfStartDate = Convert.ToDateTime(txt_startDate.Text + " " + dpl_startHour.SelectedValue + ":00:00");
                    ppi.SelfEndDate = Convert.ToDateTime(txt_endDate.Text + " " + dpl_endHour.SelectedValue + ":00:00");
                    ppi.isConfirm = 1;
                }
                else
                {
                    ppi.isConfirm = 0;
                }

                if (cbl_isunnormal.Checked)
                {
                    ppi.IsUnnormal = 1;
                    if (!dpl_unnormalReasons.SelectedValue.Equals(""))
                    {
                        ppi.UnNormalReasonsId = Convert.ToInt32(dpl_unnormalReasons.SelectedValue);
                        if (!dpl_DepartMent.SelectedValue.Equals(""))
                        {
                            if (!dpl_DepartMent.SelectedValue.Equals(""))
                            {
                                ppi.DepartMentId = Convert.ToInt32(dpl_DepartMent.SelectedValue);
                            }
                        }
                        else
                        {
                            return Translate.translateString("選擇返修原因后，請選擇責任部門");

                        }

                    }
                    else
                    {
                        return Translate.translateString("勾選返修后，請選擇返修原因");

                    }
                }
                else
                {
                    ppi.IsUnnormal = 0;
                    ppi.UnNormalReasonsId = 0;
                    ppi.DepartMentId = 0;
                }



                source.Add(ppi);

            }

            string userno = "";
            if (Session["userid"] != null)
            {
                userno = Session["userid"].ToString();
            }

            int affectedRows = ModuleWorkFlow.business.PartProcess.UpdatePartProcessFromModifyEdit(source);
            if (affectedRows == 0)
            {
                return "没有找到需要保存的工序记录";
            }
            return Lang.SAVE_SUCCESS;

        }

        private void deleteNotAvaibleProcessByPartID(string moduleid, string partnoid, int maxorder)
        {
            string querystring = "delete from tb_partprocess where moduleid=@moduleid and partno_id=@partnoid and processorder >@maxorder ";
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partnoid", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoid;
            pars[2] = new SqlParameter("@maxorder", SqlDbType.Int);
            pars[2].Value = maxorder;
            ModuleWorkFlow.business.Data.ExecuteNonQuery(querystring, pars);
        }


        private void disableProductedProcess()
        {
            string piestatus = "";
            for (int i = 0; i < MainDataGrid.Items.Count; i++)
            {
                //檢查這道工序是否已經開始做，如果做了，則選擇按鈕為虛，上移，下移為虛。
                string str = ((Label)MainDataGrid.Items[i].FindControl("dg_lab_StatusID")).Text;
                Label Label_processno = MainDataGrid.Items[i].FindControl("Label_processno") as Label;
                int processno = Convert.ToInt32(Label_processno.Text.Trim());
                Label label_processid = MainDataGrid.Items[i].FindControl("Label_ProcessID") as Label;
                Label dg_lab_pricetype = MainDataGrid.Items[i].FindControl("dg_lab_pricetype") as Label;
                //Button DELBtn = (Button)MainDataGrid.Items[i].Cells[22].Controls[0];
                Button DELBtn = (Button)MainDataGrid.Items[i].Cells[25].Controls[0];
                Button SELECTBtn = (Button)MainDataGrid.Items[i].Cells[0].Controls[0];
                //Button UpBtn = (Button)MainDataGrid.Items[i].Cells[20].Controls[0];
                Button UpBtn = (Button)MainDataGrid.Items[i].Cells[23].Controls[0];
                //Button DownBtn = (Button)MainDataGrid.Items[i].Cells[21].Controls[0];
                Button DownBtn = (Button)MainDataGrid.Items[i].Cells[24].Controls[0];
                if (processno > 0)
                {
                    DELBtn.Enabled = true;
                }
                else
                {
                    DELBtn.Enabled = true;
                }
                if (!str.Equals("Pending") && !str.Equals("Ready"))
                {

                    UpBtn.Enabled = false;

                    DownBtn.Enabled = false;
                    DELBtn.Enabled = false;
                    SELECTBtn.Enabled = false;
                    MainDataGrid.Items[i].BackColor = Color.Chocolate;
                }
                else if (!piestatus.Equals("") && !piestatus.Equals("Pending") && !piestatus.Equals("Ready"))
                {

                    UpBtn.Enabled = false;
                }




                if (i + 1 < MainDataGrid.Items.Count)
                {
                    if (!((Label)MainDataGrid.Items[i + 1].FindControl("dg_lab_StatusID")).Text.Equals("Pending") && !((Label)MainDataGrid.Items[i + 1].FindControl("dg_lab_StatusID")).Text.Equals("Ready"))
                    {

                        DownBtn.Enabled = false;
                    }
                }

                if (dg_lab_pricetype.Text.Equals("ALWAYSFIRST"))
                {

                    UpBtn.Enabled = false;
                    DownBtn.Enabled = false;
                    DELBtn.Enabled = false;
                    SELECTBtn.Enabled = false;
                    MainDataGrid.Items[i].BackColor = Color.Chocolate;
                }
                if (i > 0)
                {
                    if (((Label)MainDataGrid.Items[i - 1].FindControl("dg_lab_pricetype")).Text.Equals("ALWAYSFIRST"))
                    {

                        UpBtn.Enabled = false;
                    }
                }
                BLL.Process process = new ModuleWorkFlow.BLL.Process();

                piestatus = str;

            }
        }

        private void modifyProcessOrder(int index, int nextindex)
        {
            Label label_processid = MainDataGrid.Items[index].FindControl("Label_ProcessID") as Label;
            Label Label_CustomerProcessID = MainDataGrid.Items[index].FindControl("Label_CustomerProcessID") as Label;
            Label Label_CustomerProcessName = MainDataGrid.Items[index].FindControl("Label_CustomerProcessName") as Label;
            Label label_processno = MainDataGrid.Items[index].FindControl("Label_processno") as Label;
            DropDownList dropDownList_Day = MainDataGrid.Items[index].FindControl("DropDownList_Day") as DropDownList;
            DropDownList dropDownList_Hour = MainDataGrid.Items[index].FindControl("DropDownList_Hour") as DropDownList;
            DropDownList dropDownList_Minute = MainDataGrid.Items[index].FindControl("DropDownList_Minute") as DropDownList;
            TextBox txt_startdate = MainDataGrid.Items[index].FindControl("txt_startDate") as TextBox;
            DropDownList dpl_startHour = MainDataGrid.Items[index].FindControl("dpl_startHour") as DropDownList;
            TextBox txt_enddate = MainDataGrid.Items[index].FindControl("txt_endDate") as TextBox;
            DropDownList dpl_endHour = MainDataGrid.Items[index].FindControl("dpl_endHour") as DropDownList;
            Label label_factstartdate = MainDataGrid.Items[index].FindControl("Label_FactStartDate") as Label;
            Label label_factenddate = MainDataGrid.Items[index].FindControl("Label_FactEndDate") as Label;
            TextBox txt_processComment = MainDataGrid.Items[index].FindControl("txt_processComment") as TextBox;
            DropDownList dpl_processMachineid = MainDataGrid.Items[index].FindControl("dpl_processMachineid") as DropDownList;
            CheckBox cbl_isunnormal = MainDataGrid.Items[index].FindControl("cbl_isunnormal") as CheckBox;

            CheckBox chk_Ispcnc = MainDataGrid.Items[index].FindControl("chk_Ispcnc") as CheckBox;//程式
            Label dg_lab_relationpartno = MainDataGrid.Items[index].FindControl("dg_lab_relationpartno") as Label;
            Label dg_lab_relationpartnoId = MainDataGrid.Items[index].FindControl("dg_lab_relationpartnoId") as Label;

            DropDownList dpl_unnormalReasons = MainDataGrid.Items[index].FindControl("dpl_unnormalReasons") as DropDownList;
            DropDownList dpl_DepartMent = MainDataGrid.Items[index].FindControl("dpl_DepartMent") as DropDownList;
            DropDownList dpl_Supply = MainDataGrid.Items[index].FindControl("dpl_Supply") as DropDownList;

            //string processno = (MainDataGrid.Items[index].FindControl("Label_processno") as Label).Text;
            Label lab_pricetype = MainDataGrid.Items[index].FindControl("dg_lab_pricetype") as Label;


            Label label_processidNext = MainDataGrid.Items[nextindex].FindControl("Label_ProcessID") as Label;
            Label Label_CustomerProcessIDNext = MainDataGrid.Items[nextindex].FindControl("Label_CustomerProcessID") as Label;
            Label Label_CustomerProcessNameNext = MainDataGrid.Items[nextindex].FindControl("Label_CustomerProcessName") as Label;
            Label label_processnoNext = MainDataGrid.Items[nextindex].FindControl("Label_Processno") as Label;
            DropDownList dropDownList_DayNext = MainDataGrid.Items[nextindex].FindControl("DropDownList_Day") as DropDownList;
            DropDownList dropDownList_HourNext = MainDataGrid.Items[nextindex].FindControl("DropDownList_Hour") as DropDownList;
            DropDownList dropDownList_MinuteNext = MainDataGrid.Items[nextindex].FindControl("DropDownList_Minute") as DropDownList;
            TextBox txt_startdateNext = MainDataGrid.Items[nextindex].FindControl("txt_startDate") as TextBox;
            DropDownList dpl_startHourNext = MainDataGrid.Items[nextindex].FindControl("dpl_startHour") as DropDownList;
            TextBox txt_enddateNext = MainDataGrid.Items[nextindex].FindControl("txt_endDate") as TextBox;
            DropDownList dpl_endHourNext = MainDataGrid.Items[nextindex].FindControl("dpl_endHour") as DropDownList;
            Label label_factstartdateNext = MainDataGrid.Items[nextindex].FindControl("Label_FactStartDate") as Label;
            Label label_factenddateNext = MainDataGrid.Items[nextindex].FindControl("Label_FactEndDate") as Label;
            TextBox txt_processCommentNext = MainDataGrid.Items[nextindex].FindControl("txt_processComment") as TextBox;
            DropDownList dpl_processMachineidNext = MainDataGrid.Items[nextindex].FindControl("dpl_processMachineid") as DropDownList;
            CheckBox cbl_isunnormalNext = MainDataGrid.Items[nextindex].FindControl("cbl_isunnormal") as CheckBox;
            DropDownList dpl_unnormalReasonsNext = MainDataGrid.Items[nextindex].FindControl("dpl_unnormalReasons") as DropDownList;
            DropDownList dpl_DepartMentNext = MainDataGrid.Items[nextindex].FindControl("dpl_DepartMent") as DropDownList;
            DropDownList dpl_SupplyNext = MainDataGrid.Items[nextindex].FindControl("dpl_Supply") as DropDownList;
            //string processnoNext = (MainDataGrid.Items[nextindex].FindControl("Label_Processno") as Label).Text;
            Label lab_pricetypeNext = MainDataGrid.Items[nextindex].FindControl("dg_lab_pricetype") as Label;

            CheckBox chk_IspcncNext = MainDataGrid.Items[nextindex].FindControl("chk_Ispcnc") as CheckBox;//程式
            Label lab_relationpartnoNext = MainDataGrid.Items[nextindex].FindControl("dg_lab_relationpartno") as Label;
            Label lab_relationpartnoIdNext = MainDataGrid.Items[nextindex].FindControl("dg_lab_relationpartnoId") as Label;


            string temp_processid = label_processid.Text;
            string temp_processno = label_processno.Text;
            string temp_customerProcessid = Label_CustomerProcessID.Text;
            string temp_customerProcessname = Label_CustomerProcessName.Text;

            //string temp_statusname = label_statusname.Text;
            //string temp_tcstatusid = label_tcstatusid.Text;
            string temp_day = dropDownList_Day.SelectedValue;
            string temp_hour = dropDownList_Hour.SelectedValue;
            string temp_minute = dropDownList_Minute.SelectedValue;
            string temp_startdate = txt_startdate.Text;
            string temp_startHour = dpl_startHour.SelectedValue;
            string temp_enddate = txt_enddate.Text;
            string temp_endHour = dpl_endHour.SelectedValue;
            string temp_factstartdate = label_factstartdate.Text;
            string temp_factenddate = label_factenddate.Text;
            string temp_processcomment = txt_processComment.Text;
            string temp_processmachineid = dpl_processMachineid.SelectedValue;
            bool temp_isunnormal = cbl_isunnormal.Checked;

            bool temp_chk_Ispcnc = chk_Ispcnc.Checked;//程式
            string temp_relationpartno = dg_lab_relationpartno.Text;
            string temp_relationpartnoId = dg_lab_relationpartnoId.Text;

            string temp_unnormalreasons = dpl_unnormalReasons.SelectedValue;
            string temp_DepartMent = dpl_DepartMent.SelectedValue;
            string temp_SupplyId = dpl_Supply.SelectedValue;
            string temp_pricetype = lab_pricetype.Text;



            label_processid.Text = label_processidNext.Text;
            label_processno.Text = label_processnoNext.Text;
            Label_CustomerProcessID.Text = Label_CustomerProcessIDNext.Text;
            Label_CustomerProcessName.Text = Label_CustomerProcessNameNext.Text;
            //label_statusname.Text = label_statusnameNext.Text;
            //label_tcstatusid.Text = label_tcstatusidNext.Text;
            dropDownList_Day.SelectedValue = dropDownList_DayNext.SelectedValue;
            dropDownList_Hour.SelectedValue = dropDownList_HourNext.SelectedValue;
            dropDownList_Minute.SelectedValue = dropDownList_MinuteNext.SelectedValue;
            txt_startdate.Text = txt_startdateNext.Text;
            dpl_startHour.SelectedValue = dpl_startHourNext.SelectedValue;
            txt_enddate.Text = txt_enddateNext.Text;
            dpl_endHour.SelectedValue = dpl_endHourNext.SelectedValue;
            label_factstartdate.Text = label_factstartdateNext.Text;
            label_factenddate.Text = label_factenddateNext.Text;
            txt_processComment.Text = txt_processCommentNext.Text;
            ModuleWorkFlow.BLL.Machine machine = new ModuleWorkFlow.BLL.Machine();
            try
            {
                dpl_processMachineid.DataSource = machine.GetAllMachineList(label_processid.Text.Trim());
                dpl_processMachineid.DataValueField = "Id";
                dpl_processMachineid.DataTextField = "Id";
                dpl_processMachineid.DataBind();
                dpl_processMachineid.Items.Insert(0, "");
            }
            catch
            {
                dpl_processMachineid.Items.Clear();
                dpl_processMachineid.Items.Insert(0, "");
            }
            dpl_processMachineid.SelectedValue = dpl_processMachineidNext.SelectedValue;
            cbl_isunnormal.Checked = cbl_isunnormalNext.Checked;

            chk_Ispcnc.Checked = chk_IspcncNext.Checked;
            dg_lab_relationpartno.Text = lab_relationpartnoNext.Text;
            dg_lab_relationpartnoId.Text = lab_relationpartnoIdNext.Text;

            dpl_unnormalReasons.SelectedValue = dpl_unnormalReasonsNext.SelectedValue;
            dpl_DepartMent.SelectedValue = dpl_DepartMentNext.SelectedValue;
            dpl_Supply.SelectedValue = dpl_SupplyNext.SelectedValue;
            label_processno.Text = label_processnoNext.Text;
            lab_pricetype.Text = lab_pricetypeNext.Text;


            label_processidNext.Text = temp_processid;
            Label_CustomerProcessIDNext.Text = temp_customerProcessid;
            Label_CustomerProcessNameNext.Text = temp_customerProcessname;
            label_processnoNext.Text = temp_processno;
            //label_statusnameNext.Text = temp_statusname;
            //label_tcstatusidNext.Text = temp_tcstatusid;
            dropDownList_DayNext.SelectedValue = temp_day;
            dropDownList_HourNext.SelectedValue = temp_hour;
            dropDownList_MinuteNext.SelectedValue = temp_minute;
            txt_startdateNext.Text = temp_startdate;
            dpl_startHourNext.SelectedValue = temp_startHour;
            txt_enddateNext.Text = temp_enddate;
            dpl_endHourNext.SelectedValue = temp_endHour;
            label_factstartdateNext.Text = temp_factstartdate;
            label_factenddateNext.Text = temp_factenddate;
            txt_processCommentNext.Text = temp_processcomment;
            try
            {
                dpl_processMachineidNext.DataSource = machine.GetAllMachineList(label_processidNext.Text.Trim());
                dpl_processMachineidNext.DataValueField = "Id";
                dpl_processMachineidNext.DataTextField = "Id";
                dpl_processMachineidNext.DataBind();
                dpl_processMachineidNext.Items.Insert(0, "");
            }
            catch
            {
                dpl_processMachineidNext.Items.Clear();
                dpl_processMachineidNext.Items.Insert(0, "");
            }
            dpl_processMachineidNext.Text = temp_processmachineid;
            cbl_isunnormalNext.Checked = temp_isunnormal;

            chk_IspcncNext.Checked = temp_chk_Ispcnc;
            lab_relationpartnoNext.Text = temp_relationpartno;

            dpl_unnormalReasonsNext.SelectedValue = temp_unnormalreasons;
            dpl_DepartMentNext.SelectedValue = temp_DepartMent;
            dpl_SupplyNext.SelectedValue = temp_SupplyId;

            label_processnoNext.Text = temp_processno;
            lab_pricetypeNext.Text = temp_pricetype;


            formatDataGrid();

        }

        private void formatDataGrid()
        {
            for (int i = 0; i < MainDataGrid.Items.Count; i++)
            {
                Label Label_processno = MainDataGrid.Items[i].FindControl("Label_processno") as Label;
                int processno = Convert.ToInt32(Label_processno.Text.Trim());
                Label Label_CustomerProcessID = MainDataGrid.Items[i].FindControl("Label_CustomerProcessID") as Label;
                Label Label_CustomerProcessName = MainDataGrid.Items[i].FindControl("Label_CustomerProcessName") as Label;
                Label_CustomerProcessName.Text = ModuleWorkFlow.business.Methods.TranslateProcessName(Label_CustomerProcessID.Text);
                Label label_statusid = MainDataGrid.Items[i].FindControl("Label_StatusID") as Label;//顯示名稱的列
                string statusid = ((Label)MainDataGrid.Items[i].FindControl("dg_lab_StatusID")).Text;
                ColorConverter cc = new ColorConverter();
                MainDataGrid.Items[i].BackColor = (Color)cc.ConvertFromString(ModuleWorkFlow.business.Methods.TranslateStatusColor(statusid));
                label_statusid.Text = ModuleWorkFlow.business.Methods.TranslateStatusName(statusid);

                Button DELBtn = (Button)MainDataGrid.Items[i].Cells[24].Controls[0];
                DELBtn.Enabled = true;
            }
        }

        protected void lnkbutton_save_Click(object sender, EventArgs e)
        {
            Button_AddEdit_Click(sender, e);
        }

        protected void lnkbutton_view_Click(object sender, EventArgs e)
        {
            Tmenu menu = new Tmenu();
            TmenuInfo mi = menu.findbykey(menuid);

            if (mi != null)
            {
                if (mi.Url.Equals("WithOutRelationPartModifyList.aspx"))
                {
                    Response.Redirect("WithOutRelationPartModifyList.aspx?menuid=F302&moduleid=" + Label_ModuleId.Text + "&pageindex=" + Label_PageIndex.Text);
                }
                else
                {
                    Response.Redirect("PartModifyList.aspx?menuid=F32&moduleid=" + Label_ModuleId.Text + "&pageindex=" + Label_PageIndex.Text);
                }
            }
        }


        protected void lnkbutton_editrelation_Click(object sender, EventArgs e)
        {
            Hashtable hashchecked = new Hashtable();
            for (int i = 0; i < MainDataGrid.Items.Count; i++)
            {
                CheckBox cbx_relation = MainDataGrid.Items[i].FindControl("cbx_relation") as CheckBox;
                if (cbx_relation.Checked == true)
                {
                    hashchecked.Add(i, i);
                }
            }
            if (CheckUserInput() == true)
            {
                string msg = save();

                if (msg.Equals(Lang.SAVE_SUCCESS))
                {
                    dataBindHandly(Label_ModuleId.Text, Label_PartNoId.Text);
                    string processnolist = "";
                    for (int i = 0; i < MainDataGrid.Items.Count; i++)
                    {
                        if (hashchecked.Contains(i))
                        {
                            CheckBox cbx_relation = MainDataGrid.Items[i].FindControl("cbx_relation") as CheckBox;
                            //if (cbx_relation.Checked == true)
                            //{
                            Label Label_processno = MainDataGrid.Items[i].FindControl("Label_processno") as Label;
                            processnolist += "," + Label_processno.Text;
                            //}
                        }
                    }

                    if (processnolist.Equals(""))
                    {
                        Label_Message.Text = Translate.translateString("請選擇零件");
                        return;
                    }
                    else
                    {
                        if (new PartprocessRelation().isSamesPartProcessRelationInfos(processnolist) != null)
                        {
                            Response.Redirect("PartProcessRelationView.aspx?menuid=F32&moduleid=" + Label_ModuleId.Text + "&pageindex=" + MainDataGrid.CurrentPageIndex + "&PartProcessno=" + processnolist + "&priority=" + Label_HiddenPriority.Text);
                        }
                        else
                        {
                            Label_Message.Text = Translate.translateString("這些零件掛靠不相同零件，無法同時掛靠");
                            return;
                        }
                    }
                }
                else
                {
                    Label_Message.Text = msg;
                }
            }
        }

    }
}
