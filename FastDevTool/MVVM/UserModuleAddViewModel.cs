using FastDevTool.DataBase;
using FastDevTool.MVVM.NPCModel;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.MVVM
{
    public class UserModuleAddViewModel : Screen
    {
        public ModuleInfo MyModuleInfo { get; set; } = new ModuleInfo();

        public UserModuleAddViewModel()
        {

        }

        public void AddNew()
        {
            if(string.IsNullOrWhiteSpace(MyModuleInfo.Name))
            {
                System.Windows.MessageBox.Show("请填写名称");
                return;
            }



        }
    }
}
