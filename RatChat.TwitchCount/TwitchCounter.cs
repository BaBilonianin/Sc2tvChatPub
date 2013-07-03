using RatChat.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RatChat.TwitchCount {
    [ChatName("График кол-ва зрителей")]
    [ConfigValue(".TWITCHTVCOUNT.StreamerNick", "", "Ник на twitch:", false)]
    [ConfigValue(".CYBERTVCOUNT.StreamerNick", "", "Ник на cybergame:", false)]
    [ConfigValue(".COUNT.Seconds", "10", "Период опроса (сек.):", false)]
    public class TwitchCounter : IChatSource, INotifyPropertyChanged, IChatListener {
        public TwitchCounter() {
            History = new ObservableCollection<KeyValuePair<DateTime, int>>();
            SmilesUri = new Dictionary<string, string>();
            CountTimer = new DispatcherTimer();
            CountTimer.Tick += CountTimer_Tick;
            CountTimer.Interval = TimeSpan.FromSeconds(1);
        }

        // ?({"average_bitrate":0,"streams_count":0,"viewers_count":0})
        int PollSeconds = 1;

        int CurPollSeconds = 0;

        enum CounterSource {
            Twitch,
            Cybergame
        }

        CounterSource _CounterSource = CounterSource.Cybergame;

        //cap

        void CountTimer_Tick( object sender, EventArgs e ) {
            CurPollSeconds--;
            if (CurPollSeconds < 0) {
                CurPollSeconds = PollSeconds;

                CountTimer.Stop();
                if (!string.IsNullOrEmpty(StreamerNick)) {

                    switch (_CounterSource) {
                        case CounterSource.Twitch:
                            GetTwitchCount();
                            break;

                        case CounterSource.Cybergame:
                            GetCyberCount();
                            break;
                    }
                } else {
                    CountTimer.Start();
                }
            }
        }

        private void GetCyberCount() {
            Regex rx = new Regex("viewers.*?\\:\\\"(.*?)\\\"");
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(( b, a ) => {
                if (a.Error == null) {
                    Match m = rx.Match(a.Result);
                    if (m.Success) {
                        Header = "[Cyber] Зрителей: " + m.Groups[1].Value + ", " + StreamerNick;
                        int h;
                        if (int.TryParse(m.Groups[1].Value, out h)) {
                            History.Add(new KeyValuePair<DateTime, int>(DateTime.Now, h));
                            if (History.Count > 10)
                                History.RemoveAt(0);
                        }
                    }
                }
                CountTimer.Start();
            });
            wc.DownloadStringAsync(new Uri(
                string.Format("http://api.cybergame.tv/w/streams.php?channel={0}", StreamerNick), UriKind.RelativeOrAbsolute));
        }

        private void GetTwitchCount() {
            Regex rx = new Regex("viewers_count.*?(\\d*?)\\,");
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(( b, a ) => {
                if (a.Error == null) {
                    //{"average_bitrate": 0, "viewers_count": 272, "streams_count": 1}

                    Match m = rx.Match(a.Result);
                    if (m.Success) {
                        Header = "[Twitch] Зрителей: " + m.Groups[1].Value + ", " + StreamerNick;
                        int h;
                        if (int.TryParse(m.Groups[1].Value, out h)) {
                            History.Add(new KeyValuePair<DateTime, int>(DateTime.Now, h));
                            if (History.Count > 10)
                                History.RemoveAt(0);
                        }
                    }
                }
                CountTimer.Start();
            });
            wc.DownloadStringAsync(new Uri(
                             //http://api.justin.tv/api/stream/summary.json?channel=Knjazevdesu&jsonp=
                string.Format("http://api.justin.tv/api/stream/summary.json?channel={0}&jsonp=", StreamerNick), UriKind.RelativeOrAbsolute));
        }

        DispatcherTimer CountTimer;

        #region Common
        public string Copyright {
            get { return "Oxlamon © 2013"; }
        }

        public string Description {
            get { return "Счетчик кол-ва зрителей"; }
        }

        public string ConfigPrefix { get; set; }

        public string StreamerNick { get; set; }

        public Dictionary<string, string> SmilesUri { get; private set; }

        public event Core.OnNewMessagesArrivedDelegate OnNewMessagesArrived;
        #endregion

        #region INotifyPropertyChanged
        protected void FireChange( string PropertyName ) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        string _Header = "";
        public string Header {
            get { return _Header; }
            set {
                if (string.Compare(_Header, value) != 0) {
                    _Header = value;
                    FireChange("Header");
                }
            }
        }

        public TimeSpan? PollExpire { get; set; }

        ObservableCollection<KeyValuePair<DateTime, int>> History;

        public void BeginWork() {
            CountTimer.Start();
        }

        public void EndWork() {
            CountTimer.Stop();
            History.Clear();
        }

        public void OnLoad( Core.ConfigStorage Config ) {
            OnConfigApply(Config);
        }

        public void OnConfigApply( Core.ConfigStorage Config ) {
            PollSeconds = 10;
            History.Clear();
            string sec = Config.GetDefault(ConfigPrefix + ".COUNT.Seconds", "10");
            if (int.TryParse(sec, out PollSeconds)) {
            } else {
                PollSeconds = 10;
            }

            History.Clear();
            StreamerNick = Config.GetDefault(ConfigPrefix + ".TWITCHTVCOUNT.StreamerNick", "");
            if (!string.IsNullOrEmpty(StreamerNick)) {
                _CounterSource = CounterSource.Twitch;
                Header = "Twitch connecting, " + StreamerNick;
                return;
            }

            StreamerNick = Config.GetDefault(ConfigPrefix + ".CYBERTVCOUNT.StreamerNick", "");
            if (!string.IsNullOrEmpty(StreamerNick)) {
                _CounterSource = CounterSource.Cybergame;
                Header = "Cybergame connecting, " + StreamerNick;
                return;
            }

          
        }

        public System.Windows.Controls.UserControl CreateCustomView() {
            HistoryControl hc = new HistoryControl();
            hc.DataContext = History;
            return hc;
        }

        public void OnNewMessageReceived( List<ChatMessage> NewMessages ) {
           
        }
    }
}
