using GrpcCore.Common;
using GrpcCore.Common.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcCore.Server.Common
{
    public class MyWork
    {
        Grpc.Core.Server server = null;
        public void Start()
        {
            try
            {
                server = new Grpc.Core.Server
                {
                    Services = { gRPC.BindService(new GrpcImpl()) },
                    Ports = { new Grpc.Core.ServerPort(Tool.Setting.ServerIP, Tool.Setting.ServerPort.ToInt(), Grpc.Core.ServerCredentials.Insecure) }
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
