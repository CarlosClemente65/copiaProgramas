using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace copiaProgramas.Comun
{
    internal static class UtilidadesUI
    {
        //Metodo para devolver un string con el tiempo de copia formateado
        public static string convierteTiempo(int tiempo)
        {
            string tiempoTotal = string.Empty;
            int minutos = tiempo / 60;
            int segundos = tiempo % 60;
            if (minutos > 0)
            {
                tiempoTotal += $"{minutos} minutos ";
            }
            tiempoTotal += $"{segundos} segundos";
            return tiempoTotal;
        }

    }
}
