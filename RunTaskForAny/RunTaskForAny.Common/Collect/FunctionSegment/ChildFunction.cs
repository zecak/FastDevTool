using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class ChildFunction : BaseFunction
    {
        public override string Name => "Child";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            return new ChildFunction();
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + RightSeparator;
        }
    }
}
