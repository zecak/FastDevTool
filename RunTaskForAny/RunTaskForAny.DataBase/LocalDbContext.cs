using PWMIS.DataMap.Entity;
using PWMIS.DataProvider.Adapter;
using PWMIS.DataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.DataBase
{
    public class LocalDbContext
    {
        /// <summary>
        /// 获取一个数据库连接对象，需要使用事务时，请用此方法获取一个数据库连接对象，不要使用MyDBCenter.
        /// </summary>
        /// <returns></returns>
        public static AdoHelper GetDB()
        {
            return MyDB.GetDBHelperByProviderString("PWMIS.DataProvider.Data.MySQL,PWMIS.MySqlClient", "Server=172.16.1.63;Port=3306;database=test;uid=root;password=root;Convert Zero Datetime=True;Allow Zero Datetime=True");
        }

        public static bool Add(string name,string pwd)
        {
            using (var db = GetDB())
            {
                try
                {
                    db.BeginTransaction();
                    
                    var query = new EntityQuery<Tb_User>(db);
                    var model = new Tb_User() {  Name= name, Pwd= pwd };
                    model.JsonData = model.ToString();
                    var ok = query.ExecuteOql(OQL.From(model).Insert(model.Name, model.Pwd)) > 0;
                    if (!ok) { db.Rollback(); return false; }

                    db.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    db.Rollback();
                    return false;
                }
            }
        }
    }
}
