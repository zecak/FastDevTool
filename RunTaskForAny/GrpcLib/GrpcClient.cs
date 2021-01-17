using Grpc.Core;
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

        private event GrpcFailedHandler _chatFailed;

        public event GrpcFailedHandler ChatFailed
        {
            add => _chatFailed += value;
            remove => _chatFailed -= value;
        }
        public event GrpcFailedHandler RespChatFailed;

        private event GrpcFailedHandler _newchatFailed;

        public event GrpcFailedHandler NewChatFailed
        {
            add => _newchatFailed += value;
            remove => _newchatFailed -= value;
        }

        public event GrpcFailedHandler ExecFailed;
        public event Action<object, APIReply> Chating;
        public event Action<object, APIReply> NewChating;
        public string Target { get; set; }
        Channel channel = null;
        gRPC.gRPCClient client = null;
        AsyncDuplexStreamingCall<APIRequest, APIReply> call = null;
        public GrpcClient(string target)
        {
            Target = target;
            channel = new Channel(Target, ChannelCredentials.Insecure);
            client = new gRPC.gRPCClient(channel);
        }

        public void InitChat()
        {
            call = client.Chat();
            Task.Run(async () =>
            {
                while (await call.ResponseStream.MoveNext())
                {
                    try
                    {
                        var note = call.ResponseStream.Current;
                        Chating?.Invoke(this, note);
                    }
                    catch (RpcException ex)
                    {
                        RespChatFailed?.Invoke(this, new GrpcFailedEventArgs() { Exception = ex });
                    }
                }
            });
        }

        public void EndChat()
        {
            if(call!=null)
            {
                call.RequestStream.CompleteAsync().GetAwaiter().GetResult();
                call.Dispose();
                call = null;
            }
        }

        public void Chat(APIRequest reqData)
        {
            try
            {
                call?.RequestStream.WriteAsync(reqData).GetAwaiter().GetResult();
            }
            catch (RpcException ex)
            {
                _chatFailed?.Invoke(this, new GrpcFailedEventArgs() { Exception = ex });
                if (ex.Status.Detail== "failed to connect to all addresses"|| ex.Status.Detail == "Stream removed")
                {
                    EndChat();
                    InitChat();
                }
            }
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
                return client.ExecAsync(reqData).GetAwaiter().GetResult();
            }
            catch (RpcException ex)
            {
                ExecFailed?.Invoke(this, new GrpcFailedEventArgs() { Exception = ex });
                return null;
            }

        }
    }



}
