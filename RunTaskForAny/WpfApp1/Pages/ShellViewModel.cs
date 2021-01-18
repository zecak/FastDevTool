using System;
using Stylet;
using WpfApp1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcLib;
using WpfApp1.Common;
using GrpcLib.Service;
using System.Linq;
using System.Windows;

namespace WpfApp1.Pages
{
    public class ShellViewModel : Screen
    {
        public List<ResourceDictionaryInfo> Languages { get; set; }
        public ResourceDictionaryInfo Language { get; set; }
        public List<ResourceDictionaryInfo> Themes { get; set; }
        public ResourceDictionaryInfo Theme { get; set; }

        public string Text { get; set; } = "";
        public string TempText { get; set; }
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public DateTime LoginTime { get; set; }

        public string ChatMsg { get; set; }
        public string ChatName { get; set; }

        GrpcClient grpcClient = null;
        ServerInfo serverInfo = null;

        string ClientAppID = Guid.NewGuid().ToString();
        public void SayHello()
        {
            var req = new APIRequest() { ApiPath = "/api/getclients", AppID = ClientAppID, Data = "SayHello", Time = DateTime.Now.DateTimeToUTC() };
            req.Sign = (req.AppID + req.Data + req.Time + serverInfo.Key).ToMd5();
            grpcClient.NewChat(req);

        }

        public void SelectLang()
        {
            ResourceDictionaryHelper.SetDefault(Language, ResourceDictionaryHelper.ThemeDefault);
        }

        public void SelectTheme()
        {
            ResourceDictionaryHelper.SetDefault(ResourceDictionaryHelper.LanguageDefault, Theme);
        }

        public ShellViewModel()
        {
            Languages = ResourceDictionaryHelper.ResourceDictionaries.FindAll(r => r.ResourceType == ResourceDictionaryType.Language);
            Language = ResourceDictionaryHelper.LanguageDefault;

            Themes = ResourceDictionaryHelper.ResourceDictionaries.FindAll(r => r.ResourceType == ResourceDictionaryType.Theme);
            Theme = ResourceDictionaryHelper.ThemeDefault;

            GrpcClient grpc = new GrpcClient("172.16.1.60:8080");
            var req = new APIRequest() { ApiPath = "GetServerList", AppID = ClientAppID, Time = DateTime.Now.DateTimeToUTC() };
            req.Sign = (req.AppID + req.Data + req.Time + "123456").ToMd5();
            var resp = grpc.Exec(req);
            if (resp == null)
            {
                System.Windows.MessageBox.Show("连接代理服务失败");
                return;
            }
            if (resp.Code == 1)
            {
                var server = resp.Data.JsonTo<ServerInfo>();

                if (server.Status != "1")
                {
                    System.Windows.MessageBox.Show("服务端不在线");
                    return;
                }

                serverInfo = server;
            }
            else
            {
                System.Windows.MessageBox.Show(resp.Msg);
                return;
            }

            if (serverInfo == null)
            {
                System.Windows.MessageBox.Show("代理服务未配置服务端信息");
                return;
            }

            grpcClient = new GrpcClient(serverInfo.IP + ":" + serverInfo.Port);
            grpcClient.NewChatFailed += GrpcClient_GrpcFailed;

            grpcClient.NewChating += GrpcClient_NewChating;

            LoginTime = DateTime.Now;
            lastChatInfo = new ChatInfo() { UserName = Name, SendTime = DateTime.Now.AddSeconds(1), Msg = "首次" };

            var req2 = new APIRequest() { ApiPath = "/api/login", AppID = ClientAppID, Data = new UserInfo() { UserName = Name, LoingTime = DateTime.Now }.ToJson(), Time = DateTime.Now.DateTimeToUTC() };
            req2.Sign = (req2.AppID + req2.Data + req2.Time + serverInfo.Key).ToMd5();

            grpcClient.NewChat(req2);

            var task2TokenSource = new System.Threading.CancellationTokenSource();
            var task2 = Task.Factory.StartNew(() =>
            {
                while (!task2TokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        var req3 = new APIRequest() { ApiPath = "/api/heartbeat", AppID = ClientAppID, Data = new UserInfo() { UserName = Name, LoingTime = lastChatInfo.SendTime }.ToJson(), Time = DateTime.Now.DateTimeToUTC() };
                        req3.Sign = (req3.AppID + req3.Data + req3.Time + serverInfo.Key).ToMd5();
                        grpcClient.NewChat(req3);
                    }
                    catch (Exception ex)
                    {

                    }

                    System.Threading.Thread.Sleep(1000);
                }
            }, task2TokenSource.Token);

        }
        static object mylock = new object();
        ChatInfo lastChatInfo;
        private void GrpcClient_NewChating(object grpc, APIReply obj)
        {
            if (obj.Code == 1)
            {
                if (!string.IsNullOrWhiteSpace(obj.Data))
                {
                    var data = obj.Data.JsonTo<RespData>();
                    if (data != null)
                    {
                        if (data.Action == "ChatMsg")
                        {
                            var chat = data.Data.JsonTo<ChatInfo>();
                            lastChatInfo.SendTime = chat.SendTime;
                            lastChatInfo.Msg = chat.Msg;
                            Text += chat.UserName+":" + chat.Msg +" Time:"+ chat.SendTime + Environment.NewLine;

                        }
                    }
                    else
                    {
                        Text += "返回:" + obj.Data + Environment.NewLine;
                    }
                }


            }
            else
            {
                Text += "返回信息:" + obj.Msg + Environment.NewLine;
            }
        }

        private void GrpcClient_GrpcFailed(object sender, GrpcFailedEventArgs args)
        {
            Text += "返回错误:" + args.Exception.Message + Environment.NewLine;
        }


        public void InitChat()
        {
            //grpcClient.InitChat();
        }

        public void Chat()
        {
            var req = new APIRequest() { ApiPath = "/api/wordchat", AppID = ClientAppID, Data = new RespData() { Action = Name, Data = ChatMsg }.ToJson(), Time = DateTime.Now.DateTimeToUTC() };
            req.Sign = (req.AppID + req.Data + req.Time + serverInfo.Key).ToMd5();
            grpcClient.NewChat(req);
        }

        public void End()
        {
            //grpcClient.EndChat();
        }
    }
}
