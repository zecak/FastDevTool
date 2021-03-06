using GrpcCore.Manage.Common;
using GrpcCore.Manage.Models;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcCore.Manage.Pages
{
    public class ShellViewModel : Screen
    {
        public string Title { get; set; }

        public AgentModel Agent { get; set; } = Helper.Agent;
        public ShellViewModel()
        {
            Title = "Grpc管理";
        }
        public void SayHello()
        {

        }
    }
}
