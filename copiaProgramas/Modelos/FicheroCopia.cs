using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace copiaProgramas.Modelos
{
    internal class FicheroCopia
    {
        public int Clase { get; set; } // 1. PI, 2. NO PI, 3. Gestion, 4. Gasoleos
        public string Nombre { get; set; } // Nombre del programa para mostrar en el formulario
        public string Ruta { get; set; } // Ruta relativa dentro del servidor (ej: "ipcont08\pcont08z.tgz")
        public string Tipo { get; set; } // Contabilidad, Modelos, Documentales, Facturacion, Patrones, Laboral, Gasoleos
        public bool Seleccionado { get; set; } //Indica si el usuario ha marcado este fichero para copiar
        public string RutaOrigenCompleta { get; set; } // Ruta completa calculadada antes de iniciar la copia
        public string RutaDestino { get; set; } // Ruta destino calculada antes de iniciar la copia
    }
}
