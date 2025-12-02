using System;
using System.Windows.Forms;

namespace SistemaBanco
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Descomentar la siguiente línea para probar la conexión
            //TestConexion.ProbarConexion();
            //return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormLogin());
        }
    }
}
