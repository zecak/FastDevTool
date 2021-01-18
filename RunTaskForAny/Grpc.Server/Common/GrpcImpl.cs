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

        static object mylock = new object();

        List<string> onlineUser = new List<string>();

        public static List<ClientInfo> ClientInfos = new List<ClientInfo>();

        GroupInfo groupInfo = new GroupInfo() { Name = "世界聊天室", Users = new List<string>(), ChatInfos = new List<ChatInfo>() };
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

                    await DoworkAsync(req, responseStream, clientInfo);

                }
                catch (Exception ex)
                {
                    Tool.Log.Error("[----------------- ");
                    Tool.Log.Error(ex);
                    Tool.Log.Error(" -----------------]");
                }

            }
        }

        async Task DoworkAsync(APIRequest reqData, IServerStreamWriter<APIReply> responseStream, ClientInfo clientInfo)
        {
            var resp = new APIReply();
            if (reqData.ApiPath == "/server/online")
            {
                resp.Code = 1;
                resp.Msg = "请求成功";
                await responseStream.WriteAsync(resp);
                return;
            }
            if (reqData.ApiPath == "/api/heartbeat")
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
                var toChats = groupInfo.ChatInfos.FindAll(c => (c.SendTime > userInfo.LoingTime) && c.UserName != userInfo.UserName);

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
                case "/api/login":
                    {
                        var userInfo = reqData.Data.JsonTo<UserInfo>();
                        if (userInfo == null)
                        {
                            resp.Code = 1001;
                            resp.Msg = "请求参数不正确";
                            await responseStream.WriteAsync(resp);
                            return;
                        }
                        var user = onlineUser.FirstOrDefault(n => n == userInfo.UserName);
                        if (user == null)
                        {
                            onlineUser.Add(userInfo.UserName);
                            groupInfo.Users.Add(userInfo.UserName);
                        }
                        resp.Code = 1;
                        resp.Msg = "登录成功";

                        await responseStream.WriteAsync(resp);
                    }
                    break;
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
                        var user = onlineUser.FirstOrDefault(n => n == userInfo.Action);
                        if (user == null)
                        {
                            resp.Code = 1002;
                            resp.Msg = "未登录";
                            await responseStream.WriteAsync(resp);
                            return;
                        }
                        var chatInfo = new ChatInfo() { UserName = user, Msg = userInfo.Data, SendTime = DateTime.Now.ToTimestamp() };

                        groupInfo.ChatInfos.Add(chatInfo);
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
            Tool.Log.Debug("resp:" + resp.ToJson());
        }
    }

}
