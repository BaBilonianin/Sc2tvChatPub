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
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        void App_DispatcherUnhandledException( object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e ) {
            if (e.Exception != null) {
                MessageBox.Show(e.Exception.ToString());
            }

            e.Handled = true;
        }

        public static string RootFolder { get; private set; }
        public static string UserFolder { get; private set; }

        protected override void OnStartup( StartupEventArgs e ) {
            RootFolder = System.IO.Directory.GetCurrentDirectory();
            UserFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            System.IO.Directory.CreateDirectory(UserFolder + "\\RatChat");
            System.IO.Directory.CreateDirectory(UserFolder + "\\RatChat\\Skins");

            UserFolder += "\\RatChat";

            base.OnStartup(e);
        }
    }
}
