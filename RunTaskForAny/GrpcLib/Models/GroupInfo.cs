using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcLib.Models
{
    public class GroupInfo
    {
        public string Name { get; set; }
        public List<string> Users { get; set; }

        public List<ChatInfo> ChatInfos { get; set; }

    }
}
