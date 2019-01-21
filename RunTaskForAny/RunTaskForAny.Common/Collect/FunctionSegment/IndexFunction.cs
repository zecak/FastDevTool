using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class IndexFunction : BaseFunction
    {
        public int Index { get; set; }
        public IndexFunction()
        {
        }
        public IndexFunction(int index)
        {
            Index = index;
        }
        public override string Name => "Index";

        public override string ToSegment()
        {
            return LeftSeparator + Name + KeySeparator + Index + RightSeparator;
        }

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            var temps = segment.Substring(1, segment.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (temps.Length == 2)
            {
                return new IndexFunction(int.Parse(temps[1]));
            }
            return null;
        }
    }
}
