using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using copiaProgramas.Modelos;
using Utiles = copiaProgramas.Comun.UtilidadesUI;


namespace copiaProgramas.Servicios
{
    /// <summary>
    /// Servicio para gestionar el registro de copias
    /// </summary>
    internal class ServicioLog
    {
        private readonly string _rutaLog = "RegistroCopias.json";

        public async Task GuardarLogCopiaAsync(ResultadoCopia resumen)
        {
            try
            {
                // Leer logs existentes
                List<ResultadoCopia> logs = new List<ResultadoCopia>();
                if (File.Exists(_rutaLog))
                {
                    string jsonExistente = File.ReadAllText(_rutaLog);
                    logs = JsonConvert.DeserializeObject<List<ResultadoCopia>>(jsonExistente) ?? new List<ResultadoCopia>();
                }

                // Agregar nuevo registro
                logs.Insert(0,resumen);

                // Guardar
                string jsonNuevo = JsonConvert.SerializeObject(logs, Formatting.Indented);
                File.WriteAllText(_rutaLog, jsonNuevo);
            }
            catch (Exception ex)
            {
                // Log de error - aquí podrías usar un sistema de logging más robusto
                System.Diagnostics.Debug.WriteLine($"Error al guardar log: {ex.Message}");
            }
        }
    }
}
