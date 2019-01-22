using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Database
{
    public class SQLHelper
    {
        
        public static CommonDB GetDB(SQLType sqlType, string connectionString)
        {
            CommonDB conn = null;

            switch (sqlType)
            {
                case SQLType.MSSQL:

                    break;
                case SQLType.MYSQL:
                    conn = new MySqlDB(connectionString);
                    break;
                case SQLType.SQLite:

                    break;
                default:
                    break;
            }

            return conn;
        }
        
    }
}
