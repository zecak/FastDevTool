using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Test.PageRule.FunctionSegment
{
    public class GetAttrFunction : BaseFunction
    {
        public string AttrName { get; set; }
        public GetAttrFunction(string attrName)
        {
            AttrName = attrName;
        }
        public override string Name => "GetAttr";

        public override string ToSegment()
        {
            return LeftSeparator + Name + KeySeparator + AttrName + RightSeparator;
        }

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            var temps = segment.Substring(1, segment.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (temps.Length == 2)
            {
                return new GetAttrFunction(temps[1]);
            }
            return null;
        }
    }
}
