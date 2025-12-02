using System;
using System.Windows.Forms;

namespace SistemaBanco
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Descomentar para probar la conexión antes del login
            // TestConexionSimple.Probar();
            // return;

            Application.Run(new FormLogin());
        }
    }
}
