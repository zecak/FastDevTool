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
    public class ShellViewModel : Screen
    {
        private IWindowManager windowManager;

        LocalDbContext localDbContext = new LocalDbContext();

        public List<TableInfo> TableInfos { get; set; }


        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;

            TableInfos = localDbContext.GetTablesSchema().Where(m=>m.SystemMark!=1).ToList().ConvertAll(m=>new TableInfo() { Name=m.Name, Title=m.Title, ColumnInfos=m.Columns.ConvertAll(c=>new ColumnInfo() { Name=c.Name,Title=c.Title }) });
        }

        public string Name { get; set; } = "王者荣耀";

        public void SayHello() => Name = "Hello " + Name;

        public Person MyPerson { get; set; } = new Person() { FamilyName = "吕", GivenNames = "小布" };

        public List<Person> datas { get; set; } = new List<Person>() { new Person() { ID = 1, FamilyName = "吕", GivenNames = "小布" }, new Person() { ID = 2, FamilyName = "貂", GivenNames = "蝉" } };
        public int ID { get; set; } = 2;
        public void AddUser()
        {
            ID = 1;
        }

        public void ShowUserModuleAdd()
        {
            windowManager.ShowDialog(new UserModuleAddViewModel());
            TableInfos = localDbContext.GetTablesSchema().Where(m => m.SystemMark != 1).ToList().ConvertAll(m => new TableInfo() { Name = m.Name, Title = m.Title });
        }
    }
}
