using Grpc.Core;
using Grpc.Server.Api;
using GrpcLib;
using GrpcLib.Common;
using GrpcLib.Models;
using GrpcLib.Security.Encrypt;
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

                var api = FactoryApi.CreateFunction(request.ApiPath);
                if (api != null)
                {
                    if (!api.CheckSign(request, serverInfo))
                    {
                        return Task.FromResult(new APIReply { Code = 222, Msg = "电子签名不一致" });
                    }
                    if (api.LimitAction)
                    {
                        var client = serverInfo.Clients.FirstOrDefault(c => c.Name == context.Peer);
                        if (client == null)
                        {
                            return Task.FromResult(new APIReply { Code = 101, Msg = "未建立连接" });
                        }
                        if (client.Token != request.Token)
                        {
                            return Task.FromResult(new APIReply { Code = 102, Msg = "未登录或登录已过期" });
                        }

                    }
                    return Task.FromResult(api.ApiAction(request, context, serverInfo));
                }

                return Task.FromResult(new APIReply { Code = 100, Msg = "未知操作" });
            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex);
                return Task.FromResult(new APIReply { Code = 999, Msg = ex.Message });
            }
        }

        void Init(ServerCallContext context)
        {
            var clientInfo = serverInfo.Clients.FirstOrDefault(f => f.Name == context.Peer);
            if (clientInfo == null)
            {
                var clientType = context.RequestHeaders.FirstOrDefault(f => f.Key == "ClientType");
                var computerName = context.RequestHeaders.FirstOrDefault(f => f.Key == "ComputerName");
                var systemName = context.RequestHeaders.FirstOrDefault(f => f.Key == "SystemName");
                var userName = context.RequestHeaders.FirstOrDefault(f => f.Key == "UserName");
                var token = context.RequestHeaders.FirstOrDefault(f => f.Key == "Token");

                clientInfo = new ClientInfo() { Name = context.Peer, ClientType = EncryptHelper.DeBase64(clientType.Value), ComputerName = EncryptHelper.DeBase64(computerName.Value), SystemName = EncryptHelper.DeBase64(systemName.Value), UserName = EncryptHelper.DeBase64(userName.Value), Token = EncryptHelper.DeBase64(token.Value), Status = 1, StartTime = DateTime.Now, LastTime = DateTime.Now, HitCount = 1 };
                serverInfo.Clients.Add(clientInfo);

                Tool.Log.Debug("ClientInfo:" + clientInfo.ToJson());
            }
            else
            {
                clientInfo.Status = 1;
                clientInfo.LastTime = DateTime.Now;
                clientInfo.HitCount++;
            }

        }

        public override async Task Chat(IAsyncStreamReader<APIRequest> requestStream, IServerStreamWriter<APIReply> responseStream, ServerCallContext context)
        {

            Init(context);

            while (await requestStream.MoveNext())
            {
                try
                {
                    var req = requestStream.Current;

                    await DoworkAsync(req, responseStream, context, serverInfo);

                }
                catch (Exception ex)
                {
                    Tool.Log.Error("[----------------- ");
                    Tool.Log.Error(ex);
                    Tool.Log.Error(" -----------------]");
                }

            }
        }

        async Task DoworkAsync(APIRequest request, IServerStreamWriter<APIReply> responseStream, ServerCallContext context, ServerInfo serverInfo)
        {
            StartWork(request);
            APIReply resp = new APIReply() { Code = 100, Msg = "未知操作" };
            var api = FactoryApi.CreateFunction(request.ApiPath);
            if (api != null)
            {
                if (!api.CheckSign(request, serverInfo))
                {
                    resp.Code = 222;
                    resp.Msg = "电子签名不一致";
                    await responseStream.WriteAsync(resp);
                    return;
                }
                if (api.LimitAction)
                {
                    var client = serverInfo.Clients.FirstOrDefault(c => c.Name == context.Peer);
                    if (client == null)
                    {
                        await responseStream.WriteAsync(new APIReply { Code = 101, Msg = "未建立连接" });
                        return;
                    }
                    if (client.Token != request.Token)
                    {
                        await responseStream.WriteAsync(new APIReply { Code = 102, Msg = "未登录或登录已过期" });
                        return;
                    }

                }
                await api.ChatAction(request, responseStream, context, serverInfo);
                return;
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
