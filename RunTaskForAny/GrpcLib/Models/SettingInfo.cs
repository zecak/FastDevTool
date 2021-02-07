using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcLib.Models
{
    public class SettingInfo
    {
        public string Name { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }

        public string ConnectionString { get; set; }
        public string ProviderString { get; set; }
        public string ServerIP { get; set; }
        public string ServerPort { get; set; }

        public string ServerKey { get; set; }
    }
}
