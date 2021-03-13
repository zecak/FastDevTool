using Grpc.Core;
using GrpcCore.Common;
using GrpcCore.Common.Models;
using GrpcCore.Common.Service;
using System;
using System.Collections.Generic;
using System.Text;

public class ALoginOut : AExecAtion
{
    public override string ActionName => ActionApiPath.LoginOut;

    public override DataReply ApiAction(DataRequest request, ServerInfo serverInfo)
    {
        var resp = new DataReply();
        resp.Code = 1;
        resp.Msg = "请求成功";
        //serverInfo.OnlineUserTokens.Remove(request.Token);
#warning 根据用户和密码获取用户信息并清除token
        return resp;
    }
}

