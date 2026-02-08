using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using enums = copiaProgramas.Comun.Enums;

namespace copiaProgramas.Modelos
{
    internal class Ruta
    {
        public string Nombre { get; set; }
        public string RutaBase { get; set; } // Ruta base para el origen
        public bool EsRemoto { get; set; } // Indica si es un destino remoto
        public enums.ClaseFichero Clase { get; set; } // Clase del fichero que se corresponde con esta ruta (1. PI, 2. NO PI, 3. Gestion, 4. Gasoleos)
    }
    internal class RutasCopia
    {
        public List<Ruta> Origenes { get; set; }
        public List<Ruta> Destinos { get; set; }

        [JsonIgnore]
        public Ruta DestinoSeleccionado { get; set; }
        
        
        // Lista de servidores
        public List<ServidorCopia> ListaServidores { get; set; }

        // Propiedad para almacenar el servidor seleccionado
        [JsonIgnore]
        public ServidorCopia ServidorSeleccionado { get; set; }

    }
}
