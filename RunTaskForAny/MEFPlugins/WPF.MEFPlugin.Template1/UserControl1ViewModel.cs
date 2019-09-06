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
    [CustomExportMetadata("Plugin2", "这是第2个插件", "snake", 1, "1.0")]
    public class UserControl1ViewModel : Screen, IPluginForViewModel
    {
        public Person PersonInfo { get; set; } = new Person() { ID = 0, FamilyName = "dd", GivenNames = "hh" };
        public string TitleInfo { get; set; } = "Def";

        public UserControl1ViewModel()
        {
            PersonInfo = new Person() { ID = 0, FamilyName = "插件2", GivenNames = "1" };
            TitleInfo = "插件2 版本1";
        }

        public void ShowBox()
        {
            System.Windows.MessageBox.Show(TitleInfo);
        }

    }
}
