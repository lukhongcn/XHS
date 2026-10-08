using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ModuleWorkFlow.BLL;
using ModuleWorkFlow.Model;

namespace BarcodeDesigner
{
    public sealed class LoginForm : Form
    {
        private const string MenuId = "B03";
        private readonly TextBox userNameTextBox = new TextBox();
        private readonly TextBox passwordTextBox = new TextBox();
        private readonly Label messageLabel = new Label();
        private readonly Button loginButton = new Button();

        public string LoginUserName { get; private set; }

        public LoginForm()
        {
            Text = "BarcodeDesigner 登录";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(390, 220);

            Label userNameLabel = new Label { AutoSize = true, Text = "员工编号：", Location = new Point(42, 38) };
            userNameTextBox.Location = new Point(125, 34);
            userNameTextBox.Size = new Size(210, 23);

            Label passwordLabel = new Label { AutoSize = true, Text = "密　　码：", Location = new Point(42, 78) };
            passwordTextBox.Location = new Point(125, 74);
            passwordTextBox.Size = new Size(210, 23);
            passwordTextBox.PasswordChar = '*';

            loginButton.Text = "登录";
            loginButton.Location = new Point(125, 120);
            loginButton.Size = new Size(95, 30);
            loginButton.Click += LoginButton_Click;

            Button cancelButton = new Button { Text = "取消", Location = new Point(240, 120), Size = new Size(95, 30), DialogResult = DialogResult.Cancel };
            messageLabel.AutoSize = false;
            messageLabel.ForeColor = Color.Firebrick;
            messageLabel.Location = new Point(42, 165);
            messageLabel.Size = new Size(305, 36);

            AcceptButton = loginButton;
            CancelButton = cancelButton;
            Controls.AddRange(new Control[]
            {
                userNameLabel, userNameTextBox, passwordLabel, passwordTextBox,
                loginButton, cancelButton, messageLabel
            });
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string userName = (userNameTextBox.Text ?? string.Empty).Trim();
            string password = passwordTextBox.Text ?? string.Empty;
            string message;
            if (!TryLogin(userName, password, out message))
            {
                messageLabel.Text = message;
                passwordTextBox.SelectAll();
                passwordTextBox.Focus();
                return;
            }

            LoginUserName = userName;
            DialogResult = DialogResult.OK;
            Close();
        }

        private static bool TryLogin(string userName, string password, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                message = "请输入员工编号和密码。";
                return false;
            }

            UserInfo userInfo = new User().getUserInfoByusername(userName);
            if (userInfo == null || !string.Equals(userInfo.Password ?? string.Empty, password, StringComparison.Ordinal))
            {
                message = "登录失败，员工编号或密码不正确。";
                return false;
            }

            if (userInfo.IsResignation == 1)
            {
                message = "该账号已离职，禁止登录。";
                return false;
            }

            if (userInfo.IsAdmin == 1 || HasQueryPermission(userName, userInfo))
            {
                return true;
            }

            message = "当前账号没有 BarcodeDesigner 的查询权限。";
            return false;
        }

        private static bool HasQueryPermission(string userName, UserInfo userInfo)
        {
            IList privileges = new Private().getPrivateByUserName(userName) as IList;
            if ((privileges == null || privileges.Count == 0) && userInfo != null)
            {
                int departmentId;
                if (int.TryParse(userInfo.DepartId, out departmentId))
                {
                    privileges = new PrivateDepartMent().getPrivateByDepartmentId(departmentId, userName) as IList;
                }
            }

            if (privileges == null)
            {
                return false;
            }

            return privileges.Cast<object>()
                .OfType<PrivateInfo>()
                .Any(item => string.Equals((item.MENUID ?? string.Empty).Trim(), MenuId, StringComparison.OrdinalIgnoreCase)
                    && item.PQUERY > 0);
        }
    }
}
