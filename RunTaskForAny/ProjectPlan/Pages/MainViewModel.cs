using ProjectPlan.Helper;
using ProjectPlan.Models;
using RunTaskForAny.Common.MEF;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPlan.Pages
{
    public class MainViewModel : Screen
    {

        public List<Lazy<IPluginForViewModel, IMetadata>> Plugins { get; set; }


        public Person PersonInfo { get; set; }

        public string TitleInfo { get; set; }

        private IWindowManager windowManager;

        public MainViewModel(IWindowManager _windowManager)
        {
            this.windowManager = _windowManager;

            PersonInfo = new Person() { ID = 0, FamilyName = "K", GivenNames = "L" };

            TitleInfo = "MyPlan";

            Plugins = PluginManager.Instance.ViewModelPlugins;

        }

        public void ShowBox()
        {
            PersonInfo.FamilyName = "MOI";
            this.windowManager.ShowMessageBox("主界面ViewModel");
        }
    }
}
