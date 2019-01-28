using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.HtmlDesign
{
    public class HtmlTemplate
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Keyword { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public List<ResData> Res { get; set; }

        public string Body { get; set; }
    }
}
