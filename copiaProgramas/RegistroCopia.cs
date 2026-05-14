using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

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
            if(ListadoCopias.Count == 0)
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
            }

            // Ordenamos de más reciente a más antigua
            return ListadoCopias;

        }

        public static List<(DateTime Fecha, RegistroCopia Copia)> OrdenarCopiasLeidas(List<(DateTime Fecha, RegistroCopia Copia)> listado)
        {
            // Ordenar la lista de copias leídas por fecha
            return listado.OrderByDescending(x => x.Item1).ToList();
        }

        public static void BorrarCopiasAntiguas(int diasCopias, string passwordBorrado, string pathRegistroCopias)
        {
            // Primero, aseguramos que la lista de copias leídas esté actualizada
            ProcesarCopiasLeidas(pathRegistroCopias);

            // Se calcula la fecha límite para eliminar las copias antiguas
            DateTime fechaInicio = DateTime.MinValue;
            DateTime fechaFin = DateTime.Now.AddDays(-diasCopias);

            // Revisar la lista de copias para ver si hay alguna que caiga dentro del rango de fechas a eliminar
            // Contamos cuántos registros caen en el rango
            int registrosSeleccionados = RegistroCopia.ListadoCopias
                .Count(c => c.fecha.Date >= fechaInicio && c.fecha.Date <= fechaFin);

            // Se llama al método de eliminación pasando el rango de fechas y la contraseña
            if(registrosSeleccionados > 0)
            {
                EliminarCopias(fechaInicio, fechaFin, passwordBorrado, pathRegistroCopias, mostrarAviso: false);
            }
        }

        public static void EliminarCopias(DateTime fechaInicio, DateTime fechaFin, string passwordBorrado, string pathRegistroCopias, bool mostrarAviso = true)
        {
            // Contamos cuántos registros caen en el rango
            int registrosSeleccionados = RegistroCopia.ListadoCopias
                .Count(c => c.fecha.Date >= fechaInicio && c.fecha.Date <= fechaFin);
 
            if(mostrarAviso)
            {
                // Confirmación previa al borrado
                string mensaje = registrosSeleccionados > 0
                    ? $"¿Deseas eliminar {registrosSeleccionados} registro(s) entre los dias{fechaInicio:dd.MM.yyyy} y {fechaFin:dd.MM.yyyy}?"
                    : "¿Deseas eliminar TODOS los registros?\nEsta acción requiere contraseña.";
                
                if(MessageBox.Show(mensaje, "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }
            }

            if(registrosSeleccionados > 0)
            {
                // Eliminar solo los de los días seleccionados
                RegistroCopia.ListadoCopias.RemoveAll(r=> r.fecha.Date >= fechaInicio && r.fecha.Date <= fechaFin);
            }
            else
            {
                // Solicitar contraseña antes de eliminar todo
                using(var formPwd = new frmPassword())
                {
                    if(formPwd.ShowDialog() != DialogResult.OK || formPwd.Contraseña != passwordBorrado)
                    {
                        MessageBox.Show("Contraseña incorrecta. Operación cancelada.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                RegistroCopia.ListadoCopias.Clear();
            }

            //Guardar cambios y actualizar la lista
            var registros = RegistroCopia.ListadoCopias.Select(t => t.copia).ToList();
            string jsonSalida = JsonConvert.SerializeObject(registros, Formatting.Indented);
            File.WriteAllText(pathRegistroCopias, jsonSalida);

            if(mostrarAviso)
            {
                MessageBox.Show("Registros eliminados correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
