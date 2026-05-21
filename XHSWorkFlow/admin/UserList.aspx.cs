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
	/// Summary description for UserList.
	/// </summary>
	public class UserList : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DataGrid MainDataGrid;
		protected System.Web.UI.WebControls.TextBox TextBox_key;
		protected System.Web.UI.WebControls.Button Button_Search;
		protected System.Web.UI.WebControls.Label Label_Message;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label lab_remerber;
        protected System.Web.UI.WebControls.DropDownList dpl_department;
        protected CheckBox chk_resignation;
		private string menuid = "A21";
        protected string menuname = "";
		private void Page_Load(object sender, System.EventArgs e)
		{
         
            if (ModuleWorkFlow.BLL.Private.checkPrivate(this,menuid,"PQUERY"))
            {
                menuname = new PartTmenu().findbykey(menuid).Menuname;
				if (!this.IsPostBack)
				{
                    BindDepartment();
					bindData();
				}
				Label_Message.Text="";
				string msg = Request.Params["msg"];
				if (msg != null && msg.Length > 0) Label_Message.Text = msg;
			}
		}

        private void BindDepartment()
        {
            ModuleWorkFlow.BLL.DepartMent depart = new DepartMent();
            dpl_department.DataSource = depart.GetAllDepartment();
            dpl_department.DataTextField = "DepartmentName";
            dpl_department.DataValueField = "DepartmentId";
            dpl_department.DataBind();
            dpl_department.Items.Insert(0,"");
        }

		private void bindData()
		{
            Label_Message.Text = "";
			User user = new User();
			string content = lab_remerber.Text.Trim();
			if (!content.Equals(""))
			{
				MainDataGrid.DataSource=user.getListUserInfo(content,chk_resignation.Checked);
			}
			else
			{
                if (dpl_department.SelectedValue.Trim().Equals(""))
                {
                    MainDataGrid.DataSource = user.getListUserInfo(chk_resignation.Checked);
                }
                else
                {
                    MainDataGrid.DataSource = user.getUserInfoByDepartment(Convert.ToInt32(dpl_department.SelectedValue.Trim()),chk_resignation.Checked);
                }
			}
			MainDataGrid.DataKeyField="username";
			MainDataGrid.DataBind();
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
			this.Button_Search.Click += new System.EventHandler(this.Button_Search_Click);
			this.MainDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.MainDataGrid_PageIndexChanged);
			this.MainDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.MainDataGrid_DeleteCommand);
			this.Load += new System.EventHandler(this.Page_Load);
            this.dpl_department.SelectedIndexChanged += new System.EventHandler(this.dpl_departmentSelectedIndexChanged);
		}
		#endregion

		private void MainDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			MainDataGrid.CurrentPageIndex=e.NewPageIndex;
//			if (!TextBox_key.Text.Equals(""))
//			{
//				TextBox_key.Text=lab_remerber.Text.Trim();
//			}
			bindData();
		}

		private void Button_Search_Click(object sender, System.EventArgs e)
		{
			lab_remerber.Text=TextBox_key.Text.Trim();
			MainDataGrid.CurrentPageIndex=0;
				bindData();
//			}
//			else
//			{
//				Label_Message.Text = lang.TXT_USERLIST_InputSearch;
//			}
		}

        private void dpl_departmentSelectedIndexChanged(object sender, System.EventArgs e)
        {
            lab_remerber.Text = "";
            MainDataGrid.CurrentPageIndex = 0;
            bindData();
        }

		

		private void MainDataGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (ModuleWorkFlow.BLL.Private.checkPrivateByUrl(this,Request.RawUrl,"PDEL"))
			{
				User blluser=new User();
				Model.UserInfo userInfo=blluser.getUserInfo(MainDataGrid.DataKeys[e.Item.ItemIndex].ToString());
				BLL.ProjectBudget projectBudget=new ModuleWorkFlow.BLL.ProjectBudget();
				Model.ProjectBudgetInfo projectBudgetInfochecker=projectBudget.GetProjectBudegetByChecker(userInfo.UserName);
				Model.ProjectBudgetInfo projectBudgetInfoapprover=projectBudget.GetProjectBudegetByApprover(userInfo.UserName);
				if(projectBudgetInfochecker==null)
				{
					if(projectBudgetInfoapprover==null)
					{
						string username = MainDataGrid.DataKeys[e.Item.ItemIndex].ToString();
						ModuleWorkFlow.BLL.User user = new ModuleWorkFlow.BLL.User();
						if (!user.ExistRelationData(username))
						{
							if (user.deleteUserInfo(username))
							{					
								Label_Message.Text=Lang.TXT_DELETESUCCESS;
							}
							else
							{
								Label_Message.Text=Lang.TXT_OPERATIONERROR;
							}
						}
						else
						{
							Label_Message.Text = Lang.TXT_DELETEUSERBYORDER;
						}
						MainDataGrid.EditItemIndex=-1;
						bindData();
					}
					else
					{
						Label_Message.Text=Lang.DELETEUSERS;
					}
				}
				else
				{
					Label_Message.Text=Lang.DELETEUSERS;
				}
			}
		}

        private string getKey()
        {
            string key = null;
            foreach (DataGridItem item in MainDataGrid.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {

                    CheckBox cb = (CheckBox)item.FindControl("chk_datagrid");
                    if (cb.Checked)
                    {
                        key = MainDataGrid.DataKeys[item.ItemIndex].ToString();

                        break;
                    }
                }
            }

            return key;
        }

        protected void lnkbutton_edit_Click(object sender, EventArgs e)
        {
            string key = getKey();
            if (key == null)
                Label_Message.Text ="请选择一个用户";
            else
                Response.Redirect("UserEdit.aspx?menuid=" + menuid +"&username=" + key);
        }

        protected void lnkbutton_search_Click(object sender, EventArgs e)
        {
            Button_Search_Click(sender, e);
        }
	}
}
