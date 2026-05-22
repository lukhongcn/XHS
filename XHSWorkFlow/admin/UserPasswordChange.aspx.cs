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

using ModuleWorkFlow.BLL;
using ModuleWorkFlow.Model;
using Utility;

namespace ModuleWorkFlow.admin
{
    /// <summary>
    /// Summary description for UserEdit.
    /// </summary>
    public class UserPasswordChange : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Label lab_UserName;
        protected System.Web.UI.WebControls.Label lab_Name;
        protected System.Web.UI.WebControls.TextBox txt_Email;
        protected TextBox txt_password;
        protected System.Web.UI.WebControls.Label Label_Message;
        protected System.Web.UI.HtmlControls.HtmlTable Table4;

        protected string menuname = "";
        private string menuid = "A211";

        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Session["userid"] == null)
                {
                    Response.Redirect("../login.aspx");
                }

                TmenuInfo menuInfo = new Tmenu().findbykey(menuid);
                menuname = menuInfo == null ? "修改密码" : menuInfo.Menuname;

                string username = Session["userid"].ToString();

                ModuleWorkFlow.BLL.User user = new ModuleWorkFlow.BLL.User();
                UserInfo ui = user.getUserInfo(username);
                if (ui == null)
                {
                    string msg = Lang.TXT_USERLIST_ErrorURL;
                    Response.Redirect("../login.aspx");
                }
                else
                {
                    lab_Name.Text = ui.Name;
                    lab_UserName.Text = ui.UserName;
                    txt_Email.Text = ui.Email;
                }
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
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion

        protected void lnkbutton_save_Click(object sender, EventArgs e)
        {
            ModuleWorkFlow.BLL.User user = new ModuleWorkFlow.BLL.User();
            UserInfo ui = user.getUserInfo(lab_UserName.Text);
            if (ui == null)
            {
                Label_Message.Text = Lang.TXT_USERLIST_ErrorURL;
                return;
            }

            ui.Password = txt_password.Text.Trim();
            ui.Email = txt_Email.Text.Trim();
            bool ret = user.updateUserInfoPassWord(ui);
            if (ret == true)
            {
                Label_Message.Text = Lang.TXT_USERLIST_SaveSucc;
            }
            else
            {
                Label_Message.Text = Lang.TXT_USERLIST_Savefailed;
            }
        }
    }
}
