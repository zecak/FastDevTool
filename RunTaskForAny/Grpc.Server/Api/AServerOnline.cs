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
    public class AServerOnline : AExecAtion
    {
        public override bool LimitAction => false;
        public override string ActionName => ActionApiPath.ServerOline;

        public override Task<APIReply> ApiAction(APIRequest request, ServerCallContext context, ServerInfo serverInfo)
        {
            var resp = new APIReply();
            resp.Code = 1;
            resp.Msg = "请求成功";
            return Task.FromResult(resp);
        }
    }
}
