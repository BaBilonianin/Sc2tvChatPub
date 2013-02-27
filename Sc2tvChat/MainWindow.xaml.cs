using RatChat.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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

            ratChatCaption.Text = "RatChat v" + GetRunningVersion();

            ChatSourceManager = new RatChat.ChatSourceManager();
            achievCP.Content = ChatSourceManager.Achievment;
            Properties.Settings.Default.SettingsSaving += PropertySettingsSavingEventHandler;
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
            Chats.DataContext = ChatSourceManager.Chats;
        }

        private Version GetRunningVersion() {
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            else
                return Assembly.GetExecutingAssembly().GetName().Version;
        }

        void PropertySettingsSavingEventHandler( object sender, System.ComponentModel.CancelEventArgs e ) {
            ChatSourceManager.StoreChats(Chats);
            Properties.Settings.Default.chatConfigs = ChatSourceManager.ChatConfigStorage.Save();
        }

        string _currentSkin = null;
        
        ChatSourceManager ChatSourceManager;

        public string CurrentSkin {
            get { return _currentSkin; }
            set {
                try {
                    if (_currentSkin != value) {
                        if (_currentSkin != "По умолчанию (встроенный)") {
                            if (!string.IsNullOrEmpty(_currentSkin)) {
                                ResourceDictionary skin = new ResourceDictionary();
                                skin.Source = new Uri(App.RootFolder + "/Skins/" + _currentSkin + ".xaml", UriKind.RelativeOrAbsolute);
                                Application.Current.Resources.MergedDictionaries.Remove(skin);
                            }
                        }

                        _currentSkin = value;

                        if (_currentSkin == "По умолчанию (встроенный)") {
                            SetDefaultSkin();
                        } else
                            if (!string.IsNullOrEmpty(_currentSkin)) {
                                ResourceDictionary skin = new ResourceDictionary();
                                skin.Source = new Uri(App.RootFolder + "/Skins/" + _currentSkin + ".xaml", UriKind.RelativeOrAbsolute);
                                Application.Current.Resources.MergedDictionaries.Add(skin);
                            } else {
                                SetDefaultSkin();
                            }
                    }
                } catch (Exception e) {
                    MessageBox.Show("Ошибка в скине: " + e.Message);
                    SetDefaultSkin();
                }
            }
        }

        private void SetDefaultSkin() {
            while (Application.Current.Resources.MergedDictionaries.Count>1)
                Application.Current.Resources.MergedDictionaries.RemoveAt(1);

            ResourceDictionary skin = new ResourceDictionary();
            skin.Source = new Uri("Skins/DefaultSkin.xaml", UriKind.RelativeOrAbsolute);
            Application.Current.Resources.MergedDictionaries.Add(skin);

            Properties.Settings.Default.PropertyChanged -= Default_PropertyChanged;
            Properties.Settings.Default.currentSkin = "По умолчанию (встроенный)";
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

        void Default_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e ) {
            if (e.PropertyName == "currentSkin") {
                CurrentSkin = Properties.Settings.Default.currentSkin;
            }
            if (e.PropertyName == "customSmiles") {
                ChatSourceManager.SmilesDataDase.SetCustoms(Properties.Settings.Default.customSmiles);
            }
        }
        
        private void Window_Loaded_1( object sender, RoutedEventArgs e ) {
            CurrentSkin = Properties.Settings.Default.currentSkin;
            ChatSourceManager.Initialize( Chats, Properties.Settings.Default.chatConfigs);
            ChatSourceManager.SmilesDataDase.SetCustoms(Properties.Settings.Default.customSmiles);
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
