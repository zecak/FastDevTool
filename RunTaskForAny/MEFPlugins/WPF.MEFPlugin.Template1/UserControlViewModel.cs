using RunTaskForAny.Common.MEF;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.MEFPlugin.Template1
{
    [Export(typeof(IPluginForViewModel))]
    [CustomExportMetadata("插件示例", "这是插件示例", "作者", 1, "1.0")]
    public class UserControlViewModel : Screen, IPluginForViewModel
    {
        public string TitleInfo { get; set; } = "标题..";

        public UserControlViewModel()
        {

        }

        public void ShowBox()
        {
            System.Windows.MessageBox.Show("插件示例");
        }

    }
}
