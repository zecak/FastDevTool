using GrpcCore.Common;
using GrpcCore.Common.Models;
using GrpcCore.Common.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace GrpcCore.Server.Common
{
    public class MyHttpServer : HttpServerBase
    {
        ServerInfo serverInfo { get; set; } = new ServerInfo() { Setting = Tool.Setting, };
        /// <summary>
        /// Http服务
        /// </summary>
        /// <param name="maxThreads"></param>
        public MyHttpServer(int maxThreads) : base(maxThreads)
        { }
        protected override void ProcessHttpRequest(HttpListenerContext context)
        {
            try
            {
                var action = context.Request.RawUrl.ToLower();

                var bufs = new byte[context.Request.ContentLength64];
                context.Request.InputStream.Read(bufs, 0, bufs.Length);
                string reqstr = Encoding.UTF8.GetString(bufs)??"";

                var request = reqstr.JsonTo<DataRequest>();

                if(request==null)
                {
                    Resp(context, new DataReply {  Code = -601,  Msg = "请求参数错误" }.ToJson(), 200);
                    return;
                }

                var api = FactoryApi.CreateFunction(action);
                if (api == null)
                {
                    Resp(context, new DataReply {  Code = 100,  Msg = "未知操作" }.ToJson(), 200);
                    return;
                }
                if (!api.CheckSign(request, serverInfo))
                {
                    Resp(context, new DataReply {  Code = 222,  Msg = "电子签名不一致" }.ToJson(), 200);
                    return;
                }
                if (api.LimitAction)
                {
                    //var client = serverInfo.Clients.FirstOrDefault(c => c.Name == context.Peer);
                    //if (client == null)
                    //{
                    //    Resp(context, new DataReply { Code = 101, Msg = "未建立连接" }.ToJson(), 200);
                    //    return;
                    //}
                    //if (client.Token != request.Token)
                    //{
                    //    Resp(context, new DataReply { Code = 102, Msg = "未登录或登录已过期" }.ToJson(), 200);
                    //    return;
                    //}

                }
                var dataReply= api.ApiAction(request, serverInfo);
                Resp(context, dataReply.ToJson(), 200);
                return;
            }
            catch (Exception ex)
            {
                Resp(context, new DataReply {  Code = 999,  Msg = ex.Message }.ToJson(), 200);
                return;
            }
        }

        void Resp(HttpListenerContext context, string msg, int code)
        {
            if (msg == null) { msg = new DataReply{  Code = 9999,  Msg = "无返回值" }.ToJson(); }
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            context.Response.StatusCode = code;
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
            context.Response.Close();
        }
    }
}
