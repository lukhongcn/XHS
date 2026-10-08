using System;
using System.Windows.Forms;

namespace BarcodeDesigner
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
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                Application.Run(new MainForm(loginForm.LoginUserName));
            }
        }
    }
}
