using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace copiaProgramas
{
    public class Ficheros
    {
        string rutaJson = @"ficheros.json";
        public char opcion;

        // Lista estática para almacenar los ficheros
        public static List<Fichero> listaFicheros = new List<Fichero>();

        //Diccionario para almacenar los datos de los ficheros por defecto
        public List<Dictionary<string, object>> Valores { get; set; }

        // Clase para representar un fichero con nombre y ruta
        public class Fichero
        {
            public string Nombre { get; set; }
            public string Ruta { get; set; }
            public string Tipo { get; set; }
            public int Clase { get; set; }
        }

        //Constructor de la clase Ficheros
        public Ficheros()
        {
            leerFicheros();
        }


        // Método para cargar los ficheros desde el archivo JSON
        public void leerFicheros()
        {
            if (!File.Exists(rutaJson))
            {
                //Si no existe el fichero.json crea uno por defecto
                generarFichero();
            }
            string json = File.ReadAllText(rutaJson);
            listaFicheros = JsonConvert.DeserializeObject<List<Fichero>>(json);
        }

        public void grabarFicheros()
        {
            try
            {
                //for (int i = 0; i < listaFicheros.Count; i++)
                //{
                //    //listaFicheros[i].Ruta.Replace(@"\", "\\"); 
                //}
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
        public void AgregarFichero(string nombre, string ruta, string tipo, int clase)
        {
            listaFicheros.Add(new Fichero { Nombre = nombre, Ruta = ruta, Tipo = tipo, Clase = clase });
        }

        // Método para modificar un fichero de la lista
        public void modificarFichero(string nombre, string nuevaRuta, string nuevoTipo, int nuevaClase)
        {
            //Buscar el fichero en la lista
            Fichero item = listaFicheros.Find(f => f.Nombre == nombre);

            if (item != null)
            {
                item.Ruta = nuevaRuta;
                item.Tipo = nuevoTipo;
                item.Clase = nuevaClase;
            }

        }

        //Metodo para eliminar un fichero de la lista
        public void eliminarFichero(string nombre)
        {
            Fichero item = listaFicheros.Find(f => f.Nombre == nombre);

            if (item != null)
            {
                listaFicheros.Remove(item);
            }
        }

        //Metodo para generar la relacion de ficheros por defecto
        public void generarFichero()
        {
            //Crea un nuevo diccionario para añadir los valores por defecto al ficheros.json
            Valores = new List<Dictionary<string, object>>();

            //Añade los valores por defecto
            agregarValoresDiccionario(1, "ipcont08", "ipcont08\\pcont08z.tgz", "Contabilidad");
            agregarValoresDiccionario(1, "ipbasica", "ipbasica\\pbasicaz.tgz", "Patrones");
            agregarValoresDiccionario(1, "ipmodelo", "ipmodelo\\pmodeloz.tgz", "Modelos");
            agregarValoresDiccionario(1, "ipintegr", "ipintegr\\pintegrz.tgz", "Patrones");
            agregarValoresDiccionario(1, "ippatron", "ippatron\\ppatronz.tgz", "Patrones");
            agregarValoresDiccionario(1, "siibase", "siibase\\siibasez.tgz", "Contabilidad");
            agregarValoresDiccionario(1, "000adc", "ipcont08_mod\\000adc\\000adcz.tgz", "Contabilidad");
            agregarValoresDiccionario(1, "contalap", "ipcontal\\pcontalz.tgz", "Contabilidad");
            agregarValoresDiccionario(1, "n43base", "n43base\\n43basez.tgz", "Contabilidad");
            agregarValoresDiccionario(1, "iprent23", "iprent23\\prent23z.tgz", "Modelos");
            agregarValoresDiccionario(1, "iprent22", "iprent22\\prent22z.tgz", "Modelos");
            agregarValoresDiccionario(1, "iprent21", "iprent21\\prent21z.tgz", "Modelos");
            agregarValoresDiccionario(1, "ipconts2", "ipconts2\\pconts2z.tgz", "Modelos");
            agregarValoresDiccionario(1, "ipabogad", "ipabogad\\pabogadz.tgz", "Facturacion");
            agregarValoresDiccionario(1, "ipabogax", "ipabogax\\pabogaxz.tgz", "Facturacion");
            agregarValoresDiccionario(1, "ipabopar", "ipabopar\\paboparz.tgz", "Facturacion");
            agregarValoresDiccionario(3, "dscomer9", "dscomer9\\scomer9z.tgz", "Facturacion");
            agregarValoresDiccionario(3, "dscarter", "dscarte5\\scarte5z.tgz", "Facturacion");
            agregarValoresDiccionario(1, "dsarchi", "dsarchi\\dsarchiz.tgz", "Documentales");
            agregarValoresDiccionario(1, "certbase", "certbase\\ertbasez.tgz", "Documentales");
            agregarValoresDiccionario(1, "notibase", "notibase\\otibasez.tgz", "Documentales");
            agregarValoresDiccionario(1, "dsedespa", "dsedespa\\sedespaz.tgz", "Documentales");
            agregarValoresDiccionario(1, "dsesign", "dsesign\\dsesignz.tgz", "Documentales");
            agregarValoresDiccionario(1, "iplabor2", "iplabor2\\plabor2z.tgz", "Laboral");
            agregarValoresDiccionario(4, "gasbase", "_CEPSA\\gasbase\\gasbasez.tgz", "Gasoleos");
            agregarValoresDiccionario(4, "dscepsax", "_CEPSA\\dscepsax\\scepsaxz.tgz", "Gasoleos");
            agregarValoresDiccionario(4, "dsgalx", "_CEPSA\\dscepsax_mod\\dsgalx\\dsgalxz.tgz", "Gasoleos");

            string json = JsonConvert.SerializeObject(Valores, Formatting.Indented);
            File.WriteAllText(rutaJson, json);
        }

        private void agregarValoresDiccionario(int clase, string nombre, string ruta, string tipo)
        {
            //Metodo para ir agregando cada uno de los elementos al diccionario
            Dictionary<string, object> nuevoValor = new Dictionary<string, object>
            {
                { "Clase", clase },
                { "Nombre", nombre },
                { "Ruta", ruta },
                { "Tipo", tipo }
            };

            Valores.Add(nuevoValor);
        }


    }
}
