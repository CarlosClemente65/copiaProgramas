using copiaProgramas.Modelos;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WinSCP;
using static copiaProgramas.Comun.Enums;

namespace copiaProgramas.Infraestructura
{
    /// <summary>
    /// Implementación de copia remota usando WinSCP
    /// </summary>
    internal class CopiadorRemoto : ICopiador
    {
        public event EventHandler<ProgresoEventArgs> ProgresoActualizado;

        public async Task<ProgramasCopiados> CopiarAsync(FicheroCopia fichero, DestinoCopia destino)
        {
            var resultadoCopia = new ProgramasCopiados
            {
                Programa = fichero.Nombre,
                RutaDestino = destino.RutaDestino
            };

            try
            {
                // Notificar inicio
                NotificarProgreso(fichero.Nombre, $"Copiando el programa {fichero.Nombre}...", EstadoCopia.Iniciando);

                // Configurar opciones de sesión
                SessionOptions opcionesSesion = new SessionOptions
                {
                    Protocol = (Protocol)Enum.Parse(typeof(Protocol), destino.Servidor.Protocolo),
                    HostName = destino.Servidor.HostName,
                    UserName = destino.Servidor.UserName,
                    SshHostKeyFingerprint = destino.Servidor.HostKey,
                    SshPrivateKeyPath = destino.Servidor.PrivateKey
                };
                opcionesSesion.AddRawSettings("AgentFwd", "1");

                Session session = null;

                try
                {
                    await Task.Run(() =>
                    {
                        // Crear y conectar sesión
                        session = new Session();

                        // Suscribir al evento de progreso de WinSCP
                        session.FileTransferProgress += (sender, e) =>
                        {
                            int porcentaje = (int)(e.OverallProgress * 100);
                            NotificarProgreso(fichero.Nombre, $"Copiando {fichero.Nombre}...", EstadoCopia.Copiando, porcentaje);
                        };

                        NotificarProgreso(fichero.Nombre, "Conectando con el servidor...", EstadoCopia.Copiando);
                        session.Open(opcionesSesion);

                        // Configurar opciones de transferencia
                        TransferOptions transferOptions = new TransferOptions
                        {
                            TransferMode = TransferMode.Binary
                        };

                        NotificarProgreso(fichero.Nombre, "Iniciando copia...", EstadoCopia.Copiando);

                        // Realizar la transferencia
                        TransferOperationResult transferResult = session.PutFiles(
                            fichero.RutaOrigenCompleta,
                            destino.RutaDestino,
                            false,
                            transferOptions
                        );

                        // Verificar resultado
                        transferResult.Check();
                    });

                    resultadoCopia.MensajeResultado = "Fichero copiado correctamente";

                    NotificarProgreso(fichero.Nombre, $"Programa {fichero.Nombre} copiado correctamente", EstadoCopia.Finalizado, 100);
                }
                finally
                {
                    session?.Dispose();
                }
            }
            catch (Exception ex)
            {
                resultadoCopia.MensajeResultado = $"Error al copiar el fichero {fichero.Nombre}: {ex.Message}";

                NotificarProgreso(fichero.Nombre, $"Error al copiar {fichero.Nombre}: {ex.Message}", EstadoCopia.Error);
            }

            return resultadoCopia;
        }

        private void NotificarProgreso(string nombreFichero, string mensaje, EstadoCopia estado, int porcentaje = 0)
        {
            ProgresoActualizado?.Invoke(this, new ProgresoEventArgs(nombreFichero, mensaje, estado, porcentaje));
        }
    }
}