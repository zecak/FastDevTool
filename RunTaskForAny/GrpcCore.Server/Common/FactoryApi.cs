using GrpcCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GrpcCore.Server.Common
{
    public class FactoryApi
    {

        static List<AExecAtion> functions = null;
        public static List<AExecAtion> Functions
        {
            get
            {
                if (functions == null)
                {
                    LoadScript();
                }
                return functions;
            }
        }

        public static void LoadScript()
        {
            functions = new List<AExecAtion>();
            var scriptPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");
            var files = System.IO.Directory.GetFiles(scriptPath, "*.cs", System.IO.SearchOption.AllDirectories);
            foreach (var file in files)
            {
                functions.Add(CSScriptLib.CSScript.RoslynEvaluator.LoadFile<AExecAtion>(file));
            }
        }

        /// <summary>
        /// 获取某接口操作类型
        /// </summary>
        /// <param name="apiPath">Api地址</param>
        /// <returns></returns>
        public static AExecAtion CreateFunction(string apiPath = "")
        {
            var api = Functions.FirstOrDefault(m => m.ActionName == apiPath);
            return api ?? new ANoneAtion();
        }

    }
}
