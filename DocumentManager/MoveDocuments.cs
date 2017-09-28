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
    public partial class MoveDocuments : Form
    {
        public DataTable dtDocs;
        public DataTable dtCategory;
        public DataRow dr;
        public MoveDocuments()
        {
            InitializeComponent();

        }

        private void MoveDocuments_Load(object sender, EventArgs e)
        {
            listBox1.DataSource = dtDocs;
            listBox1.DisplayMember = "DocName";

            DataTable comDt = dtCategory.Clone();

            foreach( DataRow r in dtCategory.Rows)
            {
                DataRow[] rSelect = dtCategory.Select(String.Format("Convert(parent_node,'System.Int32') = {0}", r["code"].ToString()));
                if(rSelect.Count() ==0)
                {
                    comDt.LoadDataRow(r.ItemArray.ToArray(), LoadOption.OverwriteChanges);
                }
                
            }
            comDt = comDt.Select(String.Format("Convert(parent_node,'System.Int32') <> {0}", -99), "ac_name ASC").CopyToDataTable();
            comboBox1.DataSource = comDt; ;
            comboBox1.DisplayMember = "ac_name";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select category.");
                return;
            }

            DataRowView vrow = (DataRowView)comboBox1.SelectedItem;
            dr = vrow.Row;
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
