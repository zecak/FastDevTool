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

        List<KeyValuePair<string, string>> rpcExceptions = new List<KeyValuePair<string, string>>();


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
                    client.ClientHost = Tool.Setting.AgentIP + ":" + Tool.Setting.AgentPort;
                    client.ClientType = "Agent";
                    client.UserName = "代理服务";
                    client.Token = "";
                    client.ExecFailed += Client_ExecFailed;

                    grpcClientList.Add(client);
                    var task2TokenSource = new CancellationTokenSource();
                    task2TokenSourceList.Add(task2TokenSource);
                    var task2 = Task.Factory.StartNew(() =>
                    {
                        while (!task2TokenSource.Token.IsCancellationRequested)
                        {
                            try
                            {
                                if(Tool.Setting.ServerRun!="1")
                                {
                                    //维护
                                    serverinfo.Status = "-1";
                                    System.Threading.Thread.Sleep(2000);
                                    continue;
                                }

                                var req = new APIRequest() { ApiPath = ActionApiPath.ServerOline, AppID = "代理服务", Time = DateTime.Now.ToTimestamp() };
                                req.Sign = (req.AppID + req.Data + req.Time + serverinfo.Key).ToMd5();
                                var resp = client.Exec(req);
                                if (resp == null)
                                {
                                    serverinfo.Status = "0";
                                }
                                else
                                {
                                    if (resp.Code == 1)
                                    {
                                        serverinfo.Status = "1";
                                    }
                                    else
                                    {
                                        serverinfo.Status = "0";
                                    }
                                }
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

        private void Client_ExecFailed(object sender, GrpcFailedEventArgs args)
        {
            var rpcex = rpcExceptions.FirstOrDefault(f => f.Key == args.Exception.Status.StatusCode.ToString() && f.Value == args.Exception.Status.Detail);
            if (rpcex.Key == null)
            {
                Tool.Log.Error(args.Exception);
                rpcExceptions.Add(new KeyValuePair<string, string>(args.Exception.Status.StatusCode.ToString(), args.Exception.Status.Detail));
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
