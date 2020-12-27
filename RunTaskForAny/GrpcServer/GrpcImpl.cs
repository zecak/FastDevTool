using Grpc.Core;
using GrpcLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServer
{
    class GrpcImpl : gRPC.gRPCBase
    {
        public override Task<APIReply> Exec(APIRequest request, ServerCallContext context)
        {
            Console.WriteLine("简单rpc(Exec):" + request.Parameters);
            return Task.FromResult(new APIReply { Jsondata = "hi," + request.Parameters });
        }

        public override async Task Chat(IAsyncStreamReader<APIRequest> requestStream, IServerStreamWriter<APIReply> responseStream, ServerCallContext context)
        {
            Console.WriteLine("context.Host:" + context.Host);
            Console.WriteLine("context.Method:" + context.Method);
            Console.WriteLine("context.Peer:" + context.Peer);
            Console.WriteLine("context.Status.StatusCode:" + context.Status.StatusCode);
            Console.WriteLine("context.RequestHeaders.Count:" + context.RequestHeaders.Count);

            for (int i = 0; i < context.RequestHeaders.Count; i++)
            {
                Console.WriteLine("Key:" + context.RequestHeaders.ElementAt(i).Key+ " Value:" + context.RequestHeaders.ElementAt(i).Value);
            }


            while (await requestStream.MoveNext())
            {
                try
                {
                    var req = requestStream.Current;
                    Console.WriteLine("req.Parameters:" + req.Parameters);
                    await responseStream.WriteAsync(new APIReply() { Jsondata = "hello," + req.Parameters });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Message:" + ex.Message);
                    Console.WriteLine("StackTrace:" + ex.StackTrace);
                }
                
            }
        }
    }

}
