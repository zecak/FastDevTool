using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Collect.PageRule.FunctionSegment
{

    public class RegexAndDecodeMagnetFunction : BaseFunction
    {
        public string Pattern { get; set; }
        public RegexAndDecodeMagnetFunction(string pattern)
        {
            Pattern = pattern;
        }
        public override string Name => "RegexAndDecodeMagnet";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            var temps = segment.Substring(1, segment.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (temps.Length == 2)
            {
                return new RegexAndDecodeMagnetFunction(temps[1]);
            }
            return null;
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + KeySeparator + Pattern + RightSeparator;
        }
    }
}
