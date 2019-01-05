using RunTaskForAny.Common.Helper;
using RunTaskForAny.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace RunTaskForAny
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                HostFactory.Run(x =>
                {
                    x.Service<MyTask>(t =>
                    {
                        t.ConstructUsing(n => new MyTask());
                        t.WhenStarted(tc => tc.Start());
                        t.WhenStopped(tc => tc.Stop());
                    });

                    x.RunAsLocalSystem();
                    x.StartAutomatically();
                    x.SetDescription("运行任何任务模块");
                    x.SetDisplayName("运行服务中心");
                    x.SetServiceName("Run Service Center");
                });
            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex.Message);
                Tool.Log.Error(ex.StackTrace);
            }
            Console.ReadKey();
        }
    }
}
