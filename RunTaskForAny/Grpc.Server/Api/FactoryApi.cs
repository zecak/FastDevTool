using GrpcLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Server.Api
{
    public class FactoryApi
    {

        static List<AExecAtion> functions = null;

        /// <summary>
        /// 获取某接口操作类型
        /// </summary>
        /// <param name="apiPath">Api地址</param>
        /// <returns></returns>
        public static AExecAtion CreateFunction(string apiPath)
        {
            if (functions == null || functions.Count == 0)
            {
                functions = new List<AExecAtion>();
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] types = assembly.GetTypes();
                foreach (var t in types)
                {
                    if (t.BaseType != null)
                    {
                        if (t.BaseType.Name == nameof(AExecAtion))
                        {
                            functions.Add((AExecAtion)Activator.CreateInstance(t));
                        }
                    }
                }
            }

            return functions.FirstOrDefault(m => m.ActionName == apiPath);
        }

    }
}
