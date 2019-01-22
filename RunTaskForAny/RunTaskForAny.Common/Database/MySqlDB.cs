using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Database
{
    public class MySqlDB : CommonDB
    {
        public MySqlDB()
        {

        }

        public MySqlDB(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
            Connection.Open();
        }

        public override DataTable ExecuteDataTable(string SQL)
        {
            if (Connection == null) { return null; }
            var cmd = new MySqlCommand(SQL, (MySqlConnection)Connection);
            var da = new MySqlDataAdapter(cmd);
            var tb = new DataTable();
            da.Fill(tb);
            var result = tb;
            return result;
        }

        public override DataTable ExecuteDataTable(string SQL, CommandType commandType, IDataParameter[] parameters)
        {
            if (Connection == null) { return null; }
            var cmd = new MySqlCommand(SQL, (MySqlConnection)Connection);
            cmd.CommandType = commandType;
            cmd.Parameters.AddRange(parameters);
            var da = new MySqlDataAdapter(cmd);
            var tb = new DataTable();
            da.Fill(tb);
            var result = tb;
            return result;
        }

        public override int ExecuteNonQuery(string SQL)
        {
            if (Connection == null) { return -2; }
            var cmd = new MySqlCommand(SQL, (MySqlConnection)Connection);
            var result = cmd.ExecuteNonQuery();
            return result;
        }

        public override int ExecuteNonQuery(string SQL, CommandType commandType, IDataParameter[] parameters)
        {
            if (Connection == null) { return -2; }
            var cmd = new MySqlCommand(SQL, (MySqlConnection)Connection);
            cmd.CommandType = commandType;
            cmd.Parameters.AddRange(parameters);
            var result = cmd.ExecuteNonQuery();
            return result;
        }

        public override object ExecuteScalar(string SQL, CommandType commandType, IDataParameter[] parameters)
        {
            if (Connection == null) { return -2; }
            var cmd = new MySqlCommand(SQL, (MySqlConnection)Connection);
            cmd.CommandType = commandType;
            cmd.Parameters.AddRange(parameters);
            var result = cmd.ExecuteScalar();
            return result;
        }

        public override object ExecuteScalar(string SQL)
        {
            if (Connection == null) { return -2; }
            var cmd = new MySqlCommand(SQL, (MySqlConnection)Connection);
            var result = cmd.ExecuteScalar();
            return result;
        }
    }
}
