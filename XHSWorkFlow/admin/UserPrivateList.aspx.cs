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

using ModuleWorkFlow.business;

namespace ModuleWorkFlow.admin
{
	/// <summary>
	/// Summary description for UserPrivateList.
	/// </summary>
	public class UserPrivateList : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DataGrid MainDataGrid;
		protected System.Web.UI.HtmlControls.HtmlTable Table1;
		protected System.Web.UI.WebControls.TextBox txt_namecard_like;
		protected System.Web.UI.WebControls.Button btn_search;
		protected System.Web.UI.WebControls.Label Label_Message;

		private string menuid = "A62";
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (ModuleWorkFlow.BLL.Private.checkPrivate(this,menuid,"PQUERY"))
			{
				if (!this.IsPostBack)
				{
					bindData();
				}
				Label_Message.Text="";
				string msg = Request.Params["msg"];
				if (msg != null && msg.Length > 0) Label_Message.Text = msg;
			}
		}

		private void bindData()
		{			
			MainDataGrid.DataSource=Employee.getEmplView(txt_namecard_like.Text);
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
			this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
			this.MainDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.MainDataGrid_PageIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void MainDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			MainDataGrid.CurrentPageIndex=e.NewPageIndex;
			bindData();
		}

		private void btn_search_Click(object sender, System.EventArgs e)
		{
			bindData();
		}
	}
}