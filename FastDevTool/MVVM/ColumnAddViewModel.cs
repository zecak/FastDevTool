using FastDevTool.Common;
using FastDevTool.DataBase;
using FastDevTool.DataBase.Model;
using FastDevTool.MVVM.NPCModel;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.MVVM
{
    public class ColumnAddViewModel : Screen
    {
        public List<KeyValuePair<string, int>> MaxLengthTypeList { get; set; } = new List<KeyValuePair<string, int>>()
        {
            new KeyValuePair<string, int>("单字(1)",1),
            new KeyValuePair<string, int>("双字(2)",2),
            new KeyValuePair<string, int>("标题(50)",50),
            new KeyValuePair<string, int>("简要(200)",200),
            new KeyValuePair<string, int>("描述说明(500)",500),
            new KeyValuePair<string, int>("详细内容(2000)",2000),
        };

        public List<FieldTypeInfo> FieldTypeInfoList { get; set; }

        private IWindowManager windowManager;
        public TableInfo TableInfo { get; set; }
        LocalDbContext localDbContext = new LocalDbContext();
        public ColumnAddViewModel(IWindowManager windowManager, TableInfo tableInfo)
        {
            this.windowManager = windowManager;
            TableInfo = tableInfo;

            FieldTypeInfoList = localDbContext.QueryAllList<sys_field_type>().Where(m=>m.Status==1).OrderBy(m=>m.SortNo).ToList().ConvertAll(m=>new FieldTypeInfo() { ID=m.ID, Name=m.Name, Title=m.Title });
        }

        public void SaveData()
        {

        }
    }
}
