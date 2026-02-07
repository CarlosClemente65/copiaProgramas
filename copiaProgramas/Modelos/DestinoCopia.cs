using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace copiaProgramas.Modelos
{
    internal class DestinoCopia
    {
        public string RutaDestino { get; set; }
        public bool EsLocal { get; set; }
        public ServidorCopia Servidor { get; set; }
    }

}
