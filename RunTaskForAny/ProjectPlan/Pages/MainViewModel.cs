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
using System.Windows;
using System.Windows.Interactivity;

namespace ProjectPlan.Pages
{
    public class MainViewModel : Screen
    {
        public Window Window { get; set; }

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

        public void RequestMin()
        {
            Window = this.View as Window;
            if (Window!=null)
            Window.WindowState = WindowState.Minimized;
        }

        public void RequestMouseMove()
        { 
            Window = this.View as Window;
            if (Window != null)
            {
                if (System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    if (Window.WindowState == WindowState.Maximized)
                    {
                        var fullWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                        var left = System.Windows.Input.Mouse.GetPosition(Window).X;
                        Window.WindowState = WindowState.Normal;
                        Window.Top = 0;
                        Window.Left = left-(left/ fullWidth)*Window.Width;
                        
                    }
                    Window.DragMove();
                }
            }
        }

        public void ShowBox()
        {
            PersonInfo.FamilyName = "MOI";
            this.windowManager.ShowMessageBox("主界面ViewModel");
        }
    }
}
