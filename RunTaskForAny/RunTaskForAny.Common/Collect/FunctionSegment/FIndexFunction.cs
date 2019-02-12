using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSoup.Nodes;
using NSoup.Select;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class FIndexFunction : BaseFunction, IFindElementByList
    {
        public override string Name => "FIndex";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            return new FIndexFunction();
        }

        public Element FindElement(Elements find_elements)
        {
            return find_elements.First;
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + RightSeparator;
        }
    }
}
