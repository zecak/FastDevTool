using RunTaskForAny.Module.Test.PageRule.FunctionSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Test.PageRule.FunctionRule
{
    /// <summary>
    /// 功能段:基础功能集合
    /// </summary>
    public class FunctionRuleSegment
    {

        /// <summary>
        /// 功能段名称
        /// </summary>
        public string Name { get; set; }

        public string Segments
        {
            get { return ToString(); }
            set { LoadSegments(value); }
        }

        /// <summary>
        /// 功能分隔符
        /// </summary>
        public const string FunctionSeparator = "$$";
        /// <summary>
        /// 功能段
        /// </summary>
        List<BaseFunction> Functions;

        public List<BaseFunction> GetFunctions()
        {
            return Functions;
        }

        public BaseFunction this[int index] { get { return Functions[index]; } }

        public FunctionRuleSegment()
        {

        }

        public FunctionRuleSegment(string name, string segments)
        {
            Name = name;
            LoadSegments(segments);
        }

        void LoadSegments(string segments)
        {
            Functions = new List<BaseFunction>();
            if (!string.IsNullOrWhiteSpace(segments))
            {
                var segmentList = segments.Split(new string[] { FunctionSeparator }, StringSplitOptions.RemoveEmptyEntries);
                if (segmentList != null && segmentList.Length > 0)
                {
                    foreach (var segment in segmentList)
                    {
                        var function = FactoryFunction.CreateFunction(segment);
                        if (function != null)
                        {
                            Functions.Add(function);
                        }
                    }
                }
            }
        }

        public FunctionRuleSegment(string name, List<BaseFunction> functions)
        {
            Name = name;
            Functions = functions;
        }

        public override string ToString()
        {
            if (Functions == null || Functions.Count <= 0) { return ""; }
            var str = "";
            for (int i = 0; i < Functions.Count; i++)
            {
                var function = Functions[i];
                if ((i + 1) == Functions.Count)
                {
                    str += function.ToSegment();
                }
                else
                {
                    str += function.ToSegment() + FunctionSeparator;
                }
            }
            return str;
        }


    }
}
