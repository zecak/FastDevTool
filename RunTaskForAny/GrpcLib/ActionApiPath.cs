using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcLib
{
    public class ActionApiPath
    {
        public const string Login = "/api/login";
        public const string LoginOut = "/api/login_out";

        public const string Heartbeat = "/api/heartbeat";
        public const string ServerOline = "/server/online";
    }
}
