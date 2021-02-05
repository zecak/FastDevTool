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

        public ShellViewModel()
        {
            Title = "Grpc管理";
        }

    }
}
