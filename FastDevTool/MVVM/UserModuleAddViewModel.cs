using FastDevTool.Common;
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
        LocalDbContext localDbContext = new LocalDbContext();
        public ModuleInfo MyModuleInfo { get; set; } = new ModuleInfo();

        public UserModuleAddViewModel()
        {

        }

        public void AddNew()
        {
            if (string.IsNullOrWhiteSpace(MyModuleInfo.Title))
            {
                System.Windows.MessageBox.Show("请填写标题");
                return;
            }

            if (string.IsNullOrWhiteSpace(MyModuleInfo.Name))
            {
                System.Windows.MessageBox.Show("请填写名称");
                return;
            }
           
            if (localDbContext.ExistsTable(MyModuleInfo.Name))
            {
                System.Windows.MessageBox.Show("该模块已存在");
                return;
            }

            var template = TemplateManage.GetTemplateDef(MyModuleInfo.Name.Trim(), MyModuleInfo.Title.Trim());

            var ok = localDbContext.ExecuteNonQuery(template);
            if(!ok)
            {
                System.Windows.MessageBox.Show("新增模块失败");
                return;
            }
            this.RequestClose(true);
        }

        public void GetPinyin(string title)
        {
            if(string.IsNullOrWhiteSpace(title))
            {
                title = "";
            }
            MyModuleInfo.Name = Helper.GetPinyin(title).Trim().Replace(' ','_');
        }
    }
}
