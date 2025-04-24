using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace copiaProgramas
{
    public partial class frmPassword : Form
    {
        public string Contraseña { get; private set; } = string.Empty;
        public frmPassword()
        {
            InitializeComponent();
            txtPassword.Focus();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            Contraseña = txtPassword.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}
