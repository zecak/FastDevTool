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
        private CompositionContainer container = null;

        [ImportMany(typeof(IPlugin))]
        public List<Lazy<IPlugin, IMetadata>> Plugins { get; set; }


        public Person PersonInfo { get; set; }

        public string TitleInfo { get; set; }

        private IWindowManager windowManager;

        public MainViewModel(IWindowManager _windowManager)
        {
            this.windowManager = _windowManager;

            PersonInfo = new Person() { ID = 0, FamilyName = "K", GivenNames = "L" };

            TitleInfo = "MyPlan";

            try
            {
                var catalog = new DirectoryCatalog("Plugins");
                container = new CompositionContainer(catalog);
                container.ComposeParts(this);

                Plugins = Plugins.OrderBy(p=>p.Metadata.Name).ThenByDescending(p=>p.Metadata.VersionNumber).Distinct(new PuluginMetadataComparer()).ToList();

                //var p1 = Plugins.FirstOrDefault(p => p.Metadata.Name == "Plugin1");
                //var model = p1.Value;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public void ShowBox()
        {
            PersonInfo.FamilyName = "MOI";
            this.windowManager.ShowMessageBox("1fffffffffffsssssssssss0");
        }
    }
}
