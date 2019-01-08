using RunTaskForAny.Common.Helper;
using RunTaskForAny.Module.Collect.PageRule.FunctionRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Collect.PageRule.FunctionSegment
{
    public class ListFunction : BaseFunction
    {
        public override string Name => "List";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            return new ListFunction();
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + RightSeparator;
        }
    }
}
