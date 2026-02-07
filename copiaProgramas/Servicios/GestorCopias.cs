using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using copiaProgramas.Modelos;

namespace copiaProgramas.Servicios
{
    internal class GestorCopias
    {
        // Eventos que el formulario podrá suscribirse
        public event EventHandler<string> Mensaje;            // Para mensajes info / error
        public event EventHandler<int> Progreso;             // Para porcentaje de copia
        public event EventHandler<ResultadoCopia> CopiasFinalizadas; // Para resultados finales

        // Método principal que recibe los ficheros y el destino
        public async Task EjecutarCopiasAsync(List<FicheroCopia> ficheros, DestinoCopia destino)
        {
            if(ficheros == null || ficheros.Count == 0)
            {
                Mensaje?.Invoke(this, "No hay ficheros seleccionados para copiar.");
                return;
            }

            int totalFicheros = ficheros.Count(f => f.Seleccionado);
            if(totalFicheros == 0)
            {
                Mensaje?.Invoke(this, "No hay ficheros marcados para copiar.");
                return;
            }

            int ficherosCopiados = 0;
            var log = new StringBuilder();

            foreach(var fichero in ficheros)
            {
                if(!fichero.Seleccionado) continue;

                // Aviso de inicio
                Mensaje?.Invoke(this, $"Iniciando copia de {fichero.Nombre}...");

                try
                {
                    if(destino.EsLocal)
                    {
                        await CopiarFicheroLocal(fichero, destino);
                    }
                    else
                    {
                        await CopiarFicheroRemoto(fichero, destino);
                    }

                    // Aviso de finalización
                    Mensaje?.Invoke(this, $"Fichero {fichero.Nombre} copiado correctamente.");
                    log.AppendLine($"{DateTime.Now}: {fichero.Nombre} copiado correctamente.");

                    ficherosCopiados++;

                    // Actualización de progreso
                    int porcentajeTotal = (int)((ficherosCopiados / (double)totalFicheros) * 100);
                    Progreso?.Invoke(this, porcentajeTotal);

                }
                catch(Exception ex)
                {
                    string error = $"Error al copiar {fichero.Nombre}: {ex.Message}";
                    Mensaje?.Invoke(this, error);
                    log.AppendLine($"{DateTime.Now}: {error}");
                }
            }

            // Evento de finalización con resumen
            CopiasFinalizadas?.Invoke(this, new ResultadoCopia
            {
                Total = totalFicheros,
                Copiados = ficherosCopiados,
                Log = log.ToString()
            });
        }

        // Copia local
        private async Task CopiarFicheroLocal(FicheroCopia fichero, DestinoCopia destino)
        {
            string origen = fichero.RutaOrigenCompleta;
            string destinoFinal = Path.Combine(destino.RutaDestino, Path.GetFileName(fichero.Ruta));

            await Task.Run(() => File.Copy(origen, destinoFinal, true));
        }

        // Copia remota: vacío por ahora, se implementará con WinSCP/SFTP
        private async Task CopiarFicheroRemoto(FicheroCopia fichero, DestinoCopia destino)
        {
            // TODO: implementar copia remota
            await Task.CompletedTask;
        }
    }
}
