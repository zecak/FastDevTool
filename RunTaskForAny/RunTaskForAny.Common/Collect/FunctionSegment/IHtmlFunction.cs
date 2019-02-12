using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSoup.Nodes;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class IHtmlFunction : BaseFunction, IGetValueByElement
    {
        public override string Name => "IHtml";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            return new IHtmlFunction();
        }

        public string GetValue(Element find_element)
        {
            return find_element.Children.Html().Trim();
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + RightSeparator;
        }
    }
}
