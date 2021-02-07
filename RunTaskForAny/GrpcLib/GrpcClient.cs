using Grpc.Core;
using GrpcLib.Security.Encrypt;
using GrpcLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcLib
{
    public class GrpcFailedEventArgs : EventArgs
    {
        public RpcException Exception { get; set; }
    }
    public class GrpcClient
    {
        public delegate void GrpcFailedHandler(object sender, GrpcFailedEventArgs args);

        private event GrpcFailedHandler _newchatFailed;

        public event GrpcFailedHandler NewChatFailed
        {
            add => _newchatFailed += value;
            remove => _newchatFailed -= value;
        }

        public event GrpcFailedHandler ExecFailed;

        public event Action<object, APIReply> NewChating;
        public string Target { get; set; }
        Channel channel = null;
        gRPC.gRPCClient client = null;
        //AsyncDuplexStreamingCall<APIRequest, APIReply> call = null;

        public string ClientType { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Token { get; set; } = "";
        public GrpcClient(string target)
        {
            Target = target;
            channel = new Channel(Target, ChannelCredentials.Insecure);
            client = new gRPC.gRPCClient(channel);
        }


        public void NewChat(APIRequest reqData)
        {
            var call_temp = client.Chat();
            Task.Run(async () =>
            {
                var responseReaderTask = Task.Run(async () =>
                {
                    while (await call_temp.ResponseStream.MoveNext())
                    {
                        var note = call_temp.ResponseStream.Current;
                        NewChating?.Invoke(this, note);
                    }
                });

                try
                {
                    await call_temp.RequestStream.WriteAsync(reqData);
                    await call_temp.RequestStream.CompleteAsync();
                }
                catch (RpcException ex)
                {
                    _newchatFailed?.Invoke(this, new GrpcFailedEventArgs() { Exception = ex });
                }

                await responseReaderTask;
            });
        }

        public APIReply Exec(APIRequest reqData)
        {
            try
            {
                var meta = new Metadata();
                meta.Add("ClientType", EncryptHelper.EnBase64(ClientType));
                meta.Add("ComputerName", EncryptHelper.EnBase64(Environment.MachineName));
                meta.Add("SystemName", EncryptHelper.EnBase64(Environment.OSVersion.VersionString));
                meta.Add("UserName", EncryptHelper.EnBase64(UserName));
                meta.Add("Token", EncryptHelper.EnBase64(Token));
                return client.ExecAsync(reqData, meta).GetAwaiter().GetResult();
            }
            catch (RpcException ex)
            {
                ExecFailed?.Invoke(this, new GrpcFailedEventArgs() { Exception = ex });
                return null;
            }

        }
    }



}
