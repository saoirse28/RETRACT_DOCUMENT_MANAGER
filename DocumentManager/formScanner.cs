using System;
using System.Windows.Forms;
using System.Collections.Generic;
using DocScanner.Wia;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using AForge.Imaging.Filters;
using Tesseract;
using ImageMagick;

namespace DocumentManager
{
    public partial class formScanner : Form
    {
        private ScanEngine m_scanEngine = new ScanEngine();
        private BackgroundWorker bw = new BackgroundWorker();
        private Queue<string> m_processQueue = new Queue<string>();
        private object m_syncObject = new object();
        public DataTable dtDoc;
        public int scannedQuality =0;
        private List<string> fileList = new List<string>();
        public formScanner()
        {
            InitializeComponent();

            if (this.m_scanEngine.GetWiaDevices().Count == 0)
            {
                statusStrip1.Items["toolSSLabel"].Text = "No available scanner detected.";
            }
            else
            {
                statusStrip1.Items["toolSSLabel"].Text = "Ready ...";
                foreach (var scan in this.m_scanEngine.GetWiaDevices())
                {
                    this.comboBox1.Items.Add(scan);
                }
                this.comboBox1.SelectedIndex = 0;
            }

            this.m_scanEngine.ScanCompleted += m_scanEngine_ScanCompleted;
            bw.DoWork += new DoWorkEventHandler(this.bwImageProcess_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bwImageProcess_RunWorkerCompleted);

        }
        private void Scanner_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (fileList.Count > 0 )
            {
                foreach(string s in fileList)
                {
                    File.Delete(s);
                }
            }
            DialogResult = DialogResult.Cancel;
            Close();
        }

        void m_scanEngine_ScanCompleted(object sender, ScanCompletedEventArgs e)
        {
            string tFile = null;
            string tempFileName = null;
            string tempPath = null;

            using (MemoryStream inStream = new MemoryStream(e.Image))
            {
                tempFileName = Path.GetTempFileName();
                tempPath = Path.GetTempPath();
                tFile = Path.Combine(tempPath, tempFileName);
                Image.FromStream(inStream).Save(tFile);
            }

            Console.WriteLine("m_scanEngine_ScanCompleted =  " + tFile);

            lock (this.m_syncObject)
            {
                this.m_processQueue.Enqueue(tFile);
            }
            if (!this.bw.IsBusy)
            {
                this.bw.RunWorkerAsync();
            }
        }

        private void bwImageProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.m_processQueue.Count > 0)
            {
                string fname = "";
                lock (this.m_syncObject)
                {
                    fname = m_processQueue.Dequeue();
                }

                byte[] tempImg = File.ReadAllBytes(fname);
                string ocrText = "";

                MagickReadSettings settings = new MagickReadSettings();
                using (MagickImage image = new MagickImage(tempImg, settings))
                {
                    image.Quality = scannedQuality;
                    image.Deskew(new Percentage(100));
                    image.Trim();
                    image.AutoOrient();
                    File.Delete(fname);
                    image.Write(fname);

                    Bitmap bmp = Grayscale.CommonAlgorithms.BT709.Apply(image.ToBitmap());
                    Threshold thresholdFilter = new Threshold(127);
                    Bitmap searchOcr = thresholdFilter.Apply(bmp);
                    TesseractEngine engine = new TesseractEngine("tessdata", "eng", EngineMode.Default);
                    Page page = engine.Process(searchOcr);
                    ocrText = page.GetText();
                }


                fileList.Add(fname);
                DataRow r = dtDoc.NewRow();
                r["DocFilename"] = fname;
                r["DocValue"] = "0";
                r["DocSize"] = string.Format("{0:n0} KB", Math.Round((double)new FileInfo(fname).Length / 1024));
                r["DocOcr"] = ocrText;
                dtDoc.Rows.Add(r);
                e.Result = r;
            }



        }

        private void bwImageProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusStrip1.Items["toolSSLabel"].Text = "Please wait, processing document count " + fileList.Count.ToString();

            if (this.m_processQueue.Count > 0)
            {
                Application.DoEvents();
                this.bw.RunWorkerAsync();
            }
            else
            {
                statusStrip1.Items["toolSSLabel"].Text = "Total document count " + fileList.Count.ToString();
                comboBox1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Kindly select scanner...");
                return;
            }

            comboBox1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            try
            {
                statusStrip1.Items["toolSSLabel"].Text = "Please wait, ScanAsync..." + comboBox1.SelectedItem.ToString();
                this.m_scanEngine.ScanAsync(comboBox1.SelectedItem as ScannerInfo);
            }
            catch (Exception ex)
            {
                statusStrip1.Items["toolSSLabel"].Text = "Idle..";
                MessageBox.Show(ex.Message, this.Name);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(fileList.Count == 0 )
            {
                MessageBox.Show("Please scan atleast one document.");
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
