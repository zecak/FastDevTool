using GrpcCore.Common;
using GrpcCore.Common.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrpcCore.Server.Common
{
    public class MyWork
    {
        Grpc.Core.Server server = null;

        private MyHttpServer myHttpServer = null;
        public void Start()
        {
            try
            {
                Tool.Log.Debug("--- gRPC:监听端口[" + Tool.Setting.ServerPort + "] ---");
                server = new Grpc.Core.Server
                {
                    Services = { gRPC.BindService(new GrpcImpl()) },
                    Ports = { new Grpc.Core.ServerPort(Tool.Setting.ServerIP, Tool.Setting.ServerPort.ToInt(), Grpc.Core.ServerCredentials.Insecure) }
                };

                server.Start();
                Tool.Log.Info("[gRPC]服务->启动成功");

                Tool.Log.Debug("-------------------------------------------------");
                Tool.Log.Debug("--- Http:监听端口[" + Tool.Setting.HttpPort + "] ---");
                if (Tool.PortInUse(Tool.Setting.HttpPort.ToInt()))
                {
                    Tool.Log.Error("Http端口[" + Tool.Setting.HttpPort + "]被占用");
                    throw new Exception("Http端口[" + Tool.Setting.HttpPort + "]被占用");
                }

                var task = Task.Factory.StartNew(() =>
                {
                    myHttpServer = new MyHttpServer(10);
                    myHttpServer.Start(Tool.Setting.HttpPort.ToInt());
                    Tool.Log.Info("[HttpServer]服务->启动成功");
                });

                task.Wait();
                Tool.Log.Debug("[CScript]脚本->开始加载");
                FactoryApi.LoadScript();
                Tool.Log.Info("[CScript]脚本->加载成功");

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

                myHttpServer?.Stop();
                myHttpServer = null;
            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex);
            }

        }
    }
}
