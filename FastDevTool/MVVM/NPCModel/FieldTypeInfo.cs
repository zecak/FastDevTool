using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.MVVM.NPCModel
{
    public class FieldTypeInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int ID { get; set; }
        public string Name { get; set; }

        public string Title { get; set; }
    }
}
