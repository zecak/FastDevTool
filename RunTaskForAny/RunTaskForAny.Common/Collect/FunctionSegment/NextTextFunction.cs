using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class NextTextFunction : BaseFunction
    {
        public override string Name => "NextText";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            return new NextTextFunction();
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + RightSeparator;
        }
    }
}
