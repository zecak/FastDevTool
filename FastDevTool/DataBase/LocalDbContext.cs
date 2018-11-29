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
            var cte = CheckTableExists<sys_table>();

            //添加默认数据
            
            cte = CheckTableExists<sys_field_type>();

            //添加默认数据
            
            cte = CheckTableExists<sys_enum>();

            //添加默认数据
            
            cte = CheckTableExists<sys_table_column>();

            //添加默认数据


            return true;
        }





    }
}
