using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.HtmlDesign
{
    public class ResData
    {
        public string Name { get; set; }

        public string Version { get; set; }

        /// <summary>
        /// 是否链接,是:引用css或script;否:直接css或script内容
        /// </summary>
        public bool IsLink { get; set; }

        /// <summary>
        /// 数据,链接或内容
        /// </summary>
        public string Data { get; set; }

        public List<ResData> DependOns { get; set; }
    }
}
