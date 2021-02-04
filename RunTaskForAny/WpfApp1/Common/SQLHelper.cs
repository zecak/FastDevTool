using PWMIS.DataProvider.Adapter;
using PWMIS.DataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Common
{
    public class SQLHelper
    {
        /// <summary>
        /// 获取一个数据库连接对象，需要使用事务时，请用此方法获取一个数据库连接对象，不要使用MyDBCenter.
        /// </summary>
        /// <returns></returns>
        public static AdoHelper GetDB()
        {
            var db = MyDB.GetDBHelper(PWMIS.Common.DBMSType.SqlServer, @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"\Data\DB.mdf") + ";Integrated Security=True");
            //db.CommandTimeOut = 60;
            return db;
        }

    }
}
