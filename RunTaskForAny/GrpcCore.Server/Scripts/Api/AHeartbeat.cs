using Grpc.Core;
using GrpcCore.Common;
using GrpcCore.Common.Models;
using GrpcCore.Common.Service;
using System;
using System.Collections.Generic;
using System.Text;

public class AHeartbeat : AExecAtion
{
    public override bool LimitAction => false;
    public override string ActionName => ActionApiPath.Heartbeat;

    public override DataReply ApiAction(DataRequest request, ServerInfo serverInfo)
    {
        var resp = new DataReply();
        resp.Code = 1;
        resp.Msg = "Success";
        return resp;
    }

}

