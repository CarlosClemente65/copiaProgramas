using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace copiaProgramas.Modelos
{
    internal class Ruta
    {
        public string Nombre { get; set; }
        public string RutaBase { get; set; } // Ruta base para el origen
        public bool EsLocal { get; set; } // Indica si es un destino local o remoto
    }
    internal class RutasCopia
    {
        public List<Ruta> Origenes { get; set; }
        public List<Ruta> Destinos { get; set; }
        public List<ServidorCopia> ListaServidores { get; set; }
        public ServidorCopia ServidorSeleccionado { get; set; }
    }
}
