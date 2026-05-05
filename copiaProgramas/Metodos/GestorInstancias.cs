using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace copiaProgramas.Metodos
{
    // Clase estática para gestionar las instancias de las clases Variables, Programas y Ficheros
    public static class GestorInstancias
    {
        // Propiedades privadas para almacenar las instancias de las clases
        private static Variables _variables;
        private static Programas _programas;
        private static Ficheros _ficheros;

        // Las instancias se crean de forma estática una sola vez
        public static Variables variables
        {
            get
            {
                if(_variables == null)
                {
                    _variables = new Variables();
                }
                return _variables;
            }
        }
        public static Programas Programas
        {
            get
            {
                if(_programas == null)
                {
                    _programas = new Programas();
                }
                return _programas;
            }
        }
        public static Ficheros Ficheros
        {
            get
            {
                if(_ficheros == null)
                {
                    _ficheros = new Ficheros();
                }
                return _ficheros;
            }
        }        
    }
}