using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using enums = copiaProgramas.Comun.Enums;

namespace copiaProgramas.Modelos
{
    internal class ProgresoEventArgs : EventArgs
    {
        public string NombreFichero { get; set; }
        public string Mensaje { get; set; }
        public int PorcentajeProgreso { get; set; } // 0-100
        public enums.EstadoCopia Estado { get; set; }

        public ProgresoEventArgs(string nombreFichero, string mensaje, enums.EstadoCopia estado, int porcentaje = 0)
        {
            NombreFichero = nombreFichero;
            Mensaje = mensaje;
            Estado = estado;
            PorcentajeProgreso = porcentaje;
        }


        // Ejemplo de como implementarlo en el proceso de copia:
        /*
         // Al inicio de la clase que realiza la copia, definimos el evento
         public event EventHandler<ProgresoEventArgs> ProgresoActualizado;

        // Dentro del proceso de copia, avisamos que la copia ha empezado
        ProgresoActualizado?.Invoke(this, new ProgresoEventArgs(fichero.Nombre, "Iniciando copia", EstadoCopia.Iniciado));
         */
        // ProgresoEventArgs args = new ProgresoEventArgs(nombreFichero, "Copiando...", EstadoCopia.Copiando, porcentaje);

    }       
}
