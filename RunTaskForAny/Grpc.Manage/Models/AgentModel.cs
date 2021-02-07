using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Manage.Models
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

    }
}
