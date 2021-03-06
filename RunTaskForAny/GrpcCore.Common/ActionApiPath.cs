using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcCore.Common
{
    public class ActionApiPath
    {
        public const string Login = "/api/login";
        public const string LoginOut = "/api/login_out";

        public const string Heartbeat = "/api/heartbeat";
        public const string ServerOline = "/server/online";

        public const string GetClients = "/api/getclients";
    }
}
