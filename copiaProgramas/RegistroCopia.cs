using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace copiaProgramas
{
    public class RegistroCopia
    {
        public string FechaCopia { get; set; }
        public string TiempoTotalCopia { get; set; }
        public List<ProgramaCopiado> ProgramasCopiados { get; set; }

        public static List<(DateTime fecha, RegistroCopia copia)> ListadoCopias { get; set; } = new List<(DateTime, RegistroCopia)>(); // Lista para almacenar las copias leídas y procesadas

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
            if(File.Exists(_rutaArchivo))
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

        public static List<(DateTime Fecha, RegistroCopia Copia)> ProcesarCopiasLeidas(string rutaArchivo)
        {
            var copiasLeidas = LeerRegistroCopias(rutaArchivo);
            //var listaProcesada = new List<(DateTime, RegistroCopia)>();

            foreach(var copia in copiasLeidas)
            {
                // Intentamos convertir la fecha string a DateTime
                if(DateTime.TryParseExact(copia.FechaCopia, "'Dia:' dd.MM.yyyy '- Hora:' HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
                {
                    ListadoCopias.Add((fecha, copia));
                }
                else
                {
                    // Si la conversión falla, puedes registrar un error o simplemente ignorar esa entrada
                    Console.WriteLine($"No se pudo interpretar la fecha: {copia.FechaCopia}");
                }
            }

            // Ordenamos de más reciente a más antigua
            return ListadoCopias
                   .OrderByDescending(x => x.Item1)
                   .ToList();
        }

    }


}
