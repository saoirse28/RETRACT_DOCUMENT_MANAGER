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
    public partial class formPassword : Form
    {
        public string password1;
        public string password2;
        public formPassword()
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
                MessageBox.Show("Set password cannot be empty.");
                return;
            }

            if(textBox2.Text.Trim() == "")
            {
                MessageBox.Show("Re-Enter password cannot be empty.");
                return;
            }

            if (textBox1.Text.Trim() != textBox2.Text.Trim())
            {
                MessageBox.Show("Set password and Re-enter password did not match.");
                return;
            }

            password1 = textBox1.Text.Trim();
            password2 = textBox2.Text.Trim();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
