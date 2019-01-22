using PWMIS.DataProvider.Adapter;
using PWMIS.DataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.DataBase
{
    public class SQLHelper
    {

        /// <summary>
        /// 获取一个数据库连接对象，需要使用事务时，请用此方法获取一个数据库连接对象，不要使用MyDBCenter.
        /// </summary>
        /// <returns></returns>
        public static AdoHelper GetDB(string providerString,string connectionString)
        {
            var db = MyDB.GetDBHelperByProviderString(providerString, connectionString);
            db.CommandTimeOut = 60;
            return db;
        }

    }
}
