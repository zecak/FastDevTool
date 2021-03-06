using Grpc.Core;
using GrpcCore.Common.Models;
using GrpcCore.Common.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrpcCore.Common
{
    public abstract class AExecAtion
    {
        /// <summary>
        /// 受限制的操作:true需要登录后才能操作,false无需登录就能操作
        /// </summary>
        public virtual bool LimitAction { get; } = true;
        /// <summary>
        /// 操作api地址
        /// </summary>
        public abstract string ActionName { get; }
        /// <summary>
        /// 具体操作
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual APIReply ApiAction(APIRequest request, ServerCallContext context, ServerInfo serverInfo)
        {
            return new APIReply() { Code = 100, Msg = "UnimplementedOperation" };
        }

        public virtual async Task ChatAction(APIRequest request, IServerStreamWriter<APIReply> responseStream, ServerCallContext context, ServerInfo serverInfo)
        {
            await responseStream.WriteAsync(new APIReply { Code = 100, Msg = "UnimplementedOperation" });
        }

        public bool CheckSign(APIRequest request, ServerInfo serverInfo)
        {
            var sign = (request.AppID + request.Data + request.Time + serverInfo.Setting.ServerKey).ToMd5();
            if (sign != request.Sign)
            {
                return false;
            }
            return true;
        }

    }
}
