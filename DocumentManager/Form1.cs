using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace DocumentManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();

            DataTable catTable = CatTable();
            DataTable docTable = DocTable();
            ds.Tables.AddRange(new DataTable[] { docTable, catTable });

            DataRelation CatDocRel = new DataRelation("CatDocRel",
                new DataColumn[] { catTable.Columns["CatId"] },
                new DataColumn[] { docTable.Columns["CatId"] }
            );
            ds.Relations.Add(CatDocRel);

            catTable.Rows.Add(new Object[] { 1, "REASSIGN", "Re-Assign Transaction ID" });
            catTable.Rows.Add(new Object[] { 2, "RSU", "New RSU" });
            docTable.Rows.Add(new Object[] { 1 ,1, "SR00001","Re-Assign Transaction","","MMDDYY0001"});
            docTable.Rows.Add(new Object[] { 1, 2, "SR00002", "Re-Assign Transaction", "", "MMDDYY0002" });
            docTable.Rows.Add(new Object[] { 1, 3, "SR00003", "Re-Assign Transaction", "", "MMDDYY0003" });
            docTable.Rows.Add(new Object[] { 2, 1, "RSU0001", "New RF", "", "MMDDYY0001" });

            DataRow[] docResult = catTable.Select(String.Format("CatId = {0}", 1));
            DataRow[] docResults = docResult[0].GetChildRows(CatDocRel);

            ds.WriteXml("data.xml");
            MessageBox.Show("save");
        }

        public DataTable DocTable()
        {
            DataTable document = new DataTable("document");
            document.Columns.Add(new DataColumn("CatId", Type.GetType("System.Int32")));
            document.Columns.Add(new DataColumn("DocId", Type.GetType("System.Int32")));
            document.Columns.Add(new DataColumn("DocName", Type.GetType("System.String")));
            document.Columns.Add(new DataColumn("DocDesc", Type.GetType("System.String")));
            document.Columns.Add(new DataColumn("DocOcr", Type.GetType("System.String")));
            document.Columns.Add(new DataColumn("DocFilename", Type.GetType("System.String")));
            document.PrimaryKey = new DataColumn[] { document.Columns["CatId"], document.Columns["DocId"] };
            return document;
        }

        public DataTable CatTable()
        {
            DataTable category = new DataTable("category");
            category.Columns.Add(new DataColumn("CatId", Type.GetType("System.Int32")));
            category.Columns.Add(new DataColumn("CatName", Type.GetType("System.String")));
            category.Columns.Add(new DataColumn("CatDesc", Type.GetType("System.String")));
            category.PrimaryKey = new DataColumn[] { category.Columns["CatId"] };
            return category;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            XmlReader xmlFile;
            xmlFile = XmlReader.Create("Product.xml", new XmlReaderSettings());
            DataSet ds = new DataSet();
            ds.ReadXml(xmlFile);
            int i = 0;
            for (i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
            {
                MessageBox.Show(ds.Tables[0].Rows[i].ItemArray[0].ToString() + "\n\r" + ds.Tables[0].Rows[i].ItemArray[1].ToString() + "\n\r" + ds.Tables[0].Rows[i].ItemArray[2].ToString());
            }
        }
    }
}
