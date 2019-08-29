using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPlan.Models
{
    public class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int ID { get; set; } 
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public string FullName => $"{GivenNames} {FamilyName}";


    }
}
