using FastDevTool.Common;
using FastDevTool.DataBase.Model;
using FastDevTool.MVVM.NPCModel;
using PWMIS.Core.Extensions;
using PWMIS.DataMap.Entity;
using PWMIS.DataProvider.Adapter;
using PWMIS.DataProvider.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.DataBase
{
    /// <summary>
    /// MSSQL
    /// </summary>
    public class LocalDbContext : DbContext
    {

        public LocalDbContext()
               : base(SQLHelper.MyDBCenter)
        {

        }

        protected override bool CheckAllTableExists()
        {
            if (!CheckTableExists<sys_table>())//创建表并添加默认数据
            {
                var table_list = new List<sys_table>()
                {
                    new sys_table(){GID=Guid.NewGuid(),Name="sys_table",Title="表字典",Description="数据库表信息",SystemMark=1, Prefix="sys",RestName="table",CreateTime=DateTime.Now,UpdateTime=DateTime.Now,SortNo=0,Status=1},
                    new sys_table(){GID=Guid.NewGuid(),Name="sys_field_type",Title="字段类型",Description="数据库字段类型信息",SystemMark=1, Prefix="sys",RestName="field_type",CreateTime=DateTime.Now,UpdateTime=DateTime.Now,SortNo=0,Status=1},
                    new sys_table(){GID=Guid.NewGuid(),Name="sys_enum",Title="枚举类型",Description="数据库枚举类型信息",SystemMark=1, Prefix="sys",RestName="enum",CreateTime=DateTime.Now,UpdateTime=DateTime.Now,SortNo=0,Status=1},
                    new sys_table(){GID=Guid.NewGuid(),Name="sys_table_column",Title="列字典",Description="数据库列信息",SystemMark=1, Prefix="sys",RestName="table_column",CreateTime=DateTime.Now,UpdateTime=DateTime.Now,SortNo=0,Status=1},
                };
                AddList(table_list);

                var fieldtype_list = new List<sys_field_type>()
                {
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="int", Title="整型", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="decimal", Title="精确数值", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="money", Title="货币", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="datetime", Title="日期时间", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="nvarchar", Title="字符串", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="ntext", Title="文本", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="uniqueidentifier", Title="全局唯一标记符", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                };
                if (!CheckTableExists<sys_field_type>())
                {
                    AddList(fieldtype_list);
                }

                var enum_list = new List<sys_enum>()
                {
                    new sys_enum(){ GID=Guid.NewGuid(), Name="Status",Title="状态", JsonData="[{\"ID\":0,\"Name\":\"禁用\"},{\"ID\":1,\"Name\":\"启用\"}]", DataType=0, SystemMark=1, CreateTime=DateTime.Now, UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_enum(){ GID=Guid.NewGuid(), Name="SystemMark",Title="系统标志", JsonData="[{\"ID\":0,\"Name\":\"用户\"},{\"ID\":1,\"Name\":\"系统\"}]", DataType=0, SystemMark=1, CreateTime=DateTime.Now, UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_enum(){ GID=Guid.NewGuid(), Name="DataType",Title="数据类型", JsonData="[{\"ID\":0,\"Name\":\"普通数据\"},{\"ID\":1,\"Name\":\"表数据\"}]", DataType=0, SystemMark=1, CreateTime=DateTime.Now, UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                };
                if (!CheckTableExists<sys_enum>())
                {
                    AddList(enum_list);
                }

                if (!CheckTableExists<sys_table_column>())
                {
                    foreach (var schema in TablesSchema)
                    {
                        var model_table = table_list.FirstOrDefault(m => m.Name == schema.Name);
                        foreach (var column in schema.Columns)
                        {
                            var model_field = fieldtype_list.FirstOrDefault(m => m.Name == column.TypeName);
                            if (model_field != null)
                            {
                                var model_enum = enum_list.FirstOrDefault(m => m.Name == column.Name);
                                var enumid = 0;
                                if (model_enum != null) { enumid = model_enum.ID; }
                                Add(new sys_table_column() { GID = Guid.NewGuid(), Name = column.Name, FieldTypeID = model_field.ID, MaxLength = column.MaxLength, DefaultValue = null, EnumID = enumid, TableID = model_table.ID, CreateTime = DateTime.Now, UpdateTime = DateTime.Now, SortNo = 0, Status = 1 });
                            }
                        }
                    }



                }


            }

            return true;
        }


        List<MyTable> tablesSchema;
        public List<MyTable> TablesSchema
        {
            get
            {
                if (tablesSchema == null)
                {
                    tablesSchema = GetTablesSchema();
                }
                return tablesSchema;
            }
        }
        public List<MyTable> GetTablesSchema()
        {
            try
            {
                var mytables = new List<MyTable>();

                var ds = this.CurrentDataBase.ExecuteDataSet("SELECT [sys].[tables].[name] tb_name, [sys].[tables].[object_id] tb_id,[sys].[extended_properties].[value] tb_desc FROM [sys].[tables] LEFT JOIN [sys].[extended_properties] ON [sys].[tables].[object_id] = [sys].[extended_properties].[major_id] and [sys].[extended_properties].[minor_id]=0");
                foreach (System.Data.DataRow item in ds.Tables[0].Rows)
                {
                    var mytable = new MyTable();
                    mytable.Name = item["tb_name"].ToString();
                    mytable.TbId = Convert.ToInt32(item["tb_id"].ToString());
                    mytable.Title = item["tb_desc"].ToString();
                    mytable.SystemMark = mytable.Name.StartsWith("sys_") ? 1 : 0;

                    var strs = mytable.Name.Split('_');
                    if (strs.Length > 1)
                    {
                        mytable.Prefix = strs[0];
                        mytable.RestName = mytable.Name.Substring(strs[0].Length + 1);
                    }
                    else
                    {
                        mytable.Prefix = "";
                        mytable.RestName = mytable.Name;
                    }

                    var mytable_columns = new List<MyColumn>();
                    var ds_column = this.CurrentDataBase.ExecuteDataSet("select column_name,max_length,typename,[sys].[extended_properties].[value] column_desc from (SELECT [object_id] tb_id ,[sys].[columns].[name] column_name,[column_id],[max_length] ,[sys].[systypes].[name] typename FROM [sys].[columns] LEFT JOIN [sys].[systypes] ON [sys].[columns].[user_type_id]=[sys].[systypes].[xusertype] WHERE object_id='" + mytable.TbId + "') t1 LEFT JOIN [sys].[extended_properties] ON t1.tb_id=[sys].[extended_properties].[major_id] and t1.column_id=[sys].[extended_properties].[minor_id]");
                    foreach (System.Data.DataRow row_c in ds_column.Tables[0].Rows)
                    {
                        var mycolumn = new MyColumn();
                        mycolumn.Name = row_c["column_name"].ToString();
                        mycolumn.MaxLength = Convert.ToInt32(row_c["max_length"].ToString());

                        mycolumn.TypeName = row_c["typename"].ToString();
                        mycolumn.Title = row_c["column_desc"].ToString();

                        if (mycolumn.TypeName == "nvarchar" || mycolumn.TypeName == "varchar")
                        {
                            mycolumn.MaxLength = mycolumn.MaxLength / 2;
                        }
                        else
                        {
                            mycolumn.MaxLength = -1;
                        }

                        mytable_columns.Add(mycolumn);
                    }
                    mytable.Columns = mytable_columns;
                    mytables.Add(mytable);
                }
                return mytables;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataSet ExecuteDataSet(string sql)
        {
            return this.CurrentDataBase.ExecuteDataSet(sql);
        }

        public object ExecuteScalar(string sql)
        {
            return this.CurrentDataBase.ExecuteScalar(sql);
        }
        public bool ExecuteNonQuery(string sql)
        {
            try
            {
                this.CurrentDataBase.ExecuteNonQuery(sql);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool ExistsTable(string tablename)
        {
            var sql = "select 1 from [sys].[tables] where name ='u_" + ReplaceFieldValue(tablename) + "'";
            var num = ExecuteScalar(sql);
            return num != null;
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

        public DataTable GetListForPage(string tablename, Paging paging)
        {
            if (string.IsNullOrWhiteSpace(tablename)) { return null; }
            tablename = ReplaceFieldValue(tablename);
            var rez = Convert.ToInt32(ExecuteScalar(string.Format("select count(*) from [{0}]", tablename)).ToString());
            paging.Count = rez;
            var sql = string.Format("SELECT TOP {0} * FROM (SELECT ROW_NUMBER() OVER (ORDER BY id) AS RowNumber,* FROM [{1}]) as A WHERE RowNumber > {0}*({2}-1)", paging.PageSize, tablename, paging.PageIndex);
            var dset = ExecuteDataSet(sql);

            return dset.Tables[0];
        }

        public DataTable GetListForPage(string tablename, Paging paging, List<ColumnInfo> myColumns)
        {
            if (string.IsNullOrWhiteSpace(tablename)) { return null; }
            tablename = ReplaceFieldValue(tablename);
            var rez = Convert.ToInt32(ExecuteScalar(string.Format("select count(*) from [{0}]", tablename)).ToString());
            paging.Count = rez;
            var sql = string.Format("SELECT TOP {0} * FROM (SELECT ROW_NUMBER() OVER (ORDER BY id) AS RowNumber,* FROM [{1}]) as A WHERE RowNumber > {0}*({2}-1)", paging.PageSize, tablename, paging.PageIndex);
            var dset = ExecuteDataSet(sql);
            var dt = dset.Tables[0];
            var jsondata = dt.ToJson();
            var dt2 =new DataTable();
            dt.Columns.Remove("RowNumber");
            dt.Columns.Remove("VersonTime");
            foreach (DataColumn c in dt.Columns)
            {
                var myc = myColumns.FirstOrDefault(m => m.Name == c.ColumnName);
                
                var dc = new DataColumn();
               
                dc.ColumnName = c.ColumnName;
                
                if (c.DataType == typeof(DateTime))
                {
                    dc.DataType = typeof(String);
                }

                dc.Caption = c.Caption;

                if (myc != null)
                {
                    dc.ColumnName = myc.Title;
                }

                dt2.Columns.Add(dc);
            }
            foreach (DataRow dataRow in dt.Rows)
            {
                var dr2=dt2.NewRow();
                dr2.ItemArray = dataRow.ItemArray;
                dt2.Rows.Add(dr2);
            }

            return dt2;
        }


        public bool AddColumn(string tableName, string fieldName, string fieldType, int maxLength)
        {
            var str = "";
            if (fieldType == "nvarchar" || fieldType == "varchar")
            {
                str = " " + fieldType + "(" + maxLength.ToString() + ") ";
            }
            else
            {
                str = " " + fieldType + " ";
            }
            var sql = "alter table " + tableName + " add column " + fieldName + " " + str + ";";
            var num = ExecuteNonQuery(sql);
            return num;
        }

        public bool DelColumn(string tableName, string fieldName)
        {
            var sql = "alter table " + ReplaceFieldValue(tableName) + " drop column " + ReplaceFieldValue(fieldName) + ";";
            var num = ExecuteNonQuery(sql);
            return num;
        }
    }
}
