using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcLib
{
    public class ReqData
    {
        public string API { get; set; }
        public object Data { get; set; }
        public string APPID { get; set; }
        public string Sign { get; set; }
        public long Time { get; set; }

    }
}
