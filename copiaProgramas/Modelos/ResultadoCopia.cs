using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace copiaProgramas.Modelos
{
    internal class ResultadoCopia
    {
        public string FechaCopia { get; set; }
        public string TiempoTotalCopia { get; set; }
        public List<ProgramasCopiados> ProgramasCopiados { get; set; } = new List<ProgramasCopiados>();
        public List<string> Errores { get; set; } = new List<string>();

        public ResultadoCopia()
        {
            ProgramasCopiados = new List<ProgramasCopiados>();
        }
    }

    internal class ProgramasCopiados
    {
        public string Programa { get; set; }
        public string RutaDestino { get; set; }
        public string MensajeResultado { get; set; }
    }
}
