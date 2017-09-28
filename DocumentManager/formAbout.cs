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
    public partial class formAbout : Form
    {
        public DataTable dtSetting;
        public formAbout()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void formAbout_Load(object sender, EventArgs e)
        {
            DataRow[] r = dtSetting.Select("AppCode='LicenseTo'");
            if (r.Count() == 1)
                labelLicenseTo.Text = "License To " + r[0]["AppValue"].ToString();
            r = dtSetting.Select("AppCode='CompanyName'");
            if (r.Count() == 1)
                labelCompanyName.Text = "Company Name " + r[0]["AppValue"].ToString();
        }
    }
}
