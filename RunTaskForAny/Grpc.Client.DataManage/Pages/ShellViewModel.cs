using Grpc.Client.DataManage.Common;
using Grpc.Client.DataManage.Models;
using GrpcLib.Models;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Client.DataManage.Pages
{
    public class ShellViewModel : Screen
    {
        public string Title { get; set; }

        public AgentModel Agent { get; set; } = Helper.Agent;
        public ShellViewModel()
        {
            Title = "数据管理";
        }
        public void SayHello()
        {

        }
    }
}
