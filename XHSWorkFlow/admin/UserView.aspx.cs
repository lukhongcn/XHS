using ModuleWorkFlow.BLL;
using ModuleWorkFlow.business;
using ModuleWorkFlow.Model;
using ModuleWorkFlow.Model.System;
using System;
using System.Collections;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Utility;
namespace ModuleWorkFlow
{
    /// <summary>
    /// Summary description for WebForm1.
    /// </summary>
    public class UserView : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Label Label_StatusMsg;
        protected System.Web.UI.HtmlControls.HtmlTable Table1;
        protected System.Web.UI.WebControls.Label Label_Message;

        private User user;
        protected System.Web.UI.WebControls.TextBox txt_name;
        protected System.Web.UI.WebControls.TextBox txt_username;
        protected System.Web.UI.WebControls.TextBox txt_cardid;
  
        protected System.Web.UI.WebControls.DropDownList ddl_department;
        private string menuid = "A21";
        protected System.Web.UI.WebControls.CustomValidator val_username;
        protected System.Web.UI.WebControls.TextBox txt_email;
        protected DropDownList ddl_shift;


        protected TextBox txt_password;
        private DepartMent department;
        protected string menuname = "";
        private void Page_Load(object sender, System.EventArgs e)
        {
            user = new User();
            department = new DepartMent();
            PartTmenu menu = new PartTmenu();
            PartTmenuInfo mi = menu.findbykey(menuid);
        

            if (this.Master != null && this.Master is DefaultSub)
            {
                DefaultSub master = (DefaultSub)this.Master;

                master.Menuname = mi.Menuname;
                menuname = mi.Menuname;
            }
            if (!this.IsPostBack)
            {
                if (ModuleWorkFlow.BLL.Private.checkPrivate(this, menuid, "PQUERY"))
                {
                    bind();
                }
            }
        }
        private void bind()
        {


            ddl_department.DataSource = new ModuleWorkFlow.BLL.DepartMent().GetAllDepartment();
            ddl_department.DataTextField = "DepartmentName";
            ddl_department.DataValueField = "DepartmentId";
            ddl_department.DataBind();

            ddl_shift.DataSource = new BLL.System.Shift().GetShiftInfos().Cast<ShiftInfo>().ToList().FindAll(m => !string.IsNullOrEmpty(m.ShowName));
            ddl_shift.DataTextField = "ShowName";
            ddl_shift.DataValueField = "Id";
            ddl_shift.DataBind();
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
            this.val_username.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.val_username_ServerValidate);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        private void val_username_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            //MYD070507-2
            //MYD070601
            if (user.getUserInfo(txt_username.Text) != null)
            {
                args.IsValid = false;
                val_username.ErrorMessage = Lang.UserView_DuplicateName;
            }
            else
            {
                args.IsValid = true;
            }

        }

        private void bt_addUser_Click(object sender, System.EventArgs e)
        {
            Label_Message.Text = "";
            if (txt_username.Text.Trim().Equals("") || txt_name.Text.Trim().Equals("") || txt_cardid.Text.Trim().Equals(""))
            {
                Label_Message.Text =Translate.translateString("員工編號")+","+ Translate.translateString("姓名")+","+Translate.translateString("卡號")+":" + Lang.NO_SPACES;
                return;
            }
            string[] temp = txt_username.Text.Trim().Split('$');
            if (temp.Length > 1)
            {
                Label_Message.Text = Translate.translateString("用戶編號不能出現特殊字符")+" $ ";
                return;
            }
            if (Page.IsValid)
            {
                if (ModuleWorkFlow.BLL.Private.checkPrivate(this, menuid, "PADD"))
                {

                    ShiftInfo si = new BLL.System.Shift().findbykey(Convert.ToInt32(ddl_shift.SelectedValue));

                    UserInfo ui = new UserInfo();
                    ui.Name = txt_name.Text.Trim();
                    ui.UserName = txt_username.Text.Trim();
                    ui.CardId = txt_cardid.Text.Trim();
                    ui.DepartId = ddl_department.SelectedValue.Trim();
                    ui.Email = txt_email.Text.Trim();
                    ui.ShiftId = Convert.ToInt32(ddl_shift.SelectedValue);
                    ui.StartTime = si.StartTime;
                    ui.WorkHours = Math.Round(Convert.ToDouble(si.WorkMinutes) / 60.0, 2);

                    if (si.NoonBreakStart.Ticks > 0 && si.NoonBreakEnd.Ticks > 0)
                    {
                        ui.BreakHours = si.NoonBreakEnd.Subtract(si.NoonBreakStart).TotalHours;
                    }

                    if (si.EveBreakStart.Ticks > 0 && si.EveBreakEnd.Ticks > 0)
                    {
                        ui.BreakHours = si.EveBreakEnd.Subtract(si.EveBreakStart).TotalHours;
                    }

                    ui.BreakHours = Math.Round(ui.BreakHours, 2);
                    //MYD070424
                    ui.RoleId = "normal";
                    ui.Password = txt_password.Text.Trim();
                    ui.IsResignation = 0;

                    if (user.insertUserInfo(ui))
                    {
                        Label_Message.Text = Lang.TXT_ADDSUCCESS;
                    }

                    //JG-26026
                    ArrayList auser = new ArrayList();
                    IList ipdm = new PrivateDepartMent().getPrivateByDepartmentId(Convert.ToInt32(ddl_department.SelectedValue));
                    foreach (PrivateDepartMentInfo pdmi in ipdm)
                    {
                        ModuleWorkFlow.Model.PrivateInfo pi = new PrivateInfo();
                        pi.USERNAME = txt_username.Text;
                        pi.MENUID = pdmi.MENUID;
                        pi.PQUERY = pdmi.PQUERY;
                        pi.PADD = pdmi.PADD;
                        pi.PEDIT = pdmi.PEDIT;
                        pi.PDEL = pdmi.PDEL;
                        pi.URL = pdmi.URL;
                        auser.Add(pi);
                    }
                    ModuleWorkFlow.BLL.Private p = new Private();
                    p.updatePrivateWithUserName(txt_username.Text, auser);
                    //JG-071112
                    ArrayList argp = new ArrayList();
                    ModuleWorkFlow.BLL.RoleGroupProcess rgp = new ModuleWorkFlow.BLL.RoleGroupProcess();
                    IList rgpis = rgp.getRoleGroupProcessView(Convert.ToInt32(Convert.ToInt32(ddl_department.SelectedValue)));

                    foreach (ModuleWorkFlow.Model.RoleGroupProcessInfo rgpi in rgpis)
                    {
                        argp.Add(rgpi.Processid);
                    }
                    RoleProcess.UpdateRoleProcess(txt_username.Text, argp);
                    //
                }

                //bind();
            }
        }

        protected void lnkbutton_save_Click(object sender, EventArgs e)
        {
            bt_addUser_Click(sender, e);
        }
    }
}
