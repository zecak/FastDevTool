using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcLib
{
    public class ChatInfoUser
    { 
        public string GID { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; }
        public bool IsSended { get; set; }
    }
}
