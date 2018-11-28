using FastDevTool.DataBase.Model;
using PWMIS.Core.Extensions;
using PWMIS.DataProvider.Adapter;
using PWMIS.DataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.DataBase
{

    public class LocalDbContext : DbContext
    {
        static string DBFileName
        {
            get
            {
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"Data\FastDev.mdf");
            }
        }
        public static AdoHelper GetDB()
        {
            return MyDB.GetDBHelperByProviderString("SqlServer", @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + DBFileName + ";Integrated Security=True");
        }

        public LocalDbContext()
               : base(GetDB())
        {

        }
        
        protected override bool CheckAllTableExists()
        {
            //创建用户表
            CheckTableExists<Tb_User>();
            return true;
        }

        



    }
}
