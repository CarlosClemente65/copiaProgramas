using copiaProgramas.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace copiaProgramas.Infraestructura
{
    internal interface ICopiador
    {
        event EventHandler<ProgresoEventArgs> ProgresoActualizado;

        Task<ProgramasCopiados> CopiarAsync(FicheroCopia fichero, DestinoCopia destino);
    }
}
