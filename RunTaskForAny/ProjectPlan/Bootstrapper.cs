using System;
using Stylet;
using StyletIoC;
using System.Windows;
using System.Windows.Threading;
using ProjectPlan.Pages;
using ProjectPlan.Helper;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using RunTaskForAny.Common.MEF;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;

namespace ProjectPlan
{
    public class Bootstrapper : Bootstrapper<MainViewModel>
    {

        protected override void OnStart()
        {

        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            //加载MEF插件程序集
            builder.Autobind(PluginManager.Instance.Assemblies);

            //动态加载主界面用到的程序集
            builder.Autobind(typeof(System.Windows.Interactivity.Interaction).Assembly);//UI 事件绑定
            builder.Autobind(typeof(FontAwesome.WPF.FontAwesome).Assembly);//UI 字体


            builder.Bind<IViewManager>().To<CustomViewManager>().InSingletonScope();

        }

        protected override void Configure()
        {
            // This is called after Stylet has created the IoC container, so this.Container exists, but before the
            // Root ViewModel is launched.
            // Configure your services, etc, in here
        }

        protected override void OnLaunch()
        {
            this.RootViewModel.Closed += RootViewModel_Closed;

        }

        private void RootViewModel_Closed(object sender, CloseEventArgs e)
        {
            App.Current.Shutdown();

        }

        protected override void OnExit(ExitEventArgs e)
        {

        }

        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {

        }
    }
}
