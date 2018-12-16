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
    public class ColumnManageViewModel : Screen
    {
        private IWindowManager windowManager;
        public TableInfo TableInfo { get; set; }
        LocalDbContext localDbContext = new LocalDbContext();

        public ColumnManageViewModel(IWindowManager windowManager, TableInfo tableInfo)
        {
            this.windowManager = windowManager;
            TableInfo = tableInfo;
        }

        public void AddColumn()
        {
            windowManager.ShowDialog(new ColumnAddViewModel(windowManager, TableInfo));
        }

        public void DelColumn(string fieldName)
        {

        }
    }
}
