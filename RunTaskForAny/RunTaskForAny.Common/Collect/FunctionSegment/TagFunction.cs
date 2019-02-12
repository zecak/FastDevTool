using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSoup.Nodes;
using NSoup.Select;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class TagFunction : BaseFunction, IFindElements
    {
        public string TagName { get; set; }
        public TagFunction()
        {
        }
        public TagFunction(string tagName)
        {
            TagName = tagName;
        }

        public override string Name => "Tag";

        public override string ToSegment()
        {
            return LeftSeparator + Name + KeySeparator + TagName + RightSeparator;
        }

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            var temps = segment.Substring(1, segment.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (temps.Length == 2)
            {
                return new TagFunction(temps[1]);
            }
            return null;
        }

        public Elements FindElements(Element find_element)
        {
            return find_element.GetElementsByTag(TagName);
        }
    }
}
