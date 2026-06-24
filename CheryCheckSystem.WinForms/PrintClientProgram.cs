using System;
using System.Windows.Forms;

namespace CheryCheckSystem.PrintClient
{
    internal static class PrintClientProgram
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PrintClientForm());
        }
    }
}
