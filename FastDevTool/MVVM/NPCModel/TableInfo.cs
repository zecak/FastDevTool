using FastDevTool.Common;
using FastDevTool.View.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FastDevTool.MVVM.NPCModel
{
    public class TableInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }
        public string Title { get; set; }

        public DataTable Table { get; set; }

        public Paging Paging { get; set; }

        public List<int> PageNumberList { get; set; }

        public int DataPageIndex { get; set; }

        public List<ColumnInfo> ColumnInfos { get; set; }

        public Action<TableInfo> LoadDataAction { get; set; }

        public ICommand LoadDataCommand => new RelayCommand((obj) =>
        {
            LoadDataAction?.Invoke(this);
        });
    }
}
