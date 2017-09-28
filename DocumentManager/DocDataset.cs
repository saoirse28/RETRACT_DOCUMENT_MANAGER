using System;
using System.Data;
using System.IO;

namespace DocumentManager
{
    public class DocDataset : DataSet
    {
        private DataTable treeTable;
        private DataTable docTable;
        private FileStream streamXML;
        private DataTable settingTable;
        private String appDataPath= @"%appdata%\DocumentManager\data.xml";
        public DocDataset()
        {
            //createDataset();
            appDataPath = Environment.ExpandEnvironmentVariables(appDataPath);
        }

        ~DocDataset()
        {
            treeTable.Dispose();
            docTable.Dispose();
            streamXML.Close();
            streamXML.Dispose();
        }

        public Boolean createDataset ()
        {
            treeTable = new DataTable();
            docTable = new DataTable();
            settingTable = new DataTable();

            this.settingTable = SettingTable();
            this.treeTable = TreeTable();
            this.docTable = DocTable();
            this.DataSetName = "DocScanner";
            this.Tables.AddRange(new DataTable[] { docTable, treeTable, settingTable });
            DataRelation treeDocRelation = new DataRelation("treeDocRelation",
                new DataColumn[] { treeTable.Columns["code"] },
                new DataColumn[] { docTable.Columns["code"] }
            );
            this.Relations.Add(treeDocRelation);

            //UniqueConstraint codeUnique = new UniqueConstraint(
            //    new DataColumn[] 
            //    {
            //        treeTable.Columns["code"]
            //    }
            //);
            //codeUnique.ConstraintName = "codeConstraint";
            //treeTable.Constraints.Add(codeUnique);

            treeTable.Columns["code"].ColumnMapping = MappingType.Attribute;
            docTable.Columns["DocId"].ColumnMapping = MappingType.Attribute;

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

        public Int32 GetTreeCode()
        {
            Int32 treeCode=0;
            Int32 findCode=1;
            treeCode = treeTable.Rows.Count + 1;

            while (findCode==1)
            {
                DataRow[] docResult = treeTable.Select(String.Format("Convert(code,'System.Int32') = {0}", treeCode));
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
        public DataTable DocTable()
        {
            DataTable document = new DataTable("document");
            DataColumn column = new DataColumn();

            column = new DataColumn();
            column.ColumnName = "DocId";
            column.DataType = typeof(Int32);
            column.AutoIncrement = true;
            column.AutoIncrementSeed = 1;
            column.AutoIncrementStep = 1;
            document.Columns.Add(column);

            document.Columns.Add(new DataColumn("code", Type.GetType("System.Int32")));
            document.Columns.Add(new DataColumn("DocName", Type.GetType("System.String")));
            document.Columns.Add(new DataColumn("DocDesc", Type.GetType("System.String")));
            document.Columns.Add(new DataColumn("DocOcr", Type.GetType("System.String")));
            document.Columns.Add(new DataColumn("DocFilename", Type.GetType("System.String")));
            document.Columns.Add(new DataColumn("DocSize", Type.GetType("System.String")));

            column = new DataColumn();
            column.ColumnName = "ModifiedDate";
            column.DataType = System.Type.GetType("System.DateTime");
            column.DefaultValue = System.DateTime.Now;
            document.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "ScannedDate";
            column.DataType = System.Type.GetType("System.DateTime");
            column.DefaultValue = System.DateTime.Now;
            document.Columns.Add(column);

            document.PrimaryKey = new DataColumn[] { document.Columns["DocId"] };
            return document;
        }

        private DataTable TreeTable()
        {
            DataTable document = new DataTable("tree");
            document.Columns.Add(new DataColumn("code", Type.GetType("System.Int32")));
            document.Columns.Add(new DataColumn("ac_name", Type.GetType("System.String")));
            document.Columns.Add(new DataColumn("parent_node", typeof(Int32)));

            DataColumn column = new DataColumn();
            column.ColumnName = "dt";
            column.DataType = System.Type.GetType("System.DateTime");
            column.DefaultValue = System.DateTime.Now;
            document.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "active";
            column.DataType = System.Type.GetType("System.Int32");
            column.DefaultValue = 1;
            document.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "cntr";
            column.DataType = System.Type.GetType("System.Int32");
            column.AutoIncrement = true;
            column.AutoIncrementSeed = 1;
            column.AutoIncrementStep = 1;
            document.Columns.Add(column);

            document.PrimaryKey = new DataColumn[] { document.Columns["code"] };
            return document;
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

        public DataTable SettingTable()
        {
            DataTable settings = new DataTable("settings");

            settings.Columns.Add(new DataColumn("SetGroup", Type.GetType("System.String")));
            settings.Columns.Add(new DataColumn("SetCode", Type.GetType("System.String")));
            settings.Columns.Add(new DataColumn("SetDesc", Type.GetType("System.String")));
            settings.Columns.Add(new DataColumn("SetValue", Type.GetType("System.String")));

            DataColumn column = new DataColumn();

            column = new DataColumn();
            column.ColumnName = "SetId";
            column.DataType = System.Type.GetType("System.Int32");
            column.AutoIncrement = true;
            column.AutoIncrementSeed = 1;
            column.AutoIncrementStep = 1;
            settings.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "CreatedDate";
            column.DataType = System.Type.GetType("System.DateTime");
            column.DefaultValue = System.DateTime.Now;
            settings.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "ModifiedDate";
            column.DataType = System.Type.GetType("System.DateTime");
            column.DefaultValue = System.DateTime.Now;
            settings.Columns.Add(column);

            settings.PrimaryKey = new DataColumn[] { settings.Columns["SetCode"] };
            return settings;
        }
        private Boolean InitialData()
        {
            this.Reset();
            this.createDataset();

            treeTable.Rows.Add(new Object[] { 1, "Assets", 0 });
            treeTable.Rows.Add(new Object[] { 2, "Liabilities", 0 });
            treeTable.Rows.Add(new Object[] { 3, "Equity", 0 });
            treeTable.Rows.Add(new Object[] { 4, "Revenue", 0 });

            treeTable.Rows.Add(new Object[] { 101, "Current Assets", 1 });
            treeTable.Rows.Add(new Object[] { 102, "Fixed Assets", 1 });
            treeTable.Rows.Add(new Object[] { 201, "Short Term Liabilities", 2 });
            treeTable.Rows.Add(new Object[] { 202, "Long Term Liabilities", 2 });
            treeTable.Rows.Add(new Object[] { 301, "Owner''s Equity", 3 });
            treeTable.Rows.Add(new Object[] { 401, "Sales", 4 });
            treeTable.Rows.Add(new Object[] { 10101, "Cash and Bank", 101 });
            treeTable.Rows.Add(new Object[] { 10102, "Customer Recivables", 101 });
            treeTable.Rows.Add(new Object[] { 10103, "Inventory/Stocks", 101 });
            treeTable.Rows.Add(new Object[] { 10104, "Personal Loan Receivables", 101 });
            treeTable.Rows.Add(new Object[] { 10105, "Employees Receivables", 101 });
            treeTable.Rows.Add(new Object[] { 10106, "Other Receivables", 101 });
            treeTable.Rows.Add(new Object[] { 10201, "Non Current Assests", 102 });
            treeTable.Rows.Add(new Object[] { 20101, "Vendor Payables", 201 });
            treeTable.Rows.Add(new Object[] { 20102, "Other Payables", 201 });
            treeTable.Rows.Add(new Object[] { 20103, "Services Payables", 201 });
            treeTable.Rows.Add(new Object[] { 20201, "Personal Payables", 202 });
            treeTable.Rows.Add(new Object[] { 30101, "Capital", 301 });
            treeTable.Rows.Add(new Object[] { 40101, "Sales Income", 401 });
            treeTable.Rows.Add(new Object[] { 40102, "Other Income", 401 });
            treeTable.Rows.Add(new Object[] { 10101001, "Cash in Hand", 10101 });
            treeTable.Rows.Add(new Object[] { 10103001, "Raw Meterial Stock", 10103 });
            treeTable.Rows.Add(new Object[] { 10103002, "Finished Inventory", 10103 });
            treeTable.Rows.Add(new Object[] { 10103003, "Packing Inventory", 10103 });
            treeTable.Rows.Add(new Object[] { 10106001, "Suspense A/C", 10106 });
            treeTable.Rows.Add(new Object[] { 10201001, "ERP Software Asset", 10201 });
            treeTable.Rows.Add(new Object[] { 20102001, "Suzuki Wa ", 20102 });
            treeTable.Rows.Add(new Object[] { 30101001, "Capital A/C", 30101 });

            treeTable.Rows.Add(new Object[] { -99, "Disabled Category",0, System.DateTime.Now,0 });
            ResetSettings();

            return true;
        }

        public Boolean ResetSettings()
        {            
            settingTable.Rows.Add(new Object[] { "Data","AppData", "Application Data Location", @"%AppData%\DocumentManager" });
            settingTable.Rows.Add(new Object[] { "Data", "BackupData", "Application Data Location", @"%AppData%\DocumentManager\BackupData" });

            settingTable.Rows.Add(new Object[] { "System", "System001", "Enabled Disabled Category", "TRUE" });
            settingTable.Rows.Add(new Object[] { "System", "System002", "Enabled Reset Application", "TRUE" });
            settingTable.Rows.Add(new Object[] { "System", "System003", "Enabled View DataTables", "TRUE" });
            settingTable.Rows.Add(new Object[] { "System", "System004", "Enabled Backup Application", "TRUE" });
            settingTable.Rows.Add(new Object[] { "System", "System005", "Enabled Import Documents", "TRUE" });
            settingTable.Rows.Add(new Object[] { "System", "System006", "Enabled Scan Documents", "TRUE" });

            settingTable.Rows.Add(new Object[] { "Category", "Cat001", "Enabled Category Add", "TRUE" });
            settingTable.Rows.Add(new Object[] { "Category", "Cat002", "Enabled Category Delete", "TRUE" });
            settingTable.Rows.Add(new Object[] { "Category", "Cat003", "Enabled Category Move", "TRUE" });
            settingTable.Rows.Add(new Object[] { "Category", "Cat004", "Enabled Category Rename", "TRUE" });

            settingTable.Rows.Add(new Object[] { "Document", "Doc001", "Enabled Document Move", "TRUE" });
            settingTable.Rows.Add(new Object[] { "Document", "Doc002", "Enabled Document Update", "TRUE" });
            settingTable.Rows.Add(new Object[] { "Document", "Doc003", "Enabled Document Delete", "TRUE" });           
            settingTable.Rows.Add(new Object[] { "Document", "Doc004", "Enabled Document Print", "TRUE" });
            settingTable.Rows.Add(new Object[] { "Document", "Doc005", "Scanned Document Quality", "95" });
            settingTable.Rows.Add(new Object[] { "Document", "Doc006", "Import Document Quality", "100" });
            settingTable.Rows.Add(new Object[] { "Document", "Doc007", "Print Document with Overlay", "TRUE" });
            settingTable.Rows.Add(new Object[] { "Document", "Doc008", "Overlay Photo", "classified.png" });
            settingTable.Rows.Add(new Object[] { "Document", "Doc009", "Overlay on Photo Preview", "TRUE" });

            return true;
        }

        public Boolean GetBooleanValue(String SetCode)
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

        public Int32 GetIntValue(String SetCode)
        {
            Int32 intValue = 0;
            DataRow[] dr = this.Tables["settings"].Select("SetCode='" + SetCode + "'");
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

        public String GetStringValue(String SetCode)
        {
            String stringValue = "";
            DataRow[] dr = this.Tables["settings"].Select("SetCode='" + SetCode + "'");
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

        public String GetAppPath(String SetCode)
        {
            string appPath = "";
            DataRow[] dr = this.Tables["settings"].Select("SetCode='"+ SetCode+"'");
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

        public void BackupApplication(String filler)
        {
            this.saveDataset();
            ToolBox.GeneralBox.DirectoryCopy(GetAppPath("AppData"), GetAppPath("BackupData"),false, filler);
        }
    }
}
