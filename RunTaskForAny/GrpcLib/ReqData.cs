using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcLib
{
    public class ReqData
    {
        public string ApiPath { get; set; }
        public string Data { get; set; }
        public string AppID { get; set; }
        public string Sign { get; set; }
        public long Time { get; set; }

    }
}
