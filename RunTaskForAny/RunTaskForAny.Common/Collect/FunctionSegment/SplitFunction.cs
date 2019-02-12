using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class SplitFunction : BaseFunction, IValueToValues
    {
        public string Separator { get; set; }
        public SplitFunction()
        {
        }
        public SplitFunction(string separator)
        {
            Separator = separator;
        }
        public override string Name => "Split";

        public override string ToSegment()
        {
            return LeftSeparator + Name + KeySeparator + Separator + RightSeparator;
        }

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            var temps = segment.Substring(1, segment.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (temps.Length == 2)
            {
                return new SplitFunction(temps[1]);
            }
            return null;
        }

        public string[] GetValues(string val)
        {
            if (!string.IsNullOrWhiteSpace(val))
            {
                return val.Split(new string[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
            }
            return null;
        }
    }
}
