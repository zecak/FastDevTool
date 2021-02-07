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

        public string Status { get; set; }
    }
}
