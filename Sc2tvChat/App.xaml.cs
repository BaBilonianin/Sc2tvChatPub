using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RatChat {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public App(): base() {
            AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        System.Reflection.Assembly CurrentDomain_TypeResolve( object sender, ResolveEventArgs args ) {
            return Assembly.GetExecutingAssembly();
        }

        void App_DispatcherUnhandledException( object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e ) {
            if (e.Exception != null) {
                MessageBox.Show(e.Exception.ToString());
            }

            e.Handled = true;
        }

        public static string RootFolder { get; private set; }

        protected override void OnStartup( StartupEventArgs e ) {
            RootFolder = System.IO.Directory.GetCurrentDirectory();
            base.OnStartup(e);
        }
    }
}
