using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcLib
{
    public class RespData
    {
        public int Code { get; set; }
        public string Data { get; set; }
        public string Msg { get; set; }
    }
}
