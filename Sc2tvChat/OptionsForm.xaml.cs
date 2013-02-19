using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RatChat {
    /// <summary>
    /// Interaction logic for OptionsForm.xaml
    /// </summary>
    public partial class OptionsForm : Window {
        public OptionsForm() {
            InitializeComponent();
        }

        public string[] Skins {
            get {
                List<string> a = new List<string>(
                        from b in Directory.GetFiles(App.RootFolder + "/Skins/", "*.xaml")
                        orderby b
                        select System.IO.Path.GetFileNameWithoutExtension(b));

                a.Insert(0, "По умолчанию (встроенный)");

                return a.ToArray();
            }
        }

        private void start_Click_1( object sender, RoutedEventArgs e ) {
          
        }

       
        private void Window_Closed_1( object sender, EventArgs e ) {
            Properties.Settings.Default.Save();
        }
    }
}
