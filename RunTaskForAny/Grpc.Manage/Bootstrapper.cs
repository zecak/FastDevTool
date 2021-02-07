using Grpc.Manage.Common;
using Grpc.Manage.Models;
using Grpc.Manage.Pages;
using GrpcLib;
using GrpcLib.Common;
using GrpcLib.Service;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Grpc.Manage
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
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
                Helper.GrpcClientAgent.ClientType = "代理服务";
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
                        var server = resp.Data.JsonTo<ServerModel>();
                        if(server!=null)
                        {
                            if (server.Status != "1")
                            {
                                Helper.Agent.Status = 3;
                                Helper.Agent.Msg = "服务端不在线";
                            }
                            else
                            {
                                Helper.Agent.Status = 1;
                                Helper.Agent.StatusMsg = "在线";
                                Helper.Agent.Msg = "连接成功";
                                Helper.Agent.IP = server.IP;
                                Helper.Agent.Port = server.Port;
                                Helper.Agent.Key = server.Key;
                            }
                        }
                    }
                    else
                    {
                        Helper.Agent.Status = 4;
                        Helper.Agent.Msg = "错误信息:"+ resp.Msg;
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
            // Called on Application.Exit
        }

        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            // Called on Application.DispatcherUnhandledException
        }
    }
}
