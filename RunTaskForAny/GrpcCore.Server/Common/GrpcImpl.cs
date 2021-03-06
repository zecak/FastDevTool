using Grpc.Core;
using GrpcCore.Common;
using GrpcCore.Common.Models;
using GrpcCore.Common.Security;
using GrpcCore.Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcCore.Server.Common
{
    public class GrpcImpl : gRPC.gRPCBase
    {
        ServerInfo serverInfo { get; set; } = new ServerInfo() { Setting = Tool.Setting, };

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
                var peerHostHeader = context.RequestHeaders.FirstOrDefault(f => f.Key == "ClientHost".ToLower());
                var clientTypeHeader = context.RequestHeaders.FirstOrDefault(f => f.Key == "ClientType".ToLower());
                var computerNameHeader = context.RequestHeaders.FirstOrDefault(f => f.Key == "ComputerName".ToLower());
                var systemNameHeader = context.RequestHeaders.FirstOrDefault(f => f.Key == "SystemName".ToLower());
                var userNameHeader = context.RequestHeaders.FirstOrDefault(f => f.Key == "UserName".ToLower());
                var tokenHeader = context.RequestHeaders.FirstOrDefault(f => f.Key == "Token".ToLower());

                var peerHost = peerHostHeader == null ? "" : (peerHostHeader.Value ?? "");
                var clientType = clientTypeHeader == null ? "" : (clientTypeHeader.Value ?? "");
                var computerName = computerNameHeader == null ? "" : (computerNameHeader.Value ?? "");
                var systemName = systemNameHeader == null ? "" : (systemNameHeader.Value ?? "");
                var userName = userNameHeader == null ? "" : (userNameHeader.Value ?? "");
                var token = tokenHeader == null ? "" : (tokenHeader.Value ?? "");

                clientInfo = new ClientInfo() { Name = context.Peer, ClientHost = EncryptHelper.DeBase64(peerHost), ClientType = EncryptHelper.DeBase64(clientType), ComputerName = EncryptHelper.DeBase64(computerName), SystemName = EncryptHelper.DeBase64(systemName), UserName = EncryptHelper.DeBase64(userName), Token = EncryptHelper.DeBase64(token), Status = 1, StartTime = DateTime.Now, LastTime = DateTime.Now, HitCount = 1 };
                serverInfo.Clients.Add(clientInfo);

                Tool.Log.Debug("ClientInfo:" + clientInfo.ToJson());
                Tool.Log.Debug("[---------------------------------------------------------");
                Tool.Log.Debug(" 总连接数:" + serverInfo.Clients.Count + " " + "代理服务:" + serverInfo.Clients.Where(w => w.ClientType.Contains("Agent")).Count() + " " + "管理:" + serverInfo.Clients.Where(w => w.ClientType.Contains("Manage")).Count() + " " + "客户端:" + serverInfo.Clients.Where(w => w.ClientType.Contains("Client")).Count());
                Tool.Log.Debug(" ---------------------------------------------------------]");
            }
            else
            {
                clientInfo.Status = 1;
                clientInfo.LastTime = DateTime.Now;
                clientInfo.HitCount++;
            }
            serverInfo.Clients.RemoveAll(c => DateTime.Now.Subtract(c.LastTime).TotalSeconds >= 5);
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
