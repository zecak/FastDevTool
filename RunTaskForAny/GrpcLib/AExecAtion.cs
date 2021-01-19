using Grpc.Core;
using GrpcLib.Models;
using GrpcLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcLib
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
        public abstract Task<APIReply> ApiAction(APIRequest request, ServerCallContext context, ServerInfo serverInfo);
    }
}
