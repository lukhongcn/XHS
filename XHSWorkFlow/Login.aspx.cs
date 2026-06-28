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
using System.Xml;
using ModuleWorkFlow.BLL;
using ModuleWorkFlow.Model;
using Utility;
using System.Collections.Generic;
namespace ModuleWorkFlow
{
	/// <summary>
	/// Summary description for Login.
	/// </summary>
	public class Login : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox TextBox_UserName;
		protected System.Web.UI.WebControls.Label Label_Message;
		protected System.Web.UI.HtmlControls.HtmlInputText HTML_Password;
		protected System.Web.UI.WebControls.Button Button_Login;
        protected System.Web.UI.WebControls.Label lab_Version;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
            if (!this.IsPostBack)
            {
                Session.Abandon();
            }
            showVersion();
		}

		private bool CheckLogin(string username,string password)
		{
			bool checkSucc = false;

            //username = "admin";
            //password = "123456";

            string querystring = "select * from tb_user where username=@username and password=@password and isResignation=0";
			SqlParameter[] pars = new SqlParameter[2];
			pars[0] = new SqlParameter("@username", SqlDbType.NVarChar,50);
			pars[0].Value = username;
			pars[1] = new SqlParameter("@password", SqlDbType.NVarChar,50);
			pars[1].Value = password;
			DataRow dr = Data.getDataRow(querystring,pars);
			if (dr != null)
			{
				ModuleWorkFlow.BLL.Private p = new ModuleWorkFlow.BLL.Private();
				IList ip = p.getPrivateByUserName(dr["username"].ToString());
				if (ip.Count == 0)
				{
					ip = new PrivateDepartMent().getPrivateByDepartmentId(Convert.ToInt32(dr["DepartmentId"]), dr["username"].ToString()) ;
				}
				//if (ip.Count > 0 || Convert.ToInt32(dr["isadmin"]) == 1)
				{
					Session["islogin"] = true;
					Session["userid"] = dr["username"];
					Session["departmentid"] = dr["DepartmentId"];
					//Session["roleid"] = dr["roleid"];
					Session["isadmin"] = Convert.ToInt32(dr["isadmin"]) == 1;
					Session["iprivate"] = ip;
					checkSucc = true;
				}
				/*else
				{
					Session["islogin"] = false;
					Session["userid"] = null;
					//Session["roleid"] = null;
					Session["isadmin"] = null;
					Session["iprivate"] = null;
				}*/
			}
			else
			{
				Session["islogin"] = false;
				Session["userid"] = null;
				//Session["roleid"] = null;
				Session["isadmin"] = null;
				Session["iprivate"] = null;
			}
			return checkSucc;

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
			this.Button_Login.Click += new System.EventHandler(this.Button_Login_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void Button_Login_Click(object sender, System.EventArgs e)
		{
			string username = TextBox_UserName.Text ;
			string password = HTML_Password.Value ;
	
			//if (checkReg() == true)
			//{
				if (CheckLogin(username,password) == true)
				{
                    if (Request.Params["ReturnUrl"] != null)
                    {
                        System.Web.Security.FormsAuthentication.SetAuthCookie(username, false);
                        Response.Redirect(Request.Params["ReturnUrl"]);

                    }
                    else
                    {
                        UserInfo ui = new User().getUserInfoByusername(username);
                        if (ui != null && ui.DefaultPage != null && !ui.DefaultPage.Trim().Equals(""))
                        {
                            System.Web.Security.FormsAuthentication.SetAuthCookie(username, false);
                            Response.Redirect(ui.DefaultPage);
                        }
                        else
                        {
                            System.Web.Security.FormsAuthentication.RedirectFromLoginPage(username, false);
                        }

                    }
				}
				else
				{
					Label_Message.Text = Lang.TXT_LOGINFAILED;
					HTML_Password.Value = "";
				}
			//}
			//else
			//{
   //             Label_Message.Text = Lang.TXT_NOREGISTED + "<a href='Regno/RegnoHelp.aspx' target=blank>请点击注册</a>";
			//	HTML_Password.Value = "";
			//}
		}
		
		private bool checkReg()
		{
			//DateTime dt = DateTime.Now;
			//if (dt.Year <= 2019 && dt.Month <= 9 && dt.Day <= 31)
			//{
			return true;
			//}
			//else
			//{
			//	bool ret = false;
			//	ModuleWorkFlow.BLL.RegNo gn = new RegNo();
			//	//string no = gn.getRegNo();
			//	string no = "20:04:0F:F3:3C:6C:";
			//             //no = no.Replace("_", "").Replace("\\", "6").Replace(":", "R");
			//             string key = gn.getMD5RegNo(gn.getHardInfo());
			//             string test = gn.getMD5RegNo(no);

			//             if (key.Equals(no)) ret = true;
			//	return ret;
		//}
		}

        public void showVersion()
        {
            HttpContext context = HttpContext.Current;
            string applicationpath = context.Request.ApplicationPath;
            string filepath = "TestVersion.xml";
            string name = "1";
            try
            {
                XmlDocument newdoc = new XmlDocument();
                newdoc.Load(Server.MapPath(applicationpath + "/" + filepath));
                XmlNode node = newdoc.DocumentElement;
                XmlNode newlastnode = node.LastChild;
                if (newlastnode != null)
                {
                    name = Convert.ToString(newlastnode.Attributes["versionname"].Value);
                }
            }
            catch { }
           
           
            
            //lab_Version.Text = "セ2.0." + name;
        }
	}
}
