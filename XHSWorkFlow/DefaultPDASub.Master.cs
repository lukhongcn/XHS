using System;
using System.Web.UI;

using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using ModuleWorkFlow.BLL;
using Utility;

namespace ModuleWorkFlow
{
    public partial class DefaultPDASub : MasterPage
    {
        // PDA 标签绑定使用独立菜单项的 PADD（新增/操作）权限。
        private const string PdaMenuId = "B13";
        private const string PdaAccessContextKey = "PdaAccessAllowed";
        protected string PageTitle = "标签绑定 PDA";

        protected override void OnInit(EventArgs e)
        {
            ApplyPdaAccessState();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ApplyPdaAccessState();
        }

        private void ApplyPdaAccessState()
        {
            if (!IsPdaLoggedIn())
            {
                Context.Items[PdaAccessContextKey] = false;
                pnlPdaLogin.Visible = true;
                if (!IsPostBack)
                {
                    labPdaLoginMessage.Text = "请先登录后使用 PDA 页面。";
                    txtPdaUserName.Focus();
                }
            }
            else if (!HasPdaPermission())
            {
                Context.Items[PdaAccessContextKey] = false;
                pnlPdaLogin.Visible = true;
                labPdaLoginMessage.Text = "当前账号没有 PDA 标签绑定权限（PADD），请联系管理员。";
                txtPdaUserName.Visible = false;
                txtPdaPassword.Visible = false;
                btnPdaLogin.Visible = false;
            }
            else
            {
                Context.Items[PdaAccessContextKey] = true;
                pnlPdaLogin.Visible = false;
            }
        }

        public static bool IsPdaAccessAllowed(HttpContext context)
        {
            return context != null && context.Items[PdaAccessContextKey] is bool
                && (bool)context.Items[PdaAccessContextKey];
        }

        private bool HasPdaPermission()
        {
            if (Session == null) return false;
            if (Session["isadmin"] is bool && (bool)Session["isadmin"]) return true;

            // 这里按当前登录会话中的具体菜单权限检查，避免 checkPrivate 的历史兼容逻辑
            // 将“拥有其它菜单权限”误判为拥有 PDA 权限。
            IEnumerable privileges = Session["iprivate"] as IEnumerable;
            if (privileges == null) return false;
            foreach (object privilege in privileges)
            {
                if (privilege == null) continue;
                Type type = privilege.GetType();
                object menuValue = type.GetProperty("MENUID") == null
                    ? null : type.GetProperty("MENUID").GetValue(privilege, null);
                object paddValue = type.GetProperty("PADD") == null
                    ? null : type.GetProperty("PADD").GetValue(privilege, null);
                int padd;
                if (string.Equals(Convert.ToString(menuValue).Trim(), PdaMenuId, StringComparison.OrdinalIgnoreCase)
                    && int.TryParse(Convert.ToString(paddValue), out padd) && padd > 0)
                {
                    return true;
                }
            }
            return false;
        }

        protected void btnPdaLogin_Click(object sender, EventArgs e)
        {
            if (!CheckLogin(txtPdaUserName.Text, txtPdaPassword.Text))
            {
                pnlPdaLogin.Visible = true;
                labPdaLoginMessage.Text = "员工编号或密码错误。";
                txtPdaPassword.Text = string.Empty;
                return;
            }

            Response.Redirect(Request.RawUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private static bool IsPdaLoggedIn()
        {
            HttpContext context = HttpContext.Current;
            return context != null && context.Session != null
                && context.Session["islogin"] is bool
                && (bool)context.Session["islogin"]
                && !string.IsNullOrWhiteSpace(Convert.ToString(context.Session["userid"]));
        }

        private bool CheckLogin(string username, string password)
        {
            const string query = "select * from tb_user where username=@username and password=@password and isResignation=0";
            SqlParameter[] parameters =
            {
                new SqlParameter("@username", SqlDbType.NVarChar, 50) { Value = (object)(username ?? string.Empty).Trim() },
                new SqlParameter("@password", SqlDbType.NVarChar, 50) { Value = password ?? string.Empty }
            };
            DataRow row = Data.getDataRow(query, parameters);
            if (row == null) return false;

            ModuleWorkFlow.BLL.Private privateBiz = new ModuleWorkFlow.BLL.Private();
            IList privateList = privateBiz.getPrivateByUserName(row["username"].ToString());
            if (privateList.Count == 0)
            {
                privateList = new PrivateDepartMent().getPrivateByDepartmentId(
                    Convert.ToInt32(row["DepartmentId"]), row["username"].ToString());
            }

            Session["islogin"] = true;
            Session["userid"] = row["username"];
            Session["departmentid"] = row["DepartmentId"];
            Session["isadmin"] = Convert.ToInt32(row["isadmin"]) == 1;
            Session["iprivate"] = privateList;
            Session["custome"] = "FZXHS";
            return true;
        }
    }
}
