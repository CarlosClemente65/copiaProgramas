using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using WinSCP;
using System.Threading.Tasks;
using System.Linq;

namespace copiaProgramas
{
    public partial class frmInicio : Form
    {
        variables variable = new variables();
        Programas programa = new Programas();


        //Diccionario para el control de los checkBox con los programas que permite vincular cada uno con su varible correspondiente y saber que programas copiar
        private Dictionary<CheckBox, string> checkBoxVariables = new Dictionary<CheckBox, string>();


        public frmInicio()
        {
            InitializeComponent();

            //Suscribe al evento cuando se selecciona una pestaña del tabControl
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;

            //En este metodo se agregan al diccionario "checkBoxVariables" todos los programas con sus checkbox correspondientes.
            vincularChecboxVariables();

            // Suscribir al evento CheckedChanged para todos los CheckBoxes
            foreach (var elemento in checkBoxVariables)
            {
                elemento.Key.CheckedChanged += CheckBox_CheckedChanged;
            }

            //Lee el fichero de configuracion para cargar las variables
            variable.CargarConfiguracion();

            //Rellena los textBox con los valores cargados en las variables desde el fichero de configuracion
            cb_destinoPI.SelectedIndex = 0;
            LimpiaCbxPi();
        }


        #region Utilidades

        private void vincularChecboxVariables()
        {
            //Agrega al diccionario de checkbox los nombres de las variables a vincular
            //Programas PI
            checkBoxVariables.Add(cbx_ipcont08, "ipcont08");
            checkBoxVariables.Add(cbx_siibase, "siibase");
            checkBoxVariables.Add(cbx_000adc, "v000adc");
            checkBoxVariables.Add(cbx_n43base, "n43base");
            checkBoxVariables.Add(cbx_contalap, "contalap");
            checkBoxVariables.Add(cbx_ipmodelo, "ipmodelo");
            checkBoxVariables.Add(cbx_iprent23, "iprent23");
            checkBoxVariables.Add(cbx_iprent22, "iprent22");
            checkBoxVariables.Add(cbx_iprent21, "iprent21");
            checkBoxVariables.Add(cbx_ipconts2, "ipconts2");
            checkBoxVariables.Add(cbx_ipabogax, "ipabogax");
            checkBoxVariables.Add(cbx_ipabogad, "ipabogad");
            checkBoxVariables.Add(cbx_ipabopar, "ipabopar");
            checkBoxVariables.Add(cbx_dscomer9, "dscomer9");
            checkBoxVariables.Add(cbx_dscarter, "dscarter");
            checkBoxVariables.Add(cbx_dsarchi, "dsarchi");
            checkBoxVariables.Add(cbx_notibase, "notibase");
            checkBoxVariables.Add(cbx_certbase, "certbase");
            checkBoxVariables.Add(cbx_dsesign, "dsesign");
            checkBoxVariables.Add(cbx_desdespa, "dsedespa");
            checkBoxVariables.Add(cbx_ipintegr, "ipintegr");
            checkBoxVariables.Add(cbx_ipbasica, "ipbasica");
            checkBoxVariables.Add(cbx_ippatron, "ippatron");
            checkBoxVariables.Add(cbx_gasbase, "gasbase");
            checkBoxVariables.Add(cbx_dscepsax, "dscepsax");
            checkBoxVariables.Add(cbx_dsgalx, "dsgalx");
            checkBoxVariables.Add(cbx_iplabor2, "iplabor2");

            //Programas noPI
            checkBoxVariables.Add(cbx_star308, "star308");
            checkBoxVariables.Add(cbx_ereo, "ereo");
            checkBoxVariables.Add(cbx_esocieda, "esocieda");
            checkBoxVariables.Add(cbx_efacges, "efacges");
            checkBoxVariables.Add(cbx_ereopat, "ereopat");
            checkBoxVariables.Add(cbx_eintegra, "eintegra");
            checkBoxVariables.Add(cbx_starpat, "starpat");
            checkBoxVariables.Add(cbx_enom1, "enom1");
            checkBoxVariables.Add(cbx_enom2, "enom2");
            checkBoxVariables.Add(cbx_ered, "ered");
            checkBoxVariables.Add(cbx_enompat, "enompat");
            checkBoxVariables.Add(cbx_dscepsa, "dscepsa");
            checkBoxVariables.Add(cbx_dsgal, "dsgal");
        }

        private void LimpiaCbxPi()
        {
            //Quita las marcas de los controlBox
            foreach (Control control in panelPI.Controls)
            {
                if (control is CheckBox cbx)
                {
                    cbx.Checked = false;

                }
            }
        }

        private void LimpiaCbxnoPi()
        {
            //Quita las marcas de los controlBox
            foreach (Control control in panelnoPI.Controls)
            {
                if (control is CheckBox cbx)
                {
                    cbx.Checked = false;

                }
            }
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Cuando hay algun cambio en los checkbox se captura cual es
            CheckBox checkBox = (CheckBox)sender;

            // Se obtiene la variable asociada al CheckBox
            string nombreVariable = checkBoxVariables[checkBox];

            // Segun la variable asociada al checkbox, se modifica el valor de la variable correspondiente
            switch (nombreVariable)
            {
                case "ipcont08":
                    programa.ipcont08 = checkBox.Checked;
                    break;

                case "v000adc":
                    programa.v000adc = checkBox.Checked;
                    break;

                case "siibase":
                    programa.siibase = checkBox.Checked;
                    break;

                case "n43base":
                    programa.n43base = checkBox.Checked;
                    break;

                case "contalap":
                    programa.contalap = checkBox.Checked;
                    break;

                case "ipmodelo":
                    programa.ipmodelo = checkBox.Checked;
                    break;

                case "iprent23":
                    programa.iprent23 = checkBox.Checked;
                    break;

                case "iprent22":
                    programa.iprent22 = checkBox.Checked;
                    break;

                case "iprent21":
                    programa.iprent21 = checkBox.Checked;
                    break;

                case "ipconts2":
                    programa.ipconts2 = checkBox.Checked;
                    break;

                case "ipabogax":
                    programa.ipabogax = checkBox.Checked;
                    break;

                case "ipabogad":
                    programa.ipabogad = checkBox.Checked;
                    break;

                case "ipabopar":
                    programa.ipabopar = checkBox.Checked;
                    break;

                case "dscomer9":
                    programa.dscomer9 = checkBox.Checked;
                    break;

                case "dscarter":
                    programa.dscarter = checkBox.Checked;
                    break;

                case "dsarchi":
                    programa.dsarchi = checkBox.Checked;
                    break;

                case "notibase":
                    programa.notibase = checkBox.Checked;
                    break;

                case "certbase":
                    programa.certbase = checkBox.Checked;
                    break;

                case "dsesign":
                    programa.dsesign = checkBox.Checked;
                    break;

                case "dsedespa":
                    programa.dsedespa = checkBox.Checked;
                    break;

                case "ipintegr":
                    programa.ipintegr = checkBox.Checked;
                    break;

                case "ipbasica":
                    programa.ipbasica = checkBox.Checked;
                    break;

                case "ippatron":
                    programa.ippatron = checkBox.Checked;
                    break;

                case "gasbase":
                    programa.gasbase = checkBox.Checked;
                    break;

                case "dscepsax":
                    programa.dscepsax = checkBox.Checked;
                    break;

                case "dsgalx":
                    programa.dsgalx = checkBox.Checked;
                    break;

                case "iplabor2":
                    programa.iplabor2 = checkBox.Checked;
                    break;

                case "star308":
                    programa.star308 = checkBox.Checked;
                    break;

                case "starpat":
                    programa.starpat = checkBox.Checked;
                    break;

                case "ereo":
                    programa.ereo = checkBox.Checked;
                    break;

                case "esocieda":
                    programa.esocieda = checkBox.Checked;
                    break;

                case "efacges":
                    programa.efacges = checkBox.Checked;
                    break;

                case "eintegra":
                    programa.eintegra = checkBox.Checked;
                    break;

                case "ereopat":
                    programa.ereopat = checkBox.Checked;
                    break;

                case "dscepsa":
                    programa.dscepsa = checkBox.Checked;
                    break;

                case "dsgal":
                    programa.dsgal = checkBox.Checked;
                    break;

                case "enom1":
                    programa.enom1 = checkBox.Checked;
                    break;

                case "enom2":
                    programa.enom2 = checkBox.Checked;
                    break;

                case "ered":
                    programa.ered = checkBox.Checked;
                    break;

                case "enompat":
                    programa.enompat = checkBox.Checked;
                    break;
            }
        }

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
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Acciones a realizar segun la pestaña seleccionada
            switch (tabControl1.SelectedTab.Name)
            {
                case "tabConfiguracion":
                    // Ejecutar el método cuando se selecciona la pestaña configuracion
                    variable.CargarConfiguracion();
                    ActualizaTextBox();
                    break;

                case "tabProgramasPi":
                    txtProgresoCopiaPI.Clear();
                    cb_destinoPI.SelectedIndex = 0;
                    LimpiaCbxPi();
                    break;

                case "tabProgramasnoPI":
                    txtProgresoCopianoPI.Clear();
                    cb_destinonoPI.SelectedIndex = 0;
                    LimpiaCbxnoPi();
                    break;
            }
        }

        private void ActualizarProgreso(string resultado, int pestaña)
        {
            //Este metodo escribe en el textBox el progreso de la copia
            if (InvokeRequired) // Se asegura que esta en el mismo hilo de ejecucion
            {
                Invoke(new Action<string, int>(ActualizarProgreso), resultado, pestaña);
            }
            else
            {
                if (pestaña == 1)
                {
                    txtProgresoCopiaPI.AppendText(resultado + Environment.NewLine);
                }
                else if (pestaña == 2)
                {
                    txtProgresoCopianoPI.AppendText(resultado + Environment.NewLine);
                }
            }
        }


        private async Task<int> LanzaCopia(bool programa, string fichero, string titulo, int pestaña)
        {
            int resultado = 0;//Resultado de la copia
            if (programa) //Si esta marcado el programa pasado se lanza la copia
            {
                string origen = string.Empty;
                if (pestaña == 1)
                {
                    origen = variable.rutaPi + fichero;

                }
                else if (pestaña == 2)
                {
                    origen = variable.rutanoPi + fichero;

                }
                string nombreFichero = Path.GetFileName(fichero); //Obtiene el nombre del programa
                string destino = variable.destino + nombreFichero; //Forma la ruta completa del programa

                //Controla para hacer la copia local
                if (variable.destino == variable.destinoLocal)
                {
                    try
                    {
                        ActualizarProgreso($"Copiando el programa {titulo} ", pestaña);
                        await Task.Run(() => File.Copy(origen, destino, true)).ConfigureAwait(false);
                        ActualizarProgreso($"Programa {titulo} copiado correctamente.", pestaña);
                        resultado = 1;
                    }

                    catch (Exception ex)
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
                        SessionOptions sessionOptions = new SessionOptions
                        {
                            Protocol = Protocol.Sftp,
                            HostName = "172.31.5.149",
                            UserName = "centos",
                            SshHostKeyFingerprint = "ssh-ed25519 255 ypCFfhJskB3YSCzQzF5iHV0eaWxlBIvMeM5kRl4N46o=",
                            SshPrivateKeyPath = @"C:\Oficina_ds\Diagram\Accesos portatil\conexiones VPN\Credenciales SSH\aws_diagram_irlanda.ppk",
                        };
                        sessionOptions.AddRawSettings("AgentFwd", "1");

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
                                session.Open(sessionOptions);

                                TransferOptions transferOptions = new TransferOptions();
                                transferOptions.TransferMode = TransferMode.Binary;

                                TransferOperationResult transferResult = session.PutFiles(origen, variable.destino, false, transferOptions);
                                transferResult.Check();

                                // Muestra información sobre la transferencia al finalizar
                                foreach (TransferEventArgs transfer in transferResult.Transfers)
                                {
                                    ActualizarProgreso($"Programa {titulo} copiado correctamente.", pestaña);
                                }
                                resultado = 1;
                            }).ConfigureAwait(false);
                        }

                        catch (Exception ex)
                        {
                            ActualizarProgreso(Environment.NewLine + $"Error al copiar el programa {titulo}" + Environment.NewLine + ex.Message, pestaña);
                        }

                        finally
                        {
                            //Libera el recurso de la sesion
                            session?.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        ActualizarProgreso(Environment.NewLine + $"No se ha podido copiar el programa {titulo}" + Environment.NewLine + ex.Message, pestaña);
                    }
                }
            }
            return resultado;
        }

        private void cb_destinoPI_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Metodo para asignar las rutas destino de la copia segun el valor seleccionado en el comboBox
            switch (cb_destinoPI.SelectedIndex)
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

        private void cb_destinonoPI_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Metodo para asignar las rutas destino de la copia segun el valor seleccionado en el comboBox
            switch (cb_destinoPI.SelectedIndex)
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

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            //Metodo para desmarcar todos los checkBox y limpiar el textoBox del resultado de la copia
            txtProgresoCopiaPI.Text = string.Empty;
            foreach (Control control in panelPI.Controls)
            {
                if (control is CheckBox cbx)
                {
                    cbx.Checked = false;
                }
            }
        }

        private void btn_limpiarnoPI_Click(object sender, EventArgs e)
        {
            //Metodo para desmarcar todos los checkBox y limpiar el textoBox del resultado de la copia
            txtProgresoCopianoPI.Text = string.Empty;
            foreach (Control control in panelnoPI.Controls)
            {
                if (control is CheckBox cbx)
                {
                    cbx.Checked = false;
                }
            }
        }
        #endregion


        #region Mouse Hover
        //Metodos para cuando el raton se posiciona dentro de los iconos
        private void btnRutaPi_MouseHover(object sender, EventArgs e)
        {
            if (txtRutaPi.Enabled == false)
            {
                btnRutaPi.BackgroundImage = global::copiaProgramas.Properties.Resources.editar_hover;
            }
            else
            {
                btnRutaPi.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar_noActivo;
            }
        }

        private void btnRutanoPi_MouseHover(object sender, EventArgs e)
        {
            if (txtRutanoPi.Enabled == false)
            {
                btnRutanoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.editar_hover;
            }
            else
            {
                btnRutanoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar_noActivo;
            }
        }

        private void btnRutaGestion_MouseHover(object sender, EventArgs e)
        {
            if (txtRutaGestion.Enabled == false)
            {
                btnRutaGestion.BackgroundImage = global::copiaProgramas.Properties.Resources.editar_hover;
            }
            else
            {
                btnRutaGestion.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar_noActivo;
            }
        }

        private void btnRutaGasoleos_MouseHover(object sender, EventArgs e)
        {
            if (txtRutaGasoleos.Enabled == false)
            {
                btnRutaGasoleos.BackgroundImage = global::copiaProgramas.Properties.Resources.editar_hover;
            }
            else
            {
                btnRutaGasoleos.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar_noActivo;
            }
        }

        private void btnDestinoPi_MouseHover(object sender, EventArgs e)
        {
            if (txtDestinoPi.Enabled == false)
            {
                btnDestinoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.editar_hover;
            }
            else
            {
                btnDestinoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar_noActivo;
            }
        }

        private void btnDestinonoPi_MouseHover(object sender, EventArgs e)
        {
            if (txtDestinonoPi.Enabled == false)
            {
                btnDestinonoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.editar_hover;
            }
            else
            {
                btnDestinonoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar_noActivo;
            }
        }

        private void btnDestinoLocal_MouseHover(object sender, EventArgs e)
        {
            if (txtDestinoLocal.Enabled == false)
            {
                btnDestinoLocal.BackgroundImage = global::copiaProgramas.Properties.Resources.editar_hover;
            }
            else
            {
                btnDestinoLocal.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar_noActivo;
            }
        }

        private void btnDestinoPasesPi_MouseHover(object sender, EventArgs e)
        {
            if (txtDestinoPasesPi.Enabled == false)
            {
                btnDestinoPasesPi.BackgroundImage = global::copiaProgramas.Properties.Resources.editar_hover;
            }
            else
            {
                btnDestinoPasesPi.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar_noActivo;
            }
        }

        private void btnDestinoPasesnoPi_MouseHover(object sender, EventArgs e)
        {
            if (txtDestinoPasesnoPi.Enabled == false)
            {
                btnDestinoPasesnoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.editar_hover;
            }
            else
            {
                btnDestinoPasesnoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar_noActivo;
            }
        }

        #endregion

        #region Mouse Leave
        //Metodos para cuando el raton sale de los iconos

        private void btnRutaPi_MouseLeave(object sender, EventArgs e)
        {
            if (txtRutaPi.Enabled == false)
            {
                btnRutaPi.BackgroundImage = global::copiaProgramas.Properties.Resources.editar;
            }
            else
            {
                btnRutaPi.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar;
            }
        }

        private void btnRutanoPi_MouseLeave(object sender, EventArgs e)
        {
            if (txtRutanoPi.Enabled == false)
            {
                btnRutanoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.editar;
            }
            else
            {
                btnRutanoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar;
            }
        }

        private void btnRutaGestion_MouseLeave(object sender, EventArgs e)
        {
            if (txtRutaGestion.Enabled == false)
            {
                btnRutaGestion.BackgroundImage = global::copiaProgramas.Properties.Resources.editar;
            }
            else
            {
                btnRutaGestion.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar;
            }
        }

        private void btnRutaGasoleos_MouseLeave(object sender, EventArgs e)
        {
            if (txtRutaGasoleos.Enabled == false)
            {
                btnRutaGasoleos.BackgroundImage = global::copiaProgramas.Properties.Resources.editar;
            }
            else
            {
                btnRutaGasoleos.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar;
            }
        }

        private void btnDestinoPi_MouseLeave(object sender, EventArgs e)
        {
            if (txtDestinoPi.Enabled == false)
            {
                btnDestinoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.editar;
            }
            else
            {
                btnDestinoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar;
            }
        }

        private void btnDestinonoPi_MouseLeave(object sender, EventArgs e)
        {
            if (txtDestinonoPi.Enabled == false)
            {
                btnDestinonoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.editar;
            }
            else
            {
                btnDestinonoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar;
            }
        }

        private void btnDestinoLocal_MouseLeave(object sender, EventArgs e)
        {
            if (txtDestinoLocal.Enabled == false)
            {
                btnDestinoLocal.BackgroundImage = global::copiaProgramas.Properties.Resources.editar;
            }
            else
            {
                btnDestinoLocal.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar;
            }
        }

        private void btnDestinoPasesPi_MouseLeave(object sender, EventArgs e)
        {
            if (txtDestinoPasesPi.Enabled == false)
            {
                btnDestinoPasesPi.BackgroundImage = global::copiaProgramas.Properties.Resources.editar;
            }
            else
            {
                btnDestinoPasesPi.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar;
            }
        }

        private void btnDestinoPasesnoPi_MouseLeave(object sender, EventArgs e)
        {
            if (txtDestinoPasesnoPi.Enabled == false)
            {
                btnDestinoPasesnoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.editar;
            }
            else
            {
                btnDestinoPasesnoPi.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar;
            }
        }

        #endregion

        #region MouseClick
        //Metodos para cuando se hace clic en los iconos

        private async void btnCopiarPI_Click(object sender, EventArgs e)
        {
            int resultado = 0; //Control para si se ha hecho alguna copia correctamente
            int controlCbx = 0;//Controla si hay algun checbox marcado para hacer la copia

            foreach (Control control in panelPI.Controls)
            {
                if (control is CheckBox cbx && cbx.Checked)
                {
                    controlCbx++;
                }
            }

            if (controlCbx > 0)
            {
                //Crea una lista de las tareas de copia a realizar
                List<Task<int>> tareasCopia = new List<Task<int>>
                {
                    //Lanza el proceso de copia segun los checkBox marcados en los programas
                    //Programas PI
                    LanzaCopia(programa.ipcont08, programa.ipcont08Ruta, "ipcont08", 1),
                    LanzaCopia(programa.siibase, programa.siibaseRuta, "siibase", 1),
                    LanzaCopia(programa.v000adc, programa.v000adcRuta, "000adc", 1),
                    LanzaCopia(programa.n43base, programa.n43baseRuta, "n43base", 1),
                    LanzaCopia(programa.contalap, programa.contalapRuta, "contalap", 1),
                    LanzaCopia(programa.ipmodelo, programa.ipmodeloRuta, "ipmodelo", 1),
                    LanzaCopia(programa.iprent23, programa.iprent23Ruta, "iprent23", 1),
                    LanzaCopia(programa.iprent22, programa.iprent22Ruta, "iprent22", 1),
                    LanzaCopia(programa.iprent21, programa.iprent21Ruta, "iprent21", 1),
                    LanzaCopia(programa.ipconts2, programa.ipconts2Ruta, "ipconts2", 1),
                    LanzaCopia(programa.ipabogax, programa.ipabogaxRuta, "ipabogax", 1),
                    LanzaCopia(programa.ipabogad, programa.ipabogadRuta, "ipabogad", 1),
                    LanzaCopia(programa.ipabopar, programa.ipaboparRuta, "ipabopar", 1),
                    LanzaCopia(programa.dscomer9, programa.dscomer9Ruta, "dscomer9", 1),
                    LanzaCopia(programa.dscarter, programa.dscarterRuta, "dscarter", 1),
                    LanzaCopia(programa.dsarchi, programa.dsarchiRuta, "dsarchi", 1),
                    LanzaCopia(programa.notibase, programa.notibaseRuta, "notibase", 1),
                    LanzaCopia(programa.certbase, programa.certbaseRuta, "certbase", 1),
                    LanzaCopia(programa.dsesign, programa.dsesignRuta, "dsesign", 1),
                    LanzaCopia(programa.dsedespa, programa.dsedespaRuta, "dsedespa", 1),
                    LanzaCopia(programa.ipintegr, programa.ipintegrRuta, "ipintegr", 1),
                    LanzaCopia(programa.ipbasica, programa.ipbasicaRuta, "ipbasica", 1),
                    LanzaCopia(programa.ippatron, programa.ippatronRuta, "ippatron", 1),
                    LanzaCopia(programa.gasbase, programa.gasbaseRuta, "gasbase", 1),
                    LanzaCopia(programa.dscepsax, programa.dscepsaxRuta, "dscepsax", 1),
                    LanzaCopia(programa.dsgalx, programa.dsgalxRuta, "dsgalx", 1),
                    LanzaCopia(programa.iplabor2, programa.iplabor2Ruta, "iplabor2", 1)
                };

                //Almacena el numero de tareas que se han realizado
                resultado = (await Task.WhenAll(tareasCopia)).Sum();

                if (resultado > 0)
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
        }

        private async void btnCopiarnoPI_Click(object sender, EventArgs e)
        {
            int resultado = 0; //Control para si se ha hecho alguna copia correctamente
            int controlCbx = 0;//Controla si hay algun checbox marcado para hacer la copia

            foreach (Control control in panelnoPI.Controls)
            {
                if (control is CheckBox cbx && cbx.Checked)
                {
                    controlCbx++;
                }
            }

            if (controlCbx > 0)
            {
                //Crea una lista de las tareas de copia a realizar
                List<Task<int>> tareasCopia = new List<Task<int>>
                {
                    //Lanza el proceso de copia segun los checkBox marcados en los programas
                    //Programas noPI
                    LanzaCopia(programa.star308, programa.star308Ruta, "star308", 2),
                    LanzaCopia(programa.starpat, programa.starpatRuta, "starpat", 2),
                    LanzaCopia(programa.ereo, programa.ereoRuta, "ereo", 2),
                    LanzaCopia(programa.esocieda, programa.esociedaRuta, "esocieda", 2),
                    LanzaCopia(programa.efacges, programa.efacgesRuta, "efacges", 2),
                    LanzaCopia(programa.eintegra, programa.eintegraRuta, "eintegra", 2),
                    LanzaCopia(programa.ereopat, programa.ereopatRuta, "ereopat", 2),
                    LanzaCopia(programa.enom1, programa.enom1Ruta, "enom1", 2),
                    LanzaCopia(programa.enom2, programa.enom2Ruta, "enom2", 2),
                    LanzaCopia(programa.ered, programa.eredRuta, "ered", 2),
                    LanzaCopia(programa.enompat, programa.enompatRuta, "enompat", 2),
                    LanzaCopia(programa.dscepsa, programa.dscepsaRuta, "dscepsa", 2),
                    LanzaCopia(programa.dsgal, programa.dsgalRuta, "dsgal", 2)
                };

                //Almacena el numero de tareas que se han realizado
                resultado = (await Task.WhenAll(tareasCopia)).Sum();

                if (resultado > 0)
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
        }

        public void btnGuardarConfiguracion_MouseClick(object sender, MouseEventArgs e)
        {
            //Graba en el fichero de configuracion las variables
            variable.GuardarConfiguracion();
        }

        private void btnRutaPi_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtRutaPi.Enabled == false)
            {
                txtRutaPi.Enabled = true;
                txtRutaPi.Focus();
                txtRutaPi.SelectionStart = txtRutaPi.TextLength;
                gestionBotones(true, "btnRutaPi");
            }
            else
            {
                variable.rutaPi = txtRutaPi.Text;
                txtRutaPi.Enabled = false;
                gestionBotones(false, "btnRutaPi");
            }
        }

        private void btnRutanoPi_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtRutanoPi.Enabled == false)
            {
                txtRutanoPi.Enabled = true;
                txtRutanoPi.Focus();
                txtRutanoPi.SelectionStart = txtRutanoPi.TextLength;
                gestionBotones(true, "btnRutanoPi");
            }
            else
            {
                variable.rutanoPi = txtRutanoPi.Text;
                txtRutanoPi.Enabled = false;
                gestionBotones(false, "btnRutanoPi");
            }
        }

        private void btnRutaGestion_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtRutaGestion.Enabled == false)
            {
                txtRutaGestion.Enabled = true;
                txtRutaGestion.Focus();
                txtRutaGestion.SelectionStart = txtRutaGestion.TextLength;
                gestionBotones(true, "btnRutaGestion");
            }
            else
            {
                variable.rutaGestion = txtRutaGestion.Text;
                txtRutaGestion.Enabled = false;
                gestionBotones(false, "btnRutaGestion");
            }
        }

        private void btnRutaGasoleos_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtRutaGasoleos.Enabled == false)
            {
                txtRutaGasoleos.Enabled = true;
                txtRutaGasoleos.Focus();
                txtRutaGasoleos.SelectionStart = txtRutaGasoleos.TextLength;
                gestionBotones(true, "btnRutaGasoleos");
            }
            else
            {
                variable.rutaGasoleos = txtRutaPi.Text;
                txtRutaGasoleos.Enabled = false;
                gestionBotones(false, "btnRutaGasoleos");
            }
        }

        private void btnDestinoPi_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtDestinoPi.Enabled == false)
            {
                btnGuardarConfiguracion.Enabled = false;
                txtDestinoPi.Enabled = true;
                txtDestinoPi.Focus();
                txtDestinoPi.SelectionStart = txtDestinoPi.TextLength;
                gestionBotones(true, "btnDestinoPi");
            }
            else
            {
                variable.destinoPi = txtDestinoPi.Text;
                txtDestinoPi.Enabled = false;
                gestionBotones(false, "btnDestinoPi");
            }
        }

        private void btnDestinonoPi_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtDestinonoPi.Enabled == false)
            {
                txtDestinonoPi.Enabled = true;
                txtDestinonoPi.Focus();
                txtDestinonoPi.SelectionStart = txtDestinonoPi.TextLength;
                gestionBotones(true, "btnDestinonoPi");
            }
            else
            {
                variable.destinonoPi = txtDestinonoPi.Text;
                txtDestinonoPi.Enabled = false;
                gestionBotones(false, "btnDestinonoPi");
            }
        }

        private void btnDestinoLocal_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtDestinoLocal.Enabled == false)
            {
                txtDestinoLocal.Enabled = true;
                txtDestinoLocal.Focus();
                txtDestinoLocal.SelectionStart = txtDestinoLocal.TextLength;
                gestionBotones(true, "btnDestinoLocal");
            }
            else
            {
                variable.destinoLocal = txtDestinoLocal.Text;
                txtDestinoLocal.Enabled = false;
                gestionBotones(false, "btnDestinoLocal");
            }
        }

        private void btnDestinoPasesPi_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtDestinoPasesPi.Enabled == false)
            {
                txtDestinoPasesPi.Enabled = true;
                txtDestinoPasesPi.Focus();
                txtDestinoPasesPi.SelectionStart = txtDestinoPasesPi.TextLength;
                gestionBotones(true, "btnDestinoPasesPi");

            }
            else
            {
                variable.destinoPasesPi = txtDestinoPasesPi.Text;
                txtDestinoPasesPi.Enabled = false;
                gestionBotones(false, "btnDestinoPasesPi");
            }
        }

        private void btnDestinoPasesnoPi_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtDestinoPasesnoPi.Enabled == false)
            {
                txtDestinoPasesnoPi.Enabled = true;
                txtDestinoPasesnoPi.Focus();
                txtDestinoPasesnoPi.SelectionStart = txtDestinoPasesnoPi.TextLength;
                gestionBotones(true, "btnDestinoPasesnoPi");
            }
            else
            {
                variable.destinoPasesnoPi = txtDestinoPasesnoPi.Text;
                txtDestinoPasesnoPi.Enabled = false;
                gestionBotones(false, "btnDestinoPasesnoPi");
            }
        }

        private void gestionBotones(bool estado, string nombreBoton)
        {
            //Metodo para cambiar los iconos del resto de los botones cuando se pulsa alguno
            foreach (Control control in tabControl1.TabPages["tabConfiguracion"].Controls)
            {
                if (control is Button btn)
                {
                    if (estado)
                    {
                        if (btn.Name == nombreBoton)
                        {
                            btn.BackgroundImage = global::copiaProgramas.Properties.Resources.guardar;
                        }
                        else
                        {
                            btn.Enabled = false;
                            if (btn.Name != btnGuardarConfiguracion.Name)
                            {
                                btn.BackgroundImage = global::copiaProgramas.Properties.Resources.editar_noActivo;
                            }
                        }
                    }
                    else
                    {
                        if (btn.Name == nombreBoton)
                        {
                            btn.BackgroundImage = global::copiaProgramas.Properties.Resources.editar;
                        }
                        else
                        {
                            btn.Enabled = true;
                            if (btn.Name != btnGuardarConfiguracion.Name)
                            {
                                btn.BackgroundImage = global::copiaProgramas.Properties.Resources.editar;
                            }
                        }
                    }
                }
            }
        }

        #endregion


    }
}
