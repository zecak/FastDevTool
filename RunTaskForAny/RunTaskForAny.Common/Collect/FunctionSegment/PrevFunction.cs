using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class PrevFunction : BaseFunction
    {
        public override string Name => "Prev";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            return new PrevFunction();
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + RightSeparator;
        }
    }
}
