using copiaProgramas.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace copiaProgramas.UI
{
    internal class frmInicio : Form
    {
        // Instancia para la carga de la configruacion y ficheros de copia con la nueva clase de gestion de configuracion
        private GestorConfiguracion gestorConfiguracion;

        public frmInicio()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // frmInicio
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "frmInicio";
            this.Load += new System.EventHandler(this.frmInicio_Load);
            this.ResumeLayout(false);

        }

        private void frmInicio_Load(object sender, EventArgs e)
        {
            // Instancia del gestor de configuracion
            gestorConfiguracion = GestorConfiguracion.Instancia;

            // Cargar rutas y servidores
            gestorConfiguracion.CargarConfiguracion("configuracion_defecto.json");

            // Cargar lista de ficheros
            gestorConfiguracion.CargarFicheros("ficheros.json");

            // Ejemplo de acceso a los datos
            foreach (var fichero in gestorConfiguracion.ListaFicheros)
            {
                var ruta = fichero.Ruta;
                var tipo = fichero.Tipo;
                var clase = fichero.Clase;
                var nombre = fichero.Nombre;
                var seleccionado = fichero.Seleccionado;
                var rutaOrigen = fichero.RutaOrigenCompleta;
                var rutaDestino = fichero.RutaDestino;
            }
        }
    }

}
