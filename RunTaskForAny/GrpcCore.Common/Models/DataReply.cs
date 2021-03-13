using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcCore.Common.Models
{
    public class DataReply
    {
        public int Code { get; set; }
        public string Data { get; set; }
        public string Msg { get; set; }
        public string Action { get; set; }
    }
}
