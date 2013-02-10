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

namespace Sc2tvChat {
    /// <summary>
    /// Interaction logic for OptionsForm.xaml
    /// </summary>
    public partial class OptionsForm : Window {
        public OptionsForm() {
            InitializeComponent();
        }

        public string[] Skins {
            get {
                return (from b in Directory.GetFiles(App.RootFolder + "/Skins/", "*.xaml")
                        orderby b
                        select System.IO.Path.GetFileNameWithoutExtension(b)).ToArray();
            }
        }

        private void start_Click_1( object sender, RoutedEventArgs e ) {
            streamerNick.IsEnabled = false;
            start.IsEnabled = false;

            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(( a, b ) => {
                start.IsEnabled = true;
                streamerNick.IsEnabled = true;

                if (b.Error == null) {
                    Regex rx = new Regex("\\<link.*?\\\"canonical\\\".*?href=\\\"http://sc2tv.ru/node/(.*?)\\\"");
                    Match m = rx.Match(b.Result);
                    if (m.Success) {
                        rx= new Regex(".*?author\\\".*?title.*?\\>(.*?)\\<" );


                        Properties.Settings.Default.streamerID = int.Parse(m.Groups[1].Value);
                        Properties.Settings.Default.streamerNick = rx.Match(b.Result).Groups[1].Value;
                      //  this.DialogResult = true;
                    } else {
                        MessageBox.Show("Чат стримера не найден.");
                    }
                } else {
                    MessageBox.Show("Сетевая ошибка.");
                }
            });

            wc.DownloadStringAsync(new Uri(streamerNick.Text, UriKind.RelativeOrAbsolute));
        }

       
        private void Window_Closed_1( object sender, EventArgs e ) {
            Properties.Settings.Default.Save();
        }
    }
}
