using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Stylet;
using StyletIoC;
using WpfApp1.Common;
using WpfApp1.Pages;
using System.Linq;

namespace WpfApp1
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void OnStart()
        {

        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Bind your own types. Concrete types are automatically self-bound.
            //builder.Bind<IMyInterface>().To<MyType>();
        }

        protected override void Configure()
        {
            // This is called after Stylet has created the IoC container, so this.Container exists, but before the
            // Root ViewModel is launched.
            // Configure your services, etc, in here
        }

        protected override void OnLaunch()
        {
            // This is called just after the root ViewModel has been launched
            // Something like a version check that displays a dialog might be launched from here
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
