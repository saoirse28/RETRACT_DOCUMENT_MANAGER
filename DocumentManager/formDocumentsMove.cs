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
    public partial class formDocumentsMove : Form
    {
        public DataTable dtDocs;
        public DataTable dtCategory;
        public DataRow dr;
        public formDocumentsMove()
        {
            InitializeComponent();

        }

        private void MoveDocuments_Load(object sender, EventArgs e)
        {
            listBox1.DataSource = dtDocs;
            listBox1.DisplayMember = "DocName";

            DataTable comDt = dtCategory.Clone();

            foreach (DataRow r in dtCategory.Rows)
            {
                DataRow[] rSelect = dtCategory.Select(string.Format("Convert(parent_node,'System.Int32') = {0}", r["code"].ToString()));
                if (rSelect.Count() == 0)
                {
                    bool notDisabled = false;
                    string r_code = r["parent_node"].ToString();
                    while (1 == 1)
                    {
                        rSelect = dtCategory.Select(string.Format("Convert(code,'System.Int32') = {0}", r_code));
                        if (rSelect.Count() == 0)
                        {
                            notDisabled = true;
                            break;
                        }
                        r_code = rSelect[0]["parent_node"].ToString();
                        if (rSelect[0]["code"].ToString() == "-99") break;
                    }
                    if (notDisabled)
                    {
                        comDt.LoadDataRow(r.ItemArray.ToArray(), LoadOption.OverwriteChanges);
                    }
                }
                
            }            

            foreach(DataRow r in dtDocs.Rows)
            {
                DataRow[] rSelect = comDt.Select(string.Format("Convert(code,'System.Int32') = {0}", r["code"].ToString()));
                if (rSelect.Count() > 0)
                {
                    comDt.Rows.Remove(rSelect[0]);
                }
            }

            DataRow[] ret = comDt.Select("Convert(code,'System.Int32') >= 0 and Convert(parent_node,'System.Int32') >= 0", "ac_name ASC");
            if (ret.Count() > 0)
            {
                comDt = ret.CopyToDataTable();
                comboBox1.DataSource = comDt; ;
                comboBox1.DisplayMember = "ac_name";
            }
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
