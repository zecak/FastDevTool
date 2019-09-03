using RunTaskForAny.Common.MEF;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1
{
    [Export(typeof(IPluginForViewModel))]
    [CustomExportMetadata("Plugin1", "这是第一个插件", "snake", 2, "2.0")]
    public class UserControl1ViewModel : Screen, IPluginForViewModel
    {
        public Person PersonInfo { get; set; }= new Person() { ID = 0, FamilyName = "A", GivenNames = "B" };
        public string TitleInfo { get; set; } = "Def";

        public UserControl1ViewModel()
        {
            PersonInfo = new Person() { ID = 0, FamilyName = "插件1", GivenNames = "2" };
            TitleInfo = "插件1 版本2";
        }

        public void ShowBox()
        { 
            System.Windows.MessageBox.Show(TitleInfo);
        }

    }
}
