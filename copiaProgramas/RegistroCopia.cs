using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace copiaProgramas
{
    public class RegistroCopia
    {
        public string FechaCopia { get; set; }
        public string TiempoTotalCopia { get; set; }
        public List<ProgramaCopiado> ProgramasCopiados { get; set; }

        public class ProgramaCopiado
        {
            public string Programa { get; set; }
            public string RutaDestino { get; set; }
        }

        public RegistroCopia()
        {
            ProgramasCopiados = new List<ProgramaCopiado>();
            FechaCopia = string.Empty;
            TiempoTotalCopia = string.Empty;
        }

        public static void GuardarRegistroCopia(RegistroCopia nuevoRegistro, string _rutaArchivo)
        {
            // Guardar la configuración en un archivo
            List<RegistroCopia> registros;
            if (File.Exists(_rutaArchivo))
            {
                // Si el archivo existe, se leen los registros existentes
                registros = LeerRegistroCopias(_rutaArchivo);
            }
            else
            {
                // Si el archivo no existe, se crea una nueva lista de registros
                registros = new List<RegistroCopia>();
            }

            //Agregar el nuevo registro a la lista
            registros.Add(nuevoRegistro);

            // Serializar la lista de registros a JSON
            string jsonSalida = JsonConvert.SerializeObject(registros, Formatting.Indented);
            File.WriteAllText(_rutaArchivo, jsonSalida);
        }

        public static List<RegistroCopia> LeerRegistroCopias(string _rutaArchivo)
        {
            // Leer las copias almacenadas en un archivo
            if(!File.Exists(_rutaArchivo))
            {
                // Si el archivo no existe, se crea uno nuevo con valores por defecto
                return new List<RegistroCopia>();
            }

            // Si el archivo existe, se lee su contenido, lo deserializa y se devuelve
            string json = File.ReadAllText(_rutaArchivo);
            return JsonConvert.DeserializeObject<List<RegistroCopia>>(json) ?? new List<RegistroCopia>();


        }
    }

    
}
