using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServer
{
    class Program
    {
        const int Port = 9999;
        static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { GrpcLibrary.gRPC.BindService(new GrpcImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("GrpcService server listening on port " + Port);
            Console.WriteLine("任意键退出...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
