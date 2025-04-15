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
        public string TiempoTotalCopia { get; set; }

        public RegistroCopia()
        {
            ProgramasCopiados = new List<ProgramaCopiado>();
            FechaCopia = DateTime.MinValue;
            TiempoTotalCopia = string.Empty;
        }

        public void CrearRegistroCopia(DateTime _fechaCopia, List<ProgramaCopiado> _programasCopiados, string _tiempoTotal)
        {
            FechaCopia = _fechaCopia;
            ProgramasCopiados = _programasCopiados;
            TiempoTotalCopia = _tiempoTotal;
        }
    }

    public class ProgramaCopiado
    {
        public string Programa { get; set; }
        public string RutaDestino { get; set; }
    }
}
