using Grpc.Core;
using GrpcLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:9999", ChannelCredentials.Insecure);

            var client = new gRPC.gRPCClient(channel);
            var mdata = new Metadata();
            mdata.Add("ClientName","client");
            mdata.Add("MachineName", System.Environment.MachineName);
            mdata.Add("Is64", System.Environment.Is64BitOperatingSystem.ToString());
            mdata.Add("OSVersion", System.Environment.OSVersion.VersionString);
            mdata.Add("ComputerRunTime", (TimeSpan.FromMilliseconds(System.Environment.TickCount).TotalHours).ToString("0.0"));

            //1
            //var reply = client.Exec(new APIRequest { Parameters = "April" }, mdata);
            //Console.WriteLine("server:" + reply.Jsondata);

            //2
            var call = client.Chat(mdata);
            Task.Run(async () =>
            {
                var responseReaderTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var note = call.ResponseStream.Current;
                        Console.WriteLine("Received:" + note.Jsondata);
                    }
                });

                await call.RequestStream.WriteAsync(new APIRequest() { Parameters = "chat" });

                await call.RequestStream.CompleteAsync();
                await responseReaderTask;
            });

            Console.WriteLine("任意键退出...");
            Console.ReadKey();
            channel.ShutdownAsync().Wait();
        }

        static string C(string d)
        {
            //转成 Base64 形式的 System.String  
            return Convert.ToBase64String(Encoding.Default.GetBytes(d));
        }
    }
}
