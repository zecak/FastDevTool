using RunTaskForAny.Common.MEF;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;

namespace ProjectPlan.Helper
{
    public class NameValue
    {
        public string Name { get; set; }
        public FrameworkElement Value { get; set; }
    }

    public class CustomViewManager : ViewManager
    {
        public static string ViewModelToViewRootName = ConfigurationManager.AppSettings["ViewModelToViewRootName"] ?? "Pages";
        public static string ViewModelToViewCurName = ConfigurationManager.AppSettings["ViewModelToViewCurName"] ?? "v1";

        static List<NameValue> rootElement;

        public CustomViewManager(ViewManagerConfig config) : base(config)
        {
            foreach (var plugin in PluginManager.Instance.ViewModelPlugins)
            {
                var model = plugin.Value;
                var model_view = this.CreateAndBindViewForModelIfNecessary(model);
            }

            if (rootElement == null)
            {
                rootElement = new List<NameValue>();
                var xamlFiles = System.IO.Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ViewModelToViewRootName, ViewModelToViewCurName));
                foreach (var xamlFile in xamlFiles)
                {
                    try
                    {
                        System.IO.FileInfo fileInfo = new FileInfo(xamlFile);
                        FrameworkElement element = null;
                        using (FileStream fs = new FileStream(xamlFile, FileMode.Open, FileAccess.Read))
                        {
                            element = System.Windows.Markup.XamlReader.Load(fs) as FrameworkElement;
                            if (element != null)
                            {
                                rootElement.Add(new NameValue() { Name = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf(".")), Value = element });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("加载界面时出错:" + ex.Message + "," + xamlFile);
                    }
                }
            }
            

        }

        public override UIElement CreateViewForModel(object model)
        {
            var mtype = model.GetType();
            var nv = rootElement?.FirstOrDefault(m => m.Name + "Model" == mtype.Name);
            if (nv == null)
            {
                return base.CreateViewForModel(model);
            }
            var view = (UIElement)nv.Value;
            return view;
        }

        public override void BindViewToModel(UIElement view, object viewModel)
        {
            base.BindViewToModel(view, viewModel);
        }

        public override UIElement CreateAndBindViewForModelIfNecessary(object model)
        {
            return base.CreateAndBindViewForModelIfNecessary(model);
        }

    }

}
