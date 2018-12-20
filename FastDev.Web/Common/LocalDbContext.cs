using PWMIS.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace FastDev.Web.Common
{
    public class LocalDbContext : DbContext
    {
        public LocalDbContext()
                 : base("db_def")
        {

        }

        protected override bool CheckAllTableExists()
        {
            return false;
        }

        public DataSet ExecuteDataSet(string sql)
        {
            try
            {
                return this.CurrentDataBase.ExecuteDataSet(sql);
            }
            catch (Exception ex)
            {
                return null;
            }
           
        }

        public object ExecuteScalar(string sql)
        {
            try
            {
                return this.CurrentDataBase.ExecuteScalar(sql);
            }
            catch (Exception ex)
            {
                return "";
            }
            
        }

        public DataTable GetListForPage(string tablename, Paging paging)
        {
            if (string.IsNullOrWhiteSpace(tablename)) { return null; }
            tablename = ReplaceFieldValue(tablename);
            var rez = ExecuteScalar(string.Format("select count(*) from [{0}]", tablename)).ToString();
            if (string.IsNullOrWhiteSpace(rez)) { return null; }
            paging.Count = Convert.ToInt32(rez);
            var sql = string.Format("SELECT TOP {0} * FROM (SELECT ROW_NUMBER() OVER (ORDER BY id) AS RowNumber,* FROM [{1}]) as A WHERE RowNumber > {0}*({2}-1)", paging.PageSize, tablename, paging.PageIndex);
            var dset = ExecuteDataSet(sql);
            if(dset==null)
            {
                return null;
            }
            return dset.Tables[0];
        }

        /// <summary>
        /// 过滤sql的单引号,防注入
        /// </summary>
        /// <param name="fieldvalue"></param>
        /// <returns></returns>
        string ReplaceFieldValue(string fieldvalue)
        {
            return fieldvalue.Replace("'", "''");
        }
    }
}