using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSoup.Nodes;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class HtmlFunction : BaseFunction, IGetValueByElement
    {
        public override string Name => "Html";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            return new HtmlFunction();
        }

        public string GetValue(Element find_element)
        {
            return find_element.Html().Trim();
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + RightSeparator;
        }
    }
}
