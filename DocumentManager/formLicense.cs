using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DocumentManager
{
    public partial class formLicense : Form
    {
        public string licenseTo = "";
        public string companyName = "";
        public string licenseKey = "";
        public formLicense()
        {
            InitializeComponent();
        }

        private void formLicense_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Trim() == "")
            {
                MessageBox.Show("License To cannot be empty.");
                return;
            }

            if (textBox2.Text.Trim() == "")
            {
                MessageBox.Show("Company Name cannot be empty.");
                return;
            }

            if (textBox3.Text.Trim() == "")
            {
                MessageBox.Show("License Key cannot be empty.");
                return;
            }

            this.licenseTo = textBox1.Text.Trim();
            this.companyName = textBox2.Text.Trim();
            this.licenseKey = textBox3.Text.Trim();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
