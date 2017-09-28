using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolBox
{
    public class DataSetBox
    {
        public Boolean GetFirstCell(ref DataTable dt, ref object ref_value, string select_column, string select_where)
        {
            try
            {
                DataRow[] rs = dt.Select(select_where);
                if (rs.Length == 0)
                {
                    return false;
                }
                ref_value = rs[0][select_column];
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
