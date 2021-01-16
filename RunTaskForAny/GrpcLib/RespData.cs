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
        public object Data { get; set; }
        public string ErrorInfo { get; set; }
        public long Time { get; set; }
    }
}
