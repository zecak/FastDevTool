using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Domain
{
    [Serializable]
    public class ResInfoModel
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public KeyValuePair<string,string> Data { get; set; }
        
    }
}
