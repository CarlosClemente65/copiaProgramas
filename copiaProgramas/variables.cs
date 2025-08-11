using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using WinSCP;
using System.Collections.Generic;
using System.Linq;

namespace copiaProgramas
{

    public class variables
    {
        //Rutas
        public string rutaPi { get; set; }
        public string rutanoPi { get; set; }
        public string rutaGestion { get; set; }
        public string rutaGasoleos { get; set; }

        //Destinos
        public string destinoPi { get; set; }
        public string destinoLocal { get; set; }
        public string destinonoPi { get; set; }
        public string destinoPasesPi { get; set; }
        public string destinoPasesnoPi { get; set; }
        public string destinoGeco04 { get; set; }
        public string destino { get; set; }

        //Configuracion WinSCP
        public List<Servidores> ListaServidores { get; set; }
        public Servidores ServidorSeleccionado { get; set; }

        public variables()
        {
            /*Constructor de la clase que asigna los valores a las variables cuando se hace una instancia
             * Se cargan con estos valores por defecto para el caso de que se haya perdido el fichero de configuracion que se graba con el metodo 'GuardarConfiguracion'
            */


            //Rutas origen por defecto
            rutaPi = @"\\185.57.175.101\basprog_cyc\master9\EstandarPI\";
            rutanoPi = @"\\185.57.175.101\basprog_cyc\master9\EstandarAsesoria\";
            rutaGestion = @"\\185.57.175.101\basprog_cyc\master9\EstandarEmpresa\";
            rutaGasoleos = @"\\185.57.175.101\basprog_cyc\master9\Medida\";

            //Rutas destino por defecto
            destinoPi = @"/u/dspi/master/";
            destinoLocal = @"c:\descargas_geco72\";
            destinonoPi = @"/u/dsnopi/master/";
            destinoPasesPi = @"/u/pases_pi/master/";
            destinoPasesnoPi = @"/u/pases_nopi/master/";
            destinoGeco04 = @"/u/ase0516/master/";
            destino = destinoPi;

            ListaServidores = new List<Servidores>(); // Inicializa la lista de servidores

            // Añade el servidor geco72 a la lista y lo pone como servidor seleccionado
            CargarServidor("Geco72");

        }


        public void GuardarConfiguracion()
        {
            //Permite guardar en un fichero json el contenido de las variables
            try
            {
                string rutaArchivo = "configuracion.json";
                string jsonConfiguracion = JsonConvert.SerializeObject(
                    this,
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
                    });

                File.WriteAllText(rutaArchivo, jsonConfiguracion);
                MessageBox.Show(
                    "Fichero de configuracion actualizado correctamente",
                    "Actualizar configuracion",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show(
                    "Fichero de configuracion no se ha podido actualizar", 
                    "Actualizar configuracion", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        public void CargarConfiguracion()
        {
            //Carga el contenido de las variables del fichero de configuracion
            string rutaArchivo = "configuracion.json";

            if(File.Exists(rutaArchivo))
            {
                string jsonConfiguracion = File.ReadAllText(rutaArchivo);
                JsonConvert.PopulateObject(jsonConfiguracion, this);

                // Actualiza las variables después de cargar la configuración
                ActualizaVariables(rutaPi, rutanoPi, rutaGestion, rutaGasoleos, destinoPi, destinonoPi, destinoLocal, destinoPasesPi, destinoPasesnoPi, ServidorSeleccionado.Nombre, ServidorSeleccionado.Protocolo, ServidorSeleccionado.HostName, ServidorSeleccionado.UserName, ServidorSeleccionado.HostKey, ServidorSeleccionado.PrivateKey);
            }

        }

        public void ActualizaVariables(string nuevaRutaPi, string nuevaRutanoPi, string nuevaRutaGestion, string nuevaRutaGasoleos, string nuevoDestinoPi, string nuevoDestinonoPi, string nuevoDestinoLocal, string nuevoDestinoPasesPi, string nuevoDestinoPasesnoPi, string _nombreServidor, WinSCP.Protocol _protocolo, string _hostName, string _userName, string _hostKey, string _privateKey)
        {
            //Una vez leidas las variables del fichero de configuracion, se graban en las variables de la clase
            rutaPi = nuevaRutaPi;
            rutanoPi = nuevaRutanoPi;
            rutaGestion = nuevaRutaGestion;
            rutaGasoleos = nuevaRutaGasoleos;
            destinoPi = nuevoDestinoPi;
            destinonoPi = nuevoDestinonoPi;
            destinoLocal = nuevoDestinoLocal;
            destinoPasesPi = nuevoDestinoPasesPi;
            destinoPasesnoPi = nuevoDestinoPasesnoPi;
            ServidorSeleccionado.Nombre = _nombreServidor;
            ServidorSeleccionado.Protocolo = _protocolo;
            ServidorSeleccionado.HostName = _hostName;
            ServidorSeleccionado.UserName = _userName;
            ServidorSeleccionado.HostKey = _hostKey;
            ServidorSeleccionado.PrivateKey = _privateKey;


        }

        public void CargarServidor(string nombreServidor)
        {
            //Carga la configuracion del servidor seleccionado
            if(ListaServidores.Count > 0)
            {
                ServidorSeleccionado = ListaServidores.FirstOrDefault(s => s.Nombre == nombreServidor); // Selecciona el servidor por nombre
            }
        }
    }

    public class Servidores
    {
        public string Nombre { get; set; }
        public Protocol Protocolo { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string HostKey { get; set; }
        public string PrivateKey { get; set; }
        public Servidores(string nombre, Protocol protocolo, string hostName, string userName, string hostKey, string privateKey)
        {
            Nombre = nombre;
            Protocolo = protocolo;
            HostName = hostName;
            UserName = userName;
            HostKey = hostKey;
            PrivateKey = privateKey;
        }
    }
}
