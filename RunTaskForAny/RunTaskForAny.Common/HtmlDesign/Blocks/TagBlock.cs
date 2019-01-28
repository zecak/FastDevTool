using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.HtmlDesign.Blocks
{
    /// <summary>
    /// 标签块
    /// </summary>
    public class TagBlock
    { 
        public string Name { get; set; }
        public string Html { get; set; }
        public List<ResData> DependOnList { get; set; }

    }
}
