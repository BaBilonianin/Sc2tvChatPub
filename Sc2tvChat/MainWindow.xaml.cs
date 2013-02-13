using RatChat.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RatChat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            ChatSourceManager = new RatChat.ChatSourceManager();
            Properties.Settings.Default.SettingsSaving += PropertySettingsSavingEventHandler;
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
            Chats.DataContext = ChatSourceManager.Chats;
        }

        void PropertySettingsSavingEventHandler( object sender, System.ComponentModel.CancelEventArgs e ) {
            ChatSourceManager.StoreChats(Chats);
            Properties.Settings.Default.chatConfigs = ChatSourceManager.ChatConfigStorage.Save();
        }

        string _currentSkin = "";
        
        ChatSourceManager ChatSourceManager;

        public string CurrentSkin {
            get { return _currentSkin; }
            set {
                if (_currentSkin != value) {
                    if (!string.IsNullOrEmpty(_currentSkin)) {
                        ResourceDictionary skin = new ResourceDictionary();
                        skin.Source = new Uri(App.RootFolder + "/Skins/" + _currentSkin + ".xaml", UriKind.RelativeOrAbsolute);
                        Application.Current.Resources.MergedDictionaries.Remove(skin);
                    }
                    _currentSkin = value;

                    if (!string.IsNullOrEmpty(_currentSkin)) {
                        ResourceDictionary skin = new ResourceDictionary();
                        skin.Source = new Uri(App.RootFolder + "/Skins/" + _currentSkin + ".xaml", UriKind.RelativeOrAbsolute);
                        Application.Current.Resources.MergedDictionaries.Add(skin);
                    }
                }
            }
        }

        void Default_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e ) {
            if (e.PropertyName == "currentSkin") {
                CurrentSkin = Properties.Settings.Default.currentSkin;
            }
        }
        
        private void Window_Loaded_1( object sender, RoutedEventArgs e ) {
            CurrentSkin = Properties.Settings.Default.currentSkin;
            ChatSourceManager.Initialize( Chats, Properties.Settings.Default.chatConfigs);
        }     

        private void Window_Closing_1( object sender, System.ComponentModel.CancelEventArgs e ) {
            Properties.Settings.Default.Save();
        }

        private void AddChat_Click( object sender, RoutedEventArgs e ) {
            string chatSourceName = AddChatWindow.SelectChatSource(ChatSourceManager);

            if (!string.IsNullOrEmpty(chatSourceName)) {
                // Show config window
                string VisualId = Guid.NewGuid().ToString();
                ChatSourceManager.CreateChat(VisualId, chatSourceName);
            }
        }

        private void Thumb_DragDelta_1( object sender, DragDeltaEventArgs e ) {
            this.Left += e.HorizontalChange;
            this.Top += e.VerticalChange;
        }

        private void Button_Click_2( object sender, RoutedEventArgs e ) {
            Close();
        }

        private void Button_Click_1( object sender, RoutedEventArgs e ) {
            OptionsForm fid = new OptionsForm();
            fid.Show();
        }
    }
}
