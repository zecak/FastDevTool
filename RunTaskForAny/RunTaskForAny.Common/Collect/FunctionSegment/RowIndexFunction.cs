using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class RowIndexFunction : BaseFunction
    {
        public int RowIndex { get; set; }
        public RowIndexFunction(int rowIndex)
        {
            RowIndex = rowIndex;
        }

        public override string Name => "RowIndex";

        public override string ToSegment()
        {
            return LeftSeparator + Name + KeySeparator + RowIndex + RightSeparator;
        }

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            var temps = segment.Substring(1, segment.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (temps.Length == 2)
            {
                return new RowIndexFunction(int.Parse(temps[1]));
            }
            return null;
        }
    }
}
