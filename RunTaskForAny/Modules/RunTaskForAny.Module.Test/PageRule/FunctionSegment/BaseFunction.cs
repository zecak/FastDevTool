using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Test.PageRule.FunctionSegment
{
    public abstract class BaseFunction
    {
        public BaseFunction()
        {

        }

        public abstract string Name { get; }
        public abstract string ToSegment();
        public abstract BaseFunction AnalyzeSegment(string segment);

        public bool StartsWithPartSegment(string segment)
        {
            var str = LeftSeparator + Name;
            return segment.StartsWith(str);
        }

        public string KeySeparator { get { return "::"; } }
        public string ValueSeparator { get { return "="; } }
        public string LeftSeparator { get { return "["; } }
        public string RightSeparator { get { return "]"; } }


    }
}
