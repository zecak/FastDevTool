using System;
using Stylet;
using WpfApp1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcLib;

namespace WpfApp1.Pages
{
    public class ShellViewModel : Screen
    {
        public static Channel channel = new Channel("127.0.0.1:8090", Grpc.Core.ChannelCredentials.Insecure);
        public static GrpcLib.gRPC.gRPCClient client = new GrpcLib.gRPC.gRPCClient(channel);
        public string Text { get; set; } = "";
        public string Name { get; set; } = "王者荣耀";

        public void SayHello()
        {
            var call = client.Chat();
            Task.Run(async () =>
            {
                var responseReaderTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var note = call.ResponseStream.Current;
                        Text += "返回数据:" + note.Jsondata + Environment.NewLine;
                    }
                });

                await call.RequestStream.WriteAsync(new APIRequest() { Parameters = "SayHello" });
                await call.RequestStream.CompleteAsync();

                await responseReaderTask;
            });
        }

        public Person MyPerson { get; set; } = new Person() { FamilyName = "吕", GivenNames = "小布" };

        public List<Person> datas { get; set; } = new List<Person>() { new Person() { ID = 1, FamilyName = "吕", GivenNames = "小布" }, new Person() { ID = 2, FamilyName = "貂", GivenNames = "蝉" } };
        public int ID { get; set; } = 2;
        public ShellViewModel()
        {
           
        }
        public void AddUser()
        {
            //_windowManger.ShowDialog(_ChildDialog);
            //var ok = LocalDbContext.Add("吕布", "123");
            //if (ok)
            //{
            //    //_windowManger.ShowMessageBox(Name);
            //}

            var call = client.Chat();
            Task.Run(async () =>
            {
                var responseReaderTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var note = call.ResponseStream.Current;
                        Text += "返回:" + note.Jsondata + Environment.NewLine;
                    }
                });

                await call.RequestStream.WriteAsync(new APIRequest() { Parameters = "吕布" });
                await call.RequestStream.CompleteAsync();

                await responseReaderTask;
            });
        }
    }
}
