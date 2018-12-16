using FastDevTool.Common;
using FastDevTool.DataBase;
using FastDevTool.DataBase.Model;
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

        public int IndexTableInfo { get; set; } = -1;


        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            LoadData();
        }

        public void ShowUserModuleAdd()
        {
            windowManager.ShowDialog(new UserModuleAddViewModel());
            LoadData();
        }

        public void LoadData()
        {
            var tables = localDbContext.GetTablesSchema().Where(m => m.SystemMark != 1).ToList();
            var tableInfos = new List<TableInfo>();
            var columns = localDbContext.QueryAllList<sys_field_type>();
            foreach (var tb in tables)
            {
                var tableInfo = new TableInfo() { Name = tb.Name, Title = tb.Title, Paging = new Paging(), ColumnInfos = tb.Columns.Where(m=>m.Name != "RowNumber" && m.Name != "VersonTime").ToList().ConvertAll(m => new ColumnInfo() { Name = m.Name, Title = m.Title, MaxLength = m.MaxLength, TypeName = m.TypeName }) };
                tableInfo.Table = localDbContext.GetListForPage(tableInfo.Name, tableInfo.Paging, tableInfo.ColumnInfos);
                for (int i = 0; i < tableInfo.ColumnInfos.Count; i++)
                {
                    tableInfo.ColumnInfos[i].ID = i + 1;
                    var column = columns.FirstOrDefault(m => m.Name == tableInfo.ColumnInfos[i].TypeName);
                    if (column != null)
                    {
                        tableInfo.ColumnInfos[i].TypeTitle = column.Title;
                    }
                }
                tableInfo.PageNumberList = new List<int>();
                for (int i = tableInfo.Paging.StartIndex; i < tableInfo.Paging.EndIndex; i++)
                {
                    tableInfo.PageNumberList.Add(i + 1);
                }
                tableInfo.LoadDataAction = LDAction;
                tableInfos.Add(tableInfo);
            }
            TableInfos = tableInfos;
            IndexTableInfo = TableInfos.Count - 1;

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

        public void GoPageNumber(object obj)
        {
            var btn = obj as System.Windows.Controls.Button;
            var tableInfo = btn.Tag as TableInfo;
            tableInfo.Paging.PageIndex = (int)btn.Content;
            LDAction(tableInfo);
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

        public void ShowColumnManage(TableInfo tableInfo)
        {
            windowManager.ShowDialog(new ColumnManageViewModel(windowManager, tableInfo));
            LoadData();
        }
    }
}
