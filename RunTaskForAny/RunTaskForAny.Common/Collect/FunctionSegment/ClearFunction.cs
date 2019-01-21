using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class ClearFunction : BaseFunction
    {
        public string AttrValue { get; set; }
        public ClearFunction()
        {
        }
        public ClearFunction(string attrValue)
        {
            AttrValue = attrValue;
        }
        public override string Name => "Clear";

        public override string ToSegment()
        {
            return LeftSeparator + Name + KeySeparator + AttrValue + RightSeparator;
        }

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            var temps = segment.Substring(1, segment.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (temps.Length == 2)
            {
                return new ClearFunction(temps[1]);
            }
            return null;
        }
    }
}
