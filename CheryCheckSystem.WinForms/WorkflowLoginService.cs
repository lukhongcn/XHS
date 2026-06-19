using System;
using System.Collections;
using System.Configuration;
using System.Linq;
using ModuleWorkFlow.BLL;
using ModuleWorkFlow.Model;

namespace CheryCheckSystem.WinForms
{
    public class WorkflowLoginService
    {
        public bool TryLogin(string userName, string password, out WorkflowLoginContext loginContext, out string message)
        {
            loginContext = null;
            message = string.Empty;

            string normalizedUserName = (userName ?? string.Empty).Trim();
            string normalizedPassword = (password ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizedUserName) || string.IsNullOrWhiteSpace(normalizedPassword))
            {
                message = "请输入员工编号和密码。";
                return false;
            }

            UserInfo userInfo = new User().getUserInfoByusername(normalizedUserName);
            if (userInfo == null || !string.Equals(userInfo.Password ?? string.Empty, normalizedPassword, StringComparison.Ordinal))
            {
                message = "登录失败，员工编号或密码不正确。";
                return false;
            }

            if (userInfo.IsResignation == 1)
            {
                message = "该账号已离职，禁止登录。";
                return false;
            }

            ArrayList privileges = new Private().getPrivateByUserName(normalizedUserName) as ArrayList;
            if (privileges == null || privileges.Count == 0)
            {
                int departmentId;
                if (!int.TryParse(userInfo.DepartId, out departmentId))
                {
                    message = "当前账号未配置有效部门，无法校验权限。";
                    return false;
                }

                privileges = new PrivateDepartMent().getPrivateByDepartmentId(departmentId, normalizedUserName);
            }

            if (!HasAccess(userInfo, privileges))
            {
                message = "当前账号没有进入系统的权限。";
                return false;
            }

            loginContext = new WorkflowLoginContext
            {
                UserName = userInfo.UserName,
                DisplayName = string.IsNullOrWhiteSpace(userInfo.Name) ? userInfo.UserName : string.Format("{0}({1})", userInfo.UserName, userInfo.Name),
                IsAdmin = userInfo.IsAdmin == 1,
                DepartmentId = userInfo.DepartId
            };
            return true;
        }

        private static bool HasAccess(UserInfo userInfo, IList privileges)
        {
            if (userInfo != null && userInfo.IsAdmin == 1)
            {
                return true;
            }

            if (privileges == null || privileges.Count == 0)
            {
                return false;
            }

            string requiredMenuId = (ConfigurationManager.AppSettings["Workflow.LoginRequiredMenuId"] ?? string.Empty).Trim();
            string requiredPrivilege = (ConfigurationManager.AppSettings["Workflow.LoginRequiredPrivilege"] ?? "PQUERY").Trim().ToUpperInvariant();
            PrivateInfo[] privilegeItems = privileges.Cast<object>().OfType<PrivateInfo>().ToArray();
            if (privilegeItems.Length == 0)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(requiredMenuId))
            {
                return privilegeItems.Any(item => HasPrivilege(item, requiredPrivilege));
            }

            return privilegeItems.Any(item =>
                string.Equals((item.MENUID ?? string.Empty).Trim(), requiredMenuId, StringComparison.OrdinalIgnoreCase) &&
                HasPrivilege(item, requiredPrivilege));
        }

        private static bool HasPrivilege(PrivateInfo privateInfo, string privilegeName)
        {
            if (privateInfo == null)
            {
                return false;
            }

            switch (privilegeName)
            {
                case "PADD":
                    return privateInfo.PADD > 0;
                case "PEDIT":
                    return privateInfo.PEDIT > 0;
                case "PDEL":
                    return privateInfo.PDEL > 0;
                case "PQUERY":
                default:
                    return privateInfo.PQUERY > 0;
            }
        }
    }
}
