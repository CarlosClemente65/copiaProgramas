using copiaProgramas.Modelos;
using copiaProgramas.Servicios;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using enums = copiaProgramas.Comun.Enums;

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
            gestorConfiguracion.Inicializar();

            LanzaCopiaEjemplo();

        }

        private void LanzaCopiaEjemplo()
        {
            var servidorSeleccionado = gestorConfiguracion.Configuracion.ListaServidores.FirstOrDefault(s => s.Nombre == "geco72");
            
            // Construir un par de ficheros para copiar ---
            List<FicheroCopia> ficherosPrueba = new List<FicheroCopia>
            {
                new FicheroCopia
                {
                    Clase = enums.ClaseFichero.PI,
                    Nombre = "ipcont08",
                    Ruta = "ipcont08\\pcont08z.tgz",
                    Tipo = "Contabilidad",
                    Seleccionado = true
                },
                new FicheroCopia
                {
                    Clase = enums.ClaseFichero.PI,
                    Nombre = "ipbasica",
                    Ruta = "ipbasica\\pbasicaz.tgz",
                    Tipo = "Patrones",
                    Seleccionado = true
                }
            };

            // Simulamos la seleccion del destino a 'destinoPi'
            gestorConfiguracion.Configuracion.DestinoSeleccionado = gestorConfiguracion.Configuracion.Destinos.FirstOrDefault(d => d.Nombre == "destinoPi");

            // Se assignan las rutas de destino a cada fichero combinando el destino base con el nombre del fichero
            gestorConfiguracion.AsignarRutas(ficherosPrueba);

            // --- Mostrar rutas en depuración ---
            foreach (var f in ficherosPrueba)
            {
                Debug.WriteLine($"Fichero: {f.Nombre}");
                Debug.WriteLine($"Ruta completa origen: {f.RutaOrigenCompleta}");
                Debug.WriteLine($"Destino final: {f.RutaDestino}");
            }
        }
    }

}
