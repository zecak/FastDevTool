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
            Tool.Log.Info("Client:" + context.Peer + " Status:" + context.Status.StatusCode);

            //for (int i = 0; i < context.RequestHeaders.Count; i++)
            //{
            //    Console.WriteLine("Key:" + context.RequestHeaders.ElementAt(i).Key+ " Value:" + context.RequestHeaders.ElementAt(i).Value);
            //}

            while (await requestStream.MoveNext())
            {
                try
                {
                    var req = requestStream.Current;
                    Tool.Log.Info("req:" + req.Parameters);
                    var parameterData = req.Parameters.JsonTo<ReqData>();
                    if (parameterData == null)
                    {
                        await responseStream.WriteAsync(new APIReply() { Jsondata = new RespData() { Code = 1000, ErrorInfo = "请求数据转换失败" }.ToJson() });
                        continue;
                    }

                    switch (parameterData.API)
                    {
                        case "/api/login":
                            {
                                await responseStream.WriteAsync(new APIReply() { Jsondata = new RespData() { Code = 1, ErrorInfo = "请求了"+ parameterData.API }.ToJson() });
                            }
                            break;
                        case "/api/online":
                            {
                                await responseStream.WriteAsync(new APIReply() { Jsondata = new RespData() { Code = 1 }.ToJson() });
                            }
                            break;
                        default:
                            await responseStream.WriteAsync(new APIReply() { Jsondata = new RespData() { Code = 10000, ErrorInfo = "未知Action" }.ToJson() });
                            break;
                    }

                }
                catch (Exception ex)
                {
                    Tool.Log.Error("[----------------- ");
                    Tool.Log.Error(ex);
                    Tool.Log.Error(" -----------------]");
                    continue;
                }

            }
        }
    }

}
