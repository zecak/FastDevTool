using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSoup.Nodes;
using NSoup.Select;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class ChildFunction : BaseFunction,IFindElements
    {
        public override string Name => "Child";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            return new ChildFunction();
        }

        public Elements FindElements(Element find_element)
        {
           return find_element.Children;
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + RightSeparator;
        }
    }
}
