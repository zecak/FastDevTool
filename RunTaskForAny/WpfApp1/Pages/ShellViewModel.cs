using System;
using Stylet;
using WpfApp1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcLib;
using WpfApp1.Common;

namespace WpfApp1.Pages
{
    public class ShellViewModel : Screen
    {
        public string Text { get; set; } = "";
        public string Name { get; set; } = "L";

        GrpcClient grpcClient = null;
        ServerInfo serverInfo = null;

        public void SayHello()
        {
            grpcClient.NewChat(new ReqData() { API = "/api", APPID = "", Data = "SayHello", Sign = "", Time = DateTime.Now.DateTimeToUTC() }.ToJson());

        }

        public Person MyPerson { get; set; } = new Person() { FamilyName = "N", GivenNames = "K" };

        public List<Person> datas { get; set; } = new List<Person>() { new Person() { ID = 1, FamilyName = "吕", GivenNames = "小布" }, new Person() { ID = 2, FamilyName = "貂", GivenNames = "蝉" } };
        public int ID { get; set; } = 2;
        public ShellViewModel()
        {
            GrpcClient grpc = new GrpcClient("127.0.0.1:8080");
            var resp = grpc.Exec("GetServerList");
            if (resp == null)
            {
                System.Windows.MessageBox.Show("连接代理服务失败");
                return;
            }
            var data = resp.Jsondata.JsonTo<RespData>();
            if (data != null && data.Code == 1)
            {
                var server = (data.Data.ToString()).JsonTo<ServerInfo>();

                if (server.Status != "1")
                {
                    System.Windows.MessageBox.Show("服务端不在线");
                    return;
                }

                serverInfo = server;
            }
            else
            {
                System.Windows.MessageBox.Show(data.ErrorInfo);
            }

            if (serverInfo == null)
            {
                System.Windows.MessageBox.Show("代理服务未配置服务端信息");
                return;
            }

            grpcClient = new GrpcClient(serverInfo.IP + ":" + serverInfo.Port);
            grpcClient.ChatFailed += GrpcClient_GrpcFailed;
            grpcClient.Chating += GrpcClient_Chating;

            grpcClient.NewChat(new ReqData() { API = "/api/login", APPID = "", Data = "ShellViewModel", Sign = "", Time = DateTime.Now.DateTimeToUTC() }.ToJson());


        }

        private void GrpcClient_GrpcFailed(object sender, GrpcFailedEventArgs args)
        {
            System.Windows.MessageBox.Show(args.Exception.Message);
        }

        private void GrpcClient_Chating(object grpc, string obj)
        {
            Text += "返回:" + obj + Environment.NewLine;
        }


        public void AddUser()
        {

        }
    }
}
