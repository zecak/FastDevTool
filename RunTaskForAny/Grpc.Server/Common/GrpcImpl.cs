using Grpc.Core;
using GrpcLib;
using GrpcLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Server.Common
{
    public class GrpcImpl : gRPC.gRPCBase
    {
        public static List<ClientInfo> ClientInfos = new List<ClientInfo>();
        public override async Task Chat(IAsyncStreamReader<APIRequest> requestStream, IServerStreamWriter<APIReply> responseStream, ServerCallContext context)
        {
            //Tool.Log.Debug("Client:" + context.Peer + " Status:" + context.Status.StatusCode);

            var clientInfo = ClientInfos.FirstOrDefault(f => f.Key == context.Peer);
            if (clientInfo == null)
            {
                clientInfo = new ClientInfo() { Key = context.Peer, StartTime = DateTime.Now, LastTime = DateTime.Now, HitCount = 1 };
                ClientInfos.Add(clientInfo);
            }
            else
            {
                clientInfo.LastTime = DateTime.Now;
                clientInfo.HitCount++;
            }

            //for (int i = 0; i < context.RequestHeaders.Count; i++)
            //{
            //    Console.WriteLine("Key:" + context.RequestHeaders.ElementAt(i).Key+ " Value:" + context.RequestHeaders.ElementAt(i).Value);
            //}

            while (await requestStream.MoveNext())
            {
                try
                {
                    var req = requestStream.Current;

                    await DoworkAsync(req, responseStream);

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

        async Task DoworkAsync(APIRequest reqData, IServerStreamWriter<APIReply> responseStream)
        {
            var resp = new APIReply();
            if (reqData.ApiPath == "/server/online")
            {
                resp.Code = 1;
                resp.Msg = "请求成功";
                await responseStream.WriteAsync(resp);
                return;
            }
            StartWork(reqData);

            var sign = (reqData.AppID + reqData.Data + reqData.Time + Tool.Setting.ServerKey).ToMd5();
            if (sign != reqData.Sign)
            {
                resp.Code = 2222;
                resp.Msg = "电子签名不一致";
                await responseStream.WriteAsync(resp);
                return;
            }

            switch (reqData.ApiPath)
            {
                case "/api/login":
                    {
                        resp.Code = 1;
                        resp.Msg = "请求了" + reqData.ApiPath;
                        
                        await responseStream.WriteAsync(resp);
                    }
                    break;
                case "/api/getclients":
                    {
                        resp.Code = 1;
                        resp.Msg = "请求成功";
                        resp.Data = ClientInfos.ToJson();
                        await responseStream.WriteAsync(resp);
                    }
                    break;
                default:
                    {
                        await responseStream.WriteAsync(new APIReply() { Code = 10000, Msg = "未知Action" });
                    }
                    break;
            }
            EndWork(resp);
        }

        void StartWork(APIRequest reqData)
        {
            Tool.Log.Info("req:" + reqData.ToJson());

        }

        void EndWork(APIReply resp)
        {
            Tool.Log.Info("resp:" + resp.ToJson());
        }
    }

}
