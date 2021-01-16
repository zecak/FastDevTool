using Grpc.Core;
using GrpcLib;
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
                if(request.Parameters=="GetServerList")
                {
                    if(Tool.Setting.ServerRun!="1")
                    {
                        return Task.FromResult(new APIReply { Jsondata = new RespData() { Code = 2, ErrorInfo = "服务器维护中", Time = DateTime.Now.DateTimeToUTC() }.ToJson() });
                    }

                    var server = Tool.Setting.ServerList.FirstOrDefault();
                    if(server==null)
                    {
                        return Task.FromResult(new APIReply { Jsondata = new RespData() { Code = 1001, ErrorInfo="代理服务未配置服务端", Time = DateTime.Now.DateTimeToUTC() }.ToJson() });
                    }
                    return Task.FromResult(new APIReply { Jsondata = new RespData() { Code=1, Data= server.ToJson(), Time = DateTime.Now.DateTimeToUTC() }.ToJson() });
                }
                return Task.FromResult(new APIReply {  Jsondata = new RespData() { Code=1000, ErrorInfo="未知操作", Time = DateTime.Now.DateTimeToUTC() }.ToJson() });
            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex);
                return Task.FromResult(new APIReply { Jsondata = new RespData() { Code=-1, ErrorInfo=ex.Message, Time=DateTime.Now.DateTimeToUTC() }.ToJson() });
            }
        }
    }

}
