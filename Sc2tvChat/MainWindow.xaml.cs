using RatChat.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

            //this.plugin = XSplit.Wpf.TimedBroadcasterPlugin.CreateInstance(
            //   "9E1AB52C-6EC5-45F3-930F-1C91150C8425", this, 
            //   (int)this.Width, (int)this.Height - 25, 50);

            //if (this.plugin != null) {
            //    this.plugin.StartTimer();
            //    //this.Chats.PreviewMouseDown += this.chats_PreviewMouseDown;
            //    //this.Chats.PreviewMouseMove += this.chats_PreviewMouseMove;
            //}

            ChatSourceManager = new RatChat.ChatSourceManager();
            achievCP.Content = ChatSourceManager.Achievment;
            Properties.Settings.Default.SettingsSaving += PropertySettingsSavingEventHandler;
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
            Chats.DataContext = ChatSourceManager.Chats;

            //testTimer = new DispatcherTimer();
            //testTimer.Interval = TimeSpan.FromMilliseconds(50);
            //testTimer.Tick += testTimer_Tick;
            //testTimer.Start();
        }

        //void testTimer_Tick( object sender, EventArgs e ) {
        //    RenderVisual();
        //}

        //int test = 0;

        //DispatcherTimer testTimer;

        //void RenderVisual() {
        //    RenderTargetBitmap targetBitmap = new RenderTargetBitmap(
        //        (int)ActualWidth,
        //        (int)ActualHeight,
        //        96d, 96d,
        //        PixelFormats.Default);

        //    targetBitmap.Render(this);
        //    BmpBitmapEncoder encoder = new BmpBitmapEncoder();
        //    encoder.Frames.Add(BitmapFrame.Create(targetBitmap));


        //    using (FileStream fs = File.Open(
        //        string.Format("x:\\test\\image_{0:0000}.bmp", test++), FileMode.OpenOrCreate)) {
        //        encoder.Save(fs);
        //    }
        //}

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

                                if (_currentSkin.StartsWith("user-")) {
                                    skin.Source = new Uri(App.UserFolder + "/Skins/" + _currentSkin + ".xaml", UriKind.RelativeOrAbsolute);
                                } else {
                                    skin.Source = new Uri(App.RootFolder + "/Skins/" + _currentSkin + ".xaml", UriKind.RelativeOrAbsolute);
                                }
                                Application.Current.Resources.MergedDictionaries.Remove(skin);
                            }
                        }

                        _currentSkin = value;

                        if (_currentSkin == "По умолчанию (встроенный)") {
                            SetDefaultSkin();
                        } else
                            if (!string.IsNullOrEmpty(_currentSkin)) {
                                ResourceDictionary skin = new ResourceDictionary();
                                if (_currentSkin.StartsWith("user-")) {
                                    skin.Source = new Uri(App.UserFolder + "/Skins/" + _currentSkin + ".xaml", UriKind.RelativeOrAbsolute);
                                } else {
                                    skin.Source = new Uri(App.RootFolder + "/Skins/" + _currentSkin + ".xaml", UriKind.RelativeOrAbsolute);
                                } 
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



        #region XBS (XSplit) Dragging Support

        private Point startPoint;

        private void chats_PreviewMouseDown( object sender, MouseButtonEventArgs e ) {
            //this.startPoint = e.GetPosition(null);
        }

        private void chats_PreviewMouseMove( object sender, MouseEventArgs e ) {
            //var senderObj = sender as MainWindow;

            //if (senderObj == null) {
            //    // This shouldn't happen.
            //    return;
            //}

            //// Get the current mouse position
            //Point mousePos = e.GetPosition(null);
            //Vector diff = this.startPoint - mousePos;

            //if (e.LeftButton == MouseButtonState.Pressed &&
            //    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
            //    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)) {
            //    string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XSplit", "chat.xbs");

            //    if (File.Exists(path) == false) {
            //        return;
            //    }

            //    var strCol = new StringCollection { path };

            //    var o = new DataObject(DataFormats.FileDrop, strCol);
            //    o.SetFileDropList(strCol);
            //    DragDrop.DoDragDrop(senderObj, o, DragDropEffects.Copy);
            //}
        }


        public XSplit.Wpf.TimedBroadcasterPlugin plugin { get; set; }

        #endregion

    }
}
