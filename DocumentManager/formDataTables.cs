using System;
using System.Data;
using System.Windows.Forms;

namespace DocumentManager
{
    public partial class formDataTables : Form
    {
        public DocDataset docDs = new DocDataset();
        public formDataTables()
        {
            InitializeComponent();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CategoryTree_Load(object sender, EventArgs e)
        {
            foreach( DataTable s in docDs.Tables)
            {
                comboBox1.Items.Add(s.TableName);
            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = docDs.Tables[comboBox1.SelectedItem.ToString()] ;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
