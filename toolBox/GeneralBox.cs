using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ToolBox
{
    public static class GeneralBox
    {
        public static bool TryParseBoolean(string val)
        {
            bool bParse;
            if (!bool.TryParse(val, out bParse))
            {
                bParse = false;
            }

            return bParse;
        }
        public static void PopulateTreeView(ref TreeView treeView1,  DataTable _acountsTb, int parentId, TreeNode parentNode)
        {
            //SetButtons(EBUTTONSTATE.OPEN);

            if (_acountsTb.Rows.Count == 0)
            {
                return;
            }

            TreeNode childNode;

            DataRow[] docResult = _acountsTb.Select(string.Format(" Convert(parent_node,'System.Int32') = {0}", parentId));
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
                int treeNode = 0;
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

                PopulateTreeView(ref treeView1, _acountsTb, Convert.ToInt32(dr["code"].ToString()), childNode);
                foreach (TreeNode n in treeView1.Nodes)
                {
                    n.Expand();
                }
            }
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, string filler)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            destDirName = destDirName + "\\" + filler + string.Format("{0:MMddyyHmmssffff}", DateTime.Now);

            //using (var zip = new Ionic.Zip.ZipFile())
            //{
            //    zip.AddDirectory(destDirName);
            //    zip.Save(filler + String.Format("{0:MMddyyHmmss}", DateTime.Now) + ".zip");
            //}

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs,filler);
                }
            }
        }
    }
}