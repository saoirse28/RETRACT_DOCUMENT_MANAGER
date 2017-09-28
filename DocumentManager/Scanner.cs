using System;
using System.Windows.Forms;

namespace DocumentManager
{
    public partial class Scanner : Form
    {
        public string categoryCode;
        public string categoryName;
        public Scanner()
        {
            InitializeComponent();
        }

        private void Scanner_Load(object sender, EventArgs e)
        {
            this.Text = "Scan documents for " + categoryName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
