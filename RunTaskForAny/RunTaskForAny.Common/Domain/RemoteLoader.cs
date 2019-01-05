using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Domain
{
    public class RemoteLoader : MarshalByRefObject
    {
        List<AssemblyKeyValue> Assemblylist = new List<AssemblyKeyValue>();

        public List<string> FileNameModules { get; set; } = new List<string>();

        public RemoteLoader()
        {
        }

        public void Load(string filename)
        {
            var fileinfo = new FileInfo(filename);
            var name = fileinfo.Name.Replace(".dll", "");
            var assembly = Assemblylist.FirstOrDefault(m => m.Key == name);
            if (assembly == null)
            {
                byte[] assemblyBuf = File.ReadAllBytes(filename);
                Assemblylist.Add(new AssemblyKeyValue() { Key = name, Value = Assembly.Load(assemblyBuf) });
                FileNameModules.Add(name);
            }
            else
            {
                byte[] assemblyBuf = File.ReadAllBytes(filename);
                assembly.Value = Assembly.Load(assemblyBuf);
            }

        }

        public void UnLoad()
        {

        }

        public object Invoke(string fullClassName, string methodName, Type[] types, object[] args, object[] class_args = null)
        {
            var name = fullClassName.Substring(0, fullClassName.LastIndexOf('.'));
            var assemblykv = Assemblylist.FirstOrDefault(m => m.Key == name);
            if (assemblykv == null)
            {
                throw new Exception("找不到对应类库:" + name);
            }
            var assembly = assemblykv.Value;
            Type tp = assembly.GetType(fullClassName);
            if (tp == null)
            {
                throw new Exception("找不到类型FullClassName:" + fullClassName);
            }
            MethodInfo method = null;

            if (args == null || args.Length == 0)
            {
                method = tp.GetMethod(methodName, new Type[] { });
            }
            else
            {
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i] != args[i].GetType())
                    {
                        args[i] = Convert.ChangeType(args[i], types[i]);
                    }
                }
                method = tp.GetMethod(methodName, types);

            }

            if (method == null)
            {
                throw new Exception("找不到方法FullClassName:" + fullClassName + ",MethodName:" + methodName);
            }
            var obj = Activator.CreateInstance(tp, class_args);

            return method.Invoke(obj, args);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

    }
}
