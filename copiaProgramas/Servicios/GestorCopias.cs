using copiaProgramas.Infraestructura;
using copiaProgramas.Modelos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utiles = copiaProgramas.Comun.UtilidadesUI;

namespace copiaProgramas.Servicios
{
    internal class GestorCopias
    {
        private readonly ServicioLog _servicioLog;
        private readonly ServicioValidacion _servicioValidacion; 

        // Eventos para comunicar progreso a la UI
        public event EventHandler<ProgresoEventArgs> ProgresoActualizado;
        public event EventHandler<ResumenCopiaEventArgs> CopiaFinalizada;

        public GestorCopias()
        {
            _servicioLog = new ServicioLog();
            _servicioValidacion = new ServicioValidacion();
        }

        /// <summary>
        /// Inicia el proceso de copia de múltiples ficheros
        /// </summary>
        public async Task<ResultadoCopia> IniciarCopiaAsync(List<FicheroCopia> ficheros, DestinoCopia destino)
        {
            var resultado = new ResultadoCopia
            {
                FechaCopia = Utiles.FormatearFechaCopia(DateTime.Now)
            };

            var stopwatchTotal = Stopwatch.StartNew();

            try
            {
                // Validar datos de entrada
                var erroresValidacion = _servicioValidacion.ValidarCopia(ficheros, destino);
                if (erroresValidacion.Any())
                {
                    resultado.Errores.AddRange(erroresValidacion);
                    resultado.TiempoTotalCopia = Utiles.FormatearTiempo(stopwatchTotal.Elapsed);
                    return resultado;
                }

                // Seleccionar el copiador apropiado
                ICopiador GestorCopia = destino.EsRemoto
                    ? (ICopiador)new CopiadorRemoto()
                    : new CopiadorLocal();

                // Suscribirse a los eventos del copiador
                GestorCopia.ProgresoActualizado += (sender, e) => ProgresoActualizado?.Invoke(this, e);

                // Copiar ficheros uno a uno
                foreach (var fichero in ficheros)
                {
                    var resultadoCopia = await GestorCopia.CopiarAsync(fichero, destino);
                    if (!string.IsNullOrEmpty(resultadoCopia.MensajeError))
                    {
                        resultado.Errores.Add($"{resultadoCopia.MensajeError}");
                    }

                    resultado.ProgramasCopiados.Add(resultadoCopia);
                }

                stopwatchTotal.Stop();
                resultado.TiempoTotalCopia = Utiles.FormatearTiempo(stopwatchTotal.Elapsed);

                // Guardar log de copias
                await _servicioLog.GuardarLogCopiaAsync(resultado);

                // Notificar finalización
                CopiaFinalizada?.Invoke(this, new ResumenCopiaEventArgs(resultado));
            }
            catch (Exception ex)
            {
                stopwatchTotal.Stop();
                resultado.Errores.Add($"Error general: {ex.Message}");
            }

            return resultado;
        }
    }

    internal class ResumenCopiaEventArgs : EventArgs
    {
        public ResultadoCopia Resumen { get; set; }
        public ResumenCopiaEventArgs(ResultadoCopia resumen)
        {
            Resumen = resumen;
        }
    }
}

