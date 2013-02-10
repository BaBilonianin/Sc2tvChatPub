using Newtonsoft.Json;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Sc2tvChat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            SmilesDataDase = new SmilesDataDase();
            ChatDataSource = new ObservableCollection<VisualMessage>();
            Chat.DataContext = ChatDataSource;
            CasterAchievment = new Achievment();

            HeaderCP.Content = CasterAchievment;

            next = new DispatcherTimer();
            next.Interval = TimeSpan.FromSeconds(1);
            next.Tick += next_Tick;

            halfSecondTimer = new DispatcherTimer();
            halfSecondTimer.Interval = TimeSpan.FromSeconds(0.5);
            halfSecondTimer.Tick += halfMinuteTimer_Tick;

            pollCtrl.Visibility = System.Windows.Visibility.Hidden;
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

       


        bool NalanDetected = false;
        string _currentSkin = "DefaultSkin";
        DispatcherTimer next, halfSecondTimer;
        SmilesDataDase SmilesDataDase;
        ObservableCollection<VisualMessage> ChatDataSource;

        public Achievment CasterAchievment { get; protected set; }
        public string CurrentSkin {
            get { return _currentSkin; }
            set {
                if (_currentSkin != value) {
                    ResourceDictionary skin = new ResourceDictionary();
                    skin.Source = new Uri("Skins/" + _currentSkin + ".xaml", UriKind.Relative);
                    Application.Current.Resources.MergedDictionaries.Remove(skin);
                    _currentSkin = value;
                    skin.Source = new Uri("Skins/" + _currentSkin + ".xaml", UriKind.Relative);
                    Application.Current.Resources.MergedDictionaries.Add(skin);
                }
            }
        }

        void Default_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e ) {
            if (e.PropertyName == "streamerID") {
                CasterAchievment.Clear();
                ChatDataSource.Clear();
                pollCtrl.CancelPoll();
                pollCtrl.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        
        #region Download SC2TV.ru chat messages
        void next_Tick( object sender, EventArgs e ) {
            if (Properties.Settings.Default.streamerID != 0) {
                next.Stop();
                LoadChat(Properties.Settings.Default.streamerID);
            }
        }

        VisualMessage GetVisual( Message newOwner ) {
            for (int j = 0; j < ChatDataSource.Count; ++j)
                if (ChatDataSource[j].Data.Id == newOwner.Id)
                    return ChatDataSource[j];
            return null;
        }

        void LoadChat( int ChannelId ) {
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(( a, b ) => {
                if (b.Error == null) {
                    Messages messages = JsonConvert.DeserializeObject<Messages>(b.Result);
                    if (messages != null)
                        UpdateMessages(messages);
                } else {
                    // Кстати, тут можно нарисовать ошибку сети.
                }
                next.Start();
            });
            wc.DownloadStringAsync(new Uri("http://chat.sc2tv.ru/memfs/channel-" + ChannelId + ".json"));
        }

        private void UpdateMessages( Messages msgs ) {
            NalanDetected = false;

            for (int j = 0; j < ChatDataSource.Count; ++j) {
                ChatDataSource[j].NeedDelete = true;
                if (ChatDataSource[j].Data.Name == "Nalan") {
                    NalanDetected = true;
                }
            }

            var msg = (from b in msgs.Content
                       orderby b.Date
                       select b).ToArray();

            for (int j = 0; j < msg.Length; ++j) {
                VisualMessage rm = GetVisual(msg[j]);
                if (rm == null) {
                    rm = new VisualMessage(SmilesDataDase, msg[j]);

                    OnTextMessage(rm);
                    ChatDataSource.Add(rm);
                }
                rm.NeedDelete = false;
            }

            int i = 0;
            while (i < ChatDataSource.Count)
                if (ChatDataSource[i].NeedDelete)
                    ChatDataSource.RemoveAt(i);
                else
                    ++i;

            if (NalanDetected)
                CasterAchievment.Temperature = -273.15;

            // 10994 - Самая глубокая точка Марианской впадины — Бездна Челленджера
        }
        #endregion

        private void OnTextMessage( VisualMessage Msg ) {
            
            CasterAchievment.PekaCount += Msg.Data.Text.CountSubstring(":s:peka:");

            // TODO: Phil9l: Охламон, может стоит проверять, вдруг фейспалм ставят кому-то из зрителей?
            if (string.IsNullOrEmpty(Msg.TalkTo) || Msg.TalkTo == Properties.Settings.Default.streamerNick) {
            //double div = Math.Exp(CasterAchievment.Temperature);
               CasterAchievment.Temperature += Msg.Data.Text.CountSubstring(":s:fire:") * 10.0;

                CasterAchievment.Depth = CasterAchievment.Depth
                    + Msg.Data.Text.CountSubstring(":s:fyeah:") * 10
                    + Msg.Data.Text.CountSubstring(":s:notbad:") * 5
                    + Msg.Data.Text.CountSubstring(":s:bm:") * 5

                    - Msg.Data.Text.CountSubstring(":s:fp:") * 10
                    - Msg.Data.Text.CountSubstring(":s:crab:") * 50
                    - Msg.Data.Text.CountSubstring(":s:mimo:") * 20
                    - Msg.Data.Text.CountSubstring(":s:fpl:") * 10
                    - Msg.Data.Text.CountSubstring(":s:br:") * 5;
            }
        }

        void halfMinuteTimer_Tick( object sender, EventArgs e ) {
            CasterAchievment.Temperature -= 0.005;
        }

        #region Startup
        private void Window_Loaded_1( object sender, RoutedEventArgs e ) {
            next.Start();
            halfSecondTimer.Start();
            if (Properties.Settings.Default.streamerID == 0) {
                OptionsForm fid = new OptionsForm();
                fid.ShowDialog();
            }
        }
        #endregion

        #region Show options
        private void Button_Click_1( object sender, RoutedEventArgs e ) {
            OptionsForm fid = new OptionsForm();
            fid.ShowDialog();
        }
        #endregion

        #region Window handles
        private void Thumb_DragDelta_1( object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e ) {
            this.Left += e.HorizontalChange;
            this.Top += e.VerticalChange;
        }
        
        private void Button_Click_2( object sender, RoutedEventArgs e ) {
            Close();
        }
        #endregion
    }
}
