using ImageMagick;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ToolBox;

namespace DocumentManager
{
    public partial class MainDocManager : Form
    {
        private DocEvents photoEvent = new DocEvents();
        private BackgroundWorker bwOverlay = new BackgroundWorker();
        private BackgroundWorker bwImport = new BackgroundWorker();
        private BackgroundWorker bwLicense = new BackgroundWorker();
        private Queue<object> m_processQueue = new Queue<object>();
        private object m_syncObject = new object();

        SettingDS settingDS = new SettingDS();
        DocDataset docDs = new DocDataset();
        DataTable dtDocFilter;
        int currentPage = 0;

        MagickReadSettings magickSettingsNoEnchiper = new MagickReadSettings();
        MagickReadSettings magickSettingsEnchiper = new MagickReadSettings();
        string encipherDecipher = "docArr@y";

        private float[] zooms = { 0.1f, 0.2f, 0.4f, 0.5f, 0.75f, 1.0f, 1.25f, 1.5f, 1.75f, 2.0f, 3.0f };

        private enum EBUTTONSTATE
        {
            CLOSED,
            OPEN,
            TREEWITHDOCS,
            TREEWITHOUTDOCS,
            SINGLEPHOTO,
            MULTIPLEPHOTO,
            CATEGORYSTRIPDOC,
            CATEGORYSTRIPROOT,
            FILENOTIMAGE,
            FILEIMAGE
        }

        public void OfSettings()
        {
            contextMenuAdmin.Items["archiveCategoriesToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("System001"); //disable category
            contextMenuAdmin.Items["resetApplicationToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("System002"); //reset application
            contextMenuAdmin.Items["backupApplicationToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("System004"); //backup application
            contextMenuAdmin.Items["viewDataTablesToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("System003"); //view data tables
            contextMenuFile.Items["importImageDocumentsToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("System005"); //import document
            contextMenuFile.Items["scanDocumentsToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("System006"); //remove scan document

            if (treeView1.SelectedNode == null)
            {
                toolStripTree.Items["toolStripImport"].Enabled = false;
                toolStripTree.Items["toolStripScan"].Enabled = false;
            }
            else
            {
                if (treeView1.SelectedNode.Nodes.Count == 0)
                {
                    toolStripTree.Items["toolStripImport"].Enabled = false;
                    toolStripTree.Items["toolStripScan"].Enabled = false;
                }
                else
                {
                    toolStripTree.Items["toolStripImport"].Enabled = docDs.GetSettingBooleanValue("System005");
                    toolStripTree.Items["toolStripScan"].Enabled = docDs.GetSettingBooleanValue("System006");
                }
            }

            contextMenuTree.Items["addRootCategoryToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("Cat001");
            contextMenuTree.Items["addSubCategoryToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("Cat001");
            contextMenuTree.Items["deleteSelectedCategoryToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("Cat002");
            contextMenuTree.Items["moveSelectedCategoryToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("Cat003");
            contextMenuTree.Items["moveAsRootCategoryToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("Cat003");
            contextMenuTree.Items["toolStripMenuItem9"].Enabled = docDs.GetSettingBooleanValue("Cat004"); //Rename Category

            contextMenuDocs.Items["moveToCategoryToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("Doc001");
            contextMenuDocs.Items["openDocumentToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("Doc002");
            contextMenuDocs.Items["deleteDocumentToolStripMenuItem1"].Enabled = docDs.GetSettingBooleanValue("Doc003");

            contextMenuFile.Items["printDocumentsToolStripMenuItem"].Enabled = docDs.GetSettingBooleanValue("Doc004"); //print document


            if (listView1.Items.Count == 0)
            {
                toolStripImage.Items["toolStripDescription"].Enabled = false;
            }
            else
            {
                if (listView1.SelectedItems.Count == 0)
                {
                    toolStripImage.Items["toolStripDescription"].Enabled = false;
                }
                else
                {
                    toolStripImage.Items["toolStripDescription"].Enabled = docDs.GetSettingBooleanValue("Doc002");
                }
            }
        }
        private void SetButtons(EBUTTONSTATE a_ebuttonstate)
        {
            switch (a_ebuttonstate)
            {
                default:
                case EBUTTONSTATE.CATEGORYSTRIPDOC:
                    toolStripTree.Enabled = true;
                    toolStripTree.Items["toolStripImport"].Enabled = true && docDs.GetSettingBooleanValue("System005");
                    toolStripTree.Items["toolStripScan"].Enabled = true && docDs.GetSettingBooleanValue("System006");
                    break;
                case EBUTTONSTATE.CATEGORYSTRIPROOT:
                    toolStripTree.Enabled = true;
                    toolStripTree.Items["toolStripImport"].Enabled = false;
                    toolStripTree.Items["toolStripScan"].Enabled = false;
                    break;
                case EBUTTONSTATE.CLOSED:
                    break;
                case EBUTTONSTATE.OPEN:
                    toolStripImage.Enabled = false;
                    menuStrip1.Items["documentsToolStripMenuItem"].Enabled = false;
                    trackBar1.Enabled = false;
                    listView1.Items.Clear();
                    imagePanel1.Image = null;
                    contextMenuDocs.Enabled = false;
                    break;
                case EBUTTONSTATE.TREEWITHDOCS:
                    imagePanel1.Image = null;
                    trackBar1.Enabled = false;
                    contextMenuDocs.Enabled = true;
                    toolStripImage.Enabled = false;
                    menuStrip1.Items["documentsToolStripMenuItem"].Enabled = false;
                    break;
                case EBUTTONSTATE.TREEWITHOUTDOCS:
                    imagePanel1.Image = null;
                    trackBar1.Enabled = false;
                    contextMenuDocs.Enabled = false;
                    toolStripImage.Enabled = false;
                    menuStrip1.Items["documentsToolStripMenuItem"].Enabled = false;
                    listView1.Items.Clear();
                    break;
                case EBUTTONSTATE.SINGLEPHOTO:
                    //imagePanel1.Image = null;
                    toolStripImage.Enabled = true;
                    menuStrip1.Items["documentsToolStripMenuItem"].Enabled = true;
                    trackBar1.Enabled = true;
                    toolStripImage.Enabled = true;
                    toolStripImage.Items["toolStripDescription"].Enabled = true && docDs.GetSettingBooleanValue("Doc002");
                    toolStripImage.Items["toolStripRotateLeft"].Enabled = true;
                    toolStripImage.Items["toolStripRotateRight"].Enabled = true;
                    toolStripImage.Items["toolStripButtonCrop"].Enabled = true;
                    contextMenuDocs.Enabled = true;
                    break;
                case EBUTTONSTATE.MULTIPLEPHOTO:
                    imagePanel1.Image = null;
                    toolStripImage.Enabled = true;
                    menuStrip1.Items["documentsToolStripMenuItem"].Enabled = true;
                    trackBar1.Enabled = false;
                    toolStripImage.Enabled = true;
                    toolStripImage.Items["toolStripDescription"].Enabled = true && docDs.GetSettingBooleanValue("Doc002");
                    toolStripImage.Items["toolStripRotateLeft"].Enabled = false;
                    toolStripImage.Items["toolStripRotateRight"].Enabled = false;
                    toolStripImage.Items["toolStripButtonCrop"].Enabled = false;
                    contextMenuDocs.Enabled = true;
                    break;
                case EBUTTONSTATE.FILENOTIMAGE:
                    imagePanel1.Image = null;
                    trackBar1.Enabled = false;
                    contextMenuDocs.Enabled = true;
                    toolStripImage.Enabled = false;
                    menuStrip1.Items["documentsToolStripMenuItem"].Enabled = false;
                    break;
                case EBUTTONSTATE.FILEIMAGE:
                    trackBar1.Enabled = true;
                    contextMenuDocs.Enabled = true;
                    toolStripImage.Enabled = true;
                    menuStrip1.Items["documentsToolStripMenuItem"].Enabled = true;
                    break;
            }
        }

        private void DocScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (docDs.HasChanges())
            {
                toolStripSaveAll_Click(sender, e);
                //e.Cancel = true;
            }
        }
        private void DocScanner_Load(object sender, EventArgs e)
        {
            docDs.appDataPath = settingDS.appDataFile;
            docDs.createDataset();

            if (!docDs.readDataFromXML())
            {
                docDs.Reset();
                docDs.createDataset();
                docDs.readDataFromXML();
                MessageBox.Show("Cannot locate your application data, reseting application by default.");
            }

            docDs.AcceptChanges();
            dtDocFilter = docDs.documentTable.Clone();
            dtDocFilter.Rows.Clear();
            dtDocFilter.TableName = "find";
            SetButtons(EBUTTONSTATE.OPEN);
            GeneralBox.PopulateTreeView(ref treeView1, docDs.categoryTable, 0, null);
            lblZoom.Text = Convert.ToString(Math.Round((trackBar1.Value * 0.01f * 100))) + "%";

            OfSettings();
            magickSettingsNoEnchiper.CompressionMethod = CompressionMethod.JPEG;
            magickSettingsNoEnchiper.Format = MagickFormat.Jpeg;

            magickSettingsEnchiper.CompressionMethod = CompressionMethod.LosslessJPEG;
            magickSettingsEnchiper.Format = MagickFormat.Png;


            statusStrip1.Items["toolStripStatusLabel1"].Text = "Validating application license.";
            photoEvent.LicenseKeyValidate("", "", "");
            Console.WriteLine("Start Application.");
        }
        public MainDocManager()
        {
            InitializeComponent();

            photoEvent.OverlayCompleted += overlayCompleted;
            bwOverlay.DoWork += new DoWorkEventHandler(this.bwOverlay_DoWork);
            bwOverlay.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bwOverlay_RunWorkerCompleted);

            bwImport.DoWork += new DoWorkEventHandler(this.bwImport_DoWork);
            bwImport.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bwImport_RunWorkerCompleted);

            photoEvent.LicenseCompleted += licenseCompleted;
            bwLicense.DoWork += new DoWorkEventHandler(this.bwLicense_DoWork);
            bwLicense.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bwLicense_RunWorkerCompleted);
        }
        private void licenseCompleted(object sender, LicenseEventArgs e)
        {
            if (e.licenseKey.Trim() == "")
            {
                DataRow[] dr = settingDS.settingTable.Select("AppCode='AppKey'");
                if (dr.Count() == 1)
                {
                    e.licenseKey = Encrypt.DecryptString(dr[0]["AppValue"].ToString().Trim(),encipherDecipher);
                    Console.WriteLine(e.licenseKey);
                }
            }

            if (e.companyName.Trim() == "")
            {
                DataRow[] dr = settingDS.settingTable.Select("AppCode='CompanyName'");
                if (dr.Count() == 1)
                {
                    e.companyName = dr[0]["AppValue"].ToString().Trim();
                }
            }

            if (e.licenseTo.Trim() == "")
            {
                DataRow[] dr = settingDS.settingTable.Select("AppCode='LicenseTo'");
                if (dr.Count() == 1)
                {
                    e.licenseTo = dr[0]["AppValue"].ToString().Trim();
                }
            }

            lock (this.m_syncObject)
                this.m_processQueue.Enqueue(e);
            if (!this.bwLicense.IsBusy)
                this.bwLicense.RunWorkerAsync();
        }
        private void bwLicense_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.m_processQueue.Count > 0)
            {

                LicenseEventArgs licenseEventArgs;
                lock (this.m_syncObject)
                {
                    licenseEventArgs = (LicenseEventArgs)m_processQueue.Dequeue();
                }

                e.Result = licenseEventArgs;
            }
        }
        private void bwLicense_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.m_processQueue.Count > 0)
            {
                Application.DoEvents();
                this.bwLicense.RunWorkerAsync();
            }
            else
            {
                LicenseEventArgs licenseEventArgs = (LicenseEventArgs)e.Result;

                
                if (settingDS.ValidateLicense(licenseEventArgs.licenseKey))
                {
                    statusStrip1.Items["toolStripStatusLabel1"].Text = "Ready";
                    settingDS.settingTable.LoadDataRow(new object[] {"AppKey", Encrypt.EncryptString(licenseEventArgs.licenseKey.Trim(), encipherDecipher)}, LoadOption.OverwriteChanges);
                    settingDS.settingTable.LoadDataRow(new object[] { "LicenseTo", licenseEventArgs.licenseTo.Trim()}, LoadOption.OverwriteChanges);
                    settingDS.settingTable.LoadDataRow(new object[] {"CompanyName", licenseEventArgs.companyName.Trim()}, LoadOption.OverwriteChanges);
                    settingDS.SaveSetting();

                    if (docDs.GetSettingBooleanValue("System007"))
                    {
                        while (1 == 1)
                        {
                            formLogin frm = new formLogin();
                            var result = frm.ShowDialog();
                            if (result != DialogResult.OK)
                            {
                                frm.Dispose();
                                this.Close();
                                break;
                            }

                            if (settingDS.ValidateUser(frm.password1.Trim()))
                            {
                                frm.Dispose();
                                break;
                            }
                            else
                            {
                                frm.Dispose();
                                MessageBox.Show("Invalid password.");
                            }
                        } 
                    }
                }
                else
                {
                    formLicense form = new formLicense();
                    form.Owner = this;
                    var result = form.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        form.Dispose();
                        this.Close();
                    }
                    photoEvent.LicenseKeyValidate(form.licenseTo, form.companyName, form.licenseKey);
                }

            }
        }
        private void bwImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.m_processQueue.Count > 0)
            {
                Application.DoEvents();
                this.bwImport.RunWorkerAsync();
            }
            else
            {
                DataRow[] docResults = (DataRow[])e.Result;
                SetButtons(EBUTTONSTATE.TREEWITHOUTDOCS);
                if (docResults.Count() == 0) return;
                PopulateListView(docResults.CopyToDataTable());
                statusStrip1.Items["toolStripStatusLabel1"].Text = "Ready";
                listView1.Focus();
            }
        }
        private void bwImport_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.m_processQueue.Count > 0)
            {

                List<object> eResult = new List<object>();
                lock (this.m_syncObject)
                {
                    eResult = (List<object>)m_processQueue.Dequeue();
                }

                DataTable dt = (DataTable)eResult[1];
                string code = eResult[0].ToString();

                foreach (DataRow r in dt.Rows)
                {
                    string tempName = r["DocFilename"].ToString();

                    byte[] imageBytes = File.ReadAllBytes(tempName);
                    string newFilename = "";

                    if (GeneralBox.TryParseBoolean( r["DocEncrypt"].ToString()))
                    {
                        using (MagickImage imgEncipher = new MagickImage(imageBytes, magickSettingsEnchiper))
                        {
                            imgEncipher.Encipher(encipherDecipher);
                            newFilename = GetPathData(r["DocId"].ToString() + ".PNG");
                            imgEncipher.Write(newFilename);
                        }
                    }
                    else
                    {
                        using (MagickImage imgNoEncipher = new MagickImage(imageBytes, magickSettingsNoEnchiper))
                        {
                            newFilename = GetPathData(r["DocId"].ToString() + ".JPEG");
                            imgNoEncipher.Write(newFilename);
                        }
                    }

                    DataRow d = docDs.documentTable.NewRow();
                    d["DocName"] = r["DocName"];
                    d["DocDesc"] = r["DocDesc"];
                    d["code"] = code;                    
                    d["DocFilename"] = newFilename;
                    d["DocSize"] = string.Format("{0:n0} KB", Math.Round((double)new FileInfo(newFilename).Length / 1024));
                    d["DocValue"] = r["DocValue"];
                    d["DocOcr"] = r["DocOcr"];
                    d["DocEncrypt"] = r["DocEncrypt"];
                    docDs.documentTable.Rows.Add(d);
                    File.Delete(tempName);
                }

                DataRow[] docResult = docDs.categoryTable.Select(string.Format("Convert(code,'System.Int32') = {0}", code));
                DataRow[] docResults = docResult[0].GetChildRows(docDs.Relations["catDocRelation"]);
                e.Result = docResults;
            }
        }
        private void bwOverlay_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.m_processQueue.Count > 0)
            {
                Application.DoEvents();
                this.bwOverlay.RunWorkerAsync();
            }
            else
            {

                List<object> eResult = new List<object>();
                eResult = (List<object>)e.Result;
                DataRow dr = (DataRow)eResult[0];

                if (listView1.SelectedItems.Count == 1)
                {
                    if (dr["DocId"].ToString() == ((DataRow)listView1.SelectedItems[0].Tag)["DocId"].ToString())
                    {
                        
                        imagePanel1.Image = (Bitmap)eResult[1];
                        SetButtons(EBUTTONSTATE.SINGLEPHOTO);
                    }
                }

                labelLoading.Visible = false;
                statusStrip1.Items["toolStripStatusLabel1"].Text = "Ready";
                listView1.Focus();
            }
        }
        private void bwOverlay_DoWork(object sender, DoWorkEventArgs e)
        { 
            if (this.m_processQueue.Count > 0)
            {

                OverlayEventArgs overlayEventArgs;
                lock (this.m_syncObject)
                {
                    overlayEventArgs = (OverlayEventArgs)m_processQueue.Dequeue();
                }

                DataRow dr = overlayEventArgs.dr;
                float rotateDegrees = overlayEventArgs.rotateDegrees;

                string fname = dr["DocFilename"].ToString();
                byte[] imageBytes = File.ReadAllBytes(fname);
                string overlayPath = docDs.GetSettingStringValue("Doc008");
                bool withOverlayOnPreview = docDs.GetSettingBooleanValue("Doc009");

                List<object> eResult = new List<object>();
                eResult.Add(dr);

                if (GeneralBox.TryParseBoolean(dr["DocEncrypt"].ToString()))
                {
                    using (MagickImage imgDecipher = new MagickImage(imageBytes, magickSettingsEnchiper))
                    {
                        imgDecipher.Decipher(encipherDecipher);
                        if (rotateDegrees > 0)
                        {
                            imgDecipher.Rotate(rotateDegrees);
                            imgDecipher.Encipher(encipherDecipher);
                            imgDecipher.Write(fname);
                            imgDecipher.Decipher(encipherDecipher);
                        }

                        if (withOverlayOnPreview)
                        {

                            using (MagickImage imageOverlay = new MagickImage(overlayPath, magickSettingsEnchiper))
                            {
                                imageOverlay.Scale(imgDecipher.Width, imgDecipher.Height);
                                imgDecipher.Composite(imageOverlay, Gravity.Center, CompositeOperator.Over);
                                eResult.Add(imgDecipher.ToBitmap());
                            }

                        }
                        else
                        {
                            eResult.Add(imgDecipher.ToBitmap());
                        }
                    }
                }

                if (!GeneralBox.TryParseBoolean(dr["DocEncrypt"].ToString()))
                {
                    using (MagickImage imgNoDecipher = new MagickImage(imageBytes, magickSettingsNoEnchiper))
                    {
                        if (rotateDegrees > 0)
                        {
                            imgNoDecipher.Rotate(rotateDegrees);
                            imgNoDecipher.Write(fname);
                        }

                        if (withOverlayOnPreview)
                        {

                            using (MagickImage imageOverlay = new MagickImage(overlayPath))
                            {
                                imageOverlay.Scale(imgNoDecipher.Width, imgNoDecipher.Height);
                                imgNoDecipher.Composite(imageOverlay, Gravity.Center, CompositeOperator.Over);
                                eResult.Add(imgNoDecipher.ToBitmap());
                            }

                        }
                        else
                        {
                            eResult.Add(imgNoDecipher.ToBitmap());
                        }
                    }
                }
                e.Result = eResult;
            }
        }
        private void overlayCompleted(Object sender, OverlayEventArgs e)
        {
            lock (this.m_syncObject)
                this.m_processQueue.Enqueue(e);
            if (!this.bwOverlay.IsBusy)
                this.bwOverlay.RunWorkerAsync();
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count == 0)
            {
                SetButtons(EBUTTONSTATE.CATEGORYSTRIPDOC);
            }
            else
            {
                SetButtons(EBUTTONSTATE.CATEGORYSTRIPROOT);
            }
            dtDocFilter.Rows.Clear();
            DataRow[] docResult = docDs.categoryTable.Select(string.Format("Convert(code,'System.Int32') = {0}", e.Node.Name));
            DataRow[] docResults = docResult[0].GetChildRows(docDs.Relations["catDocRelation"]);
            listView1.Tag = e.Node;
            SetButtons(EBUTTONSTATE.TREEWITHOUTDOCS);
            if (docResults.Count() == 0) return;
            PopulateListView(docResults.CopyToDataTable());
        }
        private void PopulateListView(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                SetButtons(EBUTTONSTATE.TREEWITHOUTDOCS);
                return;
            }
            else
            {
                SetButtons(EBUTTONSTATE.TREEWITHDOCS);
            }

            listView1.Clear();

            DataView view = new DataView(dt);
            DataTable distinctValues = view.ToTable(true, "code");

            foreach (DataRow r in distinctValues.Rows)
            {
                DataRow[] CodeGroup = docDs.categoryTable.Select(string.Format("Convert(code,'System.Int32') = {0}", r["code"].ToString()));
                ListViewGroup group = new ListViewGroup(r["code"].ToString(), CodeGroup[0]["ac_name"].ToString());
                listView1.Groups.Add(group);
            }

            DataView viewSort = new DataView(dt, "", "DocId", DataViewRowState.CurrentRows);
            dt = viewSort.Table;

            if (dt.TableName == "find")
            {
                listView1.Columns.Add("DocId", 80);
                //listView1.Columns.Add("Category", 60);
                listView1.Columns.Add("Title", 100);
                listView1.Columns.Add("Description", 180);
                listView1.Columns.Add("Scanned Date", 135);
                listView1.Columns.Add("Modified Date", 135);
                listView1.Columns.Add("Size", 80, HorizontalAlignment.Right);
                listView1.Columns.Add("Value", 80, HorizontalAlignment.Right);
                listView1.Columns.Add("Encrypted", 80, HorizontalAlignment.Center);

                foreach (DataRow r in dt.Rows)
                {

                    //DataRow[] docResult = docDs.categoryTable.Select(String.Format("Convert(code,'System.Int32') = {0}", r["code"].ToString()));

                    //if (docResult.Count() > 0)
                    //{
                    ListViewItem new_item = new ListViewItem();
                    new_item.Group = listView1.Groups[r["code"].ToString()];
                    new_item.Text = r["DocId"].ToString();
                    //new_item.ImageIndex = 7;
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "DocName", Text = r["DocName"].ToString() }
                    );
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "DocDesc", Text = r["DocDesc"].ToString() }
                    );
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "ScannedDate", Text = r["ScannedDate"].ToString() }
                    );
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "ModifiedDate", Text = r["ModifiedDate"].ToString() }
                    );
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "DocSize", Text = r["DocSize"].ToString() }
                    );
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "DocValue", Text = string.Format("{0:#,##0.00}", r["DocValue"].ToString()) }
                    );
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "DocEncrypt", Text = r["DocEncrypt"].ToString() }
                    );
                    new_item.Tag = r;
                    listView1.Items.Add(new_item);
                    //}
                }
            }
            else
            {
                listView1.Columns.Add("DocId", 80);
                listView1.Columns.Add("Title", 100);
                listView1.Columns.Add("Description", 180);
                listView1.Columns.Add("Scanned Date", 135);
                listView1.Columns.Add("Modified Date", 135);
                listView1.Columns.Add("Size", 80, HorizontalAlignment.Right);
                listView1.Columns.Add("Value", 80, HorizontalAlignment.Right);
                listView1.Columns.Add("Encrypted", 80, HorizontalAlignment.Center);

                foreach (DataRow r in dt.Rows)
                {
                    ListViewItem new_item = new ListViewItem();
                    new_item.Group = listView1.Groups[r["code"].ToString()];
                    new_item.Text = r["DocId"].ToString();
                    //new_item.ImageIndex = 3;
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "DocName", Text = r["DocName"].ToString() }
                    );
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "DocDesc", Text = r["DocDesc"].ToString() }
                    );
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "ScannedDate", Text = r["ScannedDate"].ToString() }
                    );
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "ModifiedDate", Text = r["ModifiedDate"].ToString() }
                    );
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "DocSize", Text = r["DocSize"].ToString() }
                    );
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "DocValue", Text = string.Format("{0:#,##0.00}", r["DocValue"]) }
                    );
                    new_item.SubItems.Add(
                        new ListViewItem.ListViewSubItem { Name = "DocEncrypt", Text = r["DocEncrypt"].ToString() }
                    );
                    new_item.Tag = r;
                    listView1.Items.Add(new_item);
                }
            }
        }
        private void toolStripSaveAll_Click(object sender, EventArgs e)
        {
            saveAllChangesToolStripMenuItem1_Click(sender, e);
        }
        private void toolStripStartScan_Click(object sender, EventArgs e)
        {
            scanDocumentsToolStripMenuItem_Click(sender, e);
        }        
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 0)
            {
                return;
            }

            if (listView1.SelectedIndices.Count > 1)
            {
                SetButtons(EBUTTONSTATE.MULTIPLEPHOTO);
                return;
            }

            SetButtons(EBUTTONSTATE.FILENOTIMAGE);

            DataRow drDocs = (DataRow)listView1.SelectedItems[0].Tag;
            string sDocId = drDocs["DocId"].ToString();
            DataRow[] docResult = docDs.documentTable.Select(string.Format("Convert(DocId,'System.Int32') = {0}", sDocId));

            if (!File.Exists(drDocs["DocFilename"].ToString()))
            {
                return;
            }

            try
            {
                labelLoading.Visible = true;
                statusStrip1.Items["toolStripStatusLabel1"].Text = "Please wait, Processing Photo Preview...";
                photoEvent.RenderWithOverlay(docResult[0], 0);
                imagePanel1.Zoom = trackBar1.Value * 0.01f;
                lblZoom.Text = Convert.ToString(Math.Round(trackBar1.Value * 0.01f * 100)) + "%";

            }
            catch (Exception)
            {
                imagePanel1.Image = null;
                return;
            }
        }
        private void addSubCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;

            if (tn == null)
            {
                MessageBox.Show("Please select category.");
                return;
            }

            TreeNode t = new TreeNode();
            t.Name = Convert.ToString(docDs.GetTreeCode());

            formCategoryAdd form = new formCategoryAdd();
            form.treeCode = t.Name;

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            DataRow[] docTreeRelation = CategoryWithDocs(tn.Name);
            if (docTreeRelation.Count() > 0)
            {
                MessageBox.Show("Adding sub-category to catergory with scanned documents is not allowed.");
                return;
            }

            t.Text = form.treeName;

            if (tn.Level <= 2)
            {
                t.ImageIndex = tn.ImageIndex + 2;
                t.SelectedImageIndex = tn.ImageIndex + 3;
            }
            else
            {
                t.ImageIndex = 6;
                t.SelectedImageIndex = 7;
            }

            tn.Nodes.Add(t);

            docDs.categoryTable.Rows.Add(new Object[] { Convert.ToInt32(t.Name), t.Text, Convert.ToInt32(tn.Name) });
            form.Dispose();
        }
        private void addRootCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode t = new TreeNode();
            t.Name = Convert.ToString(docDs.GetTreeCode());

            formCategoryAdd form = new formCategoryAdd();
            form.treeCode = t.Name;

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            t.Text = form.treeName;
            t.ImageIndex = 0;
            t.SelectedImageIndex = 1;
            treeView1.Nodes.Add(t);

            docDs.categoryTable.Rows.Add(new Object[] { Convert.ToInt32(t.Name), t.Text, 0 });
            form.Dispose();
        }
        private void deleteSelectedCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {

            TreeNode tn = treeView1.SelectedNode;
            if (tn == null)
            {
                MessageBox.Show("Please select category.");
                return;
            }

            DialogResult result;
            result = MessageBox.Show("Delete selected category?", "Deleting...", MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
            {
                return;
            }

            if (tn.Nodes.Count > 0)
            {
                MessageBox.Show("Deleting category with sub-category is not allowed.");
                return;
            }

            DataRow[] docTreeRelation = CategoryWithDocs(tn.Name);
            if (docTreeRelation.Count() > 0)
            {
                MessageBox.Show("Deleting category with scanned documents is not allowed.");
                return;
            }

            DataRow[] treeResult = docDs.categoryTable.Select(string.Format("Convert(code,'System.Int32') = {0}", tn.Name));
            //docDs.categoryTable.Rows.Remove(treeResult[0]);
            docDs.categoryTable.Rows[docDs.categoryTable.Rows.IndexOf(treeResult[0])].Delete();
            treeView1.Nodes.Clear();
            SetButtons(EBUTTONSTATE.OPEN);
            GeneralBox.PopulateTreeView(ref treeView1, docDs.categoryTable, 0, null);
            //tn.Nodes.Remove(tn);
        }
        private void moveSelectedCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (tn == null)
            {
                MessageBox.Show("Please select category.");
                return;
            }

            DataRow[] docResult = docDs.categoryTable.Select(string.Format("Convert(code,'System.Int32') = {0}", tn.Name));

            if (docResult.Count() == 0)
            {
                MessageBox.Show("Invalid category.");
                return;
            }

            formCategoryMove form = new formCategoryMove();
            form.dt = docDs.categoryTable;
            form.dr = docResult[0];

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            DataRow[] docTreeRelation = CategoryWithDocs(form.dr["code"].ToString());
            if (docTreeRelation.Count() > 0)
            {
                MessageBox.Show("Moving category with scanned documents is not allowed.");
                return;
            }

            docResult = docDs.categoryTable.Select(string.Format("Convert(code,'System.Int32') = {0}", docResult[0]["code"]));
            docResult[0]["parent_node"] = form.dr["code"];
            docDs.categoryTable.LoadDataRow(docResult[0].ItemArray.ToArray(), LoadOption.Upsert);

            treeView1.Nodes.Clear();
            SetButtons(EBUTTONSTATE.OPEN);
            GeneralBox.PopulateTreeView(ref treeView1, docDs.categoryTable, 0, null);
        }
        private void moveAsRootCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (tn == null)
            {
                MessageBox.Show("Please select category.");
                return;
            }

            DialogResult result;
            result = MessageBox.Show("Make selected as root category?", "Moving category...", MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
            {
                return;
            }

            DataRow[] docResult = docDs.categoryTable.Select(string.Format("Convert(code,'System.Int32') = {0}", tn.Name));

            if (docResult.Count() == 0)
            {
                MessageBox.Show("Invalid category.");
                return;
            }

            docResult[0]["parent_node"] = 0;
            docDs.categoryTable.LoadDataRow(docResult[0].ItemArray.ToArray(), LoadOption.Upsert);
            treeView1.Nodes.Clear();
            SetButtons(EBUTTONSTATE.OPEN);
            GeneralBox.PopulateTreeView(ref treeView1, docDs.categoryTable, 0, null);
        }
        private DataRow[] CategoryWithDocs(string code)
        {
            DataRow[] treeResult = docDs.categoryTable.Select(string.Format("Convert(code,'System.Int32') = {0}", code));
            DataRow[] catDocRelation = treeResult[0].GetChildRows(docDs.Relations["catDocRelation"]);
            return catDocRelation;
        }
        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            TreeNode t = treeView1.SelectedNode;
            if (t == null)
            {
                MessageBox.Show("Please select category.");
                return;
            }

            formCategoryRename form = new formCategoryRename();
            form.categoryName = t.Text;

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            if (form.categoryName == "") return;
            t.Text = form.categoryName;

            DataRow[] docResult = docDs.categoryTable.Select(string.Format("Convert(code,'System.Int32') = {0}", t.Name));

            if (docResult.Count() == 0)
            {
                MessageBox.Show("Invalid category.");
                return;
            }

            docResult[0]["ac_name"] = t.Text;
            docDs.categoryTable.LoadDataRow(docResult[0].ItemArray.ToArray(), LoadOption.Upsert);

            //treeView1.Nodes.Clear();
            //PopulateTreeView(docDs.categoryTable, 0, null);

        }
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void toolStripFind_Click(object sender, EventArgs e)
        {
            findDocumentsToolStripMenuItem_Click(sender, e);

        }
        private void toolStripRotateLeft_Click(object sender, EventArgs e)
        {
            rotateDocument(270);
        }
        private void toolStripRotateRight_Click(object sender, EventArgs e)
        {
            rotateDocument(90);
        }
        public void rotateDocument(float degrees)
        {
            if (listView1.SelectedItems.Count == 0) return;
            try
            {
                SetButtons(EBUTTONSTATE.FILENOTIMAGE);
                DataRow drDocs = (DataRow)listView1.SelectedItems[0].Tag;
                if (!File.Exists(drDocs["DocFilename"].ToString())) return;

                labelLoading.Visible = true;
                statusStrip1.Items["toolStripStatusLabel1"].Text = "Please wait, Rotating Photo...";
                photoEvent.RenderWithOverlay(drDocs, degrees);
            }
            catch (Exception ex)
            {
                imagePanel1.Image = null;
                MessageBox.Show(ex.Message, this.Name);
            }
        }
        private void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            DataRow drTag = (DataRow)listView1.SelectedItems[currentPage].Tag;
            if (File.Exists(drTag["DocFilename"].ToString()))
            {
                byte[] imageBytes = File.ReadAllBytes(drTag["DocFilename"].ToString());
                if (GeneralBox.TryParseBoolean(drTag["DocEncrypt"].ToString()))
                {
                    using (MagickImage imgPrint = new MagickImage(imageBytes, magickSettingsEnchiper))
                    {
                        imgPrint.Decipher(encipherDecipher);
                        if (docDs.GetSettingBooleanValue("Doc007"))
                        {
                            string overlayPath = docDs.GetSettingStringValue("Doc008");
                            using (MagickImage imageOverlay = new MagickImage(overlayPath))
                            {
                                imageOverlay.Scale(imgPrint.Width, imgPrint.Height);
                                imgPrint.Composite(imageOverlay, Gravity.Center, CompositeOperator.Over);
                            }
                        }
                        e.Graphics.DrawImage(imgPrint.ToBitmap(), new Point(0, 0));
                    }
                }
                if (!GeneralBox.TryParseBoolean(drTag["DocEncrypt"].ToString()))
                {
                    using (MagickImage imgPrint = new MagickImage(imageBytes, magickSettingsNoEnchiper))
                    {
                        if (docDs.GetSettingBooleanValue("Doc007"))
                        {
                            string overlayPath = docDs.GetSettingStringValue("Doc008");
                            using (MagickImage imageOverlay = new MagickImage(overlayPath))
                            {
                                imageOverlay.Scale(imgPrint.Width, imgPrint.Height);
                                imgPrint.Composite(imageOverlay, Gravity.Center, CompositeOperator.Over);
                            }
                        }
                        e.Graphics.DrawImage(imgPrint.ToBitmap(), new Point(0, 0));
                    }
                }
            }


            currentPage++;
            e.HasMorePages = currentPage < listView1.SelectedItems.Count;
        }
        private void toolStripDescription_Click(object sender, EventArgs e)
        {
            openDocumentToolStripMenuItem_Click(sender, e);
        }
        private void moveToCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("No document selected.");
                return;
            }

            if (treeView1.Nodes.Count == 0)
            {
                MessageBox.Show("No category on the tree.");
                return;
            }

            DataTable dtSelected = docDs.documentTable.Clone();
            foreach (ListViewItem d in listView1.SelectedItems)
            {
                DataRow drTag = (DataRow)d.Tag;
                dtSelected.LoadDataRow(drTag.ItemArray.ToArray(), LoadOption.Upsert);
            }

            formDocumentsMove form = new formDocumentsMove();
            form.dtDocs = dtSelected;
            form.dtCategory = docDs.categoryTable;

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            DataRow[] categoryNode = docDs.categoryTable.Select(string.Format("Convert(parent_node,'System.Int32') = {0}", form.dr["code"].ToString()));
            if (categoryNode.Count() > 0)
            {
                MessageBox.Show("Moving documents to category with sub-category is not allowed.");
                return;
            }

            foreach (DataRow d in dtSelected.Rows)
            {
                d["code"] = form.dr["code"];
                d["ModifiedDate"] = System.DateTime.Now;
                docDs.documentTable.LoadDataRow(d.ItemArray.ToArray(), LoadOption.Upsert);
                if (dtDocFilter.Rows.Count > 0)
                {
                    dtDocFilter.LoadDataRow(d.ItemArray.ToArray(), LoadOption.Upsert);
                }
            }

            DataTable fillListView;
            fillListView = dtDocFilter.Clone();
            if (listView1.Tag != null)
            {
                DataRow[] fill = docDs.documentTable.Select(string.Format("Convert(code,'System.Int32') = {0}", ((TreeNode)listView1.Tag).Name));
                if (fill.Count() > 0)
                {
                    fillListView = fill.CopyToDataTable();
                }
            }

            PopulateListView(fillListView);

        }
        private void openDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("No document selected.");
                return;
            }

            DataTable dtSelected = docDs.documentTable.Clone();
            foreach (ListViewItem d in listView1.SelectedItems)
            {
                DataRow drTag = (DataRow)d.Tag;
                dtSelected.LoadDataRow(drTag.ItemArray.ToArray(), LoadOption.OverwriteChanges);
            }

            forDocumentsView form = new forDocumentsView();
            form.dtDoc = dtSelected;

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            foreach (DataRow r in form.dtDoc.Rows)
            {
                docDs.documentTable.LoadDataRow(r.ItemArray.ToArray(), LoadOption.Upsert);
                if (dtDocFilter.Rows.Count > 0)
                {
                    dtDocFilter.LoadDataRow(r.ItemArray.ToArray(), LoadOption.Upsert);
                }
            }

            DataTable fillListView;
            if (listView1.Tag == null)
            {
                fillListView = dtDocFilter;
            }
            else
            {
                fillListView = docDs.documentTable.Select(string.Format("Convert(code,'System.Int32') = {0}", ((TreeNode)listView1.Tag).Name)).CopyToDataTable();
            }

            PopulateListView(fillListView);

        }
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void deleteDocumentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("No document selected.");
                return;
            }

            DialogResult result;
            result = MessageBox.Show("Do you want to delete this document?", "Deleting...", MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
            {
                return;
            }

            foreach (ListViewItem d in listView1.SelectedItems)
            {
                DataRow drTag = (DataRow)d.Tag;
                DataRow[] selectResult = docDs.documentTable.Select(string.Format("Convert(DocId,'System.Int32') = {0}", drTag["DocId"]));
                //docDs.documentTable.Rows.Remove(selectResult[0]);
                docDs.documentTable.Rows[docDs.documentTable.Rows.IndexOf(selectResult[0])].Delete();

                if (dtDocFilter.Rows.Count > 0)
                {
                    selectResult = dtDocFilter.Select(string.Format("Convert(DocId,'System.Int32') = {0}", drTag["DocId"]));
                    dtDocFilter.Rows.Remove(selectResult[0]);
                }

                string sPath = GetPathData(drTag["DocFilename"].ToString());
                File.Delete(sPath);

            }

            DataTable fillListView;
            if (listView1.Tag == null)
            {
                fillListView = dtDocFilter;
            }
            else
            {
                DataRow[] drFill = docDs.documentTable.Select(string.Format("Convert(code,'System.Int32') = {0}", ((TreeNode)listView1.Tag).Name));
                if (drFill.Count() == 0)
                {
                    fillListView = docDs.documentTable.Clone();
                }
                else
                {
                    fillListView = drFill.CopyToDataTable();
                }
            }

            PopulateListView(fillListView);
        }
        private void toolStripImport_Click(object sender, EventArgs e)
        {
            importImageDocumentsToolStripMenuItem_Click(sender, e);
        }
        public void AddDocuments(string code, DataTable dt)
        {
            List<object> eResult = new List<object>();
            eResult.Add(code);
            eResult.Add(dt);

            statusStrip1.Items["toolStripStatusLabel1"].Text = "Please wait, Processing imports...";
            lock (this.m_syncObject)
                this.m_processQueue.Enqueue(eResult);
            if (!this.bwImport.IsBusy)
                this.bwImport.RunWorkerAsync();
        }
        public string GetPathData(string Filename)
        {
            string folderDirectory = docDs.GetAppPath("AppData");
            return Path.Combine(folderDirectory, Filename);
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (imagePanel1.Image == null) return;
            imagePanel1.Zoom = trackBar1.Value * 0.01f;
            lblZoom.Text = Convert.ToString((Math.Round(trackBar1.Value * 0.01f * 100))) + "%";
        }       
        private void administratorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void toolStripButtonCrop_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("No document selected.");
                return;
            }

            if (listView1.SelectedItems.Count > 1)
            {
                MessageBox.Show("Please select one document only.");
                return;
            }

            DataRow drTag = (DataRow)listView1.SelectedItems[0].Tag;

            formPhotoEdit  form = new formPhotoEdit();
            form.fileName = drTag["DocFilename"].ToString();

            if(GeneralBox.TryParseBoolean(drTag["DocEncrypt"].ToString()))
            {
                form.encipherDecipher = encipherDecipher;
            }
            else
            {
                form.encipherDecipher = "";
            }

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            drTag["DocSize"] = string.Format("{0:n0} KB", Math.Round((double)new FileInfo(drTag["DocFilename"].ToString()).Length / 1024));
            docDs.documentTable.LoadDataRow(drTag.ItemArray,LoadOption.Upsert);
            listView1.SelectedItems[0].Tag = drTag;
            listView1.SelectedItems[0].SubItems["DocSize"].Text = drTag["DocSize"].ToString();
            listView1_SelectedIndexChanged(sender, e);
            form.Dispose();

        }
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void settingsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (docDs.GetSettingBooleanValue("System008"))
            {
                while (1 == 1)
                {
                    formLogin frm = new formLogin();
                    var r = frm.ShowDialog();
                    if (r != DialogResult.OK)
                    {
                        frm.Dispose();
                        return;
                    }

                    if (settingDS.ValidateAdmin(frm.password1.Trim()))
                    {
                        frm.Dispose();
                        break;
                    }
                    else
                    {
                        frm.Dispose();
                        MessageBox.Show("Invalid admin password.");
                    }

                }
            }

            formSettings st = new formSettings();
            st.dtSettings = docDs.settingTable.Copy();
            st.dtAdmin = settingDS.settingTable;

            var result = st.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            settingDS.SaveSetting();

            foreach (DataRow r in st.dtSettings.Rows)
            {
                docDs.settingTable.LoadDataRow(r.ItemArray, false);
            }
            //docDs.settingTable.AcceptChanges();
            OfSettings();
        }
        private void saveAllChangesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            docDs.saveDataset();
            MessageBox.Show("Current state successfully save.");
        }
        private void findDocumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.Nodes.Count == 0)
            {
                MessageBox.Show("Category tree is empty.");
                return;
            }

            formFind form = new formFind();

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            treeView1.Nodes.Clear();
            SetButtons(EBUTTONSTATE.OPEN);
            GeneralBox.PopulateTreeView(ref treeView1, docDs.categoryTable, 0, null);

            if (form.findString[0] == "document")
            {
                string setDocFilter = string.Format("Convert(code,'System.Int32') > 0 and (DocDesc like '%{0}%' or DocName like '%{0}%' or DocOcr like '%{0}%')", form.findString[1]);
                DataRow[] docResult = docDs.documentTable.Select(setDocFilter);
                if (docResult.Count() == 0)
                {
                    dtDocFilter.Rows.Clear();
                    MessageBox.Show("Cannot find documents contains '" + form.findString[1] + "'.");
                    return;
                }

                dtDocFilter.Rows.Clear();
                foreach (DataRow r in docResult)
                {
                    dtDocFilter.LoadDataRow(r.ItemArray.ToArray(), LoadOption.OverwriteChanges);
                }

                listView1.Tag = null;
                SetButtons(EBUTTONSTATE.TREEWITHOUTDOCS);
                PopulateListView(dtDocFilter);
            }

            if (form.findString[0] == "date")
            {
                string setDocDateFilter = "";
                if (form.findString[1] == "Scanned Date")
                {
                    setDocDateFilter = string.Format("ScannedDate >= #{0}# and ScannedDate <= #{1}#", form.findString[2] + " 00:00:00", form.findString[3] + " 23:59:59");
                }
                else
                {
                    setDocDateFilter = string.Format("ModifiedDate >= #{0}# and ModifiedDate <= #{1}#", form.findString[2] + " 00:00:00", form.findString[3] + " 23:59:59");
                }

                DataRow[] docResult = docDs.documentTable.Select(setDocDateFilter);
                if (docResult.Count() == 0)
                {
                    dtDocFilter.Rows.Clear();
                    MessageBox.Show("No record found for date between " + form.findString[2] + " and " + form.findString[3] + ".");
                    return;
                }

                dtDocFilter.Rows.Clear();
                foreach (DataRow r in docResult)
                {
                    dtDocFilter.LoadDataRow(r.ItemArray.ToArray(), LoadOption.OverwriteChanges);
                }

                listView1.Tag = null;
                SetButtons(EBUTTONSTATE.TREEWITHOUTDOCS);
                PopulateListView(dtDocFilter);
            }
        }
        private void scanDocumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (tn == null)
            {
                MessageBox.Show("Please select category.");
                return;
            }

            if (tn.Nodes.Count > 0)
            {
                MessageBox.Show("Cannot assigned documents for category with sub-category.");
                return;
            }

            formDocumentsImport form = new formDocumentsImport();
            form.dtDoc = docDs.documentTable.Clone();
            form.importType = "SCAN";
            form.scannedQuality = docDs.GetSettingIntValue("Doc005");
            form.importQuality = docDs.GetSettingIntValue("Doc006");
            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            AddDocuments(tn.Name, form.dtDoc);
        }
        private void importImageDocumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (tn == null)
            {
                MessageBox.Show("Please select category.");
                return;
            }

            if (tn.Nodes.Count > 0)
            {
                MessageBox.Show("Importing document into category with sub-category is not allowed.");
                return;
            }

            formDocumentsImport form = new formDocumentsImport();
            form.dtDoc = docDs.documentTable.Clone();
            form.importType = "PHOTO";
            form.scannedQuality = docDs.GetSettingIntValue("Doc005");
            form.importQuality = docDs.GetSettingIntValue("Doc006");
            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            AddDocuments(tn.Name, form.dtDoc);
        }
        private void printDocumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 0) return;

            DialogResult dialogResult = MessageBox.Show("Do you want to print selected document?", "Printing...", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                return;
            }

            try
            {
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
                PrintDialog dlg = new PrintDialog();
                dlg.Document = printDoc;
                var result = dlg.ShowDialog();

                if (result == DialogResult.OK)
                {
                    currentPage = 0;
                    printDoc.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Name);
            }
        }
        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            formAbout form = new formAbout();
            form.dtSetting = settingDS.settingTable;
            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
        }
        private void importFromPDFDocumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (tn == null)
            {
                MessageBox.Show("Please select category.");
                return;
            }

            if (tn.Nodes.Count > 0)
            {
                MessageBox.Show("Importing pdf document into category with sub-category is not allowed.");
                return;
            }

            formDocumentsImport form = new formDocumentsImport();
            form.dtDoc = docDs.documentTable.Clone();
            form.importType = "PDF";
            form.scannedQuality = docDs.GetSettingIntValue("Doc005");
            form.importQuality = docDs.GetSettingIntValue("Doc006");
            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            AddDocuments(tn.Name, form.dtDoc);
        }
        private void exportToPDFToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 0) return;

            DialogResult dialogResult = MessageBox.Show("Do you want to export selected document?", "Export to PDF...", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                return;
            }


            SaveFileDialog saveFileDialog1;
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.DefaultExt = "pdf";
            saveFileDialog1.Filter = "Pdf Document|*.pdf";
            saveFileDialog1.Title = "Export to PDF";
            DialogResult dr = saveFileDialog1.ShowDialog();

            if (dr != DialogResult.OK)
            {
                return;
            }

            string filename = saveFileDialog1.FileName;

            using (MagickImageCollection collection = new MagickImageCollection())
            {
                foreach (ListViewItem i in listView1.SelectedItems)
                {
                    DataRow r = (DataRow)i.Tag;
                    byte[] imageBytes = File.ReadAllBytes(r["DocFilename"].ToString());

                    if (GeneralBox.TryParseBoolean(r["DocEncrypt"].ToString()))
                    {

                        MagickImage imgPDF = new MagickImage(imageBytes);
                        imgPDF.Decipher(encipherDecipher);
                        if (docDs.GetSettingBooleanValue("Doc007"))
                        {
                            byte[] overlayBytes = File.ReadAllBytes(docDs.GetSettingStringValue("Doc008"));
                            MagickImage imageOverlay = new MagickImage(overlayBytes);

                            imageOverlay.Scale(imgPDF.Width, imgPDF.Height);
                            imgPDF.Composite(imageOverlay, Gravity.Center, CompositeOperator.Over);
                        }
                        collection.Add(imgPDF);
                    }

                    if (!GeneralBox.TryParseBoolean(r["DocEncrypt"].ToString()))
                    {
                        MagickImage imgPDF = new MagickImage(imageBytes);
                        if (docDs.GetSettingBooleanValue("Doc007"))
                        {
                            byte[] overlayBytes = File.ReadAllBytes(docDs.GetSettingStringValue("Doc008"));
                            MagickImage imageOverlay = new MagickImage(overlayBytes);

                            imageOverlay.Scale(imgPDF.Width, imgPDF.Height);
                            imgPDF.Composite(imageOverlay, Gravity.Center, CompositeOperator.Over);
                        }
                        collection.Add(imgPDF);
                    }
                }
                collection.Write(filename);
            }
            
        }
        private void archiveCategoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow[] drTempFrom = docDs.categoryTable.Select(string.Format("Convert(parent_node,'System.Int32') = {0}", -99), "ac_name ASC");

            if (drTempFrom.Count() == 0)
            {
                MessageBox.Show("Disabled category is empty.");
                return;
            }

            formCategoryDisabled form = new formCategoryDisabled();
            form.dt = docDs.categoryTable;

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            if (form.isRoot)
            {
                form.drFrom["parent_node"] = 0;
            }
            else
            {
                DataRow[] docTreeRelation = CategoryWithDocs(form.drTo["code"].ToString());
                if (docTreeRelation.Count() > 0)
                {
                    MessageBox.Show("Moving category with scanned documents is not allowed.");
                    return;
                }
                form.drFrom["parent_node"] = form.drTo["code"];
            }

            docDs.categoryTable.LoadDataRow(form.drFrom.ItemArray.ToArray(), LoadOption.Upsert);

            treeView1.Nodes.Clear();
            SetButtons(EBUTTONSTATE.OPEN);
            GeneralBox.PopulateTreeView(ref treeView1, docDs.categoryTable, 0, null);
        }
        private void resetApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Reset entire application?", "Factory Reset...", MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
            {
                return;
            }

            docDs.BackupApplication("R");
            docDs.Reset();
            treeView1.Nodes.Clear();
            docDs.FactoryReset();
            SetButtons(EBUTTONSTATE.OPEN);
            GeneralBox.PopulateTreeView(ref treeView1, docDs.categoryTable, 0, null);
            SetButtons(EBUTTONSTATE.OPEN);
            OfSettings();
        }
        private void backupApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DialogResult result;
            result = MessageBox.Show("Create Backup for this application?", "Backup...", MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
            {
                return;
            }

            docDs.BackupApplication("B");
            MessageBox.Show("Application backup successfully created.\r\n" + docDs.GetAppPath("BackupData"));
        }
        private void viewDataTablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formDataTables form = new formDataTables();
            form.docDs = docDs;

            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                //MessageBox.Show("Done, update TreeView if there a changes");
            }
        }
    }
}

