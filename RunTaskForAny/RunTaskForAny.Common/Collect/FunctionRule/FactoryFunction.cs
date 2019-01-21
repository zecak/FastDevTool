using RunTaskForAny.Common.Collect.FunctionSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionRule
{
    public class FactoryFunction
    {
        static List<BaseFunction> functions = new List<BaseFunction>()
        {
            new AttrFunction(),
            new ChildFunction(),
            new ClearFunction(),
            new FIndexFunction(),
            new GetAttrFunction(),
            new HtmlFunction(),
            new IHtmlFunction(),
            new IndexFunction(),
            new LIndexFunction(),
            new LinkFunction(),
            new NextFunction(),
            new NextTextFunction(),
            new ParentFunction(),
            new PrevFunction(),
            new PrevTextFunction(),
            new RowIndexFunction(),
            new TagFunction(),
            new TextFunction(),
            new RemoveTagFunction(),
            new ListFunction(),
            new RegexFunction(),
            new RegexAndDecodeMagnetFunction(),
            new DownFunction(),
            new SplitFunction(),
            new StringsFunction(),
        };

        /// <summary>
        /// 创建基础功能
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static BaseFunction CreateFunction(string segment)
        {
            var function = functions.FirstOrDefault(m=>m.StartsWithPartSegment(segment));
            if(function!=null)
            {
               return function.AnalyzeSegment(segment);
            }
            return null;
        }

    }
}
