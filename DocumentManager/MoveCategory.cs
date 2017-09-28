using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DocumentManager
{
    public partial class MoveCategory : Form
    {
        public DataTable dt;
        public DataRow dr;
        public MoveCategory()
        {
            InitializeComponent();
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

        private void MoveCategory_Load(object sender, EventArgs e)
        {
            String code = dr["code"].ToString();
            DataRow[] drTemp = dt.Select(String.Format("Convert(code,'System.Int32') = {0}",code));
            DataTable dtRemove = new DataTable();
            DataRow[] drRemove = null;
            List<String> inCode = new List<string>();

            dtRemove = drTemp.CopyToDataTable();
            dtRemove.TableName = "tree";

            NotInCategory(ref drRemove,ref inCode,ref code, ref dtRemove);
            code = "-99";
            NotInCategory(ref drRemove, ref inCode, ref code, ref dtRemove);

            inCode = new List<string>();
            code = "";
            foreach (DataRow r in dtRemove.Rows)
            {
                inCode.Add(r["code"].ToString());
            }

            code = String.Join(",", inCode.ToArray());

            String setFilter = String.Format("Convert(code,'System.Int32') not in ({0}) and Convert(parent_node,'System.Int32') <> -99 and Convert(parent_node,'System.Int32') not in ({1})", dr["code"], code);
            String setOrder = "ac_name ASC";
            dt = dt.Select(setFilter, setOrder).CopyToDataTable();

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "ac_name";
        }

        private void NotInCategory(ref DataRow[] drRemove,ref List<string> inCode, ref String code, ref DataTable dtRemove)
        {
            while (1 == 1)
            {
                inCode = new List<string>();
                drRemove = dt.Select(String.Format("Convert(parent_node,'System.Int32') in ({0})", code));
                if (drRemove.Count() == 0)
                {
                    break;
                }

                foreach (DataRow r in drRemove)
                {
                    dtRemove.ImportRow(r);
                    inCode.Add(r["code"].ToString());
                }

                code = String.Join(",", inCode.ToArray());
            }
        }
    }
}
