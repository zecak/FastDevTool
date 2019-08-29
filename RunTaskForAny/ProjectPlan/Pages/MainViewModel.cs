using ProjectPlan.Helper;
using ProjectPlan.Models;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPlan.Pages
{
    public class MainViewModel : Screen
    {
        public Person PersonInfo { get; set; }

        private IWindowManager windowManager;

        public MainViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            PersonInfo = new Person() { ID = 0, FamilyName = "K", GivenNames = "L" };
            
        }

        public void ShowBox()
        {
            PersonInfo.FamilyName = "MOI";
            this.windowManager.ShowMessageBox("1fffffffffffsssssssssss0");
        }
    }
}
