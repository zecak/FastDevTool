using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSoup.Nodes;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class PrevTextFunction : BaseFunction, IGetValueByElement
    {
        public override string Name => "PrevText";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            return new PrevTextFunction();
        }

        public string GetValue(Element find_element)
        {
            return find_element.PreviousSibling.Attr("text").Trim();
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + RightSeparator;
        }
    }
}
