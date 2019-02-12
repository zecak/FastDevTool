using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSoup.Nodes;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class GetAttrFunction : BaseFunction, IGetValueByElement
    {
        public string AttrName { get; set; }
        public GetAttrFunction()
        {
        }
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

        public string GetValue(Element find_element)
        {
            return find_element.Attr(AttrName).Trim();
        }
    }
}
