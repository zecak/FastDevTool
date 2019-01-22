using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Database
{
    public abstract class CommonDB : IDisposable
    {
        public DbConnection Connection { get; set; }

        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }
        }

        public abstract DataTable ExecuteDataTable(string SQL);

        public abstract DataTable ExecuteDataTable(string SQL, CommandType commandType, IDataParameter[] parameters);

        public abstract int ExecuteNonQuery(string SQL);

        public abstract int ExecuteNonQuery(string SQL, CommandType commandType, IDataParameter[] parameters);

        public abstract object ExecuteScalar(string SQL, CommandType commandType, IDataParameter[] parameters);

        public abstract object ExecuteScalar(string SQL);
    }
}
