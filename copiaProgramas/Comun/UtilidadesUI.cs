using System;
using System.Collections.Generic;

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

        public static string FormatearTiempo(TimeSpan tiempo)
        {
            var partes = new List<string>();

            if (tiempo.Hours > 0)
                partes.Add($"{tiempo.Hours} hora{(tiempo.Hours > 1 ? "s" : "")}");
            if (tiempo.Minutes > 0)
                partes.Add($"{tiempo.Minutes} minuto{(tiempo.Minutes > 1 ? "s" : "")}");
            if (tiempo.Seconds > 0 || partes.Count == 0) // siempre muestra segundos
                partes.Add($"{tiempo.Seconds} segundo{(tiempo.Seconds != 1 ? "s" : "")}");

            return string.Join(" ", partes);
        }

        public static string FormatearFechaCopia(DateTime fecha)
        {
            return fecha.ToString("'Dia:' dd.MM.yyyy '- Hora:' HH:mm");
        }

    }
}
