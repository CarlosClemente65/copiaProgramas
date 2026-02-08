using System;
using System.Collections.Generic;
using System.IO;
using copiaProgramas.Modelos;

namespace copiaProgramas.Servicios
{
    /// <summary>
    /// Servicio para validar datos antes de realizar operaciones
    /// </summary>
    internal class ServicioValidacion
    {
        public List<string> ValidarCopia(List<FicheroCopia> ficheros, DestinoCopia destino)
        {
            var errores = new List<string>();

            // Validar que hay ficheros seleccionados
            if (ficheros == null || ficheros.Count == 0)
            {
                errores.Add("No hay ficheros seleccionados para copiar");
                return errores;
            }

            // Validar destino
            if (destino == null)
            {
                errores.Add("No se ha especificado un destino válido");
                return errores;
            }

            // Validar ruta destino
            if (string.IsNullOrWhiteSpace(destino.RutaDestino))
            {
                errores.Add("La ruta de destino no puede estar vacía");
            }

            // Si es copia remota, validar servidor
            if (destino.EsRemoto)
            {
                if (destino.Servidor == null)
                {
                    errores.Add("No se ha configurado el servidor para copia remota");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(destino.Servidor.HostName))
                        errores.Add("El servidor no tiene configurado el HostName");

                    if (string.IsNullOrWhiteSpace(destino.Servidor.UserName))
                        errores.Add("El servidor no tiene configurado el UserName");
                }
            }

            // Validar que existen los ficheros origen
            foreach (var fichero in ficheros)
            {
                if (string.IsNullOrWhiteSpace(fichero.RutaOrigenCompleta))
                {
                    errores.Add($"El fichero '{fichero.Nombre}' no tiene ruta de origen");
                    continue;
                }

                if (!File.Exists(fichero.RutaOrigenCompleta))
                {
                    errores.Add($"No se encuentra el fichero: {fichero.RutaOrigenCompleta}");
                }
            }

            return errores;
        }
    }
}