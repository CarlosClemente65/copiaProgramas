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
        public List<ProgramasCopiados> ProgramasCopiados { get; set; }
        public List<string> Errores { get; set; }

        public ResultadoCopia()
        {
            ProgramasCopiados = new List<ProgramasCopiados>();
            Errores = new List<string>();
        }
    }

    internal class ProgramasCopiados
    {
        public string Programa { get; set; }
        public string RutaDestino { get; set; }
        public string MensajeResultado { get; set; }
        public string MensajeError { get; set; }
    }
}
