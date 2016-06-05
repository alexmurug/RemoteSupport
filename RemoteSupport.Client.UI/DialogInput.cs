using System;
using System.Windows.Forms;

namespace RemoteSupport.Client.UI
{
    public partial class DialogInput : Form
    {
        public DialogInput(string message)
        {
            InitializeComponent();
            lbMessage.Text = message;

            btnOK.Click += btnOK_Click;
            btnCancel.Click += btnCancel_Click;
        }

        public string Result { get; set; }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Result = txtResult.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}