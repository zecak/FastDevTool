using Grpc.Core;
using Grpc.Server.Api;
using GrpcLib;
using GrpcLib.Models;
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
        ServerInfo serverInfo { get; set; } = new ServerInfo();

        public override Task<APIReply> Exec(APIRequest request, ServerCallContext context)
        {
            try
            {
                Init(context);

                var sign = (request.AppID + request.Data + request.Time + Tool.Setting.ServerKey).ToMd5();
                if (sign != request.Sign)
                {
                    return Task.FromResult(new APIReply { Code = 1002, Msg = "电子签名不一致" });
                }

                var api = FactoryApi.CreateFunction(request.ApiPath);
                if (api != null)
                {
                    if (api.LimitAction)
                    {
                        var curToken = serverInfo.OnlineUserTokens.FirstOrDefault(t => t.Key == request.Token);
                        if (string.IsNullOrWhiteSpace(request.Token)||(curToken == null))
                        {
                            return Task.FromResult(new APIReply { Code = 1001, Msg = "未登录或登录已失效" });
                        }
                        //根据token检查是否已经登录了
                        //数据库检查
#warning 需要登录检查

                    }
                    return api.ApiAction(request, context, serverInfo);
                }

                return Task.FromResult(new APIReply { Code = 1000, Msg = "未知操作" });
            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex);
                return Task.FromResult(new APIReply { Code = 999, Msg = ex.Message });
            }
        }

        void Init(ServerCallContext context)
        {
            //Tool.Log.Debug("Client:" + context.Peer + " Status:" + context.Status.StatusCode);

            //var clientInfo = serverInfo.ClientInfos.FirstOrDefault(f => f.Key == context.Peer);
            //if (clientInfo == null)
            //{
            //    clientInfo = new ClientInfo() { Key = context.Peer, StartTime = DateTime.Now, LastTime = DateTime.Now, HitCount = 1 };
            //    serverInfo.ClientInfos.Add(clientInfo);
            //}
            //else
            //{
            //    clientInfo.LastTime = DateTime.Now;
            //    clientInfo.HitCount++;
            //}

            //for (int i = 0; i < context.RequestHeaders.Count; i++)
            //{
            //    Console.WriteLine("Key:" + context.RequestHeaders.ElementAt(i).Key+ " Value:" + context.RequestHeaders.ElementAt(i).Value);
            //}
        }

        public override async Task Chat(IAsyncStreamReader<APIRequest> requestStream, IServerStreamWriter<APIReply> responseStream, ServerCallContext context)
        {

            Init(context);

            while (await requestStream.MoveNext())
            {
                try
                {
                    var req = requestStream.Current;

                    await DoworkAsync(req, responseStream, serverInfo);

                }
                catch (Exception ex)
                {
                    Tool.Log.Error("[----------------- ");
                    Tool.Log.Error(ex);
                    Tool.Log.Error(" -----------------]");
                }

            }
        }

        async Task DoworkAsync(APIRequest reqData, IServerStreamWriter<APIReply> responseStream, ServerInfo serverInfo)
        {
            var resp = new APIReply();
            if (reqData.ApiPath == ActionApiPath.Heartbeat)
            {
                var sign2 = (reqData.AppID + reqData.Data + reqData.Time + Tool.Setting.ServerKey).ToMd5();
                if (sign2 != reqData.Sign)
                {
                    resp.Code = 2222;
                    resp.Msg = "电子签名不一致";
                    await responseStream.WriteAsync(resp);
                    return;
                }

                var userInfo = reqData.Data.JsonTo<UserInfo>();
                if (userInfo == null)
                {
                    resp.Code = 1001;
                    resp.Msg = "请求参数不正确";
                    await responseStream.WriteAsync(resp);
                    return;
                }
                var toChats = serverInfo.GroupInfo.ChatInfos.FindAll(c => (c.SendTime > userInfo.LoingTime) && c.UserName != userInfo.UserName);

                foreach (var chat in toChats)
                {
                    resp.Code = 1;
                    resp.Data = new RespData() { Action = "ChatMsg", Data = chat.ToJson() }.ToJson();
                    resp.Msg = "请求成功";
                    await responseStream.WriteAsync(resp);
                }
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

                case "/api/wordchat":
                    {
                        var userInfo = reqData.Data.JsonTo<RespData>();
                        if (userInfo == null)
                        {
                            resp.Code = 1001;
                            resp.Msg = "请求参数不正确";
                            await responseStream.WriteAsync(resp);
                            return;
                        }
                        var user = serverInfo.OnlineUserTokens.FirstOrDefault(n => n.Key == userInfo.Action);
                        if (user == null)
                        {
                            resp.Code = 1002;
                            resp.Msg = "未登录";
                            await responseStream.WriteAsync(resp);
                            return;
                        }
                        var chatInfo = new ChatInfo() { UserName = user.Key, Msg = userInfo.Data, SendTime = DateTime.Now.ToTimestamp() };

                        serverInfo.GroupInfo.ChatInfos.Add(chatInfo);
                        {
                            resp.Code = 1;
                            resp.Data = new RespData() { Action = "ChatMsg", Data = chatInfo.ToJson() }.ToJson();
                            await responseStream.WriteAsync(resp);
                        }

                    }
                    break;
                case "/api/getclients":
                    {
                        resp.Code = 1;
                        resp.Msg = "请求成功";
                        //resp.Data = serverInfo.ClientInfos.ToJson();
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
            Tool.Log.Debug("resp:" + resp.ToJson());
        }
    }

}
