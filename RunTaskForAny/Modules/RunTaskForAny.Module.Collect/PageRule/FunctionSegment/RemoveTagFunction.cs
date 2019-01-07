using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Collect.PageRule.FunctionSegment
{
    public class RemoveTagFunction:BaseFunction
    {
        public string TagName { get; set; }
        public RemoveTagFunction(string tagName)
        {
            TagName = tagName;
        }

        public override string Name => "RemoveTag";

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
                return new RemoveTagFunction(temps[1]);
            }
            return null;
        }
    }
}
