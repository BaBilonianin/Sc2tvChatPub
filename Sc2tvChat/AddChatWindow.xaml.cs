using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for AddChatWindow.xaml
    /// </summary>
    public partial class AddChatWindow : Window {
        public AddChatWindow() {
            InitializeComponent();
        }

        public static string SelectChatSource( ChatSourceManager Manager ) {
            AddChatWindow dlg = new AddChatWindow();
            dlg.ChatSources = (from b in Manager.Sources.Keys
                               select b).ToArray();

            bool? ret = dlg.ShowDialog();

            if (ret.HasValue && ret.Value)
                return dlg.ChatSource;

            return null;
        }

        public readonly static DependencyProperty ChatSourcesProperty = DependencyProperty.Register(
           "ChatSources", typeof(string[]), typeof(AddChatWindow), new PropertyMetadata());

        public string[] ChatSources {
            get { return (string[])GetValue(ChatSourcesProperty); }
            set { SetValue(ChatSourcesProperty, value); }
        }

        public readonly static DependencyProperty ChatSourceProperty = DependencyProperty.Register(
            "ChatSource", typeof(string), typeof(AddChatWindow), new PropertyMetadata(""));

        public string ChatSource {
            get { return (string)GetValue(ChatSourceProperty); }
            set { SetValue(ChatSourceProperty, value); }
        }

        private void Cansel_Click_1( object sender, RoutedEventArgs e ) {
            this.DialogResult = false;
        }

        private void Commit_Click_1( object sender, RoutedEventArgs e ) {
            if (string.IsNullOrEmpty(ChatSource))
                return;
            this.DialogResult = true;
        }



        ////
    }
}
