using GrpcCore.Common;
using GrpcCore.Common.Service;
using GrpcCore.Manage.Common;
using GrpcCore.Manage.Models;
using GrpcCore.Manage.Pages;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GrpcCore.Manage
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        CancellationTokenSource task2TokenSource = new CancellationTokenSource();
        protected override void OnStart()
        {
            {
                Helper.Agent = new AgentModel();
                Helper.Agent.Name = "默认代理";
                Helper.Agent.Status = 0;
                Helper.Agent.StatusMsg = "离线";
                Helper.Agent.Msg = "未连接";
                Helper.Agent.LinkTime = DateTime.Now;

                Helper.GrpcClientAgent = new GrpcClient(Helper.Setting.ServerIP + ":" + Helper.Setting.ServerPort);
                Helper.GrpcClientAgent.ClientType = "Manage";
                Helper.GrpcClientAgent.UserName = "客户端管理";
                var req = new APIRequest() { ApiPath = "GetServerList", Time = DateTime.Now.ToTimestamp() };
                req.Sign = (req.AppID + req.Data + req.Time + Helper.Setting.ServerKey).ToMd5();
                var resp = Helper.GrpcClientAgent.Exec(req);
                if (resp == null)
                {
                    Helper.Agent.Status = 2;
                    Helper.Agent.Msg = "连接失败";
                }
                else
                {
                    if (resp.Code == 1)
                    {
                        Helper.Agent.Status = 1;
                        Helper.Agent.StatusMsg = "获取成功";
                        Helper.Agent.Msg = "代理连接成功";


                        var server = resp.Data.JsonTo<ServerModel>();
                        if (server != null)
                        {
                            if (server.Status == "0")
                            {
                                Helper.Agent.ServerStatus = 0;
                                Helper.Agent.ServerStatusMsg = "离线";
                                Helper.Agent.Msg = "服务端不在线";
                            }
                            else if(server.Status == "-1")
                            {
                                Helper.Agent.ServerStatus = -1;
                                Helper.Agent.ServerStatusMsg = "维护";
                                Helper.Agent.Msg = "服务端维护中";
                            }
                            else if (server.Status == "2")
                            {
                                Helper.Agent.ServerStatus = 2;
                                Helper.Agent.ServerStatusMsg = "受限";
                                Helper.Agent.Msg = "客户端连接数上限";
                            }
                            else
                            {

                                Helper.Agent.IP = server.IP;
                                Helper.Agent.Port = server.Port;
                                Helper.Agent.Key = server.Key;

                                Helper.GrpcClientClient = new GrpcClient(Helper.Agent.IP + ":" + Helper.Agent.Port);
                                Helper.GrpcClientClient.ClientType = "Manage";
                                Helper.GrpcClientClient.UserName = "客户端管理";
                                var task2 = Task.Factory.StartNew(() =>
                                {
                                    while (!task2TokenSource.Token.IsCancellationRequested)
                                    {
                                        try
                                        {
                                            {
                                                var req2 = new APIRequest() { ApiPath = ActionApiPath.ServerOline, AppID = "Grpc管理", Time = DateTime.Now.ToTimestamp() };
                                                req2.Sign = (req2.AppID + req2.Data + req2.Time + Helper.Agent.Key).ToMd5();
                                                var resp2 = Helper.GrpcClientClient.Exec(req2);
                                                if (resp2 == null)
                                                {
                                                    Helper.Agent.ServerStatus = 0;
                                                    Helper.Agent.ServerStatusMsg = "离线";
                                                    Helper.Agent.Msg = "连接不通";
                                                }
                                                else
                                                {
                                                    if (resp2.Code == 1)
                                                    {
                                                        Helper.Agent.ServerStatus = 1;
                                                        Helper.Agent.ServerStatusMsg = "在线";
                                                        Helper.Agent.Msg = "连接成功";
                                                    }
                                                    else
                                                    {
                                                        Helper.Agent.ServerStatus = 2;
                                                        Helper.Agent.ServerStatusMsg = resp2.Msg;
                                                        Helper.Agent.Msg = resp2.Msg;
                                                    }
                                                }
                                            }
                                            {
                                                var req2 = new APIRequest() { ApiPath = ActionApiPath.GetClients, AppID = "Grpc管理", Time = DateTime.Now.ToTimestamp() };
                                                req2.Sign = (req2.AppID + req2.Data + req2.Time + Helper.Agent.Key).ToMd5();
                                                var resp2 = Helper.GrpcClientClient.Exec(req2);
                                                if (resp2 == null)
                                                {
                                                    Helper.Agent.ServerStatus = 0;
                                                    Helper.Agent.ServerStatusMsg = "离线";
                                                    Helper.Agent.Msg = "连接不通";
                                                }
                                                else
                                                {
                                                    if (resp2.Code == 1)
                                                    {
                                                        var clients = resp2.Data.JsonTo<List<Models.ClientInfo>>();
                                                        if(clients==null)
                                                        {
                                                            Helper.Agent.Msg = "客户端信息解析为空";
                                                            continue;
                                                        }
                                                        if (Helper.Agent.Clients==null)
                                                        {
                                                            Helper.Agent.Clients = clients;
                                                        }
                                                        else
                                                        {
                                                            if(clients.Count!= Helper.Agent.Clients.Count)
                                                            {
                                                                Helper.Agent.Clients = clients;
                                                            }
                                                            else
                                                            {
                                                                Helper.Agent.Clients.ForEach(c => {
                                                                    var temp = clients.FirstOrDefault(t => t.Name == c.Name);
                                                                    if (temp != null)
                                                                    {
                                                                        c.ClientHost = temp.ClientHost;
                                                                        c.ClientType = temp.ClientType;
                                                                        c.ComputerName = temp.ComputerName;
                                                                        c.HitCount = temp.HitCount;
                                                                        c.LastTime = temp.LastTime;
                                                                        c.StartTime = temp.StartTime;
                                                                        c.Status = temp.Status;
                                                                        c.SystemName = temp.SystemName;
                                                                        c.Token = temp.Token;
                                                                        c.UserName = temp.UserName;
                                                                    }

                                                                });
                                                            }

                                                           
                                                        }

                                                    }

                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Helper.Log.Error(ex);
                                        }

                                        System.Threading.Thread.Sleep(1000);
                                    }
                                }, task2TokenSource.Token);
                            }
                        }
                    }
                    else
                    {
                        Helper.Agent.Status = 4;
                        Helper.Agent.Msg = "错误信息:" + resp.Msg;
                    }
                }

            }

        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Bind your own types. Concrete types are automatically self-bound.
            //builder.Bind<IMyInterface>().To<MyType>();
        }

        protected override void Configure()
        {
            // This is called after Stylet has created the IoC container, so this.Container exists, but before the
            // Root ViewModel is launched.
            // Configure your services, etc, in here
        }

        protected override void OnLaunch()
        {
            // This is called just after the root ViewModel has been launched
            // Something like a version check that displays a dialog might be launched from here
        }

        protected override void OnExit(ExitEventArgs e)
        {
            task2TokenSource.Cancel();
            // Called on Application.Exit
        }

        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            // Called on Application.DispatcherUnhandledException
        }
    }
}
