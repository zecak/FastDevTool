using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.MVVM.NPCModel
{
    public class TableInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }
        public string Title { get; set; }

        public List<ColumnInfo> ColumnInfos{get;set;}
    }
}
