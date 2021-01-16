using Grpc.Core;
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

        public event GrpcFailedHandler ExecFailed;
        public event Action<object, string> Chating;
        public string Target { get; set; }
        Channel channel = null;
        gRPC.gRPCClient client = null;
        // AsyncDuplexStreamingCall<APIRequest, APIReply> call = null;
        public GrpcClient(string target)
        {
            Target = target;
            channel = new Channel(Target, ChannelCredentials.Insecure);
            client = new gRPC.gRPCClient(channel);
            //call = client.Chat();

        }

        public void NewChat(string reqData)
        {
            var call_temp = client.Chat();
            Task.Run(async () =>
            {
                var responseReaderTask = Task.Run(async () =>
                {
                    while (await call_temp.ResponseStream.MoveNext())
                    {
                        var note = call_temp.ResponseStream.Current;
                        Chating?.Invoke(this, note.Jsondata);
                    }
                });

                try
                {
                    await call_temp.RequestStream.WriteAsync(new APIRequest() { Parameters = reqData });
                    await call_temp.RequestStream.CompleteAsync();
                }
                catch (RpcException ex)
                {
                    _chatFailed?.Invoke(this, new GrpcFailedEventArgs() { Exception = ex });
                }

                await responseReaderTask;
            });
        }

        public APIReply Exec(string data)
        {
            try
            {
                return client.ExecAsync(new APIRequest() { Parameters = data }).GetAwaiter().GetResult();
            }
            catch (RpcException ex)
            {
                ExecFailed?.Invoke(this, new GrpcFailedEventArgs() { Exception = ex });
                return null;
            }

        }
    }



}
