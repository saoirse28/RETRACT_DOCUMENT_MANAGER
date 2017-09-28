using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DocumentManager
{
    public partial class ImportDocuments : Form
    {
        public DataTable dtDoc;
        public ImportDocuments()
        {
            InitializeComponent();
        }

        private void ImportDocuments_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Invalid document title.");
                return;
            }

            if(textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Invalid document description");
                return;
            }

            if(listBox1.Items.Count == 0)
            {
                MessageBox.Show("No files for import.");
                return;
            }

            foreach (String f in listBox1.Items)
            {
                DataRow r =  dtDoc.NewRow();
                r["DocName"] = textBox1.Text.Trim();
                r["DocDesc"] = textBox2.Text.Trim();
                r["DocFilename"] = f;
                dtDoc.Rows.Add(r); 
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog files = new OpenFileDialog();
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;

            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                files.Filter = String.Format("{0}{1}{2} ({3})|{3}", files.Filter, sep, codecName, c.FilenameExtension);
                sep = "|";
            }

            files.Filter = String.Format("{0}{1}{2} ({3})|{3}", files.Filter, sep, "All Files", "*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif");

            files.Title = "Select files for import";
            files.Multiselect = true;
            files.CheckPathExists = true;
            files.CheckFileExists = true;
           
            files.ShowDialog(this);

            if (files.FileNames.Count() > 0)
            {
                listBox1.Items.Clear();
                foreach (String s in files.FileNames)
                {
                    listBox1.Items.Add(s);
                }
            }

        }
    }
}
