﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcCore.Common.Models
{
    public class ClientInfo
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public string ComputerName { get; set; }
        public string SystemName { get; set; }

        public string ClientHost { get; set; }
        public string ClientType { get; set; }
        public string UserName { get; set; }

        /// <summary>
        /// 状态:1在线,2离线
        /// </summary>
        public int Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime LastTime { get; set; }
        public long HitCount { get; set; }

    }
}
