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

        public MainViewModel()
        {
            PersonInfo = new Person() { ID = 0, FamilyName = "K", GivenNames = "L" };

            TitleInfo = "MyPlan";

            Plugins = PluginManager.Instance.ViewModelPlugins;

        }


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

        public void RequestMid()
        {
            Window = this.View as Window;
            if (Window != null)
            {
                if(Window.WindowState== WindowState.Maximized)
                {
                    Window.WindowState = WindowState.Normal;
                }
                else
                {
                    Window.WindowState = WindowState.Maximized;
                }
            }
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
                        if(Window.Left<0)//左屏(从屏)
                        {
                            //纠正左偏移
                            
                            Window.Left = (-fullWidth+left) - (left / fullWidth) * Window.Width;
                        }
                        else//主屏
                        {
                            Window.Left = left - (left / fullWidth) * Window.Width;
                        }
                       
                    }
                    Window.DragMove();
                }
            }
        }

        private long _oncetime = long.MaxValue;
        private Point _oncePoint;
        public void RequestMouseLeftButtonDown()
        {
            Window = this.View as Window;
            if (Window != null)
            { 
                //双击,两次单机距离不超过4像素，时间再0.5秒以内视为双击
                Point p = System.Windows.Input.Mouse.GetPosition(Window);
                long time = DateTime.Now.Ticks;
                if (Math.Abs(p.X - _oncePoint.X) < 4 && Math.Abs(p.Y - _oncePoint.Y) < 4 && (time - _oncetime < 5000000))
                {
                    _oncetime = long.MaxValue;
                    //此处促发双击事件
                    if (Window.WindowState == WindowState.Maximized)
                    {
                        Window.WindowState = WindowState.Normal;
                    }
                    else
                    {
                        Window.WindowState = WindowState.Maximized;
                    }
                    
                }
                _oncetime = time;
                _oncePoint = p;

            }
        }

        public void ShowBox()
        {
            PersonInfo.FamilyName = "MOI";
            this.windowManager.ShowMessageBox("主界面ViewModel");
        }
    }
}
