using System;
using System.Windows.Forms;

namespace copiaProgramas
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmInicio()); // Formulario de inicio anterior (desactivado para pruebas)
            //Application.Run(new UI.frmInicio()); // Formulario de inicio actualizado en el espacio de nombres UI
        }
    }
}
