using Grpc.Core;
using Grpc.Server.Common;
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
    public class ALogin : AExecAtion
    {
        public override bool LimitAction => false;
        public override string ActionName => ActionApiPath.Login;

        public override Task<APIReply> ApiAction(APIRequest request, ServerCallContext context, ServerInfo serverInfo)
        {
            var resp = new APIReply();
            var info = request.Data.JsonTo<LoginModel>();
            if (info == null)
            {
                resp.Code = 1001;
                resp.Msg = "请求参数不正确";
                return Task.FromResult(resp);
            }

#warning 根据用户和密码获取用户信息并产生token

            var token = Guid.NewGuid().ToString();
            serverInfo.OnlineUserTokens.Add(token);
            //groupInfo.Users.Add(info.UserName);

            resp.Code = 1;
            resp.Data = token;
            resp.Msg = "登录成功";

            return Task.FromResult(resp);
        }
    }
}
