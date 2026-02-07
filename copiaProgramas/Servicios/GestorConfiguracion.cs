using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using copiaProgramas.Modelos;
using Newtonsoft.Json;

namespace copiaProgramas.Servicios
{
    internal class GestorConfiguracion
    {
        // Patron Singleton para acceso global a la configuración
        private static GestorConfiguracion _instancia;
        public static GestorConfiguracion Instancia
        {
            get
            {
                if(_instancia == null)
                {
                    _instancia = new GestorConfiguracion();
                }
                return _instancia;
            }
        }

        // Propiedad para almacenar rutas origen, destino y servidores
        public RutasCopia Configuracion { get; set; }
        public List<FicheroCopia> ListaFicheros { get; set; }

        // Método para cargar rutas y servidores
        public void CargarConfiguracion(string archivoConfiguracion)
        {
            // Leer archivo JSON de rutas y servidores
            var json = File.ReadAllText(archivoConfiguracion);
            Configuracion = JsonConvert.DeserializeObject<RutasCopia>(json);
        }

        // Método para cargar ficheros candidatos
        public void CargarFicheros(string archivoFicheros)
        {
            var json = File.ReadAllText(archivoFicheros);
            ListaFicheros = JsonConvert.DeserializeObject<List<FicheroCopia>>(json);
        }
    }
}
