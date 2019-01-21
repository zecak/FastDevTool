using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class DownFunction : BaseFunction
    {
        public string FilePath { get; set; }
        public DownFunction()
        {
        }
        public DownFunction(string filePath)
        {
            FilePath = filePath;
        }
        public override string Name => "Down";

        public override string ToSegment()
        {
            return LeftSeparator + Name + KeySeparator + FilePath + RightSeparator;
        }

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            var temps = segment.Substring(1, segment.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (temps.Length == 2)
            {
                return new DownFunction(temps[1]);
            }
            return null;
        }
    }
}
