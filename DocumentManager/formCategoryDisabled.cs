using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DocumentManager
{
    public partial class formCategoryDisabled : Form
    {
        public DataTable dt;
        public DataRow drFrom;
        public DataRow drTo;
        public Boolean isRoot;

        public formCategoryDisabled()
        {
            InitializeComponent();
        }

        private void DisabledCategory_Load(object sender, EventArgs e)
        {
            DataRow[] drTempFrom = dt.Select(string.Format("Convert(parent_node,'System.Int32') = {0}", -99), "ac_name ASC");

            comboBox1.DataSource = drTempFrom.CopyToDataTable();
            comboBox1.DisplayMember = "ac_name";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select category.");
                return;
            }

            if (comboBox2.SelectedItem == null && !checkBox1.Checked)
            {
                MessageBox.Show("Please select category.");
                return;
            }

            DataRowView vrow;
            vrow = (DataRowView)comboBox1.SelectedItem;
            drFrom = vrow.Row;

            if (checkBox1.Checked)
            {
                isRoot = checkBox1.Checked;
            }
            else
            {
                vrow = (DataRowView)comboBox2.SelectedItem;
                drTo = vrow.Row;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;
            DataRowView vrow = (DataRowView)comboBox1.SelectedItem;
            string code = vrow.Row["code"].ToString();
            DataRow[] drTemp = dt.Select(string.Format("Convert(code,'System.Int32') = {0}", code));
            DataTable dtRemove = new DataTable();
            DataRow[] drRemove = null;
            List<string> inCode = new List<string>();

            dtRemove = drTemp.CopyToDataTable();
            dtRemove.TableName = "tree";

            NotInCategory(ref drRemove, ref inCode, ref code, ref dtRemove);
            code = "-99";
            NotInCategory(ref drRemove, ref inCode, ref code, ref dtRemove);

            inCode = new List<string>();
            code = "";
            foreach (DataRow r in dtRemove.Rows)
            {
                inCode.Add(r["code"].ToString());
            }

            code = string.Join(",", inCode.ToArray());

            string setFilter = string.Format("Convert(code,'System.Int32') not in ({0}) and Convert(code,'System.Int32') <> -99 and Convert(parent_node,'System.Int32') <> -99 and Convert(parent_node,'System.Int32') not in ({1})", vrow.Row["code"], code);
            string setOrder = "ac_name ASC";

            drRemove = dt.Select(setFilter, setOrder);
            if (drRemove.Count() == 0)
            {
                checkBox1.Checked = true;
                comboBox2.Enabled = false;
                checkBox1.Enabled = false;
                return;
            } else
            {
                checkBox1.Checked = false;
                comboBox2.Enabled = true;
                checkBox1.Enabled = true;
            }
            comboBox2.DataSource = drRemove.CopyToDataTable();
            comboBox2.DisplayMember = "ac_name";
        }

        private void NotInCategory(ref DataRow[] drRemove, ref List<string> inCode, ref string code, ref DataTable dtRemove)
        {
            while (1 == 1)
            {
                inCode = new List<string>();
                drRemove = dt.Select(string.Format("Convert(parent_node,'System.Int32') in ({0})", code));
                if (drRemove.Count() == 0)
                {
                    break;
                }

                foreach (DataRow r in drRemove)
                {
                    dtRemove.ImportRow(r);
                    inCode.Add(r["code"].ToString());
                }

                code = string.Join(",", inCode.ToArray());
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                comboBox2.SelectedItem = null;
                comboBox2.Enabled = false;
            }
            else
            {
                comboBox2.Enabled = true;
            }
        }
    }
}
