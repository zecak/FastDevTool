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
    public class AHeartbeat : AExecAtion
    {
        public override bool LimitAction => false;
        public override string ActionName => ActionApiPath.Heartbeat;

        public override APIReply ApiAction(APIRequest request, ServerCallContext context, ServerInfo serverInfo)
        {
            var resp = new APIReply();
            resp.Code = 1;
            resp.Msg = "Success";
            return resp;
        }

    }
}
