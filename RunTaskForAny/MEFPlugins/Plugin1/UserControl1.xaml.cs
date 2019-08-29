using RunTaskForAny.Common.MEF;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Plugin1
{
    [Export(typeof(IPlugin))]
    [CustomExportMetadata("Plugin1", "这是第一个插件", "snake", "1.0")]
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl, IPlugin
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        public void Dowork(string[] args)
        {
            
        }

    }
}
