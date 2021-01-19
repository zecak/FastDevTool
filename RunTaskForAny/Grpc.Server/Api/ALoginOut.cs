using Grpc.Core;
using GrpcLib;
using GrpcLib.Models;
using GrpcLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Server.Api
{
    public class ALoginOut : AExecAtion
    {
        public override string ActionName => ActionApiPath.LoginOut;

        public override Task<APIReply> ApiAction(APIRequest request, ServerCallContext context, ServerInfo serverInfo)
        {
            var resp = new APIReply();
            resp.Code = 1;
            resp.Msg = "请求成功";
            serverInfo.OnlineUserTokens.Remove(request.Token);
#warning 根据用户和密码获取用户信息并清除token
            return Task.FromResult(resp);
        }
    }
}
