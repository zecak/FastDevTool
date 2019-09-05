using RunTaskForAny.Common.MEF;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPlan.Helper
{
    public class PluginManager
    {
        private static PluginManager _instance;

        public static PluginManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PluginManager();
                }
                return _instance;
            }

        }


        private CompositionContainer container = null;

        [ImportMany(typeof(IPluginForViewModel))]
        public List<Lazy<IPluginForViewModel, IMetadata>> ViewModelPlugins { get; set; }

        public PluginManager()
        {
            try
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;

                var catalog = new DirectoryCatalog(System.IO.Path.Combine(path, "Plugins"));
                catalog.Changed += Catalog_Changed;
                container = new CompositionContainer(catalog);
                container.ComposeParts(this);

                ViewModelPlugins = ViewModelPlugins.OrderBy(p => p.Metadata.Name).ThenByDescending(p => p.Metadata.VersionNumber).Distinct(new PluginForViewModelMetadataComparer()).ToList();


                //var p1 = Plugins.FirstOrDefault(p => p.Metadata.Name == "Plugin1");
                //var model = p1.Value;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void Catalog_Changed(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            
        }

        private Assembly[] assemblies;

        public Assembly[] Assemblies
        {
            get
            {
                if (assemblies == null)
                {
                    var list = new List<Assembly>();
                    foreach (var plugin in ViewModelPlugins)
                    {
                        var model = plugin.Value;
                        var modelType = model.GetType();
                        var modelAssembly = Assembly.GetAssembly(modelType);
                        list.Add(modelAssembly);
                    }
                    assemblies = list.ToArray();
                }

                return assemblies;
            }
        }
    }
}
