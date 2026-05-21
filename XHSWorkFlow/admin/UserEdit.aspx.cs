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
	public class UserEdit : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox txb_UserName;
		protected System.Web.UI.WebControls.TextBox txb_CardId;
		protected System.Web.UI.WebControls.TextBox txb_Name;
		protected System.Web.UI.WebControls.TextBox txb_Email;
		protected System.Web.UI.WebControls.Button Button_Edit;
		protected System.Web.UI.WebControls.Label Label_Message;
		protected System.Web.UI.HtmlControls.HtmlTable Table4;
     
        protected TextBox txt_comment;

        protected System.Web.UI.WebControls.TextBox txt_resignDate;

		protected System.Web.UI.WebControls.Label lab_id;
		protected System.Web.UI.WebControls.Label lab_name;

        protected System.Web.UI.WebControls.CheckBox chk_resignation;
        

		private string menuid = "A21";
        protected string menuname = "";
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(ModuleWorkFlow.BLL.Private.checkPrivate(this,menuid,"PEDIT"))
			{
				if (!this.IsPostBack)
                {
                    menuname = new PartTmenu().findbykey(menuid).Menuname;
					string username = Request.Params["username"];
					ModuleWorkFlow.BLL.User user = new ModuleWorkFlow.BLL.User();
					UserInfo ui = user.getUserInfo(username);
                    if (ui.UserName.Equals("admin"))
                    {
                        txb_UserName.ReadOnly = true;
                    }
					if (ui == null)
					{
						string msg = Lang.TXT_USERLIST_ErrorURL;
						Response.Redirect("UserList.aspx?msg="+Server.UrlEncode(msg),true);
					}
					else
					{
						initUserInputInfo(ui);
					}
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
			this.Button_Edit.Click += new System.EventHandler(this.Button_Edit_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void initUserInputInfo(UserInfo ui)
		{
			lab_id.Text = ui.Id.ToString();
			lab_name.Text = ui.UserName.ToString();
			txb_UserName.Text = ui.UserName;
			txb_Name.Text = ui.Name;
			txb_CardId.Text = ui.CardId;
			txb_Email.Text = ui.Email;
            if (ui.IsResignation == 1)
            { chk_resignation.Checked = true;  }
            else 
            { chk_resignation.Checked = false; }
          
            if (ui.ResignDate.Ticks == 0)
            {
                txt_resignDate.Text = "";
            }
            else
            {
                txt_resignDate.Text = ui.ResignDate.ToShortDateString();
            }

        }

		private bool checkUserInput()
		{
			bool ret = true;
			string username = txb_UserName.Text.Trim();
			string name = txb_Name.Text.Trim();
			string cardid = txb_CardId.Text.Trim();
			string email = txb_Email.Text.Trim();
            

            string[] temp = username.Trim().Split('$');
            if (temp.Length > 1)
            {
                ret = false;
                Label_Message.Text = "用户编号不能出现特殊字符" + " $ ";
            }

			if(ret == true && username.Equals(""))
			{
				ret = false;
				Label_Message.Text = Lang.TXT_USERLIST_InputUserName;
			}
			if(!lab_name.Text.Equals(username))
			{
				if(ret)
				{
					User user = new User();
					if(user.getUserInfo(username) !=null)
					{
						ret = false;
						Label_Message.Text = Lang.TXT_USERLIST_KeyDuplicated;
					}
				}
			}
			if (ret == true && name.Equals(""))
			{
				ret = false;
				Label_Message.Text = Lang.TXT_USERLIST_InputName;
			}
			

			return ret;
		}

		private bool save()
		{
            Label_Message.Text="";
			string username = txb_UserName.Text.Trim();
			string name = txb_Name.Text.Trim();
			string cardid = txb_CardId.Text.Trim();
			string email = txb_Email.Text.Trim();
            bool resignation = chk_resignation.Checked;
            string resignDate = txt_resignDate.Text.Trim();

			UserInfo ui = new UserInfo();
			ui.Id = Convert.ToInt32(lab_id.Text);
			ui.UserName = username;
			ui.Name = name;
            ui.Comment = txt_comment.Text.Trim();
			if (!cardid.Equals("")) ui.CardId = cardid;
			if (!email.Equals("")) ui.Email = email;
            if (resignation == true)
            {
                if (resignDate.Equals(""))
                {
                    Label_Message.Text = "请填写离职日期";
                    return false;
                }
                try
                {
                    ui.ResignDate = Convert.ToDateTime(resignDate);
                }
                catch
                {
                    Label_Message.Text = "离职日期格式不正确";
                    return false;
                }
                ui.IsResignation = 1;
            }            

			ModuleWorkFlow.BLL.User user = new ModuleWorkFlow.BLL.User();
            UserInfo oldui = user.findbykey(ui.Id);
			ui.DepartId = oldui.DepartId;
			ui.StartTime = oldui.StartTime;
			ui.ShiftId = oldui.ShiftId;
            ui.WorkHours = oldui.WorkHours;
			ui.BreakHours = oldui.BreakHours;
			bool ret = user.updateUserInfo(ui,oldui.WorkHours);
			if (ret == true)
			{
				Label_Message.Text = Lang.TXT_USERLIST_SaveSucc;
			}
			else
			{
				Label_Message.Text ="保存失败";
			}


			return ret;
		}

		private void Button_Edit_Click(object sender, System.EventArgs e)
		{
			if (checkUserInput() == true)
			{
				bool succ = save();
				if (succ == true)
				{
					string msg = Label_Message.Text;
					Response.Redirect("UserList.aspx?msg="+Server.UrlEncode(msg));
					//Response.Redirect("UserList.aspx?msg="+Server.UrlEncode(msg),true);
				}
			}
		}

        protected void lnkbutton_save_Click(object sender, EventArgs e)
        {
            Button_Edit_Click(sender, e);
        }
	}
}
