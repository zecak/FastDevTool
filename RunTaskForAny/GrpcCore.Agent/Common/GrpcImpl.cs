using Grpc.Core;
using GrpcCore;
using GrpcCore.Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcCore.Agent.Common
{
    public class GrpcImpl : gRPC.gRPCBase
    {

        public override Task<APIReply> Exec(APIRequest request, ServerCallContext context)
        {
            try
            {
                var sign = (request.AppID + request.Data + request.Time + Tool.Setting.AgentKey).ToMd5();
                if (sign != request.Sign)
                {
                    return Task.FromResult(new APIReply { Code = 2222, Msg = "电子签名不一致" });
                }

                if (request.ApiPath== "GetServerList")
                {
                    if(Tool.Setting.ServerRun!="1")
                    {
                        return Task.FromResult(new APIReply { Code=2, Msg = Tool.Setting.ServerInfo  });
                    }

                    var server = Tool.Setting.ServerList.FirstOrDefault();
                    if(server==null)
                    {
                        return Task.FromResult(new APIReply { Code = 1001, Msg = "代理服务未配置服务端" });
                    }
                    return Task.FromResult(new APIReply { Code = 1, Data = server.ToJson() });
                }
                return Task.FromResult(new APIReply { Code = 1000, Msg = "未知操作" });
            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex);
                return Task.FromResult(new APIReply { Code = 999, Msg = ex.Message });
            }
        }
    }

}
