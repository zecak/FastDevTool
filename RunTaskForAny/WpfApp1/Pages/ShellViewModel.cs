using System;
using Stylet;
using WpfApp1.Models;
using System.Collections.Generic;

namespace WpfApp1.Pages
{
    public class ShellViewModel : Screen
    {
        
        public string Name { get; set; } = "王者荣耀";

        public void SayHello() => Name = "Hello " + Name;

        public Person MyPerson { get; set; } = new Person() { FamilyName = "吕", GivenNames = "小布" };

        public List<Person> datas { get; set; } = new List<Person>() { new Person() { ID=1, FamilyName = "吕", GivenNames = "小布"}, new Person() {ID=2, FamilyName = "貂", GivenNames = "蝉" } };
        public int ID { get; set; } =2;
        public void AddUser()
        {
            ID = 1;
            //_windowManger.ShowDialog(_ChildDialog);
            //var ok = LocalDbContext.Add("吕布", "123");
            //if (ok)
            //{
            //    //_windowManger.ShowMessageBox(Name);
            //}
        }
    }
}
