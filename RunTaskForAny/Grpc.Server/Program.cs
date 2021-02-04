using Grpc.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Grpc.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var sql = "UPDATE t_employee_wallet set Balance=Balance+{0} WHERE EmployeeID=(SELECT ID FROM t_employee WHERE MobileNo='{1}' and LastTransCode<>'D' LIMIT 1)";
                //Tool.Log.Debug(string.Format(sql,new string[] { "1","18718842697" }));
                Tool.Log.Info(Tool.Setting.ServerIP+":"+Tool.Setting.ServerPort);
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
                Tool.Log.Error("[--------------");
                Tool.Log.Error(ex.Message);
                Tool.Log.Error(ex);
                Tool.Log.Error(" --------------]");
                Console.ReadKey();
            }

        }
    }

}
