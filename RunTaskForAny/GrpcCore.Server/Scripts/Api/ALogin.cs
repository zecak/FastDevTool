using Grpc.Core;
using GrpcCore.Common;
using GrpcCore.Common.Models;
using GrpcCore.Common.Service;
using System;
using System.Collections.Generic;
using System.Text;

public class ALogin : AExecAtion
{
    public override bool LimitAction => false;
    public override string ActionName => ActionApiPath.Login;

    public override DataReply ApiAction(DataRequest request, ServerInfo serverInfo)
    {
        var resp = new DataReply();
        var info = request.Data.JsonTo<LoginModel>();
        if (info == null)
        {
            resp.Code = 1001;
            resp.Msg = "请求参数不正确";
            return resp;
        }

#warning 根据用户和密码获取用户信息并产生token

        var token = Guid.NewGuid().ToString();
        serverInfo.Clients.Add(new ClientInfo() { Name = token, StartTime = DateTime.Now, LastTime = DateTime.Now });
        //groupInfo.Users.Add(info.UserName);

        resp.Code = 1;
        resp.Data = token;
        resp.Msg = "登录成功";

        return resp;
    }
}

