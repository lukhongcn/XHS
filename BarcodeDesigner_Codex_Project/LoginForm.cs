using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ModuleWorkFlow.BLL;
using ModuleWorkFlow.Model;

namespace BarcodeDesigner
{
    public sealed class LoginForm : Form
    {
        private readonly TextBox userNameTextBox = new TextBox();
        private readonly TextBox passwordTextBox = new TextBox();
        private readonly ComboBox customerComboBox = new ComboBox();
        private readonly Label messageLabel = new Label();
        private readonly Button loginButton = new Button();
        private readonly List<BarcodeCustomerProfile> customerProfiles;
        private readonly LoginSettingsStore loginSettingsStore = new LoginSettingsStore();

        public string LoginUserName { get; private set; }
        public BarcodeCustomerProfile SelectedCustomer { get; private set; }

        public LoginForm()
        {
            Text = "BarcodeDesigner 登录";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(390, 260);

            string configFile = ConfigurationManager.AppSettings["BarcodeCustomerConfigFile"];
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile ?? string.Empty);
            try
            {
                customerProfiles = BarcodeCustomerProfileLoader.Load(configPath);
            }
            catch (Exception ex)
            {
                customerProfiles = new List<BarcodeCustomerProfile>();
                messageLabel.Text = ex.Message;
                loginButton.Enabled = false;
            }

            Label customerLabel = new Label { AutoSize = true, Text = "客　　户：", Location = new Point(42, 38) };
            customerComboBox.Location = new Point(125, 34);
            customerComboBox.Size = new Size(210, 23);
            customerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            customerComboBox.DisplayMember = "CustomerName";
            customerComboBox.DataSource = customerProfiles;

            Label userNameLabel = new Label { AutoSize = true, Text = "员工编号：", Location = new Point(42, 78) };
            userNameTextBox.Location = new Point(125, 74);
            userNameTextBox.Size = new Size(210, 23);

            Label passwordLabel = new Label { AutoSize = true, Text = "密　　码：", Location = new Point(42, 118) };
            passwordTextBox.Location = new Point(125, 114);
            passwordTextBox.Size = new Size(210, 23);
            passwordTextBox.PasswordChar = '*';

            loginButton.Text = "登录";
            loginButton.Location = new Point(125, 160);
            loginButton.Size = new Size(95, 30);
            loginButton.Click += LoginButton_Click;

            Button cancelButton = new Button { Text = "取消", Location = new Point(240, 160), Size = new Size(95, 30), DialogResult = DialogResult.Cancel };
            messageLabel.AutoSize = false;
            messageLabel.ForeColor = Color.Firebrick;
            messageLabel.Location = new Point(42, 205);
            messageLabel.Size = new Size(305, 45);

            AcceptButton = loginButton;
            CancelButton = cancelButton;
            Controls.AddRange(new Control[]
            {
                customerLabel, customerComboBox, userNameLabel, userNameTextBox, passwordLabel, passwordTextBox,
                loginButton, cancelButton, messageLabel
            });
            Shown += LoginForm_Shown;
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            LoginSettings settings = loginSettingsStore.Load();
            userNameTextBox.Text = settings.UserName ?? string.Empty;
            passwordTextBox.Text = loginSettingsStore.DecryptPassword(settings.EncryptedPassword);

            if (!string.IsNullOrWhiteSpace(settings.CustomerAbbr))
            {
                BarcodeCustomerProfile rememberedCustomer = customerProfiles.FirstOrDefault(
                    item => string.Equals(item.CustomerAbbr, settings.CustomerAbbr, StringComparison.OrdinalIgnoreCase));
                if (rememberedCustomer != null)
                {
                    customerComboBox.SelectedItem = rememberedCustomer;
                }
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string userName = (userNameTextBox.Text ?? string.Empty).Trim();
            string password = passwordTextBox.Text ?? string.Empty;
            BarcodeCustomerProfile selectedCustomer = customerComboBox.SelectedItem as BarcodeCustomerProfile;
            if (selectedCustomer == null)
            {
                messageLabel.Text = "请选择客户。";
                return;
            }

            string message;
            if (!TryLogin(userName, password, selectedCustomer.MenuId, out message))
            {
                messageLabel.Text = message;
                passwordTextBox.SelectAll();
                passwordTextBox.Focus();
                return;
            }

            LoginUserName = userName;
            SelectedCustomer = selectedCustomer;
            try
            {
                loginSettingsStore.Save(userName, password, selectedCustomer.CustomerAbbr);
            }
            catch
            {
                // 保存登录信息失败不影响本次登录。
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private static bool TryLogin(string userName, string password, string menuId, out string message)
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

            if (userInfo.IsAdmin == 1 || HasAddPermission(userName, userInfo, menuId))
            {
                return true;
            }

            message = "当前账号没有 BarcodeDesigner 的新增权限。";
            return false;
        }

        private static bool HasAddPermission(string userName, UserInfo userInfo, string menuId)
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
                .Any(item => string.Equals((item.MENUID ?? string.Empty).Trim(), (menuId ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                    && item.PADD > 0);
        }
    }
}
