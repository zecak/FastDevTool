using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcCore.Common.Models
{
    public class GroupInfo
    {
        public string Name { get; set; }
        public List<string> Users { get; set; }

        public List<ChatInfo> ChatInfos { get; set; }

    }
}
