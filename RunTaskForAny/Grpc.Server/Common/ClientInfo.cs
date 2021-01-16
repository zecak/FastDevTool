using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Server.Common
{
    public class ClientInfo
    {
        public string Key { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime LastTime { get; set; }
        public long HitCount { get; set; }

    }
}
