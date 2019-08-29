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

namespace ProjectPlan
{
    public class Bootstrapper : Bootstrapper<MainViewModel>
    {

       
        protected override void OnStart()
        {
           



        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Bind your own types. Concrete types are automatically self-bound.
            //builder.Bind<IMyInterface>().To<MyType>();
            builder.Bind<IViewManager>().To<CustomViewManager>();
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
            // Called on Application.Exit

            
        }

        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            // Called on Application.DispatcherUnhandledException
        }
    }
}
