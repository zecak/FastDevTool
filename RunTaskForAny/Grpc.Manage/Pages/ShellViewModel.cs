using Grpc.Manage.Models;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Manage.Pages
{
    public class ShellViewModel : Screen
    {
        public string Title { get; set; }

        public Class1 class1 { get; set; }

        public ShellViewModel()
        {
            Title = "Grpc管理";
            class1 = new Class1() { Name = "sssss" };
        }
        public void SayHello()
        {
            class1.Name = "SayHello";

        }
    }
}
