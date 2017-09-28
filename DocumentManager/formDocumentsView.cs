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
    public partial class forDocumentsView : Form
    {
        public DataTable dtDoc;
        public forDocumentsView()
        {
            InitializeComponent();
        }

        private void DocDescription_Load(object sender, EventArgs e)
        {
            if (dtDoc.Rows.Count == 1)
            {
                textBoxID.Text = dtDoc.Rows[0]["DocId"].ToString();
                textBoxTitle.Text = dtDoc.Rows[0]["DocName"].ToString();
                textBoxScanDateTime.Text = dtDoc.Rows[0]["ModifiedDate"].ToString();
                textBoxDescription.Text = dtDoc.Rows[0]["DocDesc"].ToString();
                textBoxValue.Text = dtDoc.Rows[0]["DocValue"].ToString();
                textBoxOcr.Text = dtDoc.Rows[0]["DocOcr"].ToString();
                labelMultiple.Text = "";
            } else
            {
                labelMultiple.Text = "Multiple document seleted, any changes will cascade to all selected document.";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBoxID.Text.Trim() == "" && dtDoc.Rows.Count == 1 )
            {
                MessageBox.Show("Invalid Document ID.");
                return;
            }

            if (textBoxTitle.Text.Trim() == "")
            {
                MessageBox.Show("Invalid Document Title.");
                return;
            }

            if (textBoxDescription.Text.Trim() == "")
            {
                MessageBox.Show("Invalid Document Description.");
                return;
            }

            double tryDouble;
            if (!double.TryParse(textBoxValue.Text.Trim(),out tryDouble))
            {
                MessageBox.Show("Document Value or Amount must be numeric.");
                return;
            }

            foreach (DataRow r in dtDoc.Rows)
            {
                r["DocName"] = textBoxTitle.Text.Trim();
                r["DocDesc"] = textBoxDescription.Text.Trim();
                r["ModifiedDate"] = System.DateTime.Now;
                r["DocValue"] = textBoxValue.Text.Trim();
                dtDoc.LoadDataRow(r.ItemArray.ToArray(), LoadOption.OverwriteChanges);
            }
            
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
