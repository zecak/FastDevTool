using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcCore.Common.Models
{
    public class UserInfo
    {
        public string Token { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public long LoingTime { get; set; }
    }
}
