using System;
using System.Collections.Generic;
using System.IO;
using copiaProgramas.Modelos;
using Newtonsoft.Json;

namespace copiaProgramas.Servicios
{
    internal class GestorConfiguracion
    {
        // Patron Singleton para acceso global a la configuración
        private static GestorConfiguracion _instancia;
        public static GestorConfiguracion Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new GestorConfiguracion();
                }
                return _instancia;
            }
        }

        // Propiedad para almacenar rutas origen, destino y servidores
        public RutasCopia Configuracion { get; set; }
        public List<FicheroCopia> ListaFicheros { get; set; }

        // Método para cargar rutas y servidores
        public void CargarConfiguracion(string archivoConfiguracion)
        {
            if (File.Exists(archivoConfiguracion))
            {
                generarConfiguracion();
            }
            var json = File.ReadAllText(archivoConfiguracion);
            Configuracion = JsonConvert.DeserializeObject<RutasCopia>(json);
        }


        // Método para cargar ficheros candidatos
        public void CargarFicheros(string archivoFicheros)
        {
            //Si no existe el fichero.json crea uno por defecto
            if (!File.Exists(archivoFicheros))
            {
                generarFichero();
            }

            // Carga la configuracion de ficheros
            var json = File.ReadAllText(archivoFicheros);
            ListaFicheros = JsonConvert.DeserializeObject<List<FicheroCopia>>(json);
        }



        // Método para generar un fichero de configuración con valores por defecto
        private void generarConfiguracion()
        {
            Configuracion = new RutasCopia();
            // Rutas de origen
            Configuracion.Origenes = new List<Ruta>
            {
                new Ruta {Nombre = "PI", RutaBase = @"\\185.57.175.101\basprog_cyc\master9\EstandarPI\"},
                new Ruta {Nombre = "noPI", RutaBase = @"\\185.57.175.101\basprog_cyc\master9\EstandarAsesoria"},
                new Ruta {Nombre = "Gestion", RutaBase = @"\\185.57.175.101\basprog_cyc\master9\EstandarEmpresa\"},
                new Ruta {Nombre = "Gasoleos", RutaBase = @"\\185.57.175.101\basprog_cyc\master9\Medida\"}
            };

            // Rutas de destino
            Configuracion.Destinos = new List<Ruta>
            {
                new Ruta {Nombre ="destinoPi", RutaBase ="/u/dspi/master/"},
                new Ruta {Nombre ="destinoLocal", RutaBase ="c:\\descargas_geco72\\"},
                new Ruta {Nombre ="destinonoPi", RutaBase ="/u/dsnopi/master/"},
                new Ruta {Nombre ="destinoPasesPi", RutaBase ="/u/pases_pi/master/"},
                new Ruta {Nombre ="destinoPasesnoPi", RutaBase ="/u/dsnopi/master/"}
            };

            // Servidores disponibles
            Configuracion.ListaServidores = new List<ServidorCopia>
            {
                new ServidorCopia
                {
                    Nombre = "Geco72",
                    Protocolo = "Sftp",
                    HostName ="172.31.5.149",
                    UserName ="centos",
                    HostKey ="ssh-ed25519 255 ypCFfhJskB3YSCzQzF5iHV0eaWxlBIvMeM5kRl4N46o=",
                    PrivateKey =@"C:\Oficina_ds\Diagram\Accesos portatil\conexiones VPN\Credenciales SSH\aws_diagram_irlanda.ppk"
                },
                new ServidorCopia
                {
                    Nombre = "Geco04",
                    Protocolo = "Sftp",
                    HostName = "172.31.26.21",
                    UserName = "centos",
                    HostKey = "ssh-ed25519 255 EED2o6CV3I8GXE2qqXPEopvallRrpWb8MY2hqmJshGM=",
                    PrivateKey = @"C:\Oficina_ds\Diagram\Accesos portatil\conexiones VPN\Credenciales SSH\aws_diagram_irlanda.ppk"
                }
            };

            string json = JsonConvert.SerializeObject(Configuracion, Formatting.Indented);
            File.WriteAllText("configuracion_defecto.json", json);
        }


        // Metodo para generar un fichero con valores por defecto
        public void generarFichero()
        {
            ListaFicheros = new List<FicheroCopia>();

            //Añade los valores por defecto
            agregarFichero(1, "ipcont08", "ipcont08\\pcont08z.tgz", "Contabilidad");
            agregarFichero(1, "ipbasica", "ipbasica\\pbasicaz.tgz", "Patrones");
            agregarFichero(1, "ipmodelo", "ipmodelo\\pmodeloz.tgz", "Modelos");
            agregarFichero(1, "ipintegr", "ipintegr\\pintegrz.tgz", "Patrones");
            agregarFichero(1, "ippatron", "ippatron\\ppatronz.tgz", "Patrones");
            agregarFichero(1, "siibase", "siibase\\siibasez.tgz", "Contabilidad");
            agregarFichero(1, "000adc", "ipcont08_mod\\000adc\\000adcz.tgz", "Contabilidad");
            agregarFichero(1, "contalap", "ipcontal\\pcontalz.tgz", "Contabilidad");
            agregarFichero(1, "n43base", "n43base\\n43basez.tgz", "Contabilidad");
            agregarFichero(1, "iprent23", "iprent23\\prent23z.tgz", "Modelos");
            agregarFichero(1, "iprent22", "iprent22\\prent22z.tgz", "Modelos");
            agregarFichero(1, "iprent21", "iprent21\\prent21z.tgz", "Modelos");
            agregarFichero(1, "ipconts2", "ipconts2\\pconts2z.tgz", "Modelos");
            agregarFichero(1, "ipabogad", "ipabogad\\pabogadz.tgz", "Facturacion");
            agregarFichero(1, "ipabogax", "ipabogax\\pabogaxz.tgz", "Facturacion");
            agregarFichero(1, "ipabopar", "ipabopar\\paboparz.tgz", "Facturacion");
            agregarFichero(3, "dscomer9", "dscomer9\\scomer9z.tgz", "Facturacion");
            agregarFichero(3, "dscarter", "dscarte5\\scarte5z.tgz", "Facturacion");
            agregarFichero(1, "dsarchi", "dsarchi\\dsarchiz.tgz", "Documentales");
            agregarFichero(1, "certbase", "certbase\\ertbasez.tgz", "Documentales");
            agregarFichero(1, "notibase", "notibase\\otibasez.tgz", "Documentales");
            agregarFichero(1, "dsedespa", "dsedespa\\sedespaz.tgz", "Documentales");
            agregarFichero(1, "dsesign", "dsesign\\dsesignz.tgz", "Documentales");
            agregarFichero(1, "iplabor2", "iplabor2\\plabor2z.tgz", "Laboral");
            agregarFichero(4, "gasbase", "_CEPSA\\gasbase\\gasbasez.tgz", "Gasoleos");
            agregarFichero(4, "dscepsax", "_CEPSA\\dscepsax\\scepsaxz.tgz", "Gasoleos");
            agregarFichero(4, "dsgalx", "_CEPSA\\dscepsax_mod\\dsgalx\\dsgalxz.tgz", "Gasoleos");

            string json = JsonConvert.SerializeObject(ListaFicheros, Formatting.Indented);
            File.WriteAllText("ficheros2.json", json);
        }

        private void agregarFichero(int clase, string nombre, string ruta, string tipo)
        {
            //Metodo para ir agregando cada fichero a la lista de ficheros por defecto
            FicheroCopia nuevoFichero = new FicheroCopia
            {
                Clase = clase,
                Nombre = nombre,
                Ruta = ruta,
                Tipo = tipo,
                Seleccionado = false, // Por defecto no seleccionado
                RutaOrigenCompleta = string.Empty, // Se calculará antes de la copia
                RutaDestino = string.Empty // Se calculará antes de la copia
            };

            ListaFicheros.Add(nuevoFichero);
        }
    }
}
