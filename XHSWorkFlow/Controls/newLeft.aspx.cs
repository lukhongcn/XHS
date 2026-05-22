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
using System.Data.SqlClient;
using System.Text;
using Utility;
using System.Linq;
using ModuleWorkFlow.Model;
using System.Collections.Generic;
namespace ModuleWorkFlow.controls
{
    /// <summary>
    /// Summary description for newLeft.
    /// </summary>
    public class newLeft : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Label lab_menu;
        protected Microsoft.Web.UI.WebControls.TreeView tv_menu;
        protected System.Web.UI.WebControls.Label Label_MenuDisplay;
        protected System.Web.UI.WebControls.Label lab_name;

        private void Page_Load(object sender, System.EventArgs e)
        {

            if (Session["islogin"] == null || Convert.ToBoolean(Session["islogin"]) == false)
            {
                //MYD070507-5

                HttpContext context = HttpContext.Current;
                string applicationpath = context.Request.ApplicationPath;
                ModuleWorkFlow.BLL.Private p = new ModuleWorkFlow.BLL.Private();

                System.Web.Security.FormsAuthentication.SignOut();
                Response.Write("<script language=javascript>window.top.location.href='" + applicationpath + "/login.aspx';</script>");
                //    Response.Redirect(applicationpath+"\\login.aspx");
                return;
            }

            if (Session["userid"] != null)
            {
                //获取登录者姓名

                ModuleWorkFlow.Model.UserInfo userinfo = new ModuleWorkFlow.Model.UserInfo();
                ModuleWorkFlow.BLL.User user = new BLL.User();
                userinfo = user.getUserInfoByusername((Session["userid"].ToString()));
                lab_name.Text = "(" + userinfo.Name + ")";
            }

            if (!this.IsPostBack)
            {
                string menu = Request.Params["menu"];
                if (menu == null)
                    menu = System.Configuration.ConfigurationSettings.AppSettings["menu"];

                initMenu(menu);
            }
        }

        private void initMenu(string menu)
        {
            HttpContext context = HttpContext.Current;
            //            MYD070507-5
            string applicationpath = context.Request.ApplicationPath;
            ModuleWorkFlow.BLL.Private p = new ModuleWorkFlow.BLL.Private();

            ArrayList pl = (ArrayList)Session["iprivate"];
            ArrayList alMenu = new ArrayList();
            if (pl != null)
            {
                for (int i = 0; i < pl.Count; i++)
                {
                    ModuleWorkFlow.Model.PrivateInfo pi = (ModuleWorkFlow.Model.PrivateInfo)pl[i];
                    alMenu.Add(pi.MENUID);
                }
            }

            StringBuilder sb = new StringBuilder();

            string accessUrl = applicationpath + "/" + "NoPrivateMsg.aspx";

            //begin jeason 20071026
            string userid = Session["userid"].ToString();


            bool isadmin = Convert.ToBoolean(Session["isadmin"]);

            //end
            ModuleWorkFlow.BLL.PartTmenu tmenu = new ModuleWorkFlow.BLL.PartTmenu();
            List<PartTmenuInfo> source = tmenu.GetMenuByPrivate(userid, isadmin, menu).Cast<PartTmenuInfo>().ToList();
            List<PartTmenuInfo> menuSouce = new List<PartTmenuInfo>();
            if (source.FindAll(m => !m.Url.Equals("-")).Count == 0)
            {
                source = tmenu.GetMenuByPrivate(Convert.ToInt32(Session["departmentid"]), isadmin, menu).Cast<PartTmenuInfo>().ToList();
                List<PartTmenuInfo> subSouce = source.FindAll(m => !m.Url.Equals("-"));


                foreach (PartTmenuInfo mi in subSouce)
                {
                    menuSouce.AddRange(source.FindAll(m => m.Id == mi.ParentId));
                    menuSouce.Add(mi);
                }
            }
            else
            {
                menuSouce.AddRange(source);
            }



            //switch (menu)
            //{
            //    case "1":
            //        lab_menu.Text = "系统管理";
            //        break;
            //    case "2":
            //        lab_menu.Text = "生产管理";
            //        break;
            //    //case "2":
            //    //    lab_menu.Text="仓库管理";
            //    //    break;
            //    case "4":
            //        lab_menu.Text = "文档管理";
            //        break;
            //    default:
            //        break;
            //}


            new ModuleWorkFlow.WebView.PartMenuWebView().MenuWebTree(menuSouce, tv_menu);
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
    }
}