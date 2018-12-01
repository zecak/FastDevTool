using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.DataBase
{
    public class MyTable
    {
        /// <summary>
        /// 表名,最大128字符,以字母或下划线开头
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 表临时id，注意修改表后，此id会变
        /// </summary>
        public int TbId { get; set; }

        /// <summary>
        /// 表别名
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否是系统表,0否,1是
        /// </summary>
        public int SystemMark { get; set; }
        /// <summary>
        /// 前缀(以下划线分割,前部分)
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 剩下名称(以下划线分割,后部分)
        /// </summary>
        public string RestName { get; set; }

        public List<MyColumn> Columns { get; set; }
    }
}
