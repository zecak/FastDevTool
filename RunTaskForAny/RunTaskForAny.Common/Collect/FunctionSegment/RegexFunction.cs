using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class RegexFunction : BaseFunction
    {
        public string Pattern { get; set; }
        public RegexFunction()
        {
        }
        public RegexFunction(string pattern)
        {
            Pattern = pattern;
        }
        public override string Name => "Regex";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            var temps = segment.Substring(1, segment.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (temps.Length == 2)
            {
                return new RegexFunction(temps[1]);
            }
            return null;
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + KeySeparator + Pattern + RightSeparator;
        }
    }
}
