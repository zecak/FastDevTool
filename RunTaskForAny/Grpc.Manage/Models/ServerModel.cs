using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Manage.Models
{
    public class ServerModel
    {
        public string IP { get; set; }
        public string Port { get; set; }

        public string Key { get; set; }
        /// <summary>
        /// 状态:-1维护;0离线;1在线,2受限(客户端数达到上限)
        /// </summary>

        public string Status { get; set; }
    }
}
