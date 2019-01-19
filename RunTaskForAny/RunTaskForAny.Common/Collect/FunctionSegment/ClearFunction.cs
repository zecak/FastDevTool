using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class ClearFunction : BaseFunction
    {
        public string AttrName { get; set; }
        public string AttrValue { get; set; }
        public ClearFunction(string attrName, string attrValue)
        {
            AttrName = attrName;
            AttrValue = attrValue;
        }
        public override string Name => "Clear";

        public override string ToSegment()
        {
            return LeftSeparator + Name + KeySeparator + AttrName + ValueSeparator + AttrValue + RightSeparator;
        }

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            var temps = segment.Substring(1, segment.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (temps.Length == 2)
            {
                var vals = temps[1].Split(new string[] { ValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
                if (vals.Length == 2)
                {
                    return new ClearFunction(vals[0], vals[1]);
                }
            }
            return null;
        }
    }
}
