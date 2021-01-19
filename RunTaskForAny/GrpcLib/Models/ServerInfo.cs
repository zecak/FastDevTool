using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcLib.Models
{
    public class ServerInfo
    {
        public List<ClientInfo> ClientInfos { get; set; } = new List<ClientInfo>();
        public List<string> OnlineUserTokens { get; set; } = new List<string>();
        public GroupInfo GroupInfo { get; set; } = new GroupInfo() { Name = "在线组", Users = new List<string>(), ChatInfos = new List<ChatInfo>() };

    }
}
