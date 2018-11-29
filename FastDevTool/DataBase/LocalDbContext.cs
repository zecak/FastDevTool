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
            if(!CheckTableExists<sys_table>())//创建表并添加默认数据
            {

            }

            if (!CheckTableExists<sys_field_type>())
            {

            }

            if (!CheckTableExists<sys_enum>())
            {

            }

            if (!CheckTableExists<sys_table_column>())
            {

            }
            

            return true;
        }





    }
}
