using System;
using System.Data;
using System.IO;
using DocumentManager.Data;

namespace DocumentManager
{
    public class DocDataset : DataSet
    {
        public DataTable categoryTable;
        public DataTable documentTable;
        public DataTable settingTable;
        private FileStream streamXML;
        public string appDataPath;
        public DocDataset()
        {
        }
        
        ~DocDataset()
        {
            categoryTable.Dispose();
            documentTable.Dispose();
            streamXML.Close();
            streamXML.Dispose();
        }

        public Boolean createDataset ()
        {
            categoryTable = new DataTable();
            documentTable = new DataTable();
            settingTable = new DataTable();

            this.settingTable = dataTables.settings();
            this.categoryTable = dataTables.categories();
            this.documentTable = dataTables.documents();
            this.DataSetName = "DocumentManager";
            this.Tables.AddRange(new DataTable[] { documentTable, categoryTable, settingTable });
            DataRelation catDocRelation = new DataRelation("catDocRelation",
                new DataColumn[] { categoryTable.Columns["code"] },
                new DataColumn[] { documentTable.Columns["code"] }
            );
            this.Relations.Add(catDocRelation);

            //UniqueConstraint codeUnique = new UniqueConstraint(
            //    new DataColumn[] 
            //    {
            //        categoryTable.Columns["code"]
            //    }
            //);
            //codeUnique.ConstraintName = "codeConstraint";
            //categoryTable.Constraints.Add(codeUnique);

            categoryTable.Columns["code"].ColumnMapping = MappingType.Attribute;
            documentTable.Columns["DocId"].ColumnMapping = MappingType.Attribute;

            return true;
        }

        public Boolean saveDataset()
        {
            this.AcceptChanges();
            if (streamXML != null)
            {
                streamXML.SetLength(0);
                streamXML.Flush();
            }
            this.WriteXml(streamXML, System.Data.XmlWriteMode.WriteSchema);
            return true;
        }

        public Boolean FactoryReset()
        {
            InitialData();
            if (!Directory.Exists(GetAppPath("AppData")))
            {
                Directory.CreateDirectory(GetAppPath("AppData"));
            }
            if(streamXML != null)
            {
                streamXML.Dispose();
            }
            streamXML = File.Open(appDataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            this.WriteXml(streamXML, System.Data.XmlWriteMode.WriteSchema);
            return true;
        }

        public int GetTreeCode()
        {
            int treeCode =0;
            int findCode =1;
            treeCode = categoryTable.Rows.Count + 1;

            while (findCode==1)
            {
                DataRow[] docResult = categoryTable.Select(string.Format("Convert(code,'System.Int32') = {0}", treeCode));
                if (docResult.Length==0)
                {
                    break;
                }
                else
                {
                    treeCode++;
                }
            }
            return treeCode;
        }
        
        public Boolean readDataFromXML()
        {
            if (!File.Exists(appDataPath))
            {
                this.Reset();
                this.FactoryReset();
                return false;
            }

            streamXML = File.Open(appDataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            this.ReadXml(streamXML, System.Data.XmlReadMode.IgnoreSchema);
            
            return true;
        }

        private void SettingColumnChanged(object sender, System.Data.DataColumnChangeEventArgs e)
        {
            Console.WriteLine(e);
        }

        
        private Boolean InitialData()
        {
            this.Reset();
            this.createDataset();
            //dataDefault.sampleTree(ref categoryTable);
            dataDefault.resetSettings(ref settingTable);
            dataDefault.resetTree(ref categoryTable);
            this.AcceptChanges();
            return true;
        }

        public Boolean GetSettingBooleanValue(string SetCode)
        {
            DataRow[] rs = settingTable.Select("SetCode='"+SetCode+"'");
            if (rs.Length == 0) return false;
            Boolean bParse;
            if (!Boolean.TryParse(rs[0]["SetValue"].ToString().Trim(), out bParse))
            {
                bParse = false;
            }
            return bParse;
        }

        public int GetSettingIntValue(string SetCode)
        {
            int intValue = 0;
            DataRow[] dr = settingTable.Select("SetCode='" + SetCode + "'");
            if (dr.Length == 0)
            {
                intValue = 0;
            }
            else
            {
                intValue = Convert.ToInt32(dr[0]["SetValue"]);
            }
            return intValue;
        }

        public string GetSettingStringValue(string SetCode)
        {
            string stringValue = "";
            DataRow[] dr = settingTable.Select("SetCode='" + SetCode + "'");
            if (dr.Length == 0)
            {
                stringValue = "";
            }
            else
            {
                stringValue = dr[0]["SetValue"].ToString();
            }
            return stringValue;
        }

        public string GetAppPath(string SetCode)
        {
            string appPath = "";
            DataRow[] dr = settingTable.Select("SetCode='"+ SetCode+"'");
            if (dr.Length == 0)
            {
                appPath = "";
            }
            else
            {
                appPath = Environment.ExpandEnvironmentVariables(dr[0]["SetValue"].ToString());
                if (!Directory.Exists(appPath))
                {
                    Directory.CreateDirectory(appPath);
                }
            }
            return appPath;
        }

        public void BackupApplication(string filler)
        {
            this.saveDataset();
            ToolBox.GeneralBox.DirectoryCopy(GetAppPath("AppData"), GetAppPath("BackupData"),false, filler);
            if (filler.Trim() == "R")
            {
                foreach (DataRow r in this.documentTable.Rows)
                {
                    if (File.Exists(r["DocFilename"].ToString()))
                    {
                        File.Delete(r["DocFilename"].ToString());
                    }
                }
            }
        }
    }
}
