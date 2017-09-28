using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DocumentManager
{
    public partial class formViewOcr : Form
    {
        public string textOcr;
        public string fileName;
        public formViewOcr()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void formViewOcr_Load(object sender, EventArgs e)
        {
            textBoxOcr.Text = textOcr;
            byte[] b = File.ReadAllBytes(fileName);
            MemoryStream m = new MemoryStream(b, 0, b.Length);
            imagePanel1.Image = (Bitmap)Image.FromStream(m);
        }
    }
}
