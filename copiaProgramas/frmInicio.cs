using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using WinSCP;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace copiaProgramas
{
    public partial class frmInicio : Form
    {
        variables variable = new variables(); //Instanciacion de la clase variables para acceder a las variables de configuracion
        Programas programa = new Programas(); //Instanciacion de la clase Programas
        Ficheros fichero = new Ficheros(); //Instanciacion de la clase Ficheros para acceso a los ficheros de configuracion
        int resultadoCopia = 0;
        ListView ListaFicheros = null; //Lista de ficheros para la copia
        TabPage tabPI = new TabPage();
        TabPage tabNopi = new TabPage();
        bool controlTab = true;
        string passwordBorrado = "Carlos65";
        string PathRegistroCopias = "RegistroCopias.json"; //Ruta del registro de copias

        //Variables para controlar el informe de copias
        StringBuilder informeCopia = new StringBuilder(); //Variable para el informe de copias
        string rutaInforme = "logCopia.txt"; //Ruta del informe de copias

        //Variables para controlas el registro de copias
        List<RegistroCopia.ProgramaCopiado> programasCopiados = new List<RegistroCopia.ProgramaCopiado>(); //Lista de programas copiados
        string TiempoTotalCopia = string.Empty; //Variable para el tiempo total de la copia

        //Diccionario para el control de los checkBox con los programas que permite vincular cada uno con su varible correspondiente y saber que programas copiar
        private Dictionary<CheckBox, string> checkBoxVariables = new Dictionary<CheckBox, string>();


        public frmInicio()
        {
            InitializeComponent();

            //Suscribe al evento cuando se selecciona una pestaña del tabControl
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;

            //Lee el fichero de configuracion para cargar las variables
            variable.CargarConfiguracion();

            //Rellena los textBox con los valores cargados en las variables desde el fichero de configuracion
            cbDestinoCopias.SelectedIndex = 0;
            actualizaListaFicheros(lstFicherosOrigen);
            tabControl1.SelectTab("tabCopias");
            tabPI = tabControl1.TabPages["tabProgramasPi"];
            tabNopi = tabControl1.TabPages["tabProgramasnoPI"];
            activarPestañas();

        }


        //Carga los valores de los textBox de la configuracion de rutas y Winscp
        public void ActualizaTextBox()
        {
            txtRutaPi.Text = variable.rutaPi;
            txtRutanoPi.Text = variable.rutanoPi;
            txtRutaGestion.Text = variable.rutaGestion;
            txtRutaGasoleos.Text = variable.rutaGasoleos;
            txtDestinoPi.Text = variable.destinoPi;
            txtDestinonoPi.Text = variable.destinonoPi;
            txtDestinoLocal.Text = variable.destinoLocal;
            txtDestinoPasesPi.Text = variable.destinoPasesPi;
            txtDestinoPasesnoPi.Text = variable.destinoPasesnoPi;
            txtProtocolo.Text = variable.Protocolo.ToString();
            txtHostname.Text = variable.HostName;
            txtUsername.Text = variable.UserName;
            txtHostkey.Text = variable.HostKey;
            txtPrivatekey.Text = variable.PrivateKey;
        }

        //Metodo para controlar las acciones a realizar segun la pestaña seleccionada
        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(tabControl1.SelectedTab.Name)
            {
                //Pestaña proceso copia
                case "tabCopias":
                    actualizaListaFicheros(lstFicherosOrigen); //Carga la lista de ficheros
                    cbDestinoCopias.SelectedIndex = 0; //Fija el destino de la copia a la carpeta PI
                    txtDestinoCopias.Clear(); //Limpia el textBox de destino de la copia
                    break;

                case "tabControlCopias":
                    //// Ruta del archivo JSON (ajusta si está en otra ubicación)
                    //string rutaArchivo = "RegistroCopias.json";

                    //Carga el registro de copias
                    var listaCopias = RegistroCopia.ProcesarCopiasLeidas(PathRegistroCopias);

                    //Llama al metodo para mostrar la lista de copias
                    MostrarListaCopias(listaCopias);
                    break;

                //Pestaña configuracion ficheros
                case "tabFicheros":
                    actualizaListaFicheros(lstFicheros); //Carga la lista de ficheros
                    txtTipoFichero.Location = new Point(13, 137); //Situa el textBox del tipo de fichero encima de la lista
                    btnCancelarFich.Location = new Point(13, 252); //Situa el boton cancelar una vez ajustado el tipo de fichero
                    btnValidarFich.Location = new Point(210, 252); //Situa el boton validar una vez ajustado el tipo de fichero
                    break;

                //Pestaña configuracion rutas
                case "tabConfiguracion":
                    variable.CargarConfiguracion(); //Carga las variables del fichero de configuracion
                    ActualizaTextBox(); //Carga los valores de las variables en los textBox
                    break;

                //Pestaña configuracion Winscp
                case "tabWinscp":
                    variable.CargarConfiguracion(); //Carga las variables del fichero de configuracion
                    ActualizaTextBox(); //Carga los valores de las variables en los textBox
                    break;

                //Pestaña programas PI
                case "tabProgramasPi":
                    txtProgresoCopiaPI.Clear(); //Limpia el textBox de progreso de la copia
                    cb_destinoPI.SelectedIndex = 0; //Selecciona el destino de la copia a la carpeta PI
                    LimpiaCbxPi(); //Quita las marcas de los checkBox
                    break;

                //Pestaña programas no PI
                case "tabProgramasnoPI":
                    txtProgresoCopianoPI.Clear(); //Limpia el textBox de progreso de la copia
                    cb_destinonoPI.SelectedIndex = 0; //Selecciona el destino de la copia a la carpeta no PI
                    LimpiaCbxnoPi(); //Quita las marcas de los checkBox
                    break;
            }
        }

        //Metodo para mostrar la lista de copias de la pestaña de copias
        private void MostrarListaCopias(List<(DateTime Fecha, RegistroCopia Copia)> listaCopias)
        {
            // Limpiamos el ListBox antes de mostrar los nuevos datos
            rbCopias.Clear();

            string indentado = "   ";

            //Ordenamos la lista de copias por fecha
            listaCopias = RegistroCopia.OrdenarCopiasLeidas(listaCopias);

            // Recorremos la lista de copias y las mostramos en el ListBox
            foreach(var (fecha, copia) in listaCopias)
            {
                rbCopias.AppendText("\n"); //Linea en blanco para separar elementos
                rbCopias.SelectionFont = new Font(rbCopias.Font, FontStyle.Bold); // Cambia el estilo de la fuente a negrita
                rbCopias.AppendText($"{indentado}- Fecha copia: {fecha:dd.MM.yyyy} - {fecha:HH:mm}\n");
                rbCopias.SelectionFont = new Font(rbCopias.Font, FontStyle.Regular); // Cambia el estilo de la fuente a normal
                rbCopias.AppendText($"{indentado}{indentado}Tiempo total: {copia.TiempoTotalCopia}\n");

                rbCopias.AppendText($"{indentado}{indentado}Programas copiados:\n");

                foreach(var programa in copia.ProgramasCopiados)
                {
                    rbCopias.AppendText($"{indentado}{indentado}{indentado}- {programa.Programa}\n");
                }
            }
        }

        private void ActualizarProgreso(string mensaje, int pestaña)
        {
            //Metodo para actualizar el textBox con el progreso de la copia
            if(InvokeRequired) // Se asegura que esta en el mismo hilo de ejecucion
            {
                Invoke(new Action<string, int>(ActualizarProgreso), mensaje, pestaña);
            }
            else
            {
                switch(pestaña)
                {
                    //Pestaña programas PI
                    case 1:
                        txtProgresoCopiaPI.AppendText(mensaje);
                        txtProgresoCopiaPI.AppendText(string.Empty + "\r\n");
                        break;

                    //Pestaña programas no PI
                    case 2:
                        txtProgresoCopianoPI.AppendText(mensaje);
                        txtProgresoCopianoPI.AppendText(string.Empty + "\r\n");
                        break;

                    //Pestaña copias
                    case 3:
                        txtDestinoCopias.AppendText(mensaje);
                        txtDestinoCopias.AppendText(string.Empty + "\r\n");
                        break;

                }
            }
        }

        private void actualizaListaFicheros(ListView nombreLista)
        {
            //Metodo para cargar en una lista los nombres de los ficheros del fichero de configuracion y mostrarlos en el ListView que se pase como parametro 
            ListaFicheros = nombreLista;
            ListaFicheros.Items.Clear(); //Limpia la lista de ficheros
            foreach(var fichero in Ficheros.listaFicheros) //Recorre la lista de ficheros
            {
                string nombre = fichero.Nombre; //Asigna el nombre del fichero
                string ruta = fichero.Ruta; //Asigna la ruta del fichero
                string tipo = fichero.Tipo; //Asigna el tipo del fichero
                string clase = fichero.Clase.ToString(); //Asigna la clase del fichero
                ListViewItem item = new ListViewItem(nombre); //Crea un nuevo item en la lista
                item.SubItems.Add(ruta); //Añade la ruta al item
                item.SubItems.Add(tipo); //Añade el tipo al item
                item.SubItems.Add(clase); //Añade la clase al item

                ListViewGroup grupo = null; //Crea un nuevo grupo en la lista
                foreach(ListViewGroup Grupos in ListaFicheros.Groups) //Crea los distintos grupos de la lista
                {
                    if(Grupos.Header == tipo)
                    {
                        grupo = Grupos;
                        break;
                    }
                }

                if(grupo == null)
                {
                    grupo = new ListViewGroup(tipo);
                    ListaFicheros.Groups.Add(grupo);
                }

                item.Group = grupo; //Añade el nombre del grupo al item
                ListaFicheros.Items.Add(item); //Añade el item a la lista
            }

            ListaFicheros.View = View.Details; //Muestra la lista en detalle

        }

        //Metodo para lanzar la copia desde las pestañas de PI y noPI (ocultas por defecto)
        private async Task LanzaCopia(bool programa, string fichero, string titulo, string ruta, int pestaña)
        {
            //Metodo para hacer las copias desde las pestañas de PI y noPI
            if(programa) //Si esta marcado el programa pasado se lanza la copia
            {
                string origen = ruta + fichero;
                string nombreFichero = Path.GetFileName(fichero); //Obtiene el nombre del programa
                string destino = variable.destino + nombreFichero; //Forma la ruta completa del programa

                //Controla para hacer la copia local
                if(variable.destino == variable.destinoLocal)
                {
                    try
                    {
                        ActualizarProgreso($"Copiando el programa {titulo}", pestaña);
                        await Task.Run(() =>
                        {
                            try
                            {
                                File.Copy(origen, destino, true);
                                ActualizarProgreso($"Programa {titulo} copiado correctamente." + Environment.NewLine, pestaña);
                                resultadoCopia++;
                                if(informeCopia.Length == 0)
                                {
                                    informeCopia.AppendLine($"Fecha copia: {DateTime.Now}");
                                }
                            }

                            catch(Exception ex)
                            {
                                ActualizarProgreso(Environment.NewLine + $"Error al copiar el programa {titulo}" + Environment.NewLine + ex.Message, pestaña);
                            }
                        }).ConfigureAwait(false);

                    }

                    catch(Exception ex)
                    {
                        ActualizarProgreso(Environment.NewLine + $"Error al copiar el programa {titulo}" + Environment.NewLine + ex.Message, pestaña);
                    }
                }
                else
                {
                    //Si la copia es al geco72
                    try
                    {
                        ActualizarProgreso($"Copiando el programa {titulo}", pestaña);

                        // Configuración de opciones de sesión para la copia al geco72
                        SessionOptions opcionesSesion = new SessionOptions
                        {
                            Protocol = variable.Protocolo,
                            HostName = variable.HostName,
                            UserName = variable.UserName,
                            SshHostKeyFingerprint = variable.HostKey,
                            SshPrivateKeyPath = variable.PrivateKey
                        };
                        opcionesSesion.AddRawSettings("AgentFwd", "1");

                        Session session = null;

                        try
                        {
                            await Task.Run(async () =>
                            {
                                // Conexión
                                session = new Session();

                                //Permite controlar el progreso de copia
                                session.FileTransferProgress += (sender, e) =>
                                {
                                    if(pestaña == 1)
                                    {
                                        //Actualiza la barra de progreso de copia
                                        progressBar1.Invoke((MethodInvoker)(() =>
                                        {
                                            progressBar1.Value = (int)(e.OverallProgress * 100);
                                            //Muestra el porcentaje completado
                                            int porcentaje = (int)(e.OverallProgress * 100);
                                            lbl_porcentaje.Text = $"{porcentaje}%";
                                        }));
                                    }
                                    else if(pestaña == 2)
                                    {
                                        progressBar2.Invoke((MethodInvoker)(() =>
                                        {
                                            progressBar2.Value = (int)(e.OverallProgress * 100);
                                            int porcentaje = (int)(e.OverallProgress * 100);
                                            lbl_porcentaje2.Text = $"{porcentaje}%";
                                        }));
                                    }
                                };

                                ActualizarProgreso($"Conectando con el servidor . . .", pestaña);
                                session.Open(opcionesSesion);

                                TransferOptions transferOptions = new TransferOptions();
                                transferOptions.TransferMode = TransferMode.Binary;

                                ActualizarProgreso($"Iniciando copia . . .", pestaña);
                                TransferOperationResult transferResult = session.PutFiles(origen, variable.destino, false, transferOptions);
                                transferResult.Check();

                                // Muestra información sobre la transferencia al finalizar
                                ActualizarProgreso($"Programa {titulo} copiado correctamente." + Environment.NewLine, pestaña);
                                resultadoCopia++;
                            }).ConfigureAwait(false);
                        }

                        catch(Exception ex)
                        {
                            ActualizarProgreso(Environment.NewLine + $"Error al copiar el programa {titulo}" + Environment.NewLine + ex.Message + Environment.NewLine, pestaña);
                        }

                        finally
                        {
                            //Libera el recurso de la sesion
                            session?.Dispose();
                        }
                    }
                    catch(Exception ex)
                    {
                        ActualizarProgreso(Environment.NewLine + $"No se ha podido copiar el programa {titulo}" + Environment.NewLine + ex.Message, pestaña);
                    }
                }
            }
        }


        #region pestaña_ProgramasPI
        //Metodos comunes para la pestaña de programas PI

        //Quita las marcas de los checkBox
        private void LimpiaCbxPi()
        {
            foreach(Control control in panelPI.Controls)
            {
                if(control is CheckBox cbx)
                {
                    cbx.Checked = false;
                }
            }
        }

        //Metodo para asignar las rutas destino de la copia segun el valor seleccionado en el comboBox
        private void cb_destinoPI_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(cb_destinoPI.SelectedIndex)
            {
                case 0:
                    variable.destino = variable.destinoPi;
                    break;

                case 1:
                    variable.destino = variable.destinoLocal;
                    break;

                case 2:
                    variable.destino = variable.destinoPasesPi;
                    break;
            }
        }

        //Metodo para desmarcar todos los checkBox y limpiar el textoBox del resultado de la copia
        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            txtProgresoCopiaPI.Text = string.Empty;
            foreach(Control control in panelPI.Controls)
            {
                if(control is CheckBox cbx)
                {
                    cbx.Checked = false;
                }
            }
        }

        //Metodo para lanzar la copia desde la pestaña de programas PI (oculta por defecto)
        private async void btnCopiarPI_Click(object sender, EventArgs e)
        {
            resultadoCopia = 0; //Control para si se ha hecho alguna copia correctamente
            int controlCbx = 0;//Controla si hay algun checbox marcado para hacer la copia

            tabControl1.Enabled = false; //Desactiva el tabControl para evitar que se cambie de pestaña mientras se hace la copia

            foreach(Control control in panelPI.Controls) //Reccore los controles para ver si hay alguno marcado
            {
                if(control is CheckBox cbx && cbx.Checked)
                {
                    controlCbx++;
                }
            }

            if(controlCbx > 0) //En caso de que haya alguno marcado, prepara la copia
            {
                //Crea una lista de las tareas de copia a realizar
                List<Func<Task>> tareasCopia = new List<Func<Task>>
                {
                    //Lanza el proceso de copia segun los checkBox marcados en los programas (no esta actualizado)
                    //Programas PI
                    () => LanzaCopia(programa.ipcont08, programa.ipcont08Ruta, "ipcont08", variable.rutaPi, 1),
                    () => LanzaCopia(programa.siibase, programa.siibaseRuta, "siibase", variable.rutaPi, 1),
                    () => LanzaCopia(programa.v000adc, programa.v000adcRuta, "000adc", variable.rutaPi, 1),
                    () => LanzaCopia(programa.n43base, programa.n43baseRuta, "n43base", variable.rutaPi, 1),
                    () => LanzaCopia(programa.contalap, programa.contalapRuta, "contalap", variable.rutaPi, 1),
                    () => LanzaCopia(programa.ipmodelo, programa.ipmodeloRuta, "ipmodelo", variable.rutaPi, 1),
                    () => LanzaCopia(programa.iprent23, programa.iprent23Ruta, "iprent23", variable.rutaPi, 1),
                    () => LanzaCopia(programa.iprent22, programa.iprent22Ruta, "iprent22", variable.rutaPi, 1),
                    () => LanzaCopia(programa.iprent21, programa.iprent21Ruta, "iprent21", variable.rutaPi, 1),
                    () => LanzaCopia(programa.ipconts2, programa.ipconts2Ruta, "ipconts2", variable.rutaPi, 1),
                    () => LanzaCopia(programa.ipabogax, programa.ipabogaxRuta, "ipabogax", variable.rutaPi, 1),
                    () => LanzaCopia(programa.ipabogad, programa.ipabogadRuta, "ipabogad", variable.rutaPi, 1),
                    () => LanzaCopia(programa.ipabopar, programa.ipaboparRuta, "ipabopar", variable.rutaPi, 1),
                    () => LanzaCopia(programa.dscomer9, programa.dscomer9Ruta, "dscomer9", variable.rutaGestion, 1),
                    () => LanzaCopia(programa.dscarter, programa.dscarterRuta, "dscarter", variable.rutaGestion, 1),
                    () => LanzaCopia(programa.dsarchi, programa.dsarchiRuta, "dsarchi", variable.rutaPi , 1),
                    () => LanzaCopia(programa.notibase, programa.notibaseRuta, "notibase", variable.rutaPi, 1),
                    () => LanzaCopia(programa.certbase, programa.certbaseRuta, "certbase", variable.rutaPi, 1),
                    () => LanzaCopia(programa.dsesign, programa.dsesignRuta, "dsesign", variable.rutaPi, 1),
                    () => LanzaCopia(programa.dsedespa, programa.dsedespaRuta, "dsedespa", variable.rutaPi, 1),
                    () => LanzaCopia(programa.ipintegr, programa.ipintegrRuta, "ipintegr", variable.rutaPi, 1),
                    () => LanzaCopia(programa.ipbasica, programa.ipbasicaRuta, "ipbasica", variable.rutaPi, 1),
                    () => LanzaCopia(programa.ippatron, programa.ippatronRuta, "ippatron", variable.rutaPi, 1),
                    () => LanzaCopia(programa.gasbase, programa.gasbaseRuta, "gasbase", variable.rutaGasoleos, 1),
                    () => LanzaCopia(programa.dscepsax, programa.dscepsaxRuta, "dscepsax", variable.rutaGasoleos, 1),
                    () => LanzaCopia(programa.dsgalx, programa.dsgalxRuta, "dsgalx", variable.rutaGasoleos, 1),
                    () => LanzaCopia(programa.iplabor2, programa.iplabor2Ruta, "iplabor2", variable.rutaPi, 1)
                };

                //Lanza las copias una a una
                for(int i = 0; i < tareasCopia.Count; i++)
                {
                    await tareasCopia[i]();
                }

                if(resultadoCopia > 0)
                {
                    MessageBox.Show("Copia finalizada", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error al copiar los ficheros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("No ha seleccionado ningun programa", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            tabControl1.Enabled = true; //Vuelve a activar la pestaña cuando acaba la copia
            lbl_porcentaje.Text = string.Empty; //Vacia el label del porcentaje
            progressBar1.Value = 0; //Vacia la barra de progreso
        }

        #endregion


        #region pestaña_ProgramasNoPi
        //Metodos comunes para la pestaña de programas no PI

        //Quita las marcas de los controlBox
        private void LimpiaCbxnoPi()
        {
            foreach(Control control in panelnoPI.Controls)
            {
                if(control is CheckBox cbx)
                {
                    cbx.Checked = false;
                }
            }
        }

        //Metodo para asignar las rutas destino de la copia segun el valor seleccionado en el comboBox
        private void cb_destinonoPI_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(cb_destinonoPI.SelectedIndex)
            {
                case 0:
                    variable.destino = variable.destinonoPi;
                    break;

                case 1:
                    variable.destino = variable.destinoLocal;
                    break;

                case 2:
                    variable.destino = variable.destinoPasesnoPi;
                    break;
            }
        }

        //Metodo para desmarcar todos los checkBox y limpiar el textoBox del resultado de la copia
        private void btn_limpiarnoPI_Click(object sender, EventArgs e)
        {
            txtProgresoCopianoPI.Text = string.Empty;
            foreach(Control control in panelnoPI.Controls)
            {
                if(control is CheckBox cbx)
                {
                    cbx.Checked = false;
                }
            }
        }

        //Metodo para lanzar la copia desde la pestaña de programas no PI (oculta por defecto)
        private async void btnCopiarnoPI_Click(object sender, EventArgs e)
        {
            tabControl1.Enabled = false; //Desactiva el tabControl para evitar que se cambie de pestaña mientras se hace la copia
            resultadoCopia = 0; //Control para si se ha hecho alguna copia correctamente
            int controlCbx = 0;//Controla si hay algun checbox marcado para hacer la copia

            foreach(Control control in panelnoPI.Controls) //Reccore los controles para ver si hay alguno marcado
            {
                if(control is CheckBox cbx && cbx.Checked)
                {
                    controlCbx++;
                }
            }

            if(controlCbx > 0) //En caso de que haya alguno marcado, prepara la copia
            {
                //Crea una lista de las tareas de copia a realizar
                List<Func<Task>> tareasCopia = new List<Func<Task>>
                {
                    //Lanza el proceso de copia segun los checkBox marcados en los programas
                    //Programas noPI
                    () => LanzaCopia(programa.star308, programa.star308Ruta, "star308", variable.rutanoPi ,2),
                    () => LanzaCopia(programa.starpat, programa.starpatRuta, "starpat", variable.rutanoPi, 2),
                    () => LanzaCopia(programa.ereo, programa.ereoRuta, "ereo", variable.rutanoPi, 2),
                    () => LanzaCopia(programa.esocieda, programa.esociedaRuta, "esocieda", variable.rutanoPi, 2),
                    () => LanzaCopia(programa.efacges, programa.efacgesRuta, "efacges", variable.rutanoPi, 2),
                    () => LanzaCopia(programa.eintegra, programa.eintegraRuta, "eintegra", variable.rutanoPi,2),
                    () => LanzaCopia(programa.ereopat, programa.ereopatRuta, "ereopat", variable.rutanoPi,2),
                    () => LanzaCopia(programa.enom1, programa.enom1Ruta, "enom1", variable.rutanoPi,2),
                    () => LanzaCopia(programa.enom2, programa.enom2Ruta, "enom2", variable.rutanoPi,2),
                    () => LanzaCopia(programa.ered, programa.eredRuta, "ered", variable.rutanoPi,2),
                    () => LanzaCopia(programa.enompat, programa.enompatRuta, "enompat", variable.rutanoPi,2),
                    () => LanzaCopia(programa.dscepsa, programa.dscepsaRuta, "dscepsa", variable.rutaGasoleos,2),
                    () => LanzaCopia(programa.dsgal, programa.dsgalRuta, "dsgal", variable.rutaGasoleos, 2)
                };

                //Lanza las copias una a una
                for(int i = 0; i < tareasCopia.Count; i++)
                {
                    await tareasCopia[i]();
                }

                if(resultadoCopia > 0)
                {
                    MessageBox.Show("Copia finalizada", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error al copiar los ficheros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("No ha seleccionado ningun programa", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            tabControl1.Enabled = true; //Vuelve a activar la pestaña cuando acaba la copia
            lbl_porcentaje2.Text = string.Empty; //Vacia el label del porcentaje
            progressBar2.Value = 0; //Vacia la barra de progreso
        }

        #endregion


        #region mouse_over
        //Metodos para cuando el raton pasa por encima de los iconos

        //Boton ruta PI
        private void btnRutaPi_MouseHover(object sender, EventArgs e)
        {
            if(txtRutaPi.Enabled == false)
            {
                btnRutaPi.BackgroundImage = Properties.Resources.editar_hover;
            }
            else
            {
                btnRutaPi.BackgroundImage = Properties.Resources.guardar_noActivo;
            }
        }

        //Boton ruta no PI
        private void btnRutanoPi_MouseHover(object sender, EventArgs e)
        {
            if(txtRutanoPi.Enabled == false)
            {
                btnRutanoPi.BackgroundImage = Properties.Resources.editar_hover;
            }
            else
            {
                btnRutanoPi.BackgroundImage = Properties.Resources.guardar_noActivo;
            }
        }

        //Boton ruta gestion
        private void btnRutaGestion_MouseHover(object sender, EventArgs e)
        {
            if(txtRutaGestion.Enabled == false)
            {
                btnRutaGestion.BackgroundImage = Properties.Resources.editar_hover;
            }
            else
            {
                btnRutaGestion.BackgroundImage = Properties.Resources.guardar_noActivo;
            }
        }

        //Boton ruta gasoleos
        private void btnRutaGasoleos_MouseHover(object sender, EventArgs e)
        {
            if(txtRutaGasoleos.Enabled == false)
            {
                btnRutaGasoleos.BackgroundImage = Properties.Resources.editar_hover;
            }
            else
            {
                btnRutaGasoleos.BackgroundImage = Properties.Resources.guardar_noActivo;
            }
        }

        //Boton destino PI
        private void btnDestinoPi_MouseHover(object sender, EventArgs e)
        {
            if(txtDestinoPi.Enabled == false)
            {
                btnDestinoPi.BackgroundImage = Properties.Resources.editar_hover;
            }
            else
            {
                btnDestinoPi.BackgroundImage = Properties.Resources.guardar_noActivo;
            }
        }

        //Boton destino no PI
        private void btnDestinonoPi_MouseHover(object sender, EventArgs e)
        {
            if(txtDestinonoPi.Enabled == false)
            {
                btnDestinonoPi.BackgroundImage = Properties.Resources.editar_hover;
            }
            else
            {
                btnDestinonoPi.BackgroundImage = Properties.Resources.guardar_noActivo;
            }
        }

        //Boton destino local
        private void btnDestinoLocal_MouseHover(object sender, EventArgs e)
        {
            if(txtDestinoLocal.Enabled == false)
            {
                btnDestinoLocal.BackgroundImage = Properties.Resources.editar_hover;
            }
            else
            {
                btnDestinoLocal.BackgroundImage = Properties.Resources.guardar_noActivo;
            }
        }

        //Boton destino Pases PI
        private void btnDestinoPasesPi_MouseHover(object sender, EventArgs e)
        {
            if(txtDestinoPasesPi.Enabled == false)
            {
                btnDestinoPasesPi.BackgroundImage = Properties.Resources.editar_hover;
            }
            else
            {
                btnDestinoPasesPi.BackgroundImage = Properties.Resources.guardar_noActivo;
            }
        }

        //Boton destino Pases no PI
        private void btnDestinoPasesnoPi_MouseHover(object sender, EventArgs e)
        {
            if(txtDestinoPasesnoPi.Enabled == false)
            {
                btnDestinoPasesnoPi.BackgroundImage = Properties.Resources.editar_hover;
            }
            else
            {
                btnDestinoPasesnoPi.BackgroundImage = Properties.Resources.guardar_noActivo;
            }
        }

        //Boton protocolo
        private void btnProtocolo_MouseHover(object sender, EventArgs e)
        {
            CambiarIconoHover(txtProtocolo, btnProtocolo);
        }

        //Boton hostname
        private void btnHostname_MouseHover(object sender, EventArgs e)
        {
            CambiarIconoHover(txtHostname, btnHostname);
        }

        //Boton username
        private void btnUsername_MouseHover(object sender, EventArgs e)
        {
            CambiarIconoHover(txtUsername, btnUsername);
        }

        //Boton hostkey
        private void btnHostkey_MouseHover(object sender, EventArgs e)
        {
            CambiarIconoHover(txtHostkey, btnHostkey);

        }

        //Boton privatekey
        private void btnPrivatekey_MouseHover(object sender, EventArgs e)
        {
            CambiarIconoHover(txtPrivatekey, btnPrivatekey);
        }

        //Metodo para cambiar el icono del boton al tener el foco
        private void CambiarIconoHover(TextBox txt, Button btn)
        {
            if(!txt.Enabled)
            {
                btn.BackgroundImage = Properties.Resources.editar_hover;
            }
            else
            {
                btn.BackgroundImage = Properties.Resources.guardar_noActivo;
            }
        }

        #endregion


        #region mouse_leave
        //Metodos para cuando el raton sale de los iconos

        //Boton ruta PI
        private void btnRutaPi_MouseLeave(object sender, EventArgs e)
        {
            if(txtRutaPi.Enabled == false)
            {
                btnRutaPi.BackgroundImage = Properties.Resources.editar;
            }
            else
            {
                btnRutaPi.BackgroundImage = Properties.Resources.guardar;
            }
        }

        //Boton ruta no PI
        private void btnRutanoPi_MouseLeave(object sender, EventArgs e)
        {
            if(txtRutanoPi.Enabled == false)
            {
                btnRutanoPi.BackgroundImage = Properties.Resources.editar;
            }
            else
            {
                btnRutanoPi.BackgroundImage = Properties.Resources.guardar;
            }
        }

        //Boton ruta gestion
        private void btnRutaGestion_MouseLeave(object sender, EventArgs e)
        {
            if(txtRutaGestion.Enabled == false)
            {
                btnRutaGestion.BackgroundImage = Properties.Resources.editar;
            }
            else
            {
                btnRutaGestion.BackgroundImage = Properties.Resources.guardar;
            }
        }

        //Boton ruta gasoleos
        private void btnRutaGasoleos_MouseLeave(object sender, EventArgs e)
        {
            if(txtRutaGasoleos.Enabled == false)
            {
                btnRutaGasoleos.BackgroundImage = Properties.Resources.editar;
            }
            else
            {
                btnRutaGasoleos.BackgroundImage = Properties.Resources.guardar;
            }
        }

        //Boton destino PI
        private void btnDestinoPi_MouseLeave(object sender, EventArgs e)
        {
            if(txtDestinoPi.Enabled == false)
            {
                btnDestinoPi.BackgroundImage = Properties.Resources.editar;
            }
            else
            {
                btnDestinoPi.BackgroundImage = Properties.Resources.guardar;
            }
        }

        //Boton destino no PI
        private void btnDestinonoPi_MouseLeave(object sender, EventArgs e)
        {
            if(txtDestinonoPi.Enabled == false)
            {
                btnDestinonoPi.BackgroundImage = Properties.Resources.editar;
            }
            else
            {
                btnDestinonoPi.BackgroundImage = Properties.Resources.guardar;
            }
        }

        //Boton destino local
        private void btnDestinoLocal_MouseLeave(object sender, EventArgs e)
        {
            if(txtDestinoLocal.Enabled == false)
            {
                btnDestinoLocal.BackgroundImage = Properties.Resources.editar;
            }
            else
            {
                btnDestinoLocal.BackgroundImage = Properties.Resources.guardar;
            }
        }

        //Boton destino Pases PI
        private void btnDestinoPasesPi_MouseLeave(object sender, EventArgs e)
        {
            if(txtDestinoPasesPi.Enabled == false)
            {
                btnDestinoPasesPi.BackgroundImage = Properties.Resources.editar;
            }
            else
            {
                btnDestinoPasesPi.BackgroundImage = Properties.Resources.guardar;
            }
        }

        //Boton destino Pases no PI
        private void btnDestinoPasesnoPi_MouseLeave(object sender, EventArgs e)
        {
            if(txtDestinoPasesnoPi.Enabled == false)
            {
                btnDestinoPasesnoPi.BackgroundImage = Properties.Resources.editar;
            }
            else
            {
                btnDestinoPasesnoPi.BackgroundImage = Properties.Resources.guardar;
            }
        }

        //Boton protocolo
        private void btnProtocolo_MouseLeave(object sender, EventArgs e)
        {
            CambiarIconoLeave(txtProtocolo, btnProtocolo);
        }

        //Boton hostname
        private void btnHostname_MouseLeave(object sender, EventArgs e)
        {
            CambiarIconoLeave(txtHostname, btnHostname);
        }

        //Boton username
        private void btnUsername_MouseLeave(object sender, EventArgs e)
        {
            CambiarIconoLeave(txtUsername, btnUsername);
        }

        //Boton hostkey
        private void btnHostkey_MouseLeave(object sender, EventArgs e)
        {
            CambiarIconoLeave(txtHostkey, btnHostkey);
        }

        //Boton privatekey
        private void btnPrivatekey_MouseLeave(object sender, EventArgs e)
        {
            CambiarIconoLeave(txtPrivatekey, btnPrivatekey);
        }

        //Metodo para cambiar el icono del boton al dejar de tener el foco
        private void CambiarIconoLeave(TextBox txt, Button btn)
        {
            if(!txt.Enabled)
            {
                btn.BackgroundImage = Properties.Resources.editar;
            }
            else
            {
                btn.BackgroundImage = Properties.Resources.guardar;
            }
        }


        #endregion


        #region MouseClick 
        //Metodos para cuando se hace clic en los iconos

        //Metodo para guardar la configuracion de la pestaña de rutas
        public void btnGuardarConfiguracion_MouseClick(object sender, MouseEventArgs e)
        {
            //Graba en el fichero de configuracion las variables
            variable.GuardarConfiguracion();
        }

        //Control del boton para editar el contenido del campo de la ruta PI
        private void btnRutaPi_MouseClick(object sender, MouseEventArgs e)
        {
            if(txtRutaPi.Enabled == false) //Si el campo esta desactivado, lo activa, le pone el foco y el cursor  al final del texto, y controla la imagen del fondo del resto de botones
            {
                txtRutaPi.Enabled = true;
                txtRutaPi.Focus();
                txtRutaPi.SelectionStart = txtRutaPi.TextLength;
                gestionBotones(true, "btnRutaPi");
            }
            else //Si el campo esta activado, lo desactiva, guarda el texto en la variable y controla la imagen del fondo del resto de botones
            {
                variable.rutaPi = txtRutaPi.Text;
                txtRutaPi.Enabled = false;
                gestionBotones(false, "btnRutaPi");
            }
        }

        //Boton para editar el contenido del campo de la ruta no PI
        private void btnRutanoPi_MouseClick(object sender, MouseEventArgs e)
        {
            if(txtRutanoPi.Enabled == false) //Si el campo esta desactivado, lo activa, le pone el foco y el cursor  al final del texto, y controla la imagen del fondo del resto de botones
            {
                txtRutanoPi.Enabled = true;
                txtRutanoPi.Focus();
                txtRutanoPi.SelectionStart = txtRutanoPi.TextLength;
                gestionBotones(true, "btnRutanoPi");
            }
            else //Si el campo esta activado, lo desactiva, guarda el texto en la variable y controla la imagen del fondo del resto de botones
            {
                variable.rutanoPi = txtRutanoPi.Text;
                txtRutanoPi.Enabled = false;
                gestionBotones(false, "btnRutanoPi");
            }
        }

        //Boton para editar el contenido del campo de la ruta gestion
        private void btnRutaGestion_MouseClick(object sender, MouseEventArgs e)
        {
            if(txtRutaGestion.Enabled == false) //Si el campo esta desactivado, lo activa, le pone el foco y el cursor  al final del texto, y controla la imagen del fondo del resto de botones
            {
                txtRutaGestion.Enabled = true;
                txtRutaGestion.Focus();
                txtRutaGestion.SelectionStart = txtRutaGestion.TextLength;
                gestionBotones(true, "btnRutaGestion");
            }
            else //Si el campo esta activado, lo desactiva, guarda el texto en la variable y controla la imagen del fondo del resto de botones
            {
                variable.rutaGestion = txtRutaGestion.Text;
                txtRutaGestion.Enabled = false;
                gestionBotones(false, "btnRutaGestion");
            }
        }

        //Boton para editar el contenido del campo de la ruta gasoleos
        private void btnRutaGasoleos_MouseClick(object sender, MouseEventArgs e)
        {
            if(txtRutaGasoleos.Enabled == false) //Si el campo esta desactivado, lo activa, le pone el foco y el cursor  al final del texto, y controla la imagen del fondo del resto de botones
            {
                txtRutaGasoleos.Enabled = true;
                txtRutaGasoleos.Focus();
                txtRutaGasoleos.SelectionStart = txtRutaGasoleos.TextLength;
                gestionBotones(true, "btnRutaGasoleos");
            }
            else //Si el campo esta activado, lo desactiva, guarda el texto en la variable y controla la imagen del fondo del resto de botones
            {
                variable.rutaGasoleos = txtRutaPi.Text;
                txtRutaGasoleos.Enabled = false;
                gestionBotones(false, "btnRutaGasoleos");
            }
        }

        //Boton para editar el contenido del campo de la ruta destino PI
        private void btnDestinoPi_MouseClick(object sender, MouseEventArgs e)
        {
            if(txtDestinoPi.Enabled == false) //Si el campo esta desactivado, lo activa, le pone el foco y el cursor  al final del texto, y controla la imagen del fondo del resto de botones
            {
                btnGuardarConfiguracion.Enabled = false;
                txtDestinoPi.Enabled = true;
                txtDestinoPi.Focus();
                txtDestinoPi.SelectionStart = txtDestinoPi.TextLength;
                gestionBotones(true, "btnDestinoPi");
            }
            else //Si el campo esta activado, lo desactiva, guarda el texto en la variable y controla la imagen del fondo del resto de botones
            {
                variable.destinoPi = txtDestinoPi.Text;
                txtDestinoPi.Enabled = false;
                gestionBotones(false, "btnDestinoPi");
            }
        }

        //Boton para editar el contenido del campo de la ruta destino no PI
        private void btnDestinonoPi_MouseClick(object sender, MouseEventArgs e)
        {
            if(txtDestinonoPi.Enabled == false) //Si el campo esta desactivado, lo activa, le pone el foco y el cursor  al final del texto, y controla la imagen del fondo del resto de botones
            {
                txtDestinonoPi.Enabled = true;
                txtDestinonoPi.Focus();
                txtDestinonoPi.SelectionStart = txtDestinonoPi.TextLength;
                gestionBotones(true, "btnDestinonoPi");
            }
            else //Si el campo esta activado, lo desactiva, guarda el texto en la variable y controla la imagen del fondo del resto de botones
            {
                variable.destinonoPi = txtDestinonoPi.Text;
                txtDestinonoPi.Enabled = false;
                gestionBotones(false, "btnDestinonoPi");
            }
        }

        //Boton para editar el contenido del campo de la ruta destino local
        private void btnDestinoLocal_MouseClick(object sender, MouseEventArgs e)
        {
            if(txtDestinoLocal.Enabled == false) //Si el campo esta desactivado, lo activa, le pone el foco y el cursor  al final del texto, y controla la imagen del fondo del resto de botones
            {
                txtDestinoLocal.Enabled = true;
                txtDestinoLocal.Focus();
                txtDestinoLocal.SelectionStart = txtDestinoLocal.TextLength;
                gestionBotones(true, "btnDestinoLocal");
            }
            else //Si el campo esta activado, lo desactiva, guarda el texto en la variable y controla la imagen del fondo del resto de botones
            {
                variable.destinoLocal = txtDestinoLocal.Text;
                txtDestinoLocal.Enabled = false;
                gestionBotones(false, "btnDestinoLocal");
            }
        }

        //Boton para editar el contenido del campo de la ruta destino Pases PI
        private void btnDestinoPasesPi_MouseClick(object sender, MouseEventArgs e)
        {
            if(txtDestinoPasesPi.Enabled == false) //Si el campo esta desactivado, lo activa, le pone el foco y el cursor  al final del texto, y controla la imagen del fondo del resto de botones
            {
                txtDestinoPasesPi.Enabled = true;
                txtDestinoPasesPi.Focus();
                txtDestinoPasesPi.SelectionStart = txtDestinoPasesPi.TextLength;
                gestionBotones(true, "btnDestinoPasesPi");

            }
            else //Si el campo esta activado, lo desactiva, guarda el texto en la variable y controla la imagen del fondo del resto de botones
            {
                variable.destinoPasesPi = txtDestinoPasesPi.Text;
                txtDestinoPasesPi.Enabled = false;
                gestionBotones(false, "btnDestinoPasesPi");
            }
        }

        //Boton para editar el contenido del campo de la ruta destino Pases no PI
        private void btnDestinoPasesnoPi_MouseClick(object sender, MouseEventArgs e)
        {
            if(txtDestinoPasesnoPi.Enabled == false) //Si el campo esta desactivado, lo activa, le pone el foco y el cursor  al final del texto, y controla la imagen del fondo del resto de botones
            {
                txtDestinoPasesnoPi.Enabled = true;
                txtDestinoPasesnoPi.Focus();
                txtDestinoPasesnoPi.SelectionStart = txtDestinoPasesnoPi.TextLength;
                gestionBotones(true, "btnDestinoPasesnoPi");
            }
            else //Si el campo esta activado, lo desactiva, guarda el texto en la variable y controla la imagen del fondo del resto de botones
            {
                variable.destinoPasesnoPi = txtDestinoPasesnoPi.Text;
                txtDestinoPasesnoPi.Enabled = false;
                gestionBotones(false, "btnDestinoPasesnoPi");
            }
        }

        //Metodo para editar el contenido del campo de la ruta del protocolo
        private void btnProtocolo_MouseClick(object sender, MouseEventArgs e)
        {
            PulsarBoton(txtProtocolo, btnProtocolo, "Protocolo");
        }

        //Metodo para editar el contenido del campo del hostname
        private void btnHostname_MouseClick(object sender, MouseEventArgs e)
        {
            PulsarBoton(txtHostname, btnHostname, "HostName");
        }

        //Metodo para editar el contenido del campo del username
        private void btnUsername_MouseClick(object sender, MouseEventArgs e)
        {
            PulsarBoton(txtUsername, btnUsername, "UserName");
        }

        //Metodo para editar el contenido del campo del hostkey
        private void btnHostkey_MouseClick(object sender, MouseEventArgs e)
        {
            PulsarBoton(txtHostkey, btnHostkey, "HostKey");
        }

        //Metodo para editar el contenido del campo del Privatekey
        private void btnPrivatekey_MouseClick(object sender, MouseEventArgs e)
        {
            PulsarBoton(txtPrivatekey, btnPrivatekey, "PrivateKey");
        }

        //Metodo para cambiar el icono de los botones al hacer click en la pestaña de configuracion WinSCP
        private void PulsarBoton(TextBox txt, Button btn, string nombreVariable)
        {
            if(!txt.Enabled) //Si el campo esta desactivado, desactiva el boton de guardar y activa el campo de texto, le pone el foco y el cursor  al final del texto, y controla la imagen del fondo del resto de botones
            {
                btnGuardarWinscp.Enabled = false;
                txt.Enabled = true;
                txt.Focus();
                txt.SelectionStart = txt.TextLength;
                gestionBotones(true, btn.Name, "tabWinscp");

            }
            else //Si el campo esta activado, busca la propiedad de la variable que corresponde al nombre del campo, y guarda el texto en la propiedad de la variable, desactiva el campo de texto y controla la imagen del fondo del resto de botones
            {
                var propiedad = typeof(variables).GetProperty(nombreVariable);
                // Si la propiedad es del tipo WinSCP.Protocol, se convierte el texto
                if(propiedad.PropertyType == typeof(WinSCP.Protocol))
                {
                    if(Enum.TryParse(txt.Text, true, out WinSCP.Protocol protocolo))
                    {
                        propiedad.SetValue(variable, protocolo);
                    }
                }
                else
                {
                    propiedad.SetValue(variable, txt.Text);
                }
                txt.Enabled = false;
                gestionBotones(false, txt.Name, "tabWinscp");
            }
        }

        //Metodo para cambiar los iconos del resto de los botones cuando se pulsa alguno
        private void gestionBotones(bool estado, string nombreBoton, string pestaña = "tabConfiguracion") //Para no cambiar el codigo se pone por defecto la pestaña de configuracion de rutas, y si se quiere hacer con la pestaña de configuracion WinScp se pasa como parametro desde la llamada
        {
            foreach(Control control in tabControl1.TabPages[pestaña].Controls) //Recorre todos los controles de la pestaña
            {
                if(control is Button btn) //Si el control es un boton
                {
                    if(estado) //Si esta activo el boton
                    {
                        if(btn.Name == nombreBoton) //Comprueba si el nombre pasado es el del boton y modifica la imagen del fondo
                        {
                            btn.BackgroundImage = Properties.Resources.guardar;
                        }
                        else
                        {
                            btn.Enabled = false; //En caso de que el boton no sea el que se ha pulsado lo desactiva y le cambia la imagen del fondo
                            if(btn.Name != btnGuardarConfiguracion.Name)
                            {
                                btn.BackgroundImage = Properties.Resources.editar_noActivo;
                            }
                        }
                    }
                    else //Si el boton no esta activo
                    {
                        if(btn.Name == nombreBoton) //Comprueba si el nombre pasado es el del boton y modifica la imagen del fondo
                        {
                            btn.BackgroundImage = Properties.Resources.editar;
                        }
                        else //En caso de que el boton no sea el que se ha pulsado lo activa y le cambia la imagen del fondo
                        {
                            btn.Enabled = true;
                            if(btn.Name != btnGuardarConfiguracion.Name)
                            {
                                btn.BackgroundImage = Properties.Resources.editar;
                            }
                        }
                    }
                }
            }
        }

        //Metodo para guardar la configuracion de WinSCP
        private void btnGuardarWinscp_MouseClick(object sender, MouseEventArgs e)
        {
            //Graba en el fichero de configuracion las variables
            variable.GuardarConfiguracion();
        }

        #endregion


        #region pestaña_configuracion

        //Metodo para inicializar los campos cuando se da de alta un nuevo fichero
        private void btnAddFic_Click(object sender, EventArgs e)
        {
            fichero.opcion = 'A';
            estadoBotones(false); //Desactiva los botones
            txtNombreFichero.Text = string.Empty; //Vacia el campo del nombre del fichero
            txtNombreFichero.Enabled = true; //Activa el campo del nombre del fichero
            txtRutaFichero.Text = string.Empty; //Vacia el campo de la ruta del fichero
            txtRutaFichero.Enabled = true; //Activa el campo de la ruta del fichero
            txtTipoFichero.Text = string.Empty; //Vacia el campo del tipo de fichero
            txtTipoFichero.Visible = false; //Oculta el campo del tipo de fichero
            cbTipo.Visible = true; //Muestra el comboBox que tiene los tipos de fichero
            cbTipo.SelectedIndex = 0; //Selecciona el primer elemento del tipo de fichero
            cbClaseFichero.SelectedIndex = 0; //Selecciona el primer elemento de la clase de fichero
            cbClaseFichero.Enabled = true; //Activa el comboBox de la clase de fichero
            txtNombreFichero.Focus(); //Pone el foco en el campo del nombre del fichero
        }

        //Metodo para modificar los campos del fichero seleccionado
        private void btnModificarFich_Click(object sender, EventArgs e)
        {
            // Verificar si hay algún elemento seleccionado
            if(lstFicheros.SelectedItems.Count > 0)
            {
                fichero.opcion = 'M'; //Opcion de modificacion
                estadoBotones(false); //Descativa los botones
                txtRutaFichero.Enabled = true; //Activa el campo de la ruta
                cbTipo.Visible = true; //Muestra el comboBox que tiene los tipos de fichero
                cbClaseFichero.Enabled = true; //Activa el comboBox de la clase de fichero
                txtRutaFichero.Focus(); //Pone el foco en el campo de la ruta
            }
        }

        //Metodo para eliminar un fichero de la lista de ficheros
        private void btnEliminarFich_Click(object sender, EventArgs e)
        {
            if(lstFicheros.SelectedItems.Count > 0) //Controla que haya algun fichero seleccionado, y carga el nombre del fichero para luego borrarlo de la lista
            {
                string nombre = txtNombreFichero.Text;
                fichero.opcion = 'B';
                DialogResult resultado = MessageBox.Show($"Quieres eliminar el registro {nombre}?", "Aviso", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if(resultado == DialogResult.OK)
                {
                    fichero.eliminarFichero(nombre);
                    fichero.grabarFicheros();
                    actualizaListaFicheros(lstFicheros);
                    txtNombreFichero.Text = string.Empty;
                    txtRutaFichero.Text = string.Empty;
                    txtTipoFichero.Text = string.Empty;
                }
            }
        }

        //Metodo para cancelar la edicion de un fichero
        private void btnCancelarFich_Click(object sender, EventArgs e)
        {
            estadoBotones(true); //Activa los botones
            txtNombreFichero.Text = string.Empty; //Vacia el campo del nombre del fichero
            txtNombreFichero.Enabled = false; //Desactiva el campo del nombre del fichero
            txtRutaFichero.Text = string.Empty; //Vacia el campo de la ruta del fichero
            txtRutaFichero.Enabled = false; //Desactiva el campo de la ruta del fichero
            txtTipoFichero.Text = string.Empty; //Vacia el campo del tipo de fichero
            txtTipoFichero.Visible = true; //Muestra el campo del tipo de fichero
            cbTipo.SelectedIndex = 0; //Selecciona el primer elemento del tipo de fichero
            cbTipo.Visible = false; //Oculta el comboBox que tiene los tipos de fichero
            cbClaseFichero.SelectedIndex = 0; //Selecciona el primer elemento de la clase de fichero
            cbClaseFichero.Enabled = false; //Desactiva el comboBox de la clase de fichero
        }

        //Metodo para validar el fichero seleccionado
        private void btnValidarFich_Click(object sender, EventArgs e)
        {
            //Carga las variables con los valores de los campos
            string nombre = txtNombreFichero.Text;
            string ruta = txtRutaFichero.Text;
            string tipo = txtTipoFichero.Text;
            int clase = cbClaseFichero.SelectedIndex; //Selecciona la clase de fichero

            //Comprueba que los campos no esten vacios
            if(!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(ruta) && !string.IsNullOrEmpty(tipo) && clase > 0)
            {
                //Actua de un modo u otro segun si es Alta o Modificacion
                switch(fichero.opcion)
                {
                    case 'A':
                        fichero.AgregarFichero(nombre, ruta, tipo, clase); //En caso de alta, añade el fichero a la lista
                        break;

                    case 'M':
                        fichero.modificarFichero(nombre, ruta, tipo, clase); //En caso de modificacion, modifica el fichero en la lista
                        break;
                }
                fichero.grabarFicheros(); //Graba el fichero en el archivo de configuracion
            }
            else
            {
                MessageBox.Show("Debes rellenar todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            estadoBotones(true); //Activa los botones
            txtNombreFichero.Text = string.Empty; //Vacia el campo del nombre del fichero
            txtNombreFichero.Enabled = false; //Desactiva el campo del nombre del fichero
            txtRutaFichero.Text = string.Empty; //Vacia el campo de la ruta del fichero
            txtRutaFichero.Enabled = false; //Desactiva el campo de la ruta del fichero
            txtTipoFichero.Text = string.Empty; //Vacia el campo del tipo de fichero
            txtTipoFichero.Enabled = false; //Desactiva el campo del tipo de fichero
            txtTipoFichero.Visible = true; //Muestra el campo del tipo de fichero
            cbTipo.TabIndex = 0; //Selecciona el primer elemento del tipo de fichero
            cbTipo.Visible = false; //Oculta el comboBox que tiene los tipos de fichero
            cbClaseFichero.SelectedIndex = 0; //Selecciona el primer elemento de la clase de fichero
            cbClaseFichero.Enabled = false; //Desactiva el comboBox de la clase de fichero
            actualizaListaFicheros(lstFicheros); //Actualiza la lista de ficheros
        }

        //Metodo para modificar el estado de los botones
        private void estadoBotones(bool estado)
        {
            btnAddFich.Visible = estado;
            btnModificarFich.Visible = estado;
            btnEliminarFich.Visible = estado;
            btnCancelarFich.Visible = !estado;
            btnValidarFich.Visible = !estado;
            lstFicheros.Enabled = estado;
        }

        //Metodo para cargar el tipo de fichero en el comboBox
        private void cbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string textoSeleccionado = cbTipo.Text;
            txtTipoFichero.Text = textoSeleccionado;
        }

        //Metodo para ordenar la lista de ficheros segun la columna que se haya pulsado
        private void lstFicheros_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Cambiar la dirección de ordenamiento si es la misma columna que la última vez que se hizo clic
            if(lstFicheros.Sorting == SortOrder.Ascending)
                lstFicheros.Sorting = SortOrder.Descending;
            else
                lstFicheros.Sorting = SortOrder.Ascending;

            // Establecer el comparador de elementos del ListView
            lstFicheros.ListViewItemSorter = new ListViewItemComparer(e.Column, lstFicheros.Sorting);

            // Ordenar el ListView basado en la columna en la que se hizo clic
            lstFicheros.Sort();

        }

        //Metodo para cargar los datos del fichero seleccionado en los campos de texto
        private void lstFicheros_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Verificar si hay algún elemento seleccionado
            if(lstFicheros.SelectedItems.Count > 0)
            {
                // Obtener el primer elemento seleccionado
                ListViewItem selectedItem = lstFicheros.SelectedItems[0];

                // Obtener el nombre y la ruta del fichero desde los subelementos del ListViewItem
                string nombreFichero = selectedItem.SubItems[0].Text;
                string rutaFichero = selectedItem.SubItems[1].Text;
                string tipoFichero = selectedItem.SubItems[2].Text;
                int claseFichero = Convert.ToInt32(selectedItem.SubItems[3].Text);

                // Mostrar el nombre y la ruta del fichero en los TextBox correspondientes
                txtNombreFichero.Text = nombreFichero;
                txtRutaFichero.Text = rutaFichero;
                txtTipoFichero.Text = tipoFichero;
                int indice = cbTipo.FindStringExact(tipoFichero);
                if(indice != -1)
                {
                    cbTipo.SelectedIndex = indice;
                }
                cbClaseFichero.SelectedIndex = claseFichero;
            }
            else
            {
                // Limpiar los TextBox si no hay ningún elemento seleccionado en el ListBox
                txtNombreFichero.Text = string.Empty;
                txtRutaFichero.Text = string.Empty;
                txtTipoFichero.Text = string.Empty;
                cbClaseFichero.SelectedIndex = 0;
            }
        }

        //Boton para ocultar o mostrar las pestañas de Programas PI y Programas no PI
        private void button3_Click(object sender, EventArgs e)
        {
            activarPestañas();
        }


        #endregion


        #region pestaña_ProcesoCopias

        //Metodo para quitar las marcas de los ficheros seleccionados
        private void btnLimpiarCopia_Click(object sender, EventArgs e)
        {
            txtDestinoCopias.Text = string.Empty;
            foreach(ListViewItem item in lstFicherosOrigen.CheckedItems)
            {
                item.Checked = false;
            }
        }

        //Metodo para lanzar la copia de todos los ficheros seleccionados
        private async void btnCopiarCopias_Click(object sender, EventArgs e)
        {
            //Inicializa el informe de copias
            informeCopia.Clear();

            //Evita que se pueda hacer la copia si no hay ningun programa seleccionado
            if(lstFicherosOrigen.CheckedItems.Count > 0)
            {
                tabControl1.Enabled = false; //Desactiva la pestaña de configuracion para evitar que se pueda cambiar nada mientras se hace la copia
                resultadoCopia = 0; //Controla si se ha copiado algun fichero
                List<Func<Task>> tareasCopia = new List<Func<Task>>(); //Crea una lista de tareas para hacer la copia

                Stopwatch tiempoTotal = Stopwatch.StartNew(); //Inicializa el contador de tiempo total de copia
                foreach(ListViewItem item in lstFicherosOrigen.CheckedItems) //Recorre los ficheros seleccionados para hacer la copia
                {
                    //Asinga los valores de los subitems a las variables
                    string nombre = item.SubItems[0].Text;
                    string fichero = item.SubItems[1].Text;
                    string rutaOrigen = string.Empty;

                    //Asigna la ruta de origen segun la clase de fichero
                    int claseFichero = Convert.ToInt32(item.SubItems[3].Text);
                    switch(claseFichero)
                    {
                        case 1:
                            rutaOrigen = variable.rutaPi;
                            break;

                        case 2:
                            rutaOrigen = variable.rutanoPi;
                            break;

                        case 3:
                            rutaOrigen = variable.rutaGestion;
                            break;

                        case 4:
                            rutaOrigen = variable.rutaGasoleos;
                            break;
                    }

                    //Añade a la lista de tareas la copia del fichero
                    tareasCopia.Add(() => hacerCopia(nombre, fichero, rutaOrigen));
                }


                //Lanza las copias una a una
                Stopwatch tiempoAplicacion = Stopwatch.StartNew(); //Inicializa el contador de tiempo para cada fichero
                for(int i = 0; i < tareasCopia.Count; i++) //
                {
                    await tareasCopia[i](); //Va haciendo la copia de los ficheros uno a uno
                }
                tiempoTotal.Stop(); //Para el contador de tiempo total de copia

                //En caso de haber realizado alguna copia, muestra el mensaje de finalizacion y actualiza el registro de copias
                if(resultadoCopia > 0)
                {
                    int pestaña = 3; //Pestaña de copias para mostrar el mensaje
                    string tiempo = convierteTiempo((int)tiempoTotal.Elapsed.TotalSeconds);
                    ActualizarProgreso($"Total tiempo de copia: {tiempo}" + Environment.NewLine, pestaña);
                    MessageBox.Show("Copia finalizada", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ////Actualiza el informe de copias (desactivado porque ahora se graba un registro de copias)
                    //actualizaInformeCopia($"Total tiempo de copia: {tiempo}" + Environment.NewLine);

                    //if(informeCopia.Length > 0)
                    //{
                    //    //Graba el informe de copias
                    //    File.AppendAllText("logCopias.txt", informeCopia.ToString());
                    //}

                    //Genera el informe de copias
                    RegistroCopia registroCopia = new RegistroCopia //Nueva instancia para grabar los datos de la copia
                    {
                        FechaCopia = DateTime.Now.ToString("'Dia:' dd.MM.yyyy '- Hora:' HH:mm"), //Fecha de la copia
                        ProgramasCopiados = programasCopiados, //Lista de programas copiados
                        TiempoTotalCopia = $"{tiempo}" //Tiempo total de la copia
                    };
                    //Graba el informe de copias
                    RegistroCopia.GuardarRegistroCopia(registroCopia, PathRegistroCopias);
                }
                else
                {
                    MessageBox.Show("Error al copiar los ficheros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("No hay ningun programa seleccionado. Seleccione alguno", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //Reinicia la barra de progreso y habilita la pestaña de configuracion
            tabControl1.Enabled = true;
            progressBar3.Value = 0;

            btnLimpiarCopia_Click(sender, e); //Llama al metodo para quitar las marcas de los ficheros seleccionados
        }

        //Metodo para asignar las rutas destino de la copia segun el valor seleccionado en el comboBox
        private void cbDestinoCopias_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(cbDestinoCopias.SelectedIndex)
            {
                case 0:
                    variable.destino = variable.destinoPi;
                    break;

                case 1:
                    variable.destino = variable.destinoLocal;
                    break;

                case 2:
                    variable.destino = variable.destinoPasesPi;
                    break;
            }
        }

        //Metodo que lanza la copia de cada fichero
        private async Task hacerCopia(string nombre, string fichero, string rutaOrigen)
        {
            int pestaña = 3; //Pestaña de copias para mostrar el mensaje
            string origen = rutaOrigen + fichero;
            string titulo = nombre;

            string nombreFichero = Path.GetFileName(fichero); //Obtiene el nombre del programa
            string destino = variable.destino + nombreFichero; //Forma la ruta completa del programa

            //Inicio del control de tiempo de copia
            Stopwatch tiempoAplicacion = Stopwatch.StartNew();

            //Controla si se hace la copia local o en el geco72
            if(variable.destino == variable.destinoLocal) //Copia local
            {
                try
                {
                    ActualizarProgreso($"Copiando el programa {titulo}", pestaña);
                    await Task.Run(() =>
                    {
                        try
                        {
                            File.Copy(origen, destino, true); //Copia directamente
                            ActualizarProgreso($"Programa {titulo} copiado correctamente.", pestaña);
                            tiempoAplicacion.Stop(); //Para el tiempo de copia de la aplicacion
                            string tiempo = convierteTiempo((int)tiempoAplicacion.Elapsed.TotalSeconds);
                            ActualizarProgreso($"Duracion de la copia: {tiempo}" + Environment.NewLine, pestaña);
                            resultadoCopia++; //Actualiza el contador de copias correctas

                            ////Actualiza el informe de copias (desactivado porque ahora se hace el registro de copias
                            //actualizaInformeCopia($"Copiado {titulo} a {variable.destino}");

                            //Añade el programa copiado a la lista de programas copiados
                            programasCopiados.Add(new RegistroCopia.ProgramaCopiado //Nueva instancia para grabar los datos de la copia
                            {
                                Programa = nombreFichero, //Nombre del programa
                                RutaDestino = variable.destino, //Ruta de destino
                            });
                        }

                        catch(Exception ex)
                        {
                            ActualizarProgreso(Environment.NewLine + $"Error al copiar el programa {titulo}" + Environment.NewLine + ex.Message, pestaña);
                        }
                    }).ConfigureAwait(false);

                }

                catch(Exception ex)
                {
                    ActualizarProgreso(Environment.NewLine + $"Error al copiar el programa {titulo}" + Environment.NewLine + ex.Message, pestaña);
                }
            }
            else //Si la copia es al geco72
            {
                try
                {
                    ActualizarProgreso($"Copiando el programa {titulo}", pestaña);

                    // Configuración de opciones de sesión para la copia al geco72
                    SessionOptions opcionesSesion = new SessionOptions
                    {
                        Protocol = variable.Protocolo,
                        HostName = variable.HostName,
                        UserName = variable.UserName,
                        SshHostKeyFingerprint = variable.HostKey,
                        SshPrivateKeyPath = variable.PrivateKey,
                    };
                    opcionesSesion.AddRawSettings("AgentFwd", "1");

                    Session session = null;

                    try
                    {
                        await Task.Run(async () =>
                        {
                            ////Permite hacer un log del resultado. La dejo comentada por si hace falta
                            //string logFile = @"c:\carlos\winscp.log";
                            //if (!File.Exists(logFile))
                            //{
                            //    File.Create(logFile).Close();
                            //}
                            //else
                            //{
                            //    File.Delete(logFile);
                            //    File.Create(logFile).Close();
                            //}
                            //session.SessionLogPath = logFile;

                            // Conexión
                            session = new Session();

                            //Permite controlar el progreso de copia
                            session.FileTransferProgress += (sender, e) =>
                            {
                                //Actualiza la barra de progreso de copia
                                progressBar3.Invoke((MethodInvoker)(() =>
                                {
                                    progressBar3.Value = (int)(e.OverallProgress * 100);
                                    //Muestra el porcentaje completado
                                    int porcentaje = (int)(e.OverallProgress * 100);
                                    lbl_porcentaje.Text = $"{porcentaje}%";
                                }));
                            };

                            ActualizarProgreso($"Conectando con el servidor . . .", pestaña);
                            session.Open(opcionesSesion);

                            TransferOptions transferOptions = new TransferOptions();
                            transferOptions.TransferMode = TransferMode.Binary;

                            ActualizarProgreso($"Iniciando copia . . .", pestaña);
                            TransferOperationResult transferResult = session.PutFiles(origen, variable.destino, false, transferOptions);
                            transferResult.Check();

                            // Muestra información sobre la transferencia al finalizar
                            ActualizarProgreso($"Programa {titulo} copiado correctamente.", pestaña);
                            resultadoCopia++; //Actualiza el contador de copias correctas

                            ////Actualiza el informe de copias (desactivado porque ahora se graba un registro de copias)
                            //actualizaInformeCopia($"Copiado {titulo} a {variable.destino}");

                            //Añade el programa copiado a la lista de programas copiados
                            programasCopiados.Add(new RegistroCopia.ProgramaCopiado
                            {
                                Programa = titulo,
                                RutaDestino = variable.destino,
                            });

                            //Control del tiempo de copia
                            tiempoAplicacion.Stop();
                            string tiempo = convierteTiempo((int)tiempoAplicacion.Elapsed.TotalSeconds);
                            ActualizarProgreso($"Duracion de la copia: {tiempo}" + Environment.NewLine, pestaña);

                        }).ConfigureAwait(false);
                    }

                    catch(Exception ex)
                    {
                        ActualizarProgreso(Environment.NewLine + $"Error al copiar el programa {titulo}" + Environment.NewLine + ex.Message + Environment.NewLine, pestaña);
                    }

                    finally
                    {
                        //Libera el recurso de la sesion
                        session?.Dispose();
                    }
                }
                catch(Exception ex)
                {
                    ActualizarProgreso(Environment.NewLine + $"No se ha podido copiar el programa {titulo}" + Environment.NewLine + ex.Message, pestaña);
                }
                finally
                {
                    tiempoAplicacion.Stop();
                }
            }

        }

        #endregion


        #region utilidades

        //Metodo para quitar o poner las pestañas de programas PI y no PI
        private void activarPestañas()
        {
            if(controlTab)
            {
                tabControl1.TabPages.Remove(tabPI);
                tabControl1.TabPages.Remove(tabNopi);
                controlTab = false;
            }
            else
            {
                tabControl1.TabPages.Add(tabPI);
                tabControl1.TabPages.Add(tabNopi);
                controlTab = true;
            }
        }

        //Metodo para devolver un string con el tiempo de copia formateado
        private string convierteTiempo(int tiempo)
        {
            string tiempoTotal = string.Empty;
            int minutos = tiempo / 60;
            int segundos = tiempo % 60;
            if(minutos > 0)
            {
                tiempoTotal += $"{minutos} minutos ";
            }
            tiempoTotal += $"{segundos} segundos";
            return tiempoTotal;
        }

        //Metodo para actualizar el informe de copias (desactivado porque ahora se graba un registro de copias)
        private void actualizaInformeCopia(string mensaje)
        {
            if(informeCopia.Length == 0)
            {
                informeCopia.AppendLine(new string('#', 50));
                informeCopia.AppendLine($"Fecha copia: {DateTime.Now}");
            }

            informeCopia.AppendLine(mensaje);

        }

        //Evento al modificar una fecha en el calendario
        private void mcFiltroFecha_DateChanged(object sender, DateRangeEventArgs e)
        {
            DateTime fechaSeleccionada = e.Start.Date;
            FiltrarCopiasPorFecha(fechaSeleccionada);
        }

        //Evento para filtrar por un rango de fechas
        private void mcFiltroFecha_DateSelected(object sender, DateRangeEventArgs e)
        {
            DateTime fechaInicioSeleccion = e.Start.Date;
            DateTime fechaFinSeleccion = e.End.Date;
            // Filtrar por rango de fechas
            var copiasFiltradas = RegistroCopia.ListadoCopias
                .Where(c => c.fecha.Date >= fechaInicioSeleccion && c.fecha.Date <= fechaFinSeleccion)
                .ToList();
            // Si no hay copias para ese rango
            if(!copiasFiltradas.Any())
            {
                rbCopias.Clear();
                rbCopias.AppendText($"\n    No se encontraron copias para este rango de fechas.");
            }
            else
            {
                //Llama al metodo para mostrar la lista de copias filtrada
                MostrarListaCopias(copiasFiltradas);
            }

        }

        //Evento para quitar el filtro de fecha
        private void btnBorrarFiltro_Click(object sender, EventArgs e)
        {
            MostrarListaCopias(RegistroCopia.ListadoCopias);
        }

        //Metodo para filtrar el registro de copias por fecha
        private void FiltrarCopiasPorFecha(DateTime fechaSeleccionada)
        {
            //Carga la lista con el registro de copias leidas
            var listaCopias = RegistroCopia.ListadoCopias;

            //listaCopias = RegistroCopia.OrdenarCopiasLeidas(listaCopias);

            // Filtrar por fecha exacta (ignorando hora)
            var copiasFiltradas = listaCopias
                .Where(c => c.fecha.Date == fechaSeleccionada)
                .ToList();


            // Si no hay copias para esa fecha
            if(!copiasFiltradas.Any())
            {
                rbCopias.Clear();
                rbCopias.AppendText($"\n    No se encontraron copias para esta fecha.");
            }
            else
            {
                //Llama al metodo para mostrar la lista de copias filtrada
                MostrarListaCopias(copiasFiltradas);
            }
        }


        #endregion

        private void brnBorrarCopias_Click(object sender, EventArgs e)
        {
            //Comprueba si hay alguna fecha seleccionada
            DateTime fechaInicioSeleccion = mcFiltroFecha.SelectionRange.Start.Date;
            DateTime fechaFinSeleccion = mcFiltroFecha.SelectionRange.End.Date;

            //Filtra por la fecha seleccionada
            var registrosSeleccionados = RegistroCopia.ListadoCopias
                .Where(c => c.fecha.Date >= fechaInicioSeleccion && c.fecha.Date <= fechaFinSeleccion)
                .ToList();

            bool borrarPorFecha = registrosSeleccionados.Any();

            // Confirmación previa al borrado
            string rangoFechas = fechaFinSeleccion > fechaInicioSeleccion ? $"de los dias {fechaInicioSeleccion:dd.MM.yyyy} hasta {fechaFinSeleccion:dd.MM.yyyy}?" : $"del día {fechaInicioSeleccion:dd.MM.yyyy}?";
            DialogResult confirmacion = MessageBox.Show(
                borrarPorFecha
                    ? $"¿Deseas eliminar {registrosSeleccionados.Count} registro(s) {rangoFechas}?"
                    : "¿Deseas eliminar TODOS los registros?\nEsta acción requiere contraseña.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if(confirmacion != DialogResult.Yes)
                return;

            if(borrarPorFecha)
            {
                // Eliminar solo los del día seleccionado
                RegistroCopia.ListadoCopias = RegistroCopia.ListadoCopias
                    .Where(r => r.fecha.Date < fechaInicioSeleccion || r.fecha.Date > fechaFinSeleccion)
                    .ToList();
            }
            else
            {
                // Solicitar contraseña antes de eliminar todo
                using(var formPwd = new frmPassword())
                {
                    if(formPwd.ShowDialog() != DialogResult.OK || formPwd.Contraseña != passwordBorrado)
                    {
                        MessageBox.Show("Contraseña incorrecta. Operación cancelada.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                RegistroCopia.ListadoCopias.Clear();
            }

            //Guardar cambios y actualizar la lista
            var registros = RegistroCopia.ListadoCopias.Select(t => t.copia).ToList();
            string jsonSalida = JsonConvert.SerializeObject(registros, Formatting.Indented);
            File.WriteAllText(PathRegistroCopias, jsonSalida);
            MessageBox.Show("Registros eliminados correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MostrarListaCopias(RegistroCopia.ListadoCopias);
        }

        
    }
}

public class ListViewItemComparer : IComparer
{
    private int col;
    private SortOrder order;

    public ListViewItemComparer(int column, SortOrder order)
    {
        col = column;
        this.order = order;
    }

    public int Compare(object x, object y)
    {
        int result = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);

        if(order == SortOrder.Descending)
            result = -result;

        return result;
    }
}

