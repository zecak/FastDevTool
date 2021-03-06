using Grpc.Core;
using GrpcCore.Common;
using GrpcCore.Common.Models;
using GrpcCore.Common.Service;
using System;
using System.Collections.Generic;
using System.Text;

public class AGetClients : AExecAtion
{
    public override bool LimitAction => false;
    public override string ActionName => ActionApiPath.GetClients;

    public override APIReply ApiAction(APIRequest request, ServerCallContext context, ServerInfo serverInfo)
    {
        var resp = new APIReply();
        resp.Code = 1;
        resp.Msg = "Success";
        resp.Data = serverInfo.Clients.ToJson();
        return resp;
    }
}

