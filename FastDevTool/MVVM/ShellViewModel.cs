using FastDevTool.Common;
using FastDevTool.DataBase;
using FastDevTool.MVVM.NPCModel;
using Stylet;
using System;
using System.Collections.Generic;
using System.Data;
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

        public int IndexTableInfo { get; set; }
        

        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            LoadData();
            IndexTableInfo = -1;
        }

        public void ShowUserModuleAdd()
        {
            windowManager.ShowDialog(new UserModuleAddViewModel());
            LoadData();
        }

        public void LoadData(int curindex = 1)
        {
            var tables = localDbContext.GetTablesSchema().Where(m => m.SystemMark != 1).ToList();
            var tableInfos = new List<TableInfo>();
            foreach (var tb in tables)
            {
                var tableInfo = new TableInfo() { Name = tb.Name, Title = tb.Title, Paging = new Paging(), ColumnInfos = tb.Columns.ConvertAll(m => new ColumnInfo() { Name = m.Name, Title = m.Title }) };
                tableInfo.Table = localDbContext.GetListForPage(tableInfo.Name, tableInfo.Paging, tableInfo.ColumnInfos);
                tableInfo.PageNumberList = new List<int>();
                for (int i = tableInfo.Paging.StartIndex; i < tableInfo.Paging.EndIndex; i++)
                {
                    tableInfo.PageNumberList.Add(i + 1);
                }
                tableInfo.LoadDataAction = LDAction;
                tableInfos.Add(tableInfo);
            }
            TableInfos = tableInfos;
            IndexTableInfo = 0;
            
        }

        void LDAction(TableInfo tableInfo)
        {
            tableInfo.Table.Rows.Clear();
            var temp = localDbContext.GetListForPage(tableInfo.Name, tableInfo.Paging, tableInfo.ColumnInfos);
            foreach (DataRow row in temp.Rows)
            {
                var r = tableInfo.Table.NewRow();
                r.ItemArray = row.ItemArray;
                tableInfo.Table.Rows.Add(r);
            }
        }

        public void GoPageIndex(TableInfo tableInfo)
        {
            LDAction(tableInfo);
        }

        public void GoPageFirst(TableInfo tableInfo)
        {
            tableInfo.Paging.PageIndex = tableInfo.Paging.First;
            LDAction(tableInfo);
        }

        public void GoPageLast(TableInfo tableInfo)
        {
            tableInfo.Paging.PageIndex = tableInfo.Paging.Last;
            LDAction(tableInfo);
        }

        public void GoPageNext(TableInfo tableInfo)
        {
            tableInfo.Paging.PageIndex = tableInfo.Paging.Next;
            LDAction(tableInfo);
        }

        public void GoPagePrevious(TableInfo tableInfo)
        {
            tableInfo.Paging.PageIndex = tableInfo.Paging.Previous;
            LDAction(tableInfo);
        }
         
    }
}
