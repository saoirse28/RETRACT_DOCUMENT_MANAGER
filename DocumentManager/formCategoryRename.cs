using System;
using System.Windows.Forms;

namespace DocumentManager
{
    public partial class formCategoryRename : Form
    {
        public string categoryName;
        public formCategoryRename()
        {
            InitializeComponent();
        }

        private void RenameCategory_Load(object sender, EventArgs e)
        {
            textBox1.Text = categoryName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Invalid category name.");
                return;
            }
            categoryName = textBox1.Text.Trim();
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
