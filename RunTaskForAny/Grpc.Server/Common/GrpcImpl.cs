using Grpc.Core;
using GrpcLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Server.Common
{
    public class GrpcImpl : gRPC.gRPCBase
    {

        public override async Task Chat(IAsyncStreamReader<APIRequest> requestStream, IServerStreamWriter<APIReply> responseStream, ServerCallContext context)
        {
            Tool.Log.Info("Peer:" + context.Peer+ " Status:" + context.Status.StatusCode);

            //for (int i = 0; i < context.RequestHeaders.Count; i++)
            //{
            //    Console.WriteLine("Key:" + context.RequestHeaders.ElementAt(i).Key+ " Value:" + context.RequestHeaders.ElementAt(i).Value);
            //}


            while (await requestStream.MoveNext())
            {
                try
                {
                    var req = requestStream.Current;
                    Tool.Log.Info("req.Parameters:" + req.Parameters);
                    await responseStream.WriteAsync(new APIReply() { Jsondata = "hello," + req.Parameters });
                }
                catch (Exception ex)
                {
                    Tool.Log.Error("[----------------- ");
                    Tool.Log.Error(ex);
                    Tool.Log.Error(" -----------------]");
                }
                
            }
        }
    }

}
