using AForge.Imaging.Filters;
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
    public partial class formAdjustLight : Form
    {
        public Bitmap bmp;
        public Bitmap imgLight;
        public formAdjustLight()
        {
            InitializeComponent();
        }

        private void formAdjustLight_Load(object sender, EventArgs e)
        {
            textBox1.Text = "0";
            textBox2.Text = "0";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            imgLight = new Bitmap(bmp);
            ContrastCorrection filterBright = new ContrastCorrection(trackBar2.Value);
            filterBright.ApplyInPlace(imgLight);
            BrightnessCorrection filterContrast = new BrightnessCorrection(trackBar1.Value);
            filterContrast.ApplyInPlace(imgLight);

            formPhotoEdit frm = (formPhotoEdit)this.Owner;
            frm.m_canvas.Clear();
            frm.m_canvas.Add(new BackgroundImageShape() { Image = imgLight }, "Image");
            textBox1.Text = trackBar1.Value.ToString();

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            imgLight = new Bitmap(bmp);
            ContrastCorrection filterBright = new ContrastCorrection(trackBar2.Value);
            filterBright.ApplyInPlace(imgLight);
            BrightnessCorrection filterContrast = new BrightnessCorrection(trackBar1.Value);
            filterContrast.ApplyInPlace(imgLight);


            formPhotoEdit frm = (formPhotoEdit)this.Owner;
            frm.m_canvas.Clear();
            frm.m_canvas.Add(new BackgroundImageShape() { Image = imgLight }, "Image");
            textBox2.Text = trackBar2.Value.ToString();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            int tVal;
            if (!int.TryParse(textBox1.Text,out tVal))
            {
                textBox1.Text = trackBar1.Value.ToString();
                MessageBox.Show("Brightness value must between " + trackBar1.Minimum.ToString() + " and " + trackBar1.Maximum.ToString());
                return;
            }

            if (tVal < trackBar1.Minimum)
            {
                textBox1.Text = trackBar1.Value.ToString();
                MessageBox.Show("Brightness value cannot be lower than " + trackBar1.Minimum.ToString());
                return;
            }

            if (tVal > trackBar1.Maximum)
            {
                textBox1.Text = trackBar1.Value.ToString();
                MessageBox.Show("Brightness value cannot be greater than " + trackBar1.Maximum.ToString());
                return;
            }

            trackBar1.Value = tVal;
            trackBar1_Scroll(sender, e);
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            int tVal;
            if (!int.TryParse(textBox2.Text, out tVal))
            {
                textBox2.Text = trackBar2.Value.ToString();
                MessageBox.Show("Brightness value must between " + trackBar2.Minimum.ToString() + " and " + trackBar2.Maximum.ToString());
                return;
            }

            if (tVal < trackBar2.Minimum)
            {
                textBox2.Text = trackBar2.Value.ToString();
                MessageBox.Show("Brightness value cannot be lower than " + trackBar2.Minimum.ToString());
                return;
            }

            if (tVal > trackBar2.Maximum)
            {
                textBox2.Text = trackBar1.Value.ToString();
                MessageBox.Show("Brightness value cannot be greater than " + trackBar2.Maximum.ToString());
                return;
            }

            trackBar2.Value = tVal;
            trackBar2_Scroll(sender, e);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
