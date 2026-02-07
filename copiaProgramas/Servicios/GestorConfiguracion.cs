using System.Collections.Generic;
using System.IO;
using copiaProgramas.Comun;
using copiaProgramas.Modelos;
using Newtonsoft.Json;
using enums = copiaProgramas.Comun.Enums;

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

        public void Inicializar()
        {
            string rutaConfiguracion = "configuracion_defecto.json";
            string rutaFicheros = "ficheros.json";

            // Cargar rutas y servidores
            CargarConfiguracion(rutaConfiguracion);

            // Cargar lista de ficheros
            CargarFicheros(rutaFicheros);

        }

        // Método para cargar rutas y servidores
        private void CargarConfiguracion(string archivoConfiguracion)
        {
            if (!File.Exists(archivoConfiguracion))
            {
                GenerarConfiguracion(archivoConfiguracion);
            }
            else
            {
                Configuracion = GestionArchivos.LeerJson<RutasCopia>(archivoConfiguracion);

                // Validar que se han cargado correctamente las secciones necesarias
                if (Configuracion.Origenes == null || Configuracion.Destinos == null || Configuracion.ListaServidores == null)
                {
                    GenerarConfiguracion(archivoConfiguracion); // Regenera el archivo si falta alguna sección
                }
            }

            // Establece el 'geco72' como servidor seleccionado por defecto si no se ha cargado uno previamente
            if (Configuracion.ServidorSeleccionado == null)
            {
                Configuracion.ServidorSeleccionado = Configuracion.ListaServidores.Find(s => s.Nombre == "Geco72");
            }

            // Establece el destino geco72 como destino seleccionado por defecto si no se ha cargado uno previamente
            if (Configuracion.DestinoSeleccionado == null)
            {
                Configuracion.DestinoSeleccionado = Configuracion.Destinos.Find(d => d.Nombre == "destinoPi");
            }
        }


        // Método para cargar ficheros candidatos
        private void CargarFicheros(string archivoFicheros)
        {
            //Si no existe el fichero.json crea uno por defecto
            if (!File.Exists(archivoFicheros))
            {
                GenerarFichero(archivoFicheros);
            }
            else
            {
                ListaFicheros = GestionArchivos.LeerJson<List<FicheroCopia>>(archivoFicheros);

                if (ListaFicheros == null)
                {
                    GenerarFichero(archivoFicheros); // Regenera el archivo si no se han cargado correctamente los ficheros
                }
            }
        }


        // Método para generar un fichero de configuración con valores por defecto
        private void GenerarConfiguracion(string archivoConfiguracion)
        {
            Configuracion = new RutasCopia();
            // Rutas de origen
            Configuracion.Origenes = new List<Ruta>
            {
                new Ruta {Nombre = "PI", RutaBase = @"\\185.57.175.101\basprog_cyc\master9\EstandarPI\", Clase = enums.ClaseFichero.PI},
                new Ruta {Nombre = "noPI", RutaBase = @"\\185.57.175.101\basprog_cyc\master9\EstandarAsesoria", Clase = enums.ClaseFichero.NoPI},
                new Ruta {Nombre = "Gestion", RutaBase = @"\\185.57.175.101\basprog_cyc\master9\EstandarEmpresa\", Clase = enums.ClaseFichero.Gestion},
                new Ruta {Nombre = "Gasoleos", RutaBase = @"\\185.57.175.101\basprog_cyc\master9\Medida\", Clase = enums.ClaseFichero.Gasoleos}
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
            File.WriteAllText(archivoConfiguracion, json);
        }


        // Metodo para generar un fichero con valores por defecto
        public void GenerarFichero(string archivoFicheros)
        {
            ListaFicheros = new List<FicheroCopia>();

            //Añade los valores por defecto
            AgregarFichero(enums.ClaseFichero.PI, "ipcont08", "ipcont08\\pcont08z.tgz", "Contabilidad");
            AgregarFichero(enums.ClaseFichero.PI, "ipbasica", "ipbasica\\pbasicaz.tgz", "Patrones");
            AgregarFichero(enums.ClaseFichero.PI, "ipmodelo", "ipmodelo\\pmodeloz.tgz", "Modelos");
            AgregarFichero(enums.ClaseFichero.PI, "ipintegr", "ipintegr\\pintegrz.tgz", "Patrones");
            AgregarFichero(enums.ClaseFichero.PI, "ippatron", "ippatron\\ppatronz.tgz", "Patrones");
            AgregarFichero(enums.ClaseFichero.PI, "siibase", "siibase\\siibasez.tgz", "Contabilidad");
            AgregarFichero(enums.ClaseFichero.PI, "000adc", "ipcont08_mod\\000adc\\000adcz.tgz", "Contabilidad");
            AgregarFichero(enums.ClaseFichero.PI, "contalap", "ipcontal\\pcontalz.tgz", "Contabilidad");
            AgregarFichero(enums.ClaseFichero.PI, "n43base", "n43base\\n43basez.tgz", "Contabilidad");
            AgregarFichero(enums.ClaseFichero.PI, "iprent23", "iprent23\\prent23z.tgz", "Modelos");
            AgregarFichero(enums.ClaseFichero.PI, "iprent22", "iprent22\\prent22z.tgz", "Modelos");
            AgregarFichero(enums.ClaseFichero.PI, "iprent21", "iprent21\\prent21z.tgz", "Modelos");
            AgregarFichero(enums.ClaseFichero.PI, "ipconts2", "ipconts2\\pconts2z.tgz", "Modelos");
            AgregarFichero(enums.ClaseFichero.PI, "ipabogad", "ipabogad\\pabogadz.tgz", "Facturacion");
            AgregarFichero(enums.ClaseFichero.PI, "ipabogax", "ipabogax\\pabogaxz.tgz", "Facturacion");
            AgregarFichero(enums.ClaseFichero.PI, "ipabopar", "ipabopar\\paboparz.tgz", "Facturacion");
            AgregarFichero(enums.ClaseFichero.Gestion, "dscomer9", "dscomer9\\scomer9z.tgz", "Facturacion");
            AgregarFichero(enums.ClaseFichero.Gestion, "dscarter", "dscarte5\\scarte5z.tgz", "Facturacion");
            AgregarFichero(enums.ClaseFichero.PI, "dsarchi", "dsarchi\\dsarchiz.tgz", "Documentales");
            AgregarFichero(enums.ClaseFichero.PI, "certbase", "certbase\\ertbasez.tgz", "Documentales");
            AgregarFichero(enums.ClaseFichero.PI, "notibase", "notibase\\otibasez.tgz", "Documentales");
            AgregarFichero(enums.ClaseFichero.PI, "dsedespa", "dsedespa\\sedespaz.tgz", "Documentales");
            AgregarFichero(enums.ClaseFichero.PI, "dsesign", "dsesign\\dsesignz.tgz", "Documentales");
            AgregarFichero(enums.ClaseFichero.PI, "iplabor2", "iplabor2\\plabor2z.tgz", "Laboral");
            AgregarFichero(enums.ClaseFichero.Gasoleos, "gasbase", "_CEPSA\\gasbase\\gasbasez.tgz", "Gasoleos");
            AgregarFichero(enums.ClaseFichero.Gasoleos, "dscepsax", "_CEPSA\\dscepsax\\scepsaxz.tgz", "Gasoleos");
            AgregarFichero(enums.ClaseFichero.Gasoleos, "dsgalx", "_CEPSA\\dscepsax_mod\\dsgalx\\dsgalxz.tgz", "Gasoleos");

            string json = JsonConvert.SerializeObject(ListaFicheros, Formatting.Indented);
            File.WriteAllText(archivoFicheros, json);
        }

        private void AgregarFichero(enums.ClaseFichero clase, string nombre, string ruta, string tipo)
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

        public void AsignarRutas(List<FicheroCopia> listaFicheros)
        {
            // Recorre la lista de ficheros y asigna las rutas completas de origen y destino para los ficheros seleccionados
            foreach (var fichero in listaFicheros)
            {
                // Busca la ruta de origen que corresponde a la clase del fichero
                var rutaOrigen = Configuracion.Origenes.Find(o => o.Clase == (enums.ClaseFichero)fichero.Clase);
                if (rutaOrigen != null)
                {
                    fichero.RutaOrigenCompleta = Path.Combine(rutaOrigen.RutaBase, fichero.Ruta);
                }

                // Combina la ruta base del destino seleccionado con el nombre del fichero para obtener la ruta destino completa
                if (Configuracion.DestinoSeleccionado != null)
                {
                    fichero.RutaDestino = Path.Combine(Configuracion.DestinoSeleccionado.RutaBase, Path.GetFileName(fichero.Ruta));
                }
            }

            return listaFicheros;
        }
    }
}
