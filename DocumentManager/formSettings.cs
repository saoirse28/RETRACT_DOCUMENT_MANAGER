using System;
using System.Data;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace DocumentManager
{
    public partial class formSettings : Form
    {
        public DataTable dtSettings;
        public DataTable dtAdmin;
        public formSettings()
        {
            InitializeComponent();
        }
        private void Settings_Load(object sender, EventArgs e)
        {
            DataView view = new DataView(dtSettings);
            DataTable distinctValues = view.ToTable(true, "SetGroup");

            listView1.Groups.Clear();
            foreach (DataRow r in distinctValues.Rows)
            {
                ListViewGroup group = new ListViewGroup(r["SetGroup"].ToString(),r["SetGroup"].ToString());
                listView1.Groups.Add(group);
            }

            listView1.Columns.Clear();
            listView1.CheckBoxes = true;
            listView1.Columns.Add("Set ID", 60);
            listView1.Columns.Add("Set Code", 100);
            listView1.Columns.Add("Set Description", 180);
            listView1.Columns.Add("Created Date", 150);
            //listView1.Columns.Add("Modified1 Date", 130);

            foreach (DataRow r in dtSettings.Rows)
            {
                Boolean bParse;
                if (Boolean.TryParse(r["SetValue"].ToString().Trim(), out bParse))
                {
                    ListViewItem new_item = new ListViewItem();
                    new_item.Group = listView1.Groups[r["SetGroup"].ToString()];
                    new_item.Checked = bParse;
                    new_item.Text = r["SetId"].ToString();
                    new_item.ImageIndex = 3;
                    new_item.SubItems.Add(r["SetCode"].ToString());
                    new_item.SubItems.Add(r["SetDesc"].ToString());
                    new_item.SubItems.Add(r["CreatedDate"].ToString());
                    //new_item.SubItems.Add(r["ModifiedDate"].ToString());
                    new_item.Tag = r;
                    listView1.Items.Add(new_item);
                }
                else
                {
                    string SetCode = r["SetCode"].ToString();
                    string SetValue = r["SetValue"].ToString();
                    switch (SetCode)
                    {
                        case "AppData":
                            textBoxAppData.Text = SetValue;
                            break;
                        case "BackupData":
                            textBoxBackupData.Text = SetValue;
                            break;
                        case "Doc005":
                            textBoxScannedQuality.Text = SetValue;
                            break;
                        case "Doc006":
                            textBoxImportQuality.Text = SetValue;
                            break;
                        case "Doc008":
                            textBoxOverlayPhoto.Text = SetValue;
                            break;
                    }
                }
            }
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in listView1.Items)
            {
                DataRow r = (DataRow)i.Tag;
                r["SetValue"] = i.Checked;
                dtSettings.LoadDataRow(r.ItemArray, LoadOption.OverwriteChanges);

                if(r["SetCode"].ToString() == "System007" && i.Checked == true)
                {
                    DataRow[] drAdmin = dtAdmin.Select("AppCode='UserPass'");
                    if(drAdmin.Length == 0)
                    {
                        MessageBox.Show("User password must be set.");
                        return;
                    }

                }

                if (r["SetCode"].ToString() == "System008" && i.Checked == true)
                {
                    DataRow[] drAdmin = dtAdmin.Select("AppCode='AdminPass'");
                    if (drAdmin.Length == 0)
                    {
                        MessageBox.Show("Admin password must be set.");
                        return;
                    }
                }
            }



            DataRow[] dr = dtSettings.Select("SetCode='" + "AppData" + "'");
            if (dr.Length > 0)
            {
                dr[0]["SetValue"] = textBoxAppData.Text.Trim();
                dtSettings.LoadDataRow(dr[0].ItemArray, LoadOption.OverwriteChanges);
            }

            dr = dtSettings.Select("SetCode='" + "BackupData" + "'");
            if (dr.Length > 0)
            {
                dr[0]["SetValue"] = textBoxBackupData.Text.Trim();
                dtSettings.LoadDataRow(dr[0].ItemArray, LoadOption.OverwriteChanges);
            }

            dr = dtSettings.Select("SetCode='" + "Doc005" + "'"); //Scanned Document Quality
            if (dr.Length > 0)
            {
                dr[0]["SetValue"] = textBoxScannedQuality.Text.Trim();
                dtSettings.LoadDataRow(dr[0].ItemArray, LoadOption.OverwriteChanges);
            }

            dr = dtSettings.Select("SetCode='" + "Doc006" + "'"); //Import Document Quality
            if (dr.Length > 0)
            {
                dr[0]["SetValue"] = textBoxImportQuality.Text.Trim();
                dtSettings.LoadDataRow(dr[0].ItemArray, LoadOption.OverwriteChanges);
            }

            dr = dtSettings.Select("SetCode='" + "Doc008" + "'"); //Overlay Photo
            if (dr.Length > 0)
            {
                dr[0]["SetValue"] = textBoxOverlayPhoto.Text.Trim();
                dtSettings.LoadDataRow(dr[0].ItemArray, LoadOption.OverwriteChanges);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FillPath(ref textBoxAppData);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FillPath(ref textBoxBackupData);
        }

        public void FillPath(ref TextBox txt)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txt.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
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

            files.Filter = string.Format("({0})|{1} {2}{3}", "PNG Files", "*.BMP;*.JPG;*.JPEG;*.jpeg;*.TIF", sep, files.Filter);

            files.Title = "Select Overlay photo";
            files.Multiselect = false;
            files.CheckPathExists = true;
            files.CheckFileExists = true;

            files.ShowDialog(this);

            if (files.FileNames.Length == 0) return;

            textBoxOverlayPhoto.Text = files.FileName;
        }

        private void buttonSetting_Click(object sender, EventArgs e)
        {
            formPassword frm = new formPassword();
            var result = frm.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            dtAdmin.LoadDataRow(new object[] { "AdminPass", ToolBox.Encrypt.EncryptString("admin"+frm.password1.Trim(),"erwinmacalalad84") }, LoadOption.OverwriteChanges);
        }

        private void buttonUser_Click(object sender, EventArgs e)
        {
            formPassword frm = new formPassword();
            var result = frm.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            dtAdmin.LoadDataRow(new object[] { "UserPass", ToolBox.Encrypt.EncryptString("user" + frm.password1.Trim(), "erwinmacalalad84") }, LoadOption.OverwriteChanges);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
