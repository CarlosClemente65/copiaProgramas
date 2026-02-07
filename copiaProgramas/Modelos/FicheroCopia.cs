using Newtonsoft.Json;
using enums = copiaProgramas.Comun.Enums;

namespace copiaProgramas.Modelos
{
    internal class FicheroCopia
    {
        public enums.ClaseFichero Clase { get; set; } // 1. PI, 2. NO PI, 3. Gestion, 4. Gasoleos
        public string Nombre { get; set; } // Nombre del programa para mostrar en el formulario
        public string Ruta { get; set; } // Ruta relativa dentro del servidor (ej: "ipcont08\pcont08z.tgz")
        public string Tipo { get; set; } // Contabilidad, Modelos, Documentales, Facturacion, Patrones, Laboral, Gasoleos

        [JsonIgnore]
        public bool Seleccionado { get; set; } //Indica si el usuario ha marcado este fichero para copiar

        [JsonIgnore]
        public string RutaOrigenCompleta { get; set; } // Ruta completa calculadada antes de iniciar la copia
        [JsonIgnore]
        public string RutaDestino { get; set; } // Ruta destino calculada antes de iniciar la copia
    }
}
