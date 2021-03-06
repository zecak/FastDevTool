using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcCore.Common.Models
{
    public class ServerInfo
    {
        public SettingInfo Setting { get; set; }
        public List<ClientInfo> Clients { get; set; } = new List<ClientInfo>();
        public GroupInfo GroupInfo { get; set; } = new GroupInfo() { Name = "在线组", Users = new List<string>(), ChatInfos = new List<ChatInfo>() };

    }
}
