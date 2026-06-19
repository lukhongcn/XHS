using System;
using System.Windows.Forms;

namespace CheryCheckSystem.WinForms
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (LoginForm loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() != DialogResult.OK || loginForm.LoginContext == null)
                {
                    return;
                }

                Application.Run(new ShippingGoodsWorkbenchForm(loginForm.LoginContext));
            }
        }
    }
}
