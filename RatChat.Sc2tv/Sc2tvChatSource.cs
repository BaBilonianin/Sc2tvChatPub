using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using RatChat.Core;
using System.Text.RegularExpressions;
using System.Threading;
using System.ComponentModel;
using System.Windows;
using System.Web;

namespace RatChat.Sc2tv {
    [ChatName("Чат для http://sc2tv.ru")]
    [ConfigValue(".SC2TVCHAT.StreamerURI", "http://sc2tv.ru/content/oxlamonschannel", "Адрес страницы стримера sc2tv.ru:", false)]
    public class Sc2tvChatSource : RatChat.Core.IChatSource, INotifyPropertyChanged {
        public const string UpdateSmilesUri = "http://chat.sc2tv.ru/js/smiles.js";

        DispatcherTimer next;
        string _ChannelUri = "";
        int _StreamerID = 0;
        List<Message> LoadedMessages;
        SmilesDataDase Smiles;
        Regex ExtractSmile = new Regex("\\:s\\:\\w\\w.*?\\:");

        public Sc2tvChatSource() {
            LoadedMessages = new List<Message>();
            Smiles = new SmilesDataDase();
            SmilesUri = new Dictionary<string, string>();

            //WebClient smile = new WebClient();
            //smile.DownloadString();
        }

        void next_Tick( object sender, EventArgs e ) {
            if (_StreamerID != 0) {
                next.Stop();
                LoadChat(_StreamerID);
            }
        }

        void LoadChat( int ChannelId ) {
            WebClient wc = new WebClient();
            wc.Headers.Add("user-agent", "RatChat");
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(( a, b ) => {
                if (b.Error == null) {
                    Messages messages = JsonConvert.DeserializeObject<Messages>(b.Result);
                    if (messages != null)
                        UpdateMessages(messages);
                } else {
                    // Кстати, тут можно нарисовать ошибку сети.
                }
                if (next != null)
                    next.Start();
            });
            wc.DownloadStringAsync(new Uri("http://chat.sc2tv.ru/memfs/channel-" + ChannelId + ".json"));
        }

        Message GetById( int id ) {
            for (int j = 0; j < LoadedMessages.Count; ++j)
                if (LoadedMessages[j].Id == id)
                    return LoadedMessages[j];
            return null;
        }

        private void UpdateMessages( Messages msgs ) {
            for (int j = 0; j < LoadedMessages.Count; ++j)
                LoadedMessages[j].NeedToDelete--;

            var msg = (from b in msgs.Content
                       orderby b.Date
                       select b).ToArray();

            List<Message> NewMessage = new List<Message>();
            //////////
            for (int j = 0; j < msg.Length; ++j) {
                Message m = GetById(msg[j].Id);
                if (m == null) {
                    LoadedMessages.Add(msg[j]);

                    //Match mmm = ExtractSmile.Match(msg[j].Text);
                    //if (mmm.Success) {
                    //}

                    msg[j].Text = HttpUtility.HtmlDecode(ExtractSmile.Replace(msg[j].Text, new MatchEvaluator(( match ) => {
                        return " " + match.Value + " ";
                    })).Trim());

                    NewMessage.Add(msg[j]);
                    msg[j].NeedToDelete = 60 * 5;
                } else {
                    m.NeedToDelete = 60 * 5;
                }
            }
            ///////////

            int i = 0;
            while (i < LoadedMessages.Count)
                if (LoadedMessages[i].NeedToDelete < 0)
                    LoadedMessages.RemoveAt(i);
                else
                    ++i;

            ////////////
            if (NewMessage.Count > 0) {
                //CasterAchivment.Temperature -= NewMessage.Count * 0.01;

                //foreach (var m in NewMessage)
                //    OnNewMessage(m);

                if (OnNewMessagesArrived != null) {
                    OnNewMessagesArrived((from b in NewMessage
                                          orderby b.Date
                                          select new ChatMessage() {
                                              Date = b.Date,
                                              Name = b.Name,
                                              Text = b.Text
                                          }).ToList());
                }
            }
        }

        //private void OnNewMessage( Message NewMessage ) {
        //    CasterAchivment.PekaCount += NewMessage.Text.CountSubstring(":s:peka:");

        //    CasterAchivment.Temperature += NewMessage.Text.CountSubstring(":s:fire:") * 5;

        //    CasterAchivment.Depth = CasterAchivment.Depth
        //        - NewMessage.Text.CountSubstring(":s:fp:") * 4
        //        - NewMessage.Text.CountSubstring(":s:crab:") * 5
        //        - NewMessage.Text.CountSubstring(":s:fpl:") * 2
        //        - NewMessage.Text.CountSubstring(":s:mimo:")

        //        + NewMessage.Text.CountSubstring(":s:fyeah:") * 5
        //        + NewMessage.Text.CountSubstring(":s:notbad:") * 3
        //        + NewMessage.Text.CountSubstring(":s:bm:") * 2;
        //}

        public string Copyright {
            get { return "Oxlamon © 2013"; }
        }

        public string Description {
            get { return "Чаты с http://sc2tv.ru"; }
        }

        string _Header = "http://sc2tv.ru";
        public string Header {
            get { return _Header; }
            set {
                if (string.Compare(_Header, value) != 0) {
                    _Header = value;
                    FireChange("Header");
                }
            }
        }

        public string StreamerNick { get; set; }

        public void OnLoad( ConfigStorage Config ) {
            StreamerNick = Config.GetDefault(ConfigPrefix + ".SC2TVCHAT.StreamerNick", "");
            _StreamerID = (int)Config.GetDefault(ConfigPrefix + ".SC2TVCHAT.StreamerID", 0);
            _ChannelUri = Config.GetDefault(ConfigPrefix + ".SC2TVCHAT.StreamerURI", "http://sc2tv.ru/content/oxlamonschannel");
            LoadedMessages.Clear();
            UpdateHeader();
            UpdateSmiles();
        }

        private void UpdateSmiles() {
            try {
                WebClient wc = new WebClient();
                string js = wc.DownloadString(UpdateSmilesUri);

                SmilesUri.Clear();
                Regex smiles = new Regex("\\'(.*?)\\'.*?\\'(.*?)\\',.*?\\}", RegexOptions.Multiline);

                foreach (Match m in smiles.Matches(js))
                    SmilesUri[":s" + m.Groups[1].Value + " "] = "http://chat.sc2tv.ru/img/" + m.Groups[2].Value;
            } catch {
            }
        }

        private void UpdateHeader(){
            if (string.IsNullOrEmpty(StreamerNick)) {
                Header = "http://sc2tv.ru, Чат не подключен.";
            } else {
                Header = "http://sc2tv.ru, " + StreamerNick;
            }
        }

        public void OnConfigApply( ConfigStorage Config ) {
            _ChannelUri = (string)Config.GetDefault(ConfigPrefix + ".SC2TVCHAT.StreamerURI", "http://sc2tv.ru/content/oxlamonschannel");

            ManualResetEvent mre = new ManualResetEvent(false);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            try {
                string Result = wc.DownloadString(new Uri(_ChannelUri, UriKind.RelativeOrAbsolute));
                Regex rx = new Regex("\\<link.*?\\\"canonical\\\".*?href=\\\"http://sc2tv.ru/node/(.*?)\\\"");
                Match m = rx.Match(Result);
                if (m.Success) {
                    rx = new Regex(".*?author\\\".*?title.*?\\>(.*?)\\<");

                    Config[ConfigPrefix + ".SC2TVCHAT.StreamerID"] = _StreamerID = int.Parse(m.Groups[1].Value);

                    m = rx.Match(Result);
                    Config[ConfigPrefix + ".SC2TVCHAT.StreamerNick"] = StreamerNick = m.Groups[1].Value;
                } else {
                    Config[ConfigPrefix + ".SC2TVCHAT.StreamerID"] = _StreamerID = 0;
                    Config[ConfigPrefix + ".SC2TVCHAT.StreamerNick"] = "";
                }
            } catch {
                Config[ConfigPrefix + ".SC2TVCHAT.StreamerID"] = _StreamerID = 0;
                Config[ConfigPrefix + ".SC2TVCHAT.StreamerNick"] = "";
                MessageBox.Show("Ошибка сети или стример не найден.");
            }

            LoadedMessages.Clear();
            UpdateHeader();
            UpdateSmiles();
        }

        public void BeginWork() {
            if (next == null) {
                next = new DispatcherTimer();
                next.Interval = TimeSpan.FromSeconds(1);
                next.Tick += next_Tick;
                next.Start();
            }
        }

        public void EndWork() {
            if (next != null) {
                next.Stop();
                next = null;
            }
        }

        public event Core.OnNewMessagesArrivedDelegate OnNewMessagesArrived;

        public System.Windows.FrameworkElement CreateSmile( string id ) {
            return Smiles.GetSmile(id);
        }

        protected void FireChange( string PropertyName ) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public Dictionary<string, string> SmilesUri { get; private set; }

        public System.Windows.Controls.UserControl CreateCustomView() {
            return null;
        }

        public string ConfigPrefix { get; set; }

     
    }
}
