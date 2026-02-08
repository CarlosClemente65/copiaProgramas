using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace copiaProgramas.Comun
{
    internal class Enums
    {
        public enum ClaseFichero
        {
            PI = 1,
            NoPI = 2,
            Gestion = 3,
            Gasoleos = 4
        }

        internal enum EstadoCopia
        {
            Iniciando,
            Copiando,
            Finalizado,
            Error
        }
    }
}

