using Grpc.Core;
using GrpcLib;
using GrpcLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Agent.Common
{
    public class GrpcImpl : gRPC.gRPCBase
    {

        public override Task<APIReply> Exec(APIRequest request, ServerCallContext context)
        {
            try
            {
                if(request.ApiPath== "GetServerList")
                {
                    if(Tool.Setting.ServerRun!="1")
                    {
                        return Task.FromResult(new APIReply { Code=2, Data = "服务器维护中" });
                    }

                    var server = Tool.Setting.ServerList.FirstOrDefault();
                    if(server==null)
                    {
                        return Task.FromResult(new APIReply { Code = 1001, Data = "代理服务未配置服务端" });
                    }
                    return Task.FromResult(new APIReply { Code = 1, Data = server.ToJson() });
                }
                return Task.FromResult(new APIReply { Code = 1000, Data = "未知操作" });
            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex);
                return Task.FromResult(new APIReply { Code = 999, Data = ex.Message });
            }
        }
    }

}
