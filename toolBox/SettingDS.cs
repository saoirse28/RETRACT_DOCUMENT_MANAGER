using AppSoftware.LicenceEngine.Common;
using AppSoftware.LicenceEngine.KeyVerification;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace ToolBox
{
    public class SettingDS : DataSet
    {
        public DataTable settingTable = new DataTable("AppSetting");
        public string appSettingDirectory = @"%appdata%\DocumentManager";
        public string appSettingFile = "setting.xml";
        public string appDataFile = "data.xml";
        private string passPhrase = "erwinmacalalad84";
        public SettingDS()
        {
            appSettingDirectory = Environment.ExpandEnvironmentVariables(appSettingDirectory);
            appSettingFile = Path.Combine(appSettingDirectory, appSettingFile);
            appDataFile = Path.Combine(appSettingDirectory, appDataFile);

            if (!Directory.Exists(appSettingDirectory))
            {
                Directory.CreateDirectory(appSettingDirectory);
            }

            if (!File.Exists(appSettingFile))
            {
                this.CreateLocalSetting();
                this.InsertLocalSetting();
                this.SaveSetting();
            }
            else
            {
                CreateLocalSetting();
            }

            try
            {
                settingTable.Rows.Clear();
                settingTable.AcceptChanges();
                this.ReadXml(appSettingFile, XmlReadMode.IgnoreSchema);
                DataRow[] dr = settingTable.Select("AppCode='AppData'");
                if (dr.Length > 0)
                {
                    if (dr[0]["AppValue"].ToString().Trim() != "")
                    {
                        string setPath = Environment.ExpandEnvironmentVariables(dr[0]["AppValue"].ToString());
                        if (File.Exists(setPath))
                        {
                            Console.WriteLine("Local application settings existed.");
                        }
                        else
                        {
                            this.WriteXml(setPath);
                            Console.WriteLine("Local application settings path set.");
                            File.Delete(setPath);
                        }
                        appDataFile = setPath;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Failed to initialized local application settings.");
            }
            
            settingTable.RowChanging += SettingTable_RowChanging;
        }

        private void SettingTable_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            //if (e.Row["AppCode"].ToString() == "UserPass")
            //{
            //    settingTable.LoadDataRow(new object[] { "UserPass", "user" + ToolBox.Encrypt.EncryptString("admin" + e.Row["AppValue"].ToString(), this.passPhrase) }, LoadOption.OverwriteChanges);
            //}

            //if (e.Row["AppCode"].ToString() == "AdminPass")
            //{
            //    settingTable.LoadDataRow(new object[] { "AdminPass", "admin" + ToolBox.Encrypt.EncryptString("admin" + e.Row["AppValue"].ToString(), this.passPhrase) },LoadOption.OverwriteChanges);
            //}
        }

        public void SaveSetting()
        {
            this.AcceptChanges();
            FileStream streamXML = File.Open(this.appSettingFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            streamXML.SetLength(0);
            streamXML.Flush();
            this.WriteXml(streamXML, System.Data.XmlWriteMode.WriteSchema);
            streamXML.Dispose();
        }
        public void CreateLocalSetting()
        {
            settingTable.Columns.Add(new DataColumn("AppCode", Type.GetType("System.String")));
            settingTable.Columns.Add(new DataColumn("AppValue", Type.GetType("System.String")));
            settingTable.PrimaryKey = new DataColumn[] { settingTable.Columns["AppCode"] };

            this.Tables.AddRange(new DataTable[] { settingTable });
        }

        public void InsertLocalSetting()
        {
            settingTable.Rows.Add(new Object[] { "AppData", @"%appdata%\DocumentManager\data.xml" });
            settingTable.Rows.Add(new Object[] { "UserPass", "4eY9oIgVyyXYk5n6PM4D3A==" });
            settingTable.Rows.Add(new Object[] { "AdminPass", "I61ipt19MEu9SW3Xh61ZTw==" });
        }

        public bool ValidateLicense(string licenseKey)
        {
            var pkResult = PkvLicenceKeyResult.KeyInvalid;
            var pkvKeyCheck = new PkvKeyCheck();
            var keyBytes = new[] {
                    new KeyByteSet(5, 165, 15, 132),
                    new KeyByteSet(6, 128, 175, 213)
                    };
            pkResult = pkvKeyCheck.CheckKey(licenseKey, keyBytes, 8, null);

            if (pkResult == PkvLicenceKeyResult.KeyGood)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ValidateUser(string UserPass)
        {
            DataRow[] dr = this.settingTable.Select("AppCode='UserPass' and AppValue='" + ToolBox.Encrypt.EncryptString("user"+ UserPass.Trim(), this.passPhrase) + "'");
            if (dr.Count() == 1)
                return true;
            dr = this.settingTable.Select("AppCode='AdminPass' and AppValue='" + ToolBox.Encrypt.EncryptString("admin" + UserPass.Trim(), this.passPhrase) + "'");
            if (dr.Count() == 1)
                return true;

            return false;
        }

        public bool ValidateAdmin(string AdminPass)
        {
            DataRow[] dr = this.settingTable.Select("AppCode='AdminPass' and AppValue='" + ToolBox.Encrypt.EncryptString("admin" + AdminPass.Trim(), this.passPhrase) + "'");
            if (dr.Count() == 1)
                return true;

            return false;
        }
    }
}
