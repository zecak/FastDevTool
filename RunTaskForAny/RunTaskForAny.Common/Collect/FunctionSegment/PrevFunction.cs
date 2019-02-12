using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSoup.Nodes;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class PrevFunction : BaseFunction, IFindElement
    {
        public override string Name => "Prev";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            return new PrevFunction();
        }

        public Element FindElement(Element find_element)
        {
            return find_element.PreviousElementSibling;
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + RightSeparator;
        }
    }
}
