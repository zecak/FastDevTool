using Grpc.Agent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Grpc.Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Tool.Log.Info(Tool.Setting.AgentIP + ":" + Tool.Setting.AgentPort);
                HostFactory.Run(x =>
                {
                    x.Service<MyWork>(t =>
                    {
                        t.ConstructUsing(n => new MyWork());
                        t.WhenStarted(tc => tc.Start());
                        t.WhenStopped(tc => tc.Stop());
                    });

                    x.RunAsLocalSystem();
                    x.StartAutomatically();
                    x.SetDescription(Tool.Setting.Description);
                    x.SetDisplayName(Tool.Setting.Name);
                    x.SetServiceName(Tool.Setting.ServiceName);
                });
            }
            catch (Exception ex)
            {
                Tool.Log.Error("--------------");
                Tool.Log.Error(ex.Message);
                Tool.Log.Error(ex);
                Tool.Log.Error("--------------");
                Console.ReadKey();
            }
        }
    }
}
