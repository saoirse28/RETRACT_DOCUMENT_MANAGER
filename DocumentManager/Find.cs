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
    public partial class Find : Form
    {
        public List<String> findString = new List<String>();
        public Find()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                if (textBox2.Text.Trim() == "")
                {
                    MessageBox.Show("Document to search is empty.");
                    return;
                }
                else
                {
                    findString.Add("document");
                    findString.Add(textBox2.Text.Trim());
                }
            }

            if (radioButton2.Checked == true)
            {
                if(comboBox1.SelectedItem==null)
                {
                    MessageBox.Show("Please select type of date search.");
                    return;
                }
                if(dateFrom.Value > dateTo.Value)
                {
                    MessageBox.Show("Date from cannot be greater than date to.");
                    return;
                }

                findString.Add("date");
                findString.Add(comboBox1.SelectedItem.ToString());
                findString.Add(dateFrom.Value.ToShortDateString());
                findString.Add(dateTo.Value.ToShortDateString());
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Find_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            SetButtons(EBUTTONSTATE.BYDOCUMENT);
        }


        private enum EBUTTONSTATE
        {
            BYDOCUMENT,
            BYDATE
        }

        private void SetButtons(EBUTTONSTATE a_ebuttonstate)
        {
            switch (a_ebuttonstate)
            {
                default:
                
                case EBUTTONSTATE.BYDOCUMENT:
                    dateFrom.Text = "";
                    dateTo.Text = "";
                    dateFrom.Enabled = false;
                    dateTo.Enabled = false;
                    textBox2.Enabled = true;
                    comboBox1.Text = "";
                    comboBox1.Enabled = false;
                    break;
                case EBUTTONSTATE.BYDATE:
                    textBox2.Text = "";
                    textBox2.Enabled = false;
                    dateFrom.Enabled = true;
                    dateTo.Enabled = true;
                    comboBox1.Enabled = true;
                    break;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                SetButtons(EBUTTONSTATE.BYDOCUMENT);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                SetButtons(EBUTTONSTATE.BYDATE);
            }
        }
    }
}
