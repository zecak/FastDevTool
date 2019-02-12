using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSoup.Nodes;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class NextTextFunction : BaseFunction, IGetValueByElement
    {
        public override string Name => "NextText";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            return new NextTextFunction();
        }

        public string GetValue(Element find_element)
        {
            return find_element.NextSibling.Attr("text").Trim();
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + RightSeparator;
        }
    }
}
