using copiaProgramas.Modelos;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using static copiaProgramas.Modelos.ProgresoEventArgs;
using enums = copiaProgramas.Comun.Enums;
using utiles = copiaProgramas.Comun.UtilidadesUI;

namespace copiaProgramas.Infraestructura
{
    /// <summary>
    /// Implementación de copia a filesystem local
    /// </summary>
    internal class CopiadorLocal : ICopiador
    {
        // Evento para notificar el progreso de la copia
        public event EventHandler<ProgresoEventArgs> ProgresoActualizado;

        public async Task<ProgramasCopiados> CopiarAsync(FicheroCopia fichero, DestinoCopia destino)
        {
            // Crea el objeto con los datos del programa copiado
            var resultadoCopia = new ProgramasCopiados
            {
                Programa = fichero.Nombre,
                RutaDestino = destino.RutaDestino
            };

            try
            {
                // Notificar inicio
                NotificarProgreso(fichero.Nombre, $"Iniciando copia de {fichero.Nombre}...", enums.EstadoCopia.Iniciando);

                // Validar que existe el origen
                if (!File.Exists(fichero.RutaOrigenCompleta))
                {
                    throw new FileNotFoundException($"No se encuentra el fichero origen: {fichero.RutaOrigenCompleta}");
                }

                // Realizar la copia
                NotificarProgreso(fichero.Nombre, $"Copiando {fichero.Nombre}...", enums.EstadoCopia.Copiando, 50);

                string rutaOrigen = fichero.RutaOrigenCompleta;
                string rutaDestino = Path.Combine(destino.RutaDestino, Path.GetFileName(fichero.Ruta));
                await Task.Run(() => File.Copy(rutaOrigen, rutaDestino, true));

                // Actualizar resultado
                resultadoCopia.MensajeResultado = "Fichero copiado correctamente";

                // Notificar finalización
                NotificarProgreso(fichero.Nombre, $"Programa {fichero.Nombre} copiado correctamente", enums.EstadoCopia.Finalizado, 100);
            }
            catch (Exception ex)
            {
                resultadoCopia.MensajeResultado = $"Error al copiar el fichero {fichero.Nombre}: {ex.Message}";

                // Notificar error
                NotificarProgreso(fichero.Nombre, $"Error al copiar {fichero.Nombre}: {ex.Message}", enums.EstadoCopia.Error);
            }

            return resultadoCopia;
        }

        private void NotificarProgreso(string nombreFichero, string mensaje, enums.EstadoCopia estado, int porcentaje = 0)
        {
            ProgresoActualizado?.Invoke(this, new ProgresoEventArgs(nombreFichero, mensaje, estado, porcentaje));
        }
    }
}
