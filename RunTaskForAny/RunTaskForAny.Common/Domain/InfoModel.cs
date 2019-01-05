using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Domain
{
    [Serializable]
    public class InfoModel
    {
        public ClientInfo Client { get; set; }
        public string TypeName { get; set; }

        public string ClassName { get; set; }
        public string MethodName { get; set; }

        public string JsonData { get; set; }

        public DateTime SendTime { get; set; }

        public string PluginName { get; set; }
        public int PluginVersion { get; set; }
    }
}
