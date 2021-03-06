
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcCore.Manage.Models
{
    public class AgentModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { get; set; }

        public int Status { get; set; }
        public string StatusMsg { get; set; }
        public string Msg { get; set; }
        public DateTime LinkTime { get; set; }

        public string IP { get; set; }
        public string Port { get; set; }

        public string Key { get; set; }
        /// <summary>
        /// 状态:-1维护;0离线;1在线,2受限(客户端数达到上限)
        /// </summary>
        public int ServerStatus { get; set; }
        public string ServerStatusMsg { get; set; }

        public List<ClientInfo> Clients { get; set; }

    }
}
