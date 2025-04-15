using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace copiaProgramas
{
    public class RegistroCopia
    {
        public DateTime FechaCopia { get; set; }
        public List<ProgramaCopiado> ProgramasCopiados { get; set; }
        public string TiempoTotal { get; set; }

        public RegistroCopia()
        {
            ProgramasCopiados = new List<ProgramaCopiado>();
            FechaCopia = DateTime.Now;
            TiempoTotal = string.Empty;
        }
    }

    public class ProgramaCopiado
    {
        public string Nombre { get; set; }
        public string RutaDestino { get; set; }
    }
}
