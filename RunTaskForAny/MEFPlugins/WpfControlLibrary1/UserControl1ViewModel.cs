using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1
{
    public class UserControl1ViewModel : Screen
    {
        public string TitleInfo { get; set; }

        private IWindowManager windowManager;

        public UserControl1ViewModel(IWindowManager _windowManager)
        {
            this.windowManager = _windowManager;

            TitleInfo = "插件1 版本2";
        }
    }
}
