using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcLib
{
    public class ChatInfo
    {
        public string UserName { get; set; }
        public string Msg { get; set; }
        public DateTime SendTime { get; set; }
    }
}
