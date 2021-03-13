using Grpc.Core;
using GrpcCore.Common;
using GrpcCore.Common.Models;
using GrpcCore.Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AServerOnline : AExecAtion
{
    public override bool LimitAction => false;
    public override string ActionName => ActionApiPath.ServerOline;

    public override DataReply ApiAction(DataRequest request, ServerInfo serverInfo)
    {
        var resp = new DataReply();
        resp.Code = 1;
        resp.Msg = "请求成功";
        resp.Data = serverInfo.Clients.Where(s => s.ClientType.Contains("Client")).Count().ToString();
        return resp;
    }
}

