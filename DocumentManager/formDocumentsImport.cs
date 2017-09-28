using AForge.Imaging.Filters;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Tesseract;

namespace DocumentManager
{
    public partial class formDocumentsImport : Form
    {
        private DocEvents eventPhoto = new DocEvents();
        private List<string> fileList = new List<string>();
        private BackgroundWorker bw = new BackgroundWorker();
        private Queue<object> m_processQueue = new Queue<object>();
        private object m_syncObject = new object();

        MagickReadSettings magickSettings = new MagickReadSettings();

        public DataTable dtDoc;
        public string importType;
        public int scannedQuality;
        public int importQuality;
        public bool docEncrypt;
        private enum EBUTTONSTATE
        {
            OPEN,
            NOWORKER,
            WITHWORKER
        }

        private void SetButtons(EBUTTONSTATE a_ebuttonstate)
        {
            switch (a_ebuttonstate)
            {
                default:
                case EBUTTONSTATE.OPEN:
                    button1.Enabled = true;
                    button3.Enabled = true;
                    button4.Enabled = true;
                    //buttonOcr.Enabled = true;
                    break;
                case EBUTTONSTATE.WITHWORKER:
                    button1.Enabled = false;
                    button3.Enabled = false;
                    button4.Enabled = false;
                    //buttonOcr.Enabled = false;
                    break;
            }
        }
        public formDocumentsImport()
        {
            InitializeComponent();
        }

        private void ImportDocuments_Load(object sender, EventArgs e)
        {
            if (importType.Trim() == "SCAN")
            {
                button1.Text = "Start &Scan";
                statusStrip1.Items["toolStripStatusLabel1"].Text = "Import documents from scanner.";
            }
            else if (importType.Trim() == "PHOTO")
            {
                button1.Text = "&Select Photo Document";
                statusStrip1.Items["toolStripStatusLabel1"].Text = "Import photo documents.";
            }
            else if (importType.Trim() == "PDF")
            {
                button1.Text = "&Select PDF Document";
                statusStrip1.Items["toolStripStatusLabel1"].Text = "Import PDF documents.";
            }

            listView1.Columns.Add("Filename", "FileName", 200);
            listView1.Columns.Add("Title", "Title", 100);
            listView1.Columns.Add("Description", "Description", 180);
            listView1.Columns.Add("Size", 80,HorizontalAlignment.Right);
            listView1.Columns.Add("Value", 80,HorizontalAlignment.Right);

            magickSettings.CompressionMethod = CompressionMethod.LosslessJPEG;
            magickSettings.Format = MagickFormat.Png;

            eventPhoto.ImportCompleted += importCompleted;
            bw.DoWork += new DoWorkEventHandler(this.bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bw_RunWorkerCompleted);
        }

        private void clearListView()
        {
            foreach (ListViewItem i in listView1.Items)
            {
                DataRow r = (DataRow)i.Tag;
                File.Delete(r["DocFilename"].ToString());
            }

            if (importType == "PDF")
            {
                foreach (string f in fileList)
                {
                    File.Delete(f);
                }
            }

            listView1.Items.Clear();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count > 0)
            {
                clearListView();                
            }
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(listView1.Items.Count == 0)
            {
                MessageBox.Show("No files for import.");
                return;
            }

            foreach (ListViewItem lst in listView1.Items)
            {
                if(lst.SubItems[1].Text.Trim() == "")
                {
                    MessageBox.Show("All document must have a title.");
                    return;
                }
                if (lst.SubItems[2].Text.Trim() == "")
                {
                    MessageBox.Show("All document must have a description.");
                    return;
                }
            }

            foreach(DataRow r in dtDoc.Rows)
            {
                r["DocEncrypt"] = docEncrypt == true ? "TRUE" : "FALSE";
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (importType.Trim() == "SCAN")
            {
                statusStrip1.Items["toolStripStatusLabel1"].Text = "Please wait processing scan...";
                ScanDocument();
            }
            else if (importType.Trim() == "PHOTO")
            {
                statusStrip1.Items["toolStripStatusLabel1"].Text = "Please wait processing import...";
                ImportDocument();
            }
            else if (importType.Trim() == "PDF")
            {
                statusStrip1.Items["toolStripStatusLabel1"].Text = "Please wait processing PDF...";
                PDFDocument();
            }
        }

        public void ImportDocument()
        {
            OpenFileDialog files = new OpenFileDialog();
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;

            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                files.Filter = string.Format("{0}{1}{2} ({3})|{3}", files.Filter, sep, codecName, c.FilenameExtension);
                sep = "|";
            }

            files.Filter = string.Format("({0})|{1} {2}{3}", "PICTURE Files", "*.BMP;*.JPG;*.JPEG;*.jpeg;*.TIF", sep, files.Filter);

            files.Title = "Select files for import";
            files.Multiselect = true;
            files.CheckPathExists = true;
            files.CheckFileExists = true;
            files.ShowDialog(this);

            if (files.FileNames.Count() == 0) return;

            SetButtons(EBUTTONSTATE.WITHWORKER);
            statusStrip1.Items["toolStripStatusLabel1"].Text = "Please wait processing photos...";
            clearListView();
            dtDoc.Rows.Clear();
            fileList = files.FileNames.ToList();
            eventPhoto.ImportAsync(files.FileNames);
        }

        public void PDFDocument()
        {
            OpenFileDialog files = new OpenFileDialog();
            files.Filter = "Pdf Files| *.pdf";
            files.Title = "Select PDF for import";
            files.Multiselect = false;
            files.CheckPathExists = true;
            files.CheckFileExists = true;
            files.ShowDialog(this);

            if (files.FileNames.Count() == 0)
            {
                statusStrip1.Items["toolStripStatusLabel1"].Text = "Import PDF documents.";
                return;
            }

            SetButtons(EBUTTONSTATE.WITHWORKER);
            clearListView();
            dtDoc.Rows.Clear();
            fileList = files.FileNames.ToList();
            eventPhoto.ImportPDF(files.FileNames);
        }

        public void ScanDocument()
        {
            formScanner form = new formScanner();
            form.dtDoc = dtDoc;
            form.scannedQuality = scannedQuality;
            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            if (dtDoc.Rows.Count > 0)
            {
                listView1.Items.Clear();
                foreach (DataRow r in dtDoc.Rows)
                {
                    ListViewItem new_item = new ListViewItem();
                    new_item.Tag = r;
                    new_item.Text = Path.GetFileName(r["DocFilename"].ToString());
                    new_item.SubItems.Add("");
                    new_item.SubItems.Add("");
                    new_item.SubItems.Add(r["DocSize"].ToString());
                    new_item.SubItems.Add("0.00");
                    listView1.Items.Add(new_item).Selected = true;
                }
            }
        }

        private void importCompleted(Object sender, ImportCompletedEventArgs e)
        {
            if (this.importType == "PDF")
            {
                fileList = e.pdfFileList;
            }

            lock (this.m_syncObject)
                this.m_processQueue.Enqueue(e.imgTempFile);
            if (!this.bw.IsBusy)
                this.bw.RunWorkerAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            
            if (this.m_processQueue.Count > 0)
            {
                string ocrText = "";
                string fname = "";
                lock (this.m_syncObject)
                {
                    fname = m_processQueue.Dequeue().ToString();
                    //Console.WriteLine("bw_DoWork = " + fname);
                }

                byte[] photoBytes = File.ReadAllBytes(fname);
                string tFile = "";

                string tempFileName = Path.GetTempFileName();
                string tempPath = Path.GetTempPath();
                tFile = Path.Combine(tempPath, tempFileName);

                MagickReadSettings settings = new MagickReadSettings();
                using (MagickImage image = new MagickImage(photoBytes, magickSettings))
                {
                    image.Quality = importQuality;
                    image.Write(tFile);
                    Bitmap bmp = Grayscale.CommonAlgorithms.BT709.Apply(image.ToBitmap());
                    Threshold thresholdFilter = new Threshold(127);
                    Bitmap searchOcr = thresholdFilter.Apply(bmp);
                    TesseractEngine engine = new TesseractEngine("tessdata", "eng", EngineMode.Default);
                    Page page = engine.Process(searchOcr);
                    ocrText = page.GetText();

                    if (this.importType == "PDF")
                    {
                        File.Delete(fname);
                    }
                }

                DataRow r = dtDoc.NewRow();
                r["DocName"] = "";
                r["DocDesc"] = "";
                r["DocFilename"] = tFile;
                r["DocSize"] = string.Format("{0:n0} KB", Math.Round((double)new FileInfo(tFile).Length / 1024));
                r["DocValue"] = "0.00";
                r["DocOcr"] = ocrText;
                r["DocEncrypt"] = docEncrypt == true ? "TRUE" : "FALSE";
                dtDoc.Rows.Add(r);

                e.Result = r;
            }

        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DataRow r = (DataRow)e.Result;

            ListViewItem new_item = new ListViewItem();
            new_item.Tag = r;
            new_item.Text = Path.GetFileName(r["DocFilename"].ToString());
            new_item.SubItems.Add("");
            new_item.SubItems.Add("");
            new_item.SubItems.Add(r["DocSize"].ToString());
            new_item.SubItems.Add("0.00");
            listView1.Items.Add(new_item).Selected = true;

            statusStrip1.Items["toolStripStatusLabel1"].Text = "Please wait, importing photo " + listView1.Items.Count + " of " + fileList.Count;

            if (this.m_processQueue.Count > 0)
            {
                Application.DoEvents();
                this.bw.RunWorkerAsync();
            }
            else
            {
                SetButtons(EBUTTONSTATE.OPEN);
                statusStrip1.Items["toolStripStatusLabel1"].Text = "Importing " + listView1.Items.Count + " photos.";
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("No selected documents.");
                return;
            }

            DataTable dtSelected = dtDoc.Clone();
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                dtSelected.LoadDataRow(((DataRow)item.Tag).ItemArray, true);
            }

            formTitleDescriptionInput form = new formTitleDescriptionInput();
            form.dtDoc = dtSelected.Copy();
            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            dtDoc.AcceptChanges();
            listView1.Items.Clear();
            foreach(DataRow r in form.dtDoc.Rows)
            {
                dtDoc.LoadDataRow(r.ItemArray,true);
            }

            foreach (DataRow r in dtDoc.Rows)
            {
                ListViewItem new_item = new ListViewItem();
                new_item.Text = Path.GetFileName(r["DocFilename"].ToString());
                new_item.Tag = r;
                new_item.SubItems.Add(r["DocName"].ToString());
                new_item.SubItems.Add(r["DocDesc"].ToString());
                new_item.SubItems.Add(r["DocSize"].ToString());
                new_item.SubItems.Add(string.Format("{0:#,##0.00}", r["DocValue"].ToString()));
                listView1.Items.Add(new_item);
            }

        }

        private void buttonOcr_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select one item on the list.");
                return;
            }

            if(listView1.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select only one item on the list.");
                return;
            }

            DataRow r = (DataRow)listView1.SelectedItems[0].Tag;
            formViewOcr frm = new formViewOcr();
            frm.textOcr = r["DocOcr"].ToString();
            frm.fileName = r["DocFilename"].ToString();
            DialogResult result = frm.ShowDialog();

            if(result == DialogResult.Cancel)
            {
                return;
            }
        }

        private void checkEncrypt_CheckedChanged(object sender, EventArgs e)
        {
            docEncrypt = checkEncrypt.Checked;

            if (docEncrypt)
            {
                MessageBox.Show("Storing encrypted photo can take a lot of disk space.");
            }
        }
    }
}
