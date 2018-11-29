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

        public LocalDbContext()
               : base(SQLHelper.MyDBCenter)
        {

        }

        protected override bool CheckAllTableExists()
        {
            if (!CheckTableExists<sys_table>())//创建表并添加默认数据
            {
                AddList(new List<sys_table>()
                {
                    new sys_table(){GID=Guid.NewGuid(),Name="sys_table",Title="表字典",Description="数据库表信息",SystemMark=1, Prefix="sys",RestName="table",CreateTime=DateTime.Now,UpdateTime=DateTime.Now,SortNo=0,Status=1},
                    new sys_table(){GID=Guid.NewGuid(),Name="sys_field_type",Title="字段类型",Description="数据库字段类型信息",SystemMark=1, Prefix="sys",RestName="field_type",CreateTime=DateTime.Now,UpdateTime=DateTime.Now,SortNo=0,Status=1},
                    new sys_table(){GID=Guid.NewGuid(),Name="sys_enum",Title="枚举类型",Description="数据库枚举类型信息",SystemMark=1, Prefix="sys",RestName="enum",CreateTime=DateTime.Now,UpdateTime=DateTime.Now,SortNo=0,Status=1},
                    new sys_table(){GID=Guid.NewGuid(),Name="sys_table_column",Title="列字典",Description="数据库列信息",SystemMark=1, Prefix="sys",RestName="table_column",CreateTime=DateTime.Now,UpdateTime=DateTime.Now,SortNo=0,Status=1},
                });
            }

            if (!CheckTableExists<sys_field_type>())
            {
                //mssql
                AddList(new List<sys_field_type>()
                {
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="int", Title="整型", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="decimal", Title="精确数值", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="money", Title="货币", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="datetime", Title="日期时间", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="nvarchar", Title="字符串", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="ntext", Title="文本", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="timestamp", Title="时间戳", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_field_type(){ GID=Guid.NewGuid(), Name="uniqueidentifier", Title="全局唯一标记符", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                });

                ////mysql
                //AddList(new List<sys_field_type>()
                //{
                //    new sys_field_type(){ GID=Guid.NewGuid(), Name="int", Title="整型", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                //    new sys_field_type(){ GID=Guid.NewGuid(), Name="decimal", Title="精确数值", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                   
                //    new sys_field_type(){ GID=Guid.NewGuid(), Name="datetime", Title="日期时间", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                //    new sys_field_type(){ GID=Guid.NewGuid(), Name="varchar", Title="字符串", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                //    new sys_field_type(){ GID=Guid.NewGuid(), Name="longtext", Title="文本", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                //    new sys_field_type(){ GID=Guid.NewGuid(), Name="timestamp", Title="时间戳", SystemMark=1, CreateTime=DateTime.Now,UpdateTime=DateTime.Now, SortNo=0, Status=1 },

                //});
            }

            if (!CheckTableExists<sys_enum>())
            {
                AddList(new List<sys_enum>()
                {
                    new sys_enum(){ GID=Guid.NewGuid(), Name="状态", JsonData="[{\"ID\":0,\"Name\":\"禁用\"},{\"ID\":1,\"Name\":\"启用\"}]", DataType=0, SystemMark=1, CreateTime=DateTime.Now, UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_enum(){ GID=Guid.NewGuid(), Name="系统标志", JsonData="[{\"ID\":0,\"Name\":\"用户\"},{\"ID\":1,\"Name\":\"系统\"}]", DataType=0, SystemMark=1, CreateTime=DateTime.Now, UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                    new sys_enum(){ GID=Guid.NewGuid(), Name="枚举状态", JsonData="[{\"ID\":0,\"Name\":\"普通数据\"},{\"ID\":1,\"Name\":\"表数据\"}]", DataType=0, SystemMark=1, CreateTime=DateTime.Now, UpdateTime=DateTime.Now, SortNo=0, Status=1 },
                });
            }

            if (!CheckTableExists<sys_table_column>())
            {

            }


            return true;
        }





    }
}
