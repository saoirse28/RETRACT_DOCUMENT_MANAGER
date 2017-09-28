using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DocumentManager.Data
{
    public class dataTables : DataTable
    {
        public static DataTable settings()
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

        public static DataTable documents()
        {
            DataTable document = new DataTable("documents");
            DataColumn column = new DataColumn();

            column = new DataColumn();
            column.ColumnName = "DocId";
            column.DataType = typeof(int);
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

            column = new DataColumn();
            column.ColumnName = "DocValue";
            column.DataType = typeof(decimal);
            column.DefaultValue = 0.0;
            document.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "DocEncrypt";
            column.DataType = typeof(string);
            column.DefaultValue = "FALSE";
            document.Columns.Add(column);

            document.PrimaryKey = new DataColumn[] { document.Columns["DocId"] };
            return document;
        }

        public static DataTable categories()
        {
            DataTable document = new DataTable("categories");
            document.Columns.Add(new DataColumn("code", Type.GetType("System.Int32")));
            document.Columns.Add(new DataColumn("ac_name", Type.GetType("System.String")));
            document.Columns.Add(new DataColumn("parent_node", typeof(int)));

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
    }

    public class dataDefault
    {
        public static Boolean resetSettings(ref DataTable dt)
        {
            dt.Rows.Add(new Object[] { "Data", "AppData", "Application Data Location", @"%AppData%\DocumentManager" });
            dt.Rows.Add(new Object[] { "Data", "BackupData", "Application Data Location", @"%AppData%\DocumentManager\BackupData" });

            dt.Rows.Add(new Object[] { "System", "System001", "Enabled Archive Category", "TRUE" });
            dt.Rows.Add(new Object[] { "System", "System002", "Enabled Reset Application", "FALSE" });
            dt.Rows.Add(new Object[] { "System", "System003", "Enabled View DataTables", "TRUE" });
            dt.Rows.Add(new Object[] { "System", "System004", "Enabled Backup Application", "TRUE" });
            dt.Rows.Add(new Object[] { "System", "System005", "Enabled Import Documents", "TRUE" });
            dt.Rows.Add(new Object[] { "System", "System006", "Enabled Scan Documents", "TRUE" });
            dt.Rows.Add(new Object[] { "System", "System007", "Enabled User Password", "FALSE" });
            dt.Rows.Add(new Object[] { "System", "System008", "Enabled Admin Password", "FALSE" });

            dt.Rows.Add(new Object[] { "Category", "Cat001", "Enabled Category Add", "TRUE" });
            dt.Rows.Add(new Object[] { "Category", "Cat002", "Enabled Category Delete", "TRUE" });
            dt.Rows.Add(new Object[] { "Category", "Cat003", "Enabled Category Move", "TRUE" });
            dt.Rows.Add(new Object[] { "Category", "Cat004", "Enabled Category Rename", "TRUE" });

            dt.Rows.Add(new Object[] { "Document", "Doc001", "Enabled Document Move", "TRUE" });
            dt.Rows.Add(new Object[] { "Document", "Doc002", "Enabled Document Update", "TRUE" });
            dt.Rows.Add(new Object[] { "Document", "Doc003", "Enabled Document Delete", "TRUE" });
            dt.Rows.Add(new Object[] { "Document", "Doc004", "Enabled Document Print", "TRUE" });
            dt.Rows.Add(new Object[] { "Document", "Doc005", "Scanned Document Quality", "80" });
            dt.Rows.Add(new Object[] { "Document", "Doc006", "Import Document Quality", "80" });
            dt.Rows.Add(new Object[] { "Document", "Doc007", "Print Document with Overlay", "TRUE" });
            dt.Rows.Add(new Object[] { "Document", "Doc008", "Overlay Photo", "classified.png" });
            dt.Rows.Add(new Object[] { "Document", "Doc009", "Overlay on Photo Preview", "FALSE" });
            return true;
        }

        public static bool resetTree(ref DataTable dt)
        {
            dt.Rows.Add(new Object[] { -99, "Archive Category", 0, System.DateTime.Now, 0 });
            return true;
        }
        public static bool sampleTree(ref DataTable dt)
        {
            dt.Rows.Add(new Object[] { 1, "Assets", 0 });
            dt.Rows.Add(new Object[] { 2, "Liabilities", 0 });
            dt.Rows.Add(new Object[] { 3, "Equity", 0 });
            dt.Rows.Add(new Object[] { 4, "Revenue", 0 });

            dt.Rows.Add(new Object[] { 101, "Current Assets", 1 });
            dt.Rows.Add(new Object[] { 102, "Fixed Assets", 1 });
            dt.Rows.Add(new Object[] { 201, "Short Term Liabilities", 2 });
            dt.Rows.Add(new Object[] { 202, "Long Term Liabilities", 2 });
            dt.Rows.Add(new Object[] { 301, "Owner''s Equity", 3 });
            dt.Rows.Add(new Object[] { 401, "Sales", 4 });
            dt.Rows.Add(new Object[] { 10101, "Cash and Bank", 101 });
            dt.Rows.Add(new Object[] { 10102, "Customer Recivables", 101 });
            dt.Rows.Add(new Object[] { 10103, "Inventory/Stocks", 101 });
            dt.Rows.Add(new Object[] { 10104, "Personal Loan Receivables", 101 });
            dt.Rows.Add(new Object[] { 10105, "Employees Receivables", 101 });
            dt.Rows.Add(new Object[] { 10106, "Other Receivables", 101 });
            dt.Rows.Add(new Object[] { 10201, "Non Current Assests", 102 });
            dt.Rows.Add(new Object[] { 20101, "Vendor Payables", 201 });
            dt.Rows.Add(new Object[] { 20102, "Other Payables", 201 });
            dt.Rows.Add(new Object[] { 20103, "Services Payables", 201 });
            dt.Rows.Add(new Object[] { 20201, "Personal Payables", 202 });
            dt.Rows.Add(new Object[] { 30101, "Capital", 301 });
            dt.Rows.Add(new Object[] { 40101, "Sales Income", 401 });
            dt.Rows.Add(new Object[] { 40102, "Other Income", 401 });
            dt.Rows.Add(new Object[] { 10101001, "Cash in Hand", 10101 });
            dt.Rows.Add(new Object[] { 10103001, "Raw Meterial Stock", 10103 });
            dt.Rows.Add(new Object[] { 10103002, "Finished Inventory", 10103 });
            dt.Rows.Add(new Object[] { 10103003, "Packing Inventory", 10103 });
            dt.Rows.Add(new Object[] { 10106001, "Suspense A/C", 10106 });
            dt.Rows.Add(new Object[] { 10201001, "ERP Software Asset", 10201 });
            dt.Rows.Add(new Object[] { 20102001, "Suzuki Wa ", 20102 });
            dt.Rows.Add(new Object[] { 30101001, "Capital A/C", 30101 });

            dt.Rows.Add(new Object[] { -99, "Archive Category", 0, System.DateTime.Now, 0 });

            return true;
        }
    }

}
