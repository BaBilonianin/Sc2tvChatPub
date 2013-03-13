using RatChat.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
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
using System.Windows.Interop;
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

            Me = this;
            _hookID = SetHook(_proc);

            ratChatCaption.Text = "RatChat v" + GetRunningVersion();

            ChatSourceManager = new RatChat.ChatSourceManager();
            achievCP.Content = ChatSourceManager.Achievment;
            Properties.Settings.Default.SettingsSaving += PropertySettingsSavingEventHandler;
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
            Chats.DataContext = ChatSourceManager.Chats;

           // Plugin = XSplit.Wpf.TimedBroadcasterPlugin.CreateInstance("3A1184B5-19A5-4384-BF12-8BB48A3C4111", this);
        }

        #region Click throught
        static MainWindow Me;

        private static IntPtr SetHook( LowLevelKeyboardProc proc ) {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule) {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam );

        private static IntPtr HookCallback( int nCode, IntPtr wParam, IntPtr lParam ) {
            if (Properties.Settings.Default.allowTransClick) {
                if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) {
                    int vkCode = Marshal.ReadInt32(lParam);
                    if (vkCode == 162) {
                        Me.setUnTransparent();
                    }
                } else
                    if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP) {
                        int vkCode = Marshal.ReadInt32(lParam);
                        if (vkCode == 162) {
                            Me.setTransparent();
                        }
                    }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx( int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId );

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx( IntPtr hhk );

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx( IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle( string lpModuleName );

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong( IntPtr hwnd, int index );

        [DllImport("user32.dll")]
        public static extern int SetWindowLong( IntPtr hwnd, int index, int newStyle );

        int _NormalWindowStyle;

        protected override void OnSourceInitialized( EventArgs e ) {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            _NormalWindowStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            base.OnSourceInitialized(e);
        }

        void setTransparent() {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_EXSTYLE, _NormalWindowStyle | WS_EX_TRANSPARENT);

            ratChatCaption.Text = "RatChat* v" + GetRunningVersion();
        }

        void setUnTransparent() {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_EXSTYLE, _NormalWindowStyle );
            ratChatCaption.Text = "RatChat v" + GetRunningVersion();
        }

        public bool ClickTransparent {
            get { return (bool)GetValue(ClickTransparentProperty); }
            set { SetValue(ClickTransparentProperty, value); }
        }

        public static DependencyProperty ClickTransparentProperty = DependencyProperty.Register(
            "ClickTransparent", typeof(bool), typeof(MainWindow), new PropertyMetadata(false, ClickTransparentPropertyChangedCallback));

        static void ClickTransparentPropertyChangedCallback( DependencyObject d, DependencyPropertyChangedEventArgs e ) {
            bool NewValue = (bool)e.NewValue;

            if (NewValue) {
                ((MainWindow)d).setTransparent();
            } else {
                ((MainWindow)d).setUnTransparent();
            }
        }
        #endregion


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

                               // skin.Add
                                UpdateSupports(skin);
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

        void UpdateSupports( ResourceDictionary skin ) {
            if (skin.Contains("Supports"))
                CurrentSupports = (string)skin["Supports"];
            else
                CurrentSupports = "";

            // CheckSupports
            if (options != null)
                options.Supports = CurrentSupports;
        }

        string CurrentSupports;
   

        private void SetDefaultSkin() {
            while (Application.Current.Resources.MergedDictionaries.Count>1)
                Application.Current.Resources.MergedDictionaries.RemoveAt(1);

            ResourceDictionary skin = new ResourceDictionary();
            skin.Source = new Uri("Skins/DefaultSkin.xaml", UriKind.RelativeOrAbsolute);
            UpdateSupports(skin);
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
            if (e.PropertyName == "allowTransClick") {
                if (Properties.Settings.Default.allowTransClick)
                    setTransparent();
                else
                    setUnTransparent();
            }
        }
        
        private void Window_Loaded_1( object sender, RoutedEventArgs e ) {
            CurrentSkin = Properties.Settings.Default.currentSkin;
            ChatSourceManager.Initialize( Chats, Properties.Settings.Default.chatConfigs);
            ChatSourceManager.SmilesDataDase.SetCustoms(Properties.Settings.Default.customSmiles);
        }     

        private void Window_Closing_1( object sender, System.ComponentModel.CancelEventArgs e ) {
            if (options != null)
                options.Close();
            Properties.Settings.Default.Save();
            UnhookWindowsHookEx(_hookID);
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

        OptionsForm options;

        private void Button_Click_1( object sender, RoutedEventArgs e ) {
            if (options != null)
                options.Close();
            options = new OptionsForm();
            options.Supports = CurrentSupports;
            options.Closed += options_Closed;
            options.Show();
        }

        void options_Closed( object sender, EventArgs e ) {
            options = null;
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
            //string path = System.IO.Path.Combine(App.RootFolder, "XSplit", "chat.xbs");
            //if (!File.Exists(path))
            //    return;
            //var strCol = new StringCollection { path };
            //var o = new DataObject(DataFormats.FileDrop, strCol);
            //o.SetFileDropList(strCol);
            //DragDrop.DoDragDrop(this, o, DragDropEffects.Copy);
        }


       // public XSplit.Wpf.TimedBroadcasterPlugin Plugin { get; private set; }

        #endregion

    }
}
