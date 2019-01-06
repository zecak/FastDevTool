﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Test.PageRule.FunctionSegment
{
    public class LinkFunction : BaseFunction
    {
        public override string Name => "Link";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            return new LinkFunction();
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + RightSeparator;
        }
    }
}
