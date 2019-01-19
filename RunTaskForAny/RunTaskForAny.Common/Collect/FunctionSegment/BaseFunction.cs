using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
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
            if (segment.Length <= str.Length) { return false; }
            var ok = segment.StartsWith(str);
            var nextstr = segment.Substring(str.Length, 1);
            var ok2 = (nextstr == RightSeparator || nextstr == KeySeparator.Substring(0, 1));
            return ok && ok2;
        }

        public string KeySeparator { get { return "::"; } }
        public string ValueSeparator { get { return "="; } }
        public string LeftSeparator { get { return "["; } }
        public string RightSeparator { get { return "]"; } }


    }
}
