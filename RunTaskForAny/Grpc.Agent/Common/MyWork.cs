using Grpc.Core;
using GrpcLib;
using GrpcLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Agent.Common
{
    public class MyWork
    {
        Core.Server server = null;

        List<CancellationTokenSource> task2TokenSourceList = new List<CancellationTokenSource>();
        List<GrpcClient> grpcClientList = new List<GrpcClient>();

        List<KeyValuePair<string,string>> rpcExceptions = new List<KeyValuePair<string, string>>();
        public void Start()
        {
            try
            {
                server = new Core.Server
                {
                    Services = { gRPC.BindService(new GrpcImpl()) },
                    Ports = { new Core.ServerPort(Tool.Setting.AgentIP, Tool.Setting.AgentPort.ToInt(), Core.ServerCredentials.Insecure) }
                };

                server.Start();

                Tool.Log.Info("ServerCount:" + Tool.Setting.ServerList?.Count);
                foreach (var serverinfo in Tool.Setting.ServerList)
                {
                    Tool.Log.Info("ServerInfo:" + serverinfo.IP + ":" + serverinfo.Port);
                    var client = new GrpcClient(serverinfo.IP + ":" + serverinfo.Port);
                    client.NewChatFailed += Client_GrpcFailed;
                    client.NewChating += Client_Chating;
                    //client.InitChat();
                    grpcClientList.Add(client);
                    var task2TokenSource = new CancellationTokenSource();
                    task2TokenSourceList.Add(task2TokenSource);
                    var task2 = Task.Factory.StartNew(() =>
                    {
                        while (!task2TokenSource.Token.IsCancellationRequested)
                        {
                            try
                            {
                                var req = new APIRequest() { ApiPath = "/server/online", AppID = "代理服务", Time = DateTime.Now.DateTimeToUTC() };
                                req.Sign = (req.AppID + req.Data + req.Time + serverinfo.Key).ToMd5();
                                client.NewChat(req);
                            }
                            catch (Exception ex)
                            {
                                Tool.Log.Error(ex);
                            }
                            
                            System.Threading.Thread.Sleep(1000);
                        }
                    }, task2TokenSource.Token);
                }



            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex);
            }

        }


        private void Client_GrpcFailed(object sender, GrpcFailedEventArgs args)
        {
            var rpcex=rpcExceptions.FirstOrDefault(f => f.Key == args.Exception.Status.StatusCode.ToString()&& f.Value == args.Exception.Status.Detail);
            if(rpcex.Key==null)
            {
                Tool.Log.Error(args.Exception);
                rpcExceptions.Add(new KeyValuePair<string, string>(args.Exception.Status.StatusCode.ToString(), args.Exception.Status.Detail));
            }
            
            var grpc = (GrpcClient)sender;
            var serverinfo = Tool.Setting.ServerList.FirstOrDefault(f => (f.IP + ":" + f.Port) == grpc.Target);
            if (serverinfo != null)
            {
                serverinfo.Status = "0";
            }

        }

        private void Client_Chating(object sender, APIReply resp)
        {
            var grpc = (GrpcClient)sender;

            if (resp.Code != 1)
            {
                Tool.Log.Error("操作失败:" + resp.Msg);
                return;
            }
            var serverinfo = Tool.Setting.ServerList.FirstOrDefault(f => (f.IP + ":" + f.Port) == grpc.Target);
            if (serverinfo != null)
            {
                serverinfo.Status = "1";
            }

        }

        public void Stop()
        {
            try
            {
                foreach (var task2TokenSource in task2TokenSourceList)
                {
                    task2TokenSource.Cancel();
                }

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
