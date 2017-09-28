using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DocumentManager
{
    public partial class DocScanner : Form
    {
        DocDataset docDs = new DocDataset();
        DataTable dtDocFilter;
        Int32 currentPage = 0;
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

        private void SetButtons(EBUTTONSTATE a_ebuttonstate)
        {
            switch (a_ebuttonstate)
            {
                default:
                case EBUTTONSTATE.CATEGORYSTRIPDOC:
                    toolStripTree.Enabled = true;
                    toolStripTree.Items["toolStripImport"].Enabled = true;
                    toolStripTree.Items["toolStripScan"].Enabled = true;
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
                    toolStripImage.Items["toolStripPrint"].Enabled = true;
                    toolStripImage.Items["toolStripDescription"].Enabled = true;
                    toolStripImage.Items["toolStripDelete"].Enabled = true;
                    toolStripImage.Items["toolStripRotateLeft"].Enabled = true;
                    toolStripImage.Items["toolStripRotateRight"].Enabled = true;
                    contextMenuDocs.Enabled = true;
                    break;
                case EBUTTONSTATE.MULTIPLEPHOTO:
                    imagePanel1.Image = null;
                    toolStripImage.Enabled = true;
                    menuStrip1.Items["documentsToolStripMenuItem"].Enabled = true;
                    trackBar1.Enabled = false;
                    toolStripImage.Enabled = true;
                    toolStripImage.Items["toolStripPrint"].Enabled = true;
                    toolStripImage.Items["toolStripDescription"].Enabled = true;
                    toolStripImage.Items["toolStripDelete"].Enabled = true;
                    toolStripImage.Items["toolStripRotateLeft"].Enabled = false;
                    toolStripImage.Items["toolStripRotateRight"].Enabled = false;
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
        public DocScanner()
        {
            InitializeComponent();
        }

        private void DocScanner_Load(object sender, EventArgs e)
        {
            docDs.createDataset();
            docDs.readDataFromXML("data.xml");
            docDs.AcceptChanges();
            dtDocFilter = docDs.Tables["document"].Clone();
            dtDocFilter.Rows.Clear();
            dtDocFilter.TableName = "find";
            PopulateTreeView(docDs.Tables["tree"], 0, null);
            treeView1.Focus();
            treeView1.Nodes[0].TreeView.Select();
            lblZoom.Text = Convert.ToString(Math.Round( (trackBar1.Value * 0.02f * 100) / 2)) + "%";
        }

        private void PopulateTreeView(DataTable _acountsTb, Int32 parentId, TreeNode parentNode)
        {
            
            SetButtons(EBUTTONSTATE.OPEN);

            if (docDs.Tables["tree"].Rows.Count == 0)
            {
                return;
            }

            TreeNode childNode;

            DataRow[] docResult = _acountsTb.Select(String.Format(" Convert(parent_node,'System.Int32') = {0}", parentId));
            foreach (DataRow dr in docResult)
            {
                if (Convert.ToInt32(dr["code"]) < 0)
                {
                    continue;
                }

                TreeNode t = new TreeNode();
                //t.Text = dr["code"].ToString() + " - " + dr["ac_name"].ToString();
                t.Text = dr["ac_name"].ToString();
                t.Name = dr["code"].ToString();
                t.Tag = _acountsTb.Rows.IndexOf(dr);
                Int32 treeNode = 0;
                if (parentNode == null)
                {
                    treeNode = treeView1.Nodes.Add(t);
                    childNode = t;
                }
                else
                {
                    treeNode = parentNode.Nodes.Add(t);
                    childNode = t;
                }

                if (childNode.Level == 0)
                {
                    childNode.ImageIndex = 0;
                    childNode.SelectedImageIndex = 1;
                }
                else if (childNode.Level == 1)
                {
                    childNode.ImageIndex = 2;
                    childNode.SelectedImageIndex = 3;
                }
                else if (childNode.Level == 2)
                {
                    childNode.ImageIndex = 4;
                    childNode.SelectedImageIndex = 5;
                }
                else if (childNode.Level == 3)
                {
                    childNode.ImageIndex = 6;
                    childNode.SelectedImageIndex = 7;
                }
                else
                {
                    childNode.ImageIndex = 6;
                    childNode.SelectedImageIndex = 7;
                }

                PopulateTreeView(_acountsTb, Convert.ToInt32(dr["code"].ToString()), childNode);
                foreach (TreeNode n in treeView1.Nodes)
                {
                    n.Expand();
                }
            }
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
            DataRow[] docResult = docDs.Tables["tree"].Select(String.Format("Convert(code,'System.Int32') = {0}",e.Node.Name));
            DataRow[] docResults = docResult[0].GetChildRows(docDs.Relations["treeDocRelation"]);
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

            if(dt.TableName == "find")
            {
                listView1.Columns.Add("DocId", 60);
                listView1.Columns.Add("Category", 60);
                listView1.Columns.Add("Title", 100);
                listView1.Columns.Add("Description", 180);
                listView1.Columns.Add("Scanned Date", 130);
                listView1.Columns.Add("Modified Date", 130);
                listView1.Columns.Add("Size", 80);

                foreach (DataRow r in dt.Rows)
                {
                    DataRow[] docResult = docDs.Tables["tree"].Select(String.Format("Convert(code,'System.Int32') = {0}", r["code"].ToString()));

                    if (docResult.Count() > 0)
                    {
                        ListViewItem new_item = new ListViewItem();
                        new_item.Text =  r["DocId"].ToString();
                        new_item.ImageIndex = 7;
                        new_item.SubItems.Add(docResult[0]["ac_name"].ToString());
                        new_item.SubItems.Add(r["DocName"].ToString());
                        new_item.SubItems.Add(r["DocDesc"].ToString());
                        new_item.SubItems.Add(r["ScannedDate"].ToString());
                        new_item.SubItems.Add(r["ModifiedDate"].ToString());
                        new_item.Tag = r;
                        listView1.Items.Add(new_item);
                    }
                }
            }
            else
            {
                listView1.Columns.Add("DocId", 60);
                listView1.Columns.Add("Title", 100);
                listView1.Columns.Add("Description", 180);
                listView1.Columns.Add("Scanned Date", 130);
                listView1.Columns.Add("Modified Date", 130);
                listView1.Columns.Add("Size", 80);

                foreach (DataRow r in dt.Rows)
                {
                    ListViewItem new_item = new ListViewItem();
                    new_item.Text = r["DocId"].ToString();
                    new_item.ImageIndex = 3;
                    new_item.SubItems.Add(r["DocName"].ToString());
                    new_item.SubItems.Add(r["DocDesc"].ToString());
                    new_item.SubItems.Add(r["ScannedDate"].ToString());
                    new_item.SubItems.Add(r["ModifiedDate"].ToString());
                    new_item.Tag = r;
                    listView1.Items.Add(new_item);
                }
            }
        }
        private void toolStripSaveAll_Click(object sender, EventArgs e)
        {
            docDs.saveDataset();
            MessageBox.Show("Current state successfully save.");
        }

        private void toolStripStartScan_Click(object sender, EventArgs e)
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

            Scanner form = new Scanner();
            form.categoryCode = tn.Name;
            form.categoryName = tn.Text;

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
        }

        private void scanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStartScan_Click(sender, e);
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
            else
            {
                SetButtons(EBUTTONSTATE.SINGLEPHOTO);
            }

            DataRow drDocs = (DataRow)listView1.SelectedItems[0].Tag;
            String sDocId = drDocs["DocId"].ToString();
            DataRow[] docResult = docDs.Tables["document"].Select(String.Format("Convert(DocId,'System.Int32') = {0}",sDocId));            
            
            if (!File.Exists(GetPathData(sDocId+".64")))
            {
                SetButtons(EBUTTONSTATE.FILENOTIMAGE);
                return;
            }
            try
            {
                Image img = Base64ToImage(docResult[0]);
                imagePanel1.Image = (Bitmap)img;

                imagePanel1.Zoom = trackBar1.Value * 0.02f;
                lblZoom.Text = Convert.ToString(Math.Round(trackBar1.Value * 0.02f * 100) / 2) + "%";
                
            } catch (Exception)
            {
                SetButtons(EBUTTONSTATE.FILENOTIMAGE);
                return;
            }
        }

        private void categoryTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CategoryTree form = new CategoryTree();
            form.docDs = docDs;

            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                //MessageBox.Show("Done, update TreeView if there a changes");
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

            AddCategory form = new AddCategory();
            form.treeCode = t.Name;

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            DataRow[] docTreeRelation = CategoryWithDocs(tn.Name);
            if (docTreeRelation.Count() > 0)
            {
                MessageBox.Show("Deleting category with scanned documents is not allowed.");
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

            docDs.Tables["tree"].Rows.Add(new Object[] { Convert.ToInt32(t.Name), t.Text, Convert.ToInt32(tn.Name) });
            form.Dispose();
        }

        private void addRootCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode t = new TreeNode();
            t.Name = Convert.ToString(docDs.GetTreeCode());

            AddCategory form = new AddCategory();
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

            docDs.Tables["tree"].Rows.Add(new Object[] { Convert.ToInt32(t.Name), t.Text, 0 });
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
            if (docTreeRelation.Count()>0)
            {
                MessageBox.Show("Deleting category with scanned documents is not allowed.");
                return;
            }

            DataRow[] treeResult = docDs.Tables["tree"].Select(String.Format("Convert(code,'System.Int32') = {0}", tn.Name));
            docDs.Tables["tree"].Rows.Remove(treeResult[0]);
            PopulateTreeView(docDs.Tables["tree"], 0, null);        
            //tn.Nodes.Remove(tn);
        }

        private void factoryResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Reset entire application?", "Factory Reset...", MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
            {
                return;
            }

            docDs.Reset();
            treeView1.Nodes.Clear();
            docDs.FactoryReset();
            PopulateTreeView(docDs.Tables["tree"], 0, null);
            SetButtons(EBUTTONSTATE.OPEN);
        }

        private void moveSelectedCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (tn == null)
            {
                MessageBox.Show("Please select category.");
                return;
            }

            DataRow[] docResult = docDs.Tables["tree"].Select(String.Format("Convert(code,'System.Int32') = {0}", tn.Name));

            if (docResult.Count() == 0)
            {
                MessageBox.Show("Invalid category.");
                return;
            }

            MoveCategory form = new MoveCategory();
            form.dt = docDs.Tables["tree"];
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

            docResult = docDs.Tables["tree"].Select(String.Format("Convert(code,'System.Int32') = {0}", docResult[0]["code"]));
            docResult[0]["parent_node"] = form.dr["code"];
            docDs.Tables["tree"].LoadDataRow(docResult[0].ItemArray.ToArray(),LoadOption.Upsert);

            treeView1.Nodes.Clear();
            PopulateTreeView(docDs.Tables["tree"], 0, null);
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

            DataRow[] docResult = docDs.Tables["tree"].Select(String.Format("Convert(code,'System.Int32') = {0}", tn.Name));

            if (docResult.Count() == 0)
            {
                MessageBox.Show("Invalid category.");
                return;
            }

            docResult[0]["parent_node"] = 0;
            docDs.Tables["tree"].LoadDataRow(docResult[0].ItemArray.ToArray(), LoadOption.Upsert);
            treeView1.Nodes.Clear();
            PopulateTreeView(docDs.Tables["tree"], 0, null);
        }

        private DataRow[] CategoryWithDocs(String code)
        {
            DataRow[] treeResult = docDs.Tables["tree"].Select(String.Format("Convert(code,'System.Int32') = {0}", code));
            DataRow[] treeDocRelation = treeResult[0].GetChildRows(docDs.Relations["treeDocRelation"]);
            return treeDocRelation;
        }
        private void disabledCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow[] drTempFrom = docDs.Tables["tree"].Select(String.Format("Convert(parent_node,'System.Int32') = {0}", -99), "ac_name ASC");

            if (drTempFrom.Count() == 0)
            {
                MessageBox.Show("Disabled category is empty.");
                return;
            }

            DisabledCategory form = new DisabledCategory();
            form.dt = docDs.Tables["tree"];

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
            
            docDs.Tables["tree"].LoadDataRow(form.drFrom.ItemArray.ToArray(), LoadOption.Upsert);

            treeView1.Nodes.Clear();
            PopulateTreeView(docDs.Tables["tree"], 0, null);
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            TreeNode t = treeView1.SelectedNode;
            if (t == null)
            {
                MessageBox.Show("Please select category.");
                return;
            }

            RenameCategory form = new RenameCategory();
            form.categoryName = t.Text;

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            if (form.categoryName == "") return;
            t.Text = form.categoryName;

            DataRow[] docResult = docDs.Tables["tree"].Select(String.Format("Convert(code,'System.Int32') = {0}", t.Name));

            if (docResult.Count() == 0)
            {
                MessageBox.Show("Invalid category.");
                return;
            }

            docResult[0]["ac_name"] = t.Text;
            docDs.Tables["tree"].LoadDataRow(docResult[0].ItemArray.ToArray(), LoadOption.Upsert);

            //treeView1.Nodes.Clear();
            //PopulateTreeView(docDs.Tables["tree"], 0, null);

        }

        private void backupCurrentStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Currently not available.");
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Currently not available.");
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Document Manager. @06092017 version 1.0.0.0");
        }

        private void saveAllChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripSaveAll_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (docDs.HasChanges())
            {
                DialogResult result;
                result = MessageBox.Show("Do you want to save changes in your application?", "Closing...", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Cancel)
                {
                    return;
                }

                if (result == DialogResult.Yes)
                {
                    toolStripSaveAll_Click(sender, e);
                }

            }
            Close();
        }

        private void toolStripFind_Click(object sender, EventArgs e)
        {
            
            if (treeView1.Nodes.Count == 0)
            {
                MessageBox.Show("Category tree is empty.");
                return;
            }

            Find form = new Find();

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            treeView1.Nodes.Clear();
            PopulateTreeView(docDs.Tables["tree"], 0, null);

            if (form.findString[0] == "document")
            {
                String setDocFilter = String.Format("DocDesc like '%{0}%' or DocName like '%{0}%'", form.findString[1]);
                DataRow[] docResult = docDs.Tables["document"].Select(setDocFilter);
                if (docResult.Count() == 0 )
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
                String setDocDateFilter = "";
                if (form.findString[1] == "Scanned Date")
                {
                    setDocDateFilter = String.Format("ScannedDate >= #{0}# and ScannedDate <= #{1}#", form.findString[2] + " 00:00:00", form.findString[3] + " 23:59:59");
                }
                else
                {
                    setDocDateFilter = String.Format("ModifiedDate >= #{0}# and ModifiedDate <= #{1}#",form.findString[2] + " 00:00:00",form.findString[3] + " 23:59:59");
                }

                DataRow[] docResult = docDs.Tables["document"].Select(setDocDateFilter);
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

        private void toolStripRotateLeft_Click(object sender, EventArgs e)
        {
            int intselectedindex = listView1.SelectedIndices[0];

            if (listView1.SelectedItems.Count == 0) return;

            foreach (ListViewItem d in listView1.SelectedItems)
            {
                DataRow drTag = (DataRow)d.Tag;
                Image img = Base64ToImage(drTag);
                img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                imagePanel1.Image = (Bitmap)img;
                ImageToBase64(drTag,img);    
            }
        }

        private void toolStripRotateRight_Click(object sender, EventArgs e)
        {
            int intselectedindex = listView1.SelectedIndices[0];

            if (listView1.SelectedItems.Count == 0) return;

            foreach (ListViewItem d in listView1.SelectedItems)
            {                         
                try
                {
                    DataRow drTag = (DataRow)d.Tag;
                    Image img = Base64ToImage(drTag);
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    imagePanel1.Image = (Bitmap)img;
                    ImageToBase64(drTag,img);
                }
                catch (Exception ex)
                {
                    imagePanel1.Image = null;
                    MessageBox.Show(ex.Message, this.Name);             
                }
            }
        }

        private void toolStripPrint_Click(object sender, EventArgs e)
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
        private void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            DataRow drTag = (DataRow)listView1.SelectedItems[currentPage].Tag;
            Image img = Base64ToImage(drTag);            
            e.Graphics.DrawImage(img,new Point(0, 0));
            currentPage++;
            e.HasMorePages = currentPage < listView1.SelectedItems.Count;
        }

        private void toolStripDescription_Click(object sender, EventArgs e)
        {
            openDocumentToolStripMenuItem_Click(sender, e);
        }

        private void toolStripDelete_Click(object sender, EventArgs e)
        {
            deleteDocumentToolStripMenuItem1_Click(sender, e);
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripFind_Click(sender, e);
        }

        private void updateDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripDescription_Click(sender, e);
        }

        private void deleteDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripDelete_Click(sender, e);
        }

        private void printDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripPrint_Click(sender, e);
        }

        private void renameCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void moveToCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("No document selected.");
                return;
            }

            if(treeView1.Nodes.Count == 0)
            {
                MessageBox.Show("No category on the tree.");
                return;
            }

            DataTable dtSelected = docDs.DocTable().Clone();
            foreach (ListViewItem d in listView1.SelectedItems)
            {
                DataRow drTag = (DataRow)d.Tag;
                dtSelected.LoadDataRow(drTag.ItemArray.ToArray(),LoadOption.Upsert);
            }

            MoveDocuments form = new MoveDocuments();
            form.dtDocs = dtSelected;
            form.dtCategory = docDs.Tables["tree"];

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            DataRow[] categoryNode = docDs.Tables["tree"].Select(String.Format("Convert(parent_node,'System.Int32') = {0}", form.dr["code"].ToString()));
            if (categoryNode.Count() > 0)
            {
                MessageBox.Show("Moving documents to category with sub-category is not allowed.");
                return;
            }

            foreach (DataRow d in dtSelected.Rows)
            {
                d["code"] = form.dr["code"];
                d["ModifiedDate"] = System.DateTime.Now;
                docDs.Tables["document"].LoadDataRow(d.ItemArray.ToArray(), LoadOption.Upsert);
                if (dtDocFilter.Rows.Count > 0)
                {
                    dtDocFilter.LoadDataRow(d.ItemArray.ToArray(), LoadOption.Upsert);
                }
            }

            DataTable fillListView;
            if (listView1.Tag == null)
            {
                fillListView = dtDocFilter;
            }
            else
            {
                fillListView = docDs.Tables["document"].Select(String.Format("Convert(code,'System.Int32') = {0}", ((TreeNode)listView1.Tag).Name)).CopyToDataTable();
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

            DataTable dtSelected = docDs.DocTable().Clone();
            foreach (ListViewItem d in listView1.SelectedItems)
            {
                DataRow drTag = (DataRow)d.Tag;
                dtSelected.LoadDataRow(drTag.ItemArray.ToArray(), LoadOption.OverwriteChanges);
            }

            DocDescription form = new DocDescription();
            form.dtDoc = dtSelected;

            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            //docDs.Tables["document"].Merge(form.dtDoc);

            foreach (DataRow r in form.dtDoc.Rows)
            {
                docDs.Tables["document"].LoadDataRow(r.ItemArray.ToArray(), LoadOption.Upsert);
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
                fillListView = docDs.Tables["document"].Select(String.Format("Convert(code,'System.Int32') = {0}", ((TreeNode)listView1.Tag).Name)).CopyToDataTable();
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
                DataRow[] selectResult = docDs.Tables["document"].Select(String.Format("Convert(DocId,'System.Int32') = {0}", drTag["DocId"]));
                docDs.Tables["document"].Rows.Remove(selectResult[0]);
                if (dtDocFilter.Rows.Count > 0)
                {
                    selectResult = dtDocFilter.Select(String.Format("Convert(DocId,'System.Int32') = {0}", drTag["DocId"]));
                    dtDocFilter.Rows.Remove(selectResult[0]);
                }

                String sPath = GetPathData(drTag["DocId"].ToString() + ".64");
                File.Delete(sPath);

            }

            DataTable fillListView;
            if (listView1.Tag == null)
            {
                fillListView = dtDocFilter;
            }
            else
            {                
                DataRow[] drFill = docDs.Tables["document"].Select(String.Format("Convert(code,'System.Int32') = {0}", ((TreeNode)listView1.Tag).Name));
                if(drFill.Count() == 0)
                {
                    fillListView = docDs.DocTable().Clone();
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

            ImportDocuments form = new ImportDocuments();
            form.dtDoc = docDs.DocTable();
            var result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            AddDocuments(tn.Name, form.dtDoc);

        }

        public void AddDocuments(String code, DataTable dt)
        {
            foreach (DataRow r in dt.Rows)
            {
                DataRow d = docDs.Tables["document"].NewRow();
                d["DocName"] = r["DocName"];
                d["DocDesc"] = r["DocDesc"];
                d["DocFilename"] = r["DocFilename"];
                d["code"] = code;
                DataRow row64 = ImageToBase64(d,null);
                d["DocFilename"] = row64["DocFilename"];
                docDs.Tables["document"].Rows.Add(d);
            }
           
            DataRow[] docResult = docDs.Tables["tree"].Select(String.Format("Convert(code,'System.Int32') = {0}", code));
            DataRow[] docResults = docResult[0].GetChildRows(docDs.Relations["treeDocRelation"]);
            SetButtons(EBUTTONSTATE.TREEWITHOUTDOCS);
            if (docResults.Count() == 0) return;
            PopulateListView(docResults.CopyToDataTable());
        }

        public DataRow ImageToBase64(DataRow d,Image image)
        {
            if (image == null)
            {
                image = Image.FromFile(d["Docfilename"].ToString());
            }

            using (MemoryStream m = new MemoryStream())
            {
                try
                {

                    if (imagePanel1.Tag == null)
                    {
                        image.Save(m, image.RawFormat); 
                    }
                    else
                    {
                        image.Save(m, (System.Drawing.Imaging.ImageFormat)imagePanel1.Tag);
                    }
                }
                catch (Exception)
                {
                    image.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                byte[] imageBytes = m.ToArray();
                String s = Convert.ToBase64String(imageBytes);
                File.WriteAllText(GetPathData(d["DocId"].ToString() + ".64"), s);
                d["Docfilename"] = GetPathData(d["DocId"].ToString() + ".64");
            }
            
            return d;
        }

        public String GetPathData(String filename)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "DOCUMENTMANAGERABCD");
            if (!Directory.Exists(specificFolder))
            {
                Directory.CreateDirectory(specificFolder);
            }

            return Path.Combine(specificFolder, filename);
        }
        public Image Base64ToImage(DataRow d)
        {
            if(!File.Exists(GetPathData(d["DocId"].ToString() + ".64")))
            {
                return null;
            }
            Image img;
            String contents = File.ReadAllText(GetPathData(d["DocId"].ToString() + ".64"));
            byte[] imageBytes = Convert.FromBase64String(contents);
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                img = Image.FromStream(ms, true);
                imagePanel1.Tag =  img.RawFormat;
            }
            return img;  
        }

    private void importImageFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripImport_Click(sender, e);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (imagePanel1.Image == null) return;
            imagePanel1.Zoom = trackBar1.Value * 0.02f;
            lblZoom.Text = Convert.ToString((trackBar1.Value * 0.02f * 100) / 2) + "%";
        }
    }
}
