using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetData
{
    class Program
    {
        static void Main(string[] args)
        {
            ToolBox.SettingDS SettingDS = new ToolBox.SettingDS();
            SettingDS.CreateLocalSetting();
            SettingDS.InsertLocalSetting();
            SettingDS.SaveSetting();
        }
    }
}
