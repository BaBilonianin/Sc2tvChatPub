using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Sc2tvChat {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public static string RootFolder { get; private set; }

        protected override void OnStartup( StartupEventArgs e ) {
            RootFolder = System.IO.Directory.GetCurrentDirectory();
            base.OnStartup(e);
        }
    }
}
