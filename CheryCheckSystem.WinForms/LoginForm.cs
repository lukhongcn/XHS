using System;
using System.Drawing;
using System.Windows.Forms;

namespace CheryCheckSystem.WinForms
{
    public class LoginForm : Form
    {
        private readonly WorkflowLoginService _loginService = new WorkflowLoginService();
        private Label lblUserName;
        private Label lblPassword;
        private TextBox txtUserName;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnCancel;
        private Label lblMessage;

        public WorkflowLoginContext LoginContext { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            lblUserName = new Label();
            lblPassword = new Label();
            txtUserName = new TextBox();
            txtPassword = new TextBox();
            btnLogin = new Button();
            btnCancel = new Button();
            lblMessage = new Label();

            SuspendLayout();

            Text = "系统登录";
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(420, 235);

            lblUserName.AutoSize = true;
            lblUserName.Location = new Point(46, 42);
            lblUserName.Text = "员工编号：";

            txtUserName.Location = new Point(126, 38);
            txtUserName.Size = new Size(220, 23);
            txtUserName.KeyDown += InputControl_KeyDown;

            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(60, 87);
            lblPassword.Text = "密  码：";

            txtPassword.Location = new Point(126, 83);
            txtPassword.Size = new Size(220, 23);
            txtPassword.PasswordChar = '*';
            txtPassword.KeyDown += InputControl_KeyDown;

            btnLogin.Location = new Point(126, 135);
            btnLogin.Size = new Size(95, 32);
            btnLogin.Text = "登录";
            btnLogin.Click += BtnLogin_Click;

            btnCancel.Location = new Point(251, 135);
            btnCancel.Size = new Size(95, 32);
            btnCancel.Text = "取消";
            btnCancel.Click += BtnCancel_Click;

            lblMessage.AutoSize = false;
            lblMessage.Location = new Point(49, 184);
            lblMessage.Size = new Size(320, 32);
            lblMessage.ForeColor = Color.Firebrick;

            AcceptButton = btnLogin;
            CancelButton = btnCancel;

            Controls.Add(lblUserName);
            Controls.Add(txtUserName);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            Controls.Add(btnCancel);
            Controls.Add(lblMessage);

            ResumeLayout(false);
            PerformLayout();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            WorkflowLoginContext loginContext;
            string message;
            if (!_loginService.TryLogin(txtUserName.Text, txtPassword.Text, out loginContext, out message))
            {
                lblMessage.Text = message;
                txtPassword.SelectAll();
                txtPassword.Focus();
                return;
            }

            LoginContext = loginContext;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void InputControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnLogin_Click(sender, EventArgs.Empty);
            }
        }
    }
}
