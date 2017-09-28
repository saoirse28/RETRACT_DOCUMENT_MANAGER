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
    public partial class formTitleDescriptionInput : Form
    {
        public DataTable dtDoc;
        public formTitleDescriptionInput()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void TitleDescriptionInput_Load(object sender, EventArgs e)
        {
            if (dtDoc.Rows.Count > 1) return;
            textBoxTitle.Text = dtDoc.Rows[0]["DocName"].ToString();
            textBoxDescription.Text = dtDoc.Rows[0]["DocDesc"].ToString();
            textBoxValue.Text = dtDoc.Rows[0]["DocValue"].ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBoxTitle.Text.Trim() == "")
            {
                MessageBox.Show("Document title cannot be empty.");
                return;
            }

            if (textBoxDescription.Text.Trim() == "")
            {
                MessageBox.Show("Document description cannot be empty.");
                return;
            }

            double tryDouble;
            if (!double.TryParse(textBoxValue.Text.Trim(),out tryDouble))
            {
                MessageBox.Show("Document value/amount must be in number.");
                return;
            }

            foreach (DataRow r in dtDoc.Rows)
            {
                r["DocName"] = textBoxTitle.Text.Trim();
                r["DocDesc"] = textBoxDescription.Text.Trim();
                r["DocValue"] = textBoxValue.Text.Trim() == "" ? "0" : textBoxValue.Text.Trim();
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
