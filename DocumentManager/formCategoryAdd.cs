using System;
using System.Windows.Forms;

namespace DocumentManager
{
    public partial class formCategoryAdd : Form
    {
        public string treeCode;
        public string treeName;
        public formCategoryAdd()
        {
            InitializeComponent();
        }

        private void AddCategory_Load(object sender, EventArgs e)
        {
            textBoxCode.Text = treeCode;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            treeName = textBoxName.Text.Trim();
            if (treeName != null && treeName != "")
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid category name.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
