using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcCore.Common.Models
{
    public class DataRequest
    {
		public string ApiPath { get; set; }
		public string Data { get; set; }
		public string AppID { get; set; }
		public string Sign { get; set; }
		public long Time { get; set; }
		public string Token { get; set; }
	}
}
