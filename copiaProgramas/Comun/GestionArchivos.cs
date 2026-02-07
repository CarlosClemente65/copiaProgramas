using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace copiaProgramas.Comun
{
    internal static class GestionArchivos
    {
        public static T LeerJson<T>(string ruta)
        {
            // Valida si el archi existe
            if (!File.Exists(ruta)) return default;

            // Lee el contenido del archivo y lo deserializa a un objeto del tipo T
            var json = File.ReadAllText(ruta);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void GrabarJson<T>(string ruta, T obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(ruta, json);
        }
    }
}
