using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

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
        public string destino {  get; set; }

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
            destino = destinoPi;
        }


        public void GuardarConfiguracion()
        {
            //Permite guardar en un fichero json el contenido de las variables
            try
            {
                string rutaArchivo = "configuracion.json";
                string jsonConfiguracion = JsonConvert.SerializeObject(this);
                File.WriteAllText(rutaArchivo, jsonConfiguracion);
                MessageBox.Show("Fichero de configuracion actualizado correctamente", "Actualizar configuracion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Fichero de configuracion no se ha podido actualizar", "Actualizar configuracion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void CargarConfiguracion()
        {
            //Carga el contenido de las variables del fichero de configuracion
            string rutaArchivo = "configuracion.json";

            if (File.Exists(rutaArchivo))
            {
                string jsonConfiguracion = File.ReadAllText(rutaArchivo);
                JsonConvert.PopulateObject(jsonConfiguracion, this);

                // Actualiza las variables después de cargar la configuración
                ActualizaVariables(rutaPi, rutanoPi, rutaGestion, rutaGasoleos, destinoPi, destinonoPi, destinoLocal, destinoPasesPi, destinoPasesnoPi);
            }

        }

        public void ActualizaVariables(string nuevaRutaPi, string nuevaRutanoPi, string nuevaRutaGestion, string nuevaRutaGasoleos, string nuevoDestinoPi, string nuevoDestinonoPi, string nuevoDestinoLocal, string nuevoDestinoPasesPi, string nuevoDestinoPasesnoPi)
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
        }
    }

    

}
