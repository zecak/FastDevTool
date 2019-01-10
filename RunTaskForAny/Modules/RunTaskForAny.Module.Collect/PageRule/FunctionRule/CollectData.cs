using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Collect.PageRule.FunctionRule
{
    public class CollectData
    {
        
        public DataTable FirstData { get; set; }

        public DataTable NextPageData { get; set; }
        public DataTable ListData { get; set; }
        public DataTable ContentData { get; set; }
    }
}
