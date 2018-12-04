using FastDevTool.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        public DataTable Table { get; set; }

        public Paging Paging { get; set; }
    }
}
