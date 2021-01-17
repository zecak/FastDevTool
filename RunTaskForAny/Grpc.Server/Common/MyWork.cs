using GrpcLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Server.Common
{
    public class MyWork
    {
        Core.Server server = null;
        public void Start()
        {
            try
            {
                server = new Core.Server
                {
                    Services = { gRPC.BindService(new GrpcImpl()) },
                    Ports = { new Core.ServerPort(Tool.Setting.ServerIP, Tool.Setting.ServerPort.ToInt(), Core.ServerCredentials.Insecure) }
                };

                server.Start();


            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex);
            }
           
        }

        public void Stop()
        {
            try
            {
                server?.ShutdownAsync().Wait();
                server = null;
            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex);
            }
           
        }
    }
}
