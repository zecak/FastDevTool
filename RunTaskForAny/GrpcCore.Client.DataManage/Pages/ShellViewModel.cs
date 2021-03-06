using GrpcCore.Client.DataManage.Common;
using GrpcCore.Client.DataManage.Models;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcCore.Client.DataManage.Pages
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
