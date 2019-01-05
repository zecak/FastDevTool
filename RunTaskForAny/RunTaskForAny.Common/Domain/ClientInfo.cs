using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Domain
{
    [Serializable]
    public class ClientInfo
    {
        public Guid GID { get; set; }
        public string ComputerName { get; set; }
        public string MAC { get; set; }
        public string IP { get; set; }
        public DateTime StartTime { get; set; }
        public string ClientName { get; set; }
    }
}
