using PWMIS.DataProvider.Adapter;
using PWMIS.DataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.DataBase
{
    public class SQLHelper
    {
        static string DBFileName
        {
            get
            {
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"Data\FastDev.mdf");
            }
        }

        public static AdoHelper MyDBCenter
        {
            get
            {
                return MyDB.GetDBHelperByProviderString("SqlServer", @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + DBFileName + ";Integrated Security=True");
            }
        }



    }
}
