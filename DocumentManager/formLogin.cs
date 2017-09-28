using Encryption;
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
    public partial class formLogin : Form
    {
        public string password1;
        public formLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Password cannot be empty.");
                return;
            }

            password1 = textBox1.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
