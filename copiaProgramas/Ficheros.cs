using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using static copiaProgramas.Ficheros;


namespace copiaProgramas
{
    public class Ficheros
    {
        string rutaJson = @"ficheros.json";
        public char opcion;

        // Lista estática para almacenar los ficheros
        public static List<Fichero> listaFicheros = new List<Fichero>();

        // Clase para representar un fichero con nombre y ruta
        public class Fichero
        {
            public string Nombre { get; set; }
            public string Ruta { get; set; }
            public string Tipo { get; set; }
        }

        //Constructor de la clase Ficheros
        public Ficheros()
        {
            leerFicheros();
        }


        // Método para cargar los ficheros desde el archivo JSON
        public void leerFicheros()
        {
            if (File.Exists(rutaJson))
            {
                string json = File.ReadAllText(rutaJson);
                listaFicheros = JsonConvert.DeserializeObject<List<Fichero>>(json);
            }
        }

        public void grabarFicheros()
        {
            try
            {
                for (int i = 0; i < listaFicheros.Count; i++)
                {
                    //listaFicheros[i].Ruta.Replace(@"\", "\\"); 
                }
                // Serializar la lista de ficheros a JSON
                string json = JsonConvert.SerializeObject(listaFicheros);

                //Guardar el json
                File.WriteAllText(rutaJson, json);
            }

            catch (Exception e)
            {
                MessageBox.Show("No se ha podido actualizar el fichero de configuracion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

        }

        // Método para agregar un fichero a la lista
        public void AgregarFichero(string nombre, string ruta, string tipo)
        {
            listaFicheros.Add(new Fichero { Nombre = nombre, Ruta = ruta , Tipo = tipo});
        }

        // Método para modificar un fichero de la lista
        public void modificarFichero(string nombre, string nuevaRuta, string nuevoTipo)
        {
            //Buscar el fichero en la lista
            Fichero item = listaFicheros.Find(f =>  f.Nombre == nombre);

            if (item != null)
            {
                item.Ruta = nuevaRuta;
                item.Tipo = nuevoTipo;
            }

        }

        //Metodo para eliminar un fichero de la lista
        public void eliminarFichero (string nombre)
        {
            Fichero item = listaFicheros.Find(f => f.Nombre == nombre);

            if (item != null)
            {
                listaFicheros.Remove(item);
            }
        }


    }
}
