using RunTaskForAny.Common.Collect.FunctionSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionRule
{
    public class FactoryFunction
    {
        static List<BaseFunction> functions = null;

        /// <summary>
        /// 创建基础功能
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static BaseFunction CreateFunction(string segment)
        {
            if (functions == null || functions.Count == 0)
            {
                functions = new List<BaseFunction>();
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] types = assembly.GetTypes();
                foreach (var t in types)
                {
                    if (t.BaseType != null)
                    {
                        if (t.BaseType.Name == nameof(BaseFunction))
                        {
                            functions.Add((BaseFunction)Activator.CreateInstance(t));
                        }
                    }
                }
            }

            var function = functions.FirstOrDefault(m => m.StartsWithPartSegment(segment));
            if (function != null)
            {
                return function.AnalyzeSegment(segment);
            }
            return null;
        }

    }
}
