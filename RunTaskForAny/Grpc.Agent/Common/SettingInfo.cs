using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Agent.Common
{
    public class SettingInfo
    {
        public string Name { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }

        public string AgentIP { get; set; }
        public string AgentPort { get; set; }

        public string AgentKey { get; set; }

        public string ServerRun { get; set; }
        public string ServerInfo { get; set; }
        public int MaxClientCount { get; set; }
        public List<ServerModel> ServerList { get; set; }
    }

    public class ServerModel
    {
        public string IP { get; set; }
        public string Port { get; set; }
        /// <summary>
        /// 通讯Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 状态:-1维护;0离线;1在线,2受限(客户端数达到上限)
        /// </summary>
        public string Status { get; set; }
    }
}
