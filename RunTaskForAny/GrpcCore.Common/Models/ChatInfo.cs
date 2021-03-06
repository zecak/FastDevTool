using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcCore.Common.Models
{
    public class ChatInfo
    {
        public string UserName { get; set; }
        public string Msg { get; set; }
        public long SendTime { get; set; }
    }
}
