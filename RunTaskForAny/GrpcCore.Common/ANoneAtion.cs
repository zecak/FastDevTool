using Grpc.Core;
using GrpcCore.Common.Models;
using GrpcCore.Common.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrpcCore.Common
{
    public class ANoneAtion : AExecAtion
    {
        public override string ActionName => "";
        public override DataReply ApiAction(DataRequest request, ServerInfo serverInfo)
        {
            return base.ApiAction(request, serverInfo);
        }

        public override Task ChatAction(APIRequest request, IServerStreamWriter<APIReply> responseStream, ServerCallContext context, ServerInfo serverInfo)
        {
            return base.ChatAction(request, responseStream, context, serverInfo);
        }
        public override bool LimitAction => false;
    }
}
